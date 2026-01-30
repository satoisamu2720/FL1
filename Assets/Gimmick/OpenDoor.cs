using UnityEngine;
using static MapManager;

public class OpenDoor : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 0.5f, 0);
    public float speed = 2f;

    public int mapID;

    [Header("条件")]
    public RoomConditionType conditionType;

    [Header("必要数（ギミック用）")]
    public int requiredCount = 1;

    private bool opened = false;
    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;

        Open(); // 初期は開いている
    }

    private void Update()
    {
        if (opened) 
        { 
            return; 
        }
        if (MapManager.Instance.currentMapID != mapID) 
        { 
            return; 
        }

        if (conditionType == RoomConditionType.Battle)
        {
            if (MapManager.Instance.IsConditionCleared())
            {
                Open();
            }
        }

        if (conditionType == RoomConditionType.Gimmick)
        {
            if (MapManager.Instance.currentCount >= requiredCount)
            {
                Open();
            }
        }
    

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("OPEN");
            Open();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("CLOSE");
            Close();
        }
#endif
    }

    public void Close()
    {
        if (!opened) 
        { 
            return; 
        }
        opened = false;
        StopAllCoroutines();
        StartCoroutine(MoveTo(closedPos));
    }

    public void Open()
    {
        if (opened)
        {
            return;
        }
        opened = true;
        StopAllCoroutines();
        StartCoroutine(MoveTo(openPos));
    }

    System.Collections.IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                Time.deltaTime * speed
            );
            yield return null;
        }
    }
}
