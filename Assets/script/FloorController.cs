using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class FloorController : MonoBehaviour
{
    public GameObject cameraLookTarget;
    public int previousBoost = 0;
    public bool isLastFloor;
    public GameObject largeBarrier;
    public GameObject smallBarrier;
    public GameObject swiper;
    public GameObject spikeRoller;
    public GameObject wall;
    public GameObject star;
    public GameObject undead;
    public GameObject HPUp;
    public GameObject stopWall;
    private GameManage gameManage;
    private Dictionary<int,GameObject> typeOfObstacle = new Dictionary<int,GameObject>();
    public int floorNumber;
    public bool isCorner = false;
    int positionX;
    int positionZ;
    public void SetIsLastFloor(){
        isLastFloor = true;
    }
    public void SetFloorNumber(int number){
        floorNumber = number;
    }
    public void SetIsCorner(){
        isCorner = true;
    }
    public void SetPreviousBoost(int value){
        previousBoost = value;
    }
    private void GenerateObstacle(){
        int obstacleNumber = Random.Range(1,5);
        positionX = Random.Range(-1,2);
        positionZ = Random.Range(-1,2);
        if(obstacleNumber == 3) positionZ = 0;
        if(transform.rotation.y > 0){
            (positionX,positionZ) = (-positionZ,positionX);
        }
        else if(transform.rotation.y < 0){
            (positionX,positionZ) = (positionZ,-positionX);
        }
        if(transform.rotation.y != 0){
            switch(obstacleNumber){
                case 1:
                    if(positionZ<0) positionZ = -1;
                    else positionZ = 1;
                    break;
                default:
                    positionZ *= 2;
                    break;
            }
        }
        else{
            switch(obstacleNumber){
                case 1:
                    if(positionX<0) positionX = -1;
                    else positionX = 1;
                    break;
                default:
                    positionX *= 2;
                    break;
            }
        }
        var objectPosition = new Vector3(transform.position.x + positionX , 0.2f , transform.position.z + positionZ);
        GameObject newObstacle = Instantiate(typeOfObstacle[obstacleNumber],objectPosition,transform.rotation);
        gameManage.AddTarget(newObstacle);
    }
    private void GenerateTerrain(){
        int terrainLineQuantity = Random.Range(1,3);
        positionZ = Random.Range(-1,2);
        positionX = Random.Range(-1,2);
        List<int> existWall = new List<int>();
        for(int i = 1; i <= terrainLineQuantity; i++){
            int positionZNote = positionZ;
            while(existWall.Contains(positionX)) positionX = Random.Range(-1,2);
            existWall.Add(positionX);
            if(transform.rotation.y > 0){
                (positionX,positionZNote) = (-positionZNote,positionX);
                positionZNote *= 2;
            }
            else if(transform.rotation.y < 0){
                (positionX,positionZNote) = (positionZNote,-positionX);
                positionZNote *= 2;
            }
            else positionX *= 2;
            var objectPosition = new Vector3(transform.position.x + positionX , 0.2f , transform.position.z + positionZNote);
            GameObject newObstacle = Instantiate(wall,objectPosition,transform.rotation*Quaternion.Euler(0,90,0));
            if(transform.rotation.y == 0) positionX /= 2;
            gameManage.AddTarget(newObstacle);
        }
    }
    private void GenerateStopWall(){
        positionZ = 1;
        for(int i = 1; i <= 3; i++){
            int positionZNote = positionZ;
            positionX = i - 2;
            if(transform.rotation.y > 0){
                (positionX,positionZNote) = (-positionZNote,positionX);
                positionZNote *= 2;
            }
            else if(transform.rotation.y < 0){
                (positionX,positionZNote) = (positionZNote,-positionX);
                positionZNote *= 2;
            }
            else positionX *= 2;
            var objectPosition = new Vector3(transform.position.x + positionX , 0.2f , transform.position.z + positionZNote);
            GameObject newObstacle = Instantiate(stopWall,objectPosition,transform.rotation*Quaternion.Euler(0,90,0));
            if(transform.rotation.y == 0) positionX /= 2;
            gameManage.AddTarget(newObstacle);
        }
    }
    private void GenerateItem(int itemNumber){
        positionX = Random.Range(-1,2);
        positionZ = Random.Range(-1,2);
        if(itemNumber == 3) positionZ = 0;
        if(transform.rotation.y > 0){
            (positionX,positionZ) = (-positionZ,positionX);
        }
        else if(transform.rotation.y < 0){
            (positionX,positionZ) = (positionZ,-positionX);
        }
        if(transform.rotation.y != 0){
            positionZ *= 2;
        }
        else{
            positionX *= 2;
        }
        var objectPosition = new Vector3(transform.position.x + positionX , 0.7f , transform.position.z + positionZ);
        GameObject itemObject = (itemNumber == 1 ) ? star : ( (itemNumber == 2) ? HPUp : undead);
        GameObject newObstacle = Instantiate(itemObject,objectPosition,Quaternion.identity);
        gameManage.AddTarget(newObstacle);
    }
    void Start(){
        gameManage = GameObject.Find("GameManager").GetComponent<GameManage>();
        UIManager UIManagerScript = GameObject.Find("UIManager").GetComponent<UIManager>();
        int numberOfFloor = (UIManagerScript.gameLevel - 1) * 50 + 100;
        typeOfObstacle.Add(1,largeBarrier);
        typeOfObstacle.Add(2,smallBarrier);
        typeOfObstacle.Add(3,swiper);
        typeOfObstacle.Add(4,spikeRoller);
        if(floorNumber == (int)(numberOfFloor / 4) || floorNumber == (int)(numberOfFloor * 3 / 4) || floorNumber == (int)(numberOfFloor / 2)){
            GenerateItem(1);
            return;
        }
        if(((floorNumber > (int)(numberOfFloor / 3) && floorNumber < (int)(numberOfFloor / 2)) || (floorNumber > (int)(numberOfFloor * 2 / 3))) && Random.Range(-1,5) < 0){
            if(UIManagerScript.CheckMaxValue("HPUp")){
                GenerateItem(2);
                return;
            }
        }
        else if(((floorNumber > (int)(numberOfFloor / 2) && floorNumber < (int)(numberOfFloor * 2 / 3)) || (floorNumber > (int)(numberOfFloor * 2 / 3) + 1)) && Random.Range(-1,5) < 0){
            if(UIManagerScript.CheckMaxValue("undead")){
                GenerateItem(3);
                return;
            }
        }
        int canGenerateObstacle = Random.Range(-2,2);
        if(isLastFloor){
            GenerateStopWall();
            return;
        }
        if(floorNumber > 2 && !isCorner){
            switch(canGenerateObstacle){
                case >0:
                    return;
                case <0:
                    GenerateObstacle();
                    break;
                case 0:
                    GenerateTerrain();
                    break;
            }
        }
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("CameraTarget") && floorNumber % 10 == 1){
            PlayerMovement playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            playerMovement.SetRespawn(transform);
        }
    }
}
