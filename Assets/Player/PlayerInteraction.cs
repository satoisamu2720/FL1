using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInteraction : MonoBehaviour
{
    private Inventory inventory; //プレイヤーのインベントリ参照
    private Chest chest; //宝箱のアセット


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        // 宝箱と接触中で、Eキーが押されたら開く
        if (chest != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("宝箱を開けた！");
            chest.OpenChest(inventory);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            chest = other.GetComponent<Chest>();
            Debug.Log("宝箱の前に立った");
        }
    }
    // 宝箱から離れたとき
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            if (chest == other.GetComponent<Chest>())
                chest = null;
        }
    }
}
