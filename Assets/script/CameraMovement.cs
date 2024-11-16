using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public bool gameMenu;
    public bool canMoveToTarget;
    private Vector3 offset = new Vector3(0,4f,-4.5f);
    private GameObject Player;
    private PlayerMovement playerMovement;
    public void SetGameMenu(bool value){
        gameMenu = value;
        if(value) canMoveToTarget = true;
    }
    void Start(){
        Player = GameObject.FindWithTag("Player");
        playerMovement = Player.GetComponent<PlayerMovement>();
    }
    void LateUpdate()
    {
        Vector3 rotatedOffset = target.TransformDirection(offset);
        Vector3 desiredPosition = target.position + rotatedOffset;
        if(gameMenu){
            playerMovement.SetCanTurn(false);
            transform.position = new Vector3(desiredPosition.x,desiredPosition.y,-desiredPosition.z);
            transform.LookAt(target);
            transform.Rotate(-25,0,0);
            return;
        }
        if(canMoveToTarget){
            if(transform.position.z > desiredPosition.z){
                transform.LookAt(target);
                transform.Rotate(-25,0,0);
                return;
            }
            canMoveToTarget = false;
            playerMovement.SetCanTurn(true);
        }
        transform.position = desiredPosition;
        transform.LookAt(target);
        transform.Rotate(-25,0,0);
    }
}
