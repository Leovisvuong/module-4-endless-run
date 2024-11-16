using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTargetController : MonoBehaviour
{
    public GameObject player;
    public Quaternion presentRotation;
    public bool resetRotation = false;
    private Vector3 startAngle = new Vector3(0,0,0);
    public void changRotation(Quaternion rotationChange){
        presentRotation = rotationChange;
    }
    public void ResetRotation(){
        resetRotation = true;
    }
    void Start()
    {
        transform.rotation = player.transform.rotation;
    }

    void LateUpdate()
    {
        if(resetRotation){
            transform.rotation = presentRotation = Quaternion.Euler(startAngle);
            resetRotation = false;
        }
        transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        if(transform.rotation.y != presentRotation.y){
            float maxDegreesDelta = 90f / 0.5f * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, presentRotation, maxDegreesDelta);
        }
    }
    void OnTriggerStay(Collider other){
        if(other.gameObject.CompareTag("Floor")){
            presentRotation = other.transform.rotation;
        }
    }
}
