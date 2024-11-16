using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public GameObject segmentPrefab; 
    private Vector3 nextSpawnPoint;
    private Quaternion nextRotation = Quaternion.identity;
    private int currentTotalRotation = 0;
    private bool nextIsCorner= false;
    public bool canStart = false;
    public int gameLevel;
    public void SetGameLevel(int level){
        gameLevel = level;
    }
    public void SetCanStart(){
        canStart = true;
    }
    void Update()
    {
        if(!canStart){
            nextSpawnPoint = GameObject.Find("Floor").transform.forward*6;
            nextRotation = Quaternion.identity;
            currentTotalRotation = 0;
            nextIsCorner = false;
            return;
        }
        for (int i = 0; i < 100 + (gameLevel - 1) * 50; i++) 
        {
            GenerateSegment(i+1);
        }
        canStart = false;
    }

    void GenerateSegment(int segmentCount)
    {
        GameObject newSegment = Instantiate(segmentPrefab, nextSpawnPoint, nextRotation);
        FloorController floorController = newSegment.GetComponent<FloorController>();;
        if(segmentCount == 100 + (gameLevel - 1) * 50){
            PlayerMovement playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            playerMovement.SetFinalFloor(newSegment);
            floorController.SetIsLastFloor();
        }
        GameObject gameManager = GameObject.Find("GameManager");
        GameManage gameManage = gameManager.GetComponent<GameManage>();
        gameManage.AddTarget(newSegment);
        floorController.SetFloorNumber(segmentCount);
        if(nextIsCorner){
            floorController.SetIsCorner();
            nextIsCorner = false;
        }
        if(segmentCount % 10 == 0)
        {
            int rotationAngle = RandomTurnWithConstraint();
            nextRotation *= Quaternion.Euler(0, rotationAngle, 0);
            currentTotalRotation += rotationAngle;
            if(rotationAngle != 0) nextIsCorner = true;
        }
        nextSpawnPoint += newSegment.transform.forward*6;
    }
    int RandomTurnWithConstraint(){
        int randomTurn;
        switch(currentTotalRotation){
            case 90:
                randomTurn = Random.Range(-1,0) * 90;
                break;
            case -90:
                randomTurn = Random.Range(0,2) * 90;
                break;
            default:
                randomTurn = Random.Range(-1,2) * 90;
                break;                
        }
        return randomTurn;
    }
}
