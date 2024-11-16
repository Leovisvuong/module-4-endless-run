using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private PlayerMovement playerScript;
    private BoxCollider boxCollider;
    private CapsuleCollider capsuleCollider;
    void Start(){
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            if(playerScript.isUndead){
                if(boxCollider != null) boxCollider.enabled = false;
                if(capsuleCollider != null) capsuleCollider.enabled = true;
            }
            else{
                UIManager UIManagerScript = GameObject.Find("UIManager").GetComponent<UIManager>();
                UIManagerScript.MinusLife();
            }
        }
    }
}
