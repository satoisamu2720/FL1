using UnityEngine;
using static MapManager;

public class OpenDoor : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 0.5f, 0);
    public float speed = 2f;

    public int mapID;

    [Header("ğŒ")]
    public MapManager.RoomConditionType conditionType;
    public int requiredCount = 1;

    [Header("í“¬")]
    public bool closeOnBattleStart = true;

    [Header("‰Šúó‘Ô")]
    public bool startOpened = false;

    private bool opened;
    private Vector3 closedPos;
    private Vector3 openPos;

    private AudioSource audioSource;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;

        audioSource = GetComponent<AudioSource>();

        if (startOpened)
        {
            opened = true;
            transform.position = openPos;   // ‘¦À‚ÉŠJ‚¢‚½ˆÊ’u
        }
        else
        {
            opened = false;
            transform.position = closedPos;
        }
    }

    void Update()
    {
        if (opened) return;
        if (MapManager.Instance.currentMapID != mapID) return;

        if (MapManager.Instance.CanOpen(conditionType, requiredCount))
        {
            Open();
        }
    }

    public void CloseByBattle()
    {
        if (!closeOnBattleStart) return;
        Close();
    }
    public void Close()
    {
        if (!opened) return;

        opened = false;
        StopAllCoroutines();
        StartCoroutine(MoveTo(closedPos));
    }
    public void Open()
    {
        if (opened) return;

        opened = true;
        StopAllCoroutines();
        StartCoroutine(MoveTo(openPos));
        audioSource.Play();
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
