using UnityEngine;
using static Switch;

public class OpenDoor : MonoBehaviour, IMechanism
{
    [Header("ŠJ‚­Œü‚«")]
    public Vector3 openOffset = new Vector3(0, 0.5f, 0);

    [Header("‘¬“x")]
    public float speed = 2f;

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

    private System.Collections.IEnumerator OpenTheDoor()
    {
        while (Vector3.Distance(transform.position, openPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos, Time.deltaTime * speed);
            yield return null;
        }
    }
}
