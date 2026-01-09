using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Switch;

public class OpenDoor : MonoBehaviour, IMechanism
{
    [Header("開く向き")]
    public Vector3 openOffset = new Vector3(0, 0.5f, 0);

    [Header("速度")]
    public float speed = 2f;

    public int mapID;
    public int doorID;

    [Header("この扉が開くために必要な数")]
    public int requiredCount = 1;


    private bool opened = false;
    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset; 
    }

    public void Activate()
    {
        if (!opened)
        {
            opened = true;
            StartCoroutine(OpenTheDoor());
        }
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

        if (MapManager.Instance.OpenDoorNum >= requiredCount)
        {
            StartCoroutine(OpenTheDoor());
        }
    }


    private System.Collections.IEnumerator OpenTheDoor()
    {
        while (Vector3.Distance(transform.position, openPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos, Time.deltaTime * speed);
            yield return null;
        }
    }
}
