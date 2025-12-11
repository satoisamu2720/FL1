using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public int currentMapID = 0;


    public bool[] isGimickClearMap = new bool[6];

    public bool isOpenDoor = false;

    public int OpenDoorNum = 0;

    private void Awake()
    {
        Instance = this;

    }

    private void Update()
    {
        ApplyGimmicks(currentMapID, 1);
    }

    public void MapTransition(int doorID, int toMapID)
    {
        //// ①カメラ移動
        //Camera.main.GetComponent<CameraController>().MoveTo(cameraPos);

        //// ②プレイヤー移動
        //Player.Instance.transform.position = playerPos;

        // マップID更新
        currentMapID = toMapID;

        // ギミック更新
        ApplyGimmicks(toMapID, doorID);
    }

    private void ApplyGimmicks(int mapID, int doorID)
    {

        if (mapID == 0)
        {
            //isGimickClearMap[0] = true;

            if(OpenDoorNum == 3)
            {
                isGimickClearMap[0] = true;
            }
            else
            {
                isGimickClearMap[0] = false;
            }

            if (isGimickClearMap[0] == true)
            {
                //isOpenDoor = true;
                SceneManager.LoadScene("ClearScene");
                Reset();
            }
            else
            {
                isOpenDoor = false;
            }

        }

        if (mapID == 1)
        {
            if (OpenDoorNum == 1)
            {
                isGimickClearMap[1] = true;
            }

            if (isGimickClearMap[1] == true)
            {
                isOpenDoor = true;
            }
            else
            {
                isOpenDoor = false;
            }


        }

        if (mapID == 2)
        {

            if(OpenDoorNum == 3)
            {
                isGimickClearMap[2] = true;
            }

            if (isGimickClearMap[2] == true)
            {
                isOpenDoor = true;
            }
            else
            {
                isOpenDoor = false;
            }
        }

        if (mapID == 3)
        {

            isGimickClearMap[3] = true;

            if (isGimickClearMap[3] == true)
            {
                isOpenDoor = true;
            }
            else
            {
                isOpenDoor = false;
            }
        }

        if (mapID == 4)
        {

            if(OpenDoorNum == 4)
            {
                isGimickClearMap[4] = true;
            }

            if (isGimickClearMap[4] == true)
            {
                isOpenDoor = true;
            }
            else
            {
                isOpenDoor = false;
            }
        }

        if (mapID == 5)
        {
            if (isGimickClearMap[5] == true)
            {
                isOpenDoor = true;
            }
            else
            {
                isOpenDoor = false;
            }
        }

    }

    public void Reset()
    {
        //isOpenDoor = false;
        OpenDoorNum = 0;
    }

}
