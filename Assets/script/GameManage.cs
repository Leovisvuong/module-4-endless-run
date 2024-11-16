using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    public bool gameStart;
    public bool canActive;
    public bool isStop;
    public bool canChangeIsStop;
    public bool isOnFinishLine = false;
    private PlayerMovement playerMovement;
    public List<GameObject> target = new List<GameObject>();
    public void AddTarget(GameObject targetAdd){
        target.Add(targetAdd);
    }
    public void SetGameStart(bool start){
        canActive = true;
        gameStart = start;
    }
    public void SetGameStop(bool value){
        isStop = value;
        canChangeIsStop = true;
    }
    public void SetIsOnFinishLine(bool value){
        isOnFinishLine = value;
    }
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if(canChangeIsStop){
            if(isStop && gameStart){
                StopAll();
            }
            if(!isStop && gameStart){
                EndStopAll();
            }
            canChangeIsStop = false;
        }
        if(!canActive) return;
        if(gameStart){
            ActiveAll();
            canActive = false;
        }
        else{
            EndAll();
            canActive = false;
        }
    }
    private void ActiveAll(){
        GameObject pathGenerator = GameObject.Find("PathGenerator");
        PathGenerator pathGeneratorScript = pathGenerator.GetComponent<PathGenerator>();
        pathGeneratorScript.SetCanStart();
        playerMovement.setAnimationRun(true);
        playerMovement.SetStop(1);
    }
    private void EndAll(){
        foreach(var i in target){
            if(i == null) continue;
            TargetOfGameManager targetOfGameManager = i.GetComponent<TargetOfGameManager>();
            targetOfGameManager.DestroySelf();
        }
        target.Clear();
        playerMovement.setAnimationRun(false);
        playerMovement.SetStop(1);
        GameObject cameraLookTarget = GameObject.Find("CameraLookTarget");
        LookTargetController lookTargetController = cameraLookTarget.GetComponent<LookTargetController>();
        lookTargetController.ResetRotation();
    }
    private void StopAll(){
        foreach(var i in target){
            if(i == null) continue;
            ObstacleRolling obstacleRolling = i.GetComponent<ObstacleRolling>();
            ItemsIdle itemsIdle = i.GetComponent<ItemsIdle>();
            if(obstacleRolling != null) obstacleRolling.SetStop(true);
            if(itemsIdle != null) itemsIdle.SetStop(true);
        }
        if(!isOnFinishLine)playerMovement.SetStop(0);
    }
    private void EndStopAll(){
        foreach(var i in target){
            if(i == null) continue;
            ObstacleRolling obstacleRolling = i.GetComponent<ObstacleRolling>();
            ItemsIdle itemsIdle = i.GetComponent<ItemsIdle>();
            if(obstacleRolling != null) obstacleRolling.SetStop(false);
            if(itemsIdle != null) itemsIdle.SetStop(false);
        }
        playerMovement.SetStop(1);
    }
}
