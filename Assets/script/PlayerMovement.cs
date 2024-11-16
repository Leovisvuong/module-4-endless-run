using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public bool canTurn;
    public bool canStart;
    public float jumpStrength;
    private bool canJump = true;
    public bool isUndead;
    public int undeadTime;
    public bool isCountingDown;
    private bool canPlayWinSound;
    private bool isOnFinishLine;
    private bool jumpButtonIsPressed;
    public Vector3 respawnPosition;
    public Transform defaultRespawnTransform;
    public Quaternion respawnRotation;
    private Rigidbody rb;
    public GameObject finalFloor;
    private Vector3 startPosition;
    private Vector3 startAngle;
    private Animator animator;
    private bool isTurning = false;
    private Quaternion targetRotation;
    public AudioSource jumpSound1;
    public AudioSource jumpSound2;
    public AudioSource jumpSound3;
    public AudioSource winSound;
    public AudioSource hurtSound;
    public AudioSource turnSound;
    UIManager UIManagerScript;
    public void SetIsUndead(int undeadTimeValue){
        undeadTime = undeadTimeValue;
        isUndead = true;
        if(undeadTime == 15) speed += 2;
        if(!isCountingDown)StartCoroutine(UndeadCountDown());
    }
    public void SetFinalFloor(GameObject value){
        finalFloor = value;
    }
    public void SetStop(float value){
        Time.timeScale = value;
    }
    public void SetCanTurn(bool value){
        canTurn = value;
    }
    public void ResetOnFinishFloor(){
        isOnFinishLine = false;
    }
    public void setAnimationRun(bool isRunning){
        animator.SetBool("isRunning", isRunning);
        if(!isRunning) canStart = false;
        else canStart = true;
    }
    public void SetRespawn(Transform respawnTransform){
        if(!isOnFinishLine){
            respawnPosition = respawnTransform.position;
            respawnRotation = respawnTransform.rotation;
        }
    }
    public void Respawn(bool canUndead){
        hurtSound.Play();
        transform.position = respawnPosition  + new Vector3(0,1,0);
        transform.rotation = targetRotation = respawnRotation;
        if(canUndead) SetIsUndead(3);
    }
    void Start(){
        UIManagerScript = GameObject.Find("UIManager").GetComponent<UIManager>();
        respawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;
        startPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        startAngle = transform.rotation.eulerAngles;
        animator = gameObject.GetComponent<Animator>();
    }
    void Update()
    {
        if(!canStart){
            if(isOnFinishLine) return;
            transform.position = startPosition;
            transform.rotation = targetRotation = Quaternion.Euler(startAngle);
            canPlayWinSound = true;
            return;
        }
        jumpButtonIsPressed = (Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Vertical") > 0) ? true : false;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(transform.position.y < -3){
            if(isUndead) Respawn(false);
            else UIManagerScript.MinusLife();
        }
        if (transform.rotation != targetRotation){
            float maxDegreesDelta = 90f / 10f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta);
            return;
        }
        if(!canTurn) return;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0 && !isTurning){
            turn(90);
            isTurning = true;
        }
        if (horizontalInput < 0 && !isTurning){
            turn(-90);
            isTurning = true;
        }
        if (horizontalInput == 0) isTurning = false;
    }
    void FixedUpdate(){
        if(!canStart) return;
        if(jumpButtonIsPressed && canJump){
            rb.AddForce(new Vector3(0,jumpStrength,0),ForceMode.Impulse);
            canJump = false;
            animator.SetBool("isJumping",true);
            int randomSound = Random.Range(-1,2);
            switch(randomSound){
                case < 0:
                    jumpSound1.Play();
                    break;
                case 0:
                    jumpSound2.Play();
                    break;
                case > 0:
                    jumpSound3.Play();
                    break;
            }
        }
    }
    void turn(int angle){
        turnSound.Play();
        targetRotation *= Quaternion.Euler(0, angle, 0);
    }
    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Floor")){
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if(finalFloor == other.gameObject){
                respawnRotation = defaultRespawnTransform.rotation;
                respawnPosition = defaultRespawnTransform.position;
                GameManage gameManage = GameObject.Find("GameManager").GetComponent<GameManage>();
                gameManage.SetIsOnFinishLine(true);
                UIManagerScript.DoDoneLevel();
            }
            canJump = true;
            animator.SetBool("isJumping",false);
        }
        if(other.gameObject.CompareTag("Finish")){
            isOnFinishLine = true;
            setAnimationRun(false);
        }
    }
    void OnCollisionStay(Collision other){
        if(other.gameObject.CompareTag("Floor") && finalFloor == other.gameObject){
            isOnFinishLine = true;
        if(canPlayWinSound){
            canPlayWinSound = false;
            winSound.Play();
        }
        }
    }
    IEnumerator UndeadCountDown(){
        isCountingDown = true;
        while(undeadTime >= 0){
            yield return new WaitForSeconds(1f);
            undeadTime--;
        }
        isUndead = false;
        isCountingDown = false;
        speed = 5;
    }
}
