using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public int currentMapID = 0;

    bool isOpenDoor = false;

    int OpenDoorNum = 0;

    private void Awake()
    {
        Instance = this;
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
        // ここでマップ + 扉 に応じてギミック変化
        // 例：仕掛けの ON/OFF、敵の配置、スイッチの状態など

        if (mapID == 1)
        {
            //if (doorID == 1)
            //{
            //    Debug.Log("マップ1の扉1から来た → ギミックAをON");
            //    // ギミックA.On();
            //}
            //else if (doorID == 2)
            //{
            //    Debug.Log("マップ1の扉2から来た → ギミックBをON");
            //    // ギミックB.On();
            //}

            if(OpenDoorNum  == 3)
            {
                isOpenDoor = true;
            }


        }
    }

    void Reset()
    {
        isOpenDoor = false;
        OpenDoorNum = 0;
    }

}
