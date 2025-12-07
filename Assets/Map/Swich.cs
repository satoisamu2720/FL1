using UnityEngine;

public class Swich : MonoBehaviour
{

    public bool isStepOn;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MapGimmick mapGimmick = GetComponent<MapGimmick>();


        if (isStepOn == true)
        {
            mapGimmick.openNum++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isStepOn = true;
        }
    }

}
