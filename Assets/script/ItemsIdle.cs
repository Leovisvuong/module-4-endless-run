using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ItemsIdle : MonoBehaviour
{
    private float maxY = 1.5f;
    private float minY = 1f;
    private int direction = 1;
    private float speed = 0.01f;
    public bool stop;
    public AudioSource starSound;
    public AudioSource powerUpSound;
    UIManager UIManagerScript;
    public void SetStop(bool value){
        stop = value;
    }
    void Start(){
        UIManagerScript = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
    void FixedUpdate()
    {
        if(stop || transform.position.y > 100f) return;
        if(direction > 0){
            if(transform.position.y >= maxY) direction = -1;
        }
        else if(direction < 0){
            if(transform.position.y <= minY) direction = 1;
        }
        transform.position += new Vector3(0,speed * direction,0);
        transform.rotation *= Quaternion.Euler(0,2.5f,0);
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            bool soundIsPLayed = false;
            if(gameObject.tag == "star"){
                UIManagerScript.PlusValue("star");
                starSound.Play();
                soundIsPLayed = true;
            }
            else if(gameObject.tag == "HP") UIManagerScript.PlusValue("life");
            else{
                PlayerMovement playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
                playerMovement.SetIsUndead(15);
            }
            if(!soundIsPLayed) powerUpSound.Play();
            Destroy(gameObject);
        }
    }
}
