using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRolling : MonoBehaviour
{
    private float speed;
    private int rollRotation = 0;
    public bool stop;
    public void SetStop(bool value){
        stop = value;
    }
    void Start(){
        speed = Random.Range(0.5f,1f);
        while(rollRotation == 0 ) rollRotation = Random.Range(-1,2);
    }
    void FixedUpdate()
    {
        if(stop) return;
        transform.rotation *= Quaternion.Euler(0,speed * rollRotation * 5,0);
    }
}
