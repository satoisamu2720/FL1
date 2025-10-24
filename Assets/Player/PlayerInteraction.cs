using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 2f; //宝箱との距離
    private PlayerInventory inventory; //プレイヤーのインベントリ参照


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TryOpenChest();
        }
    }

    private void TryOpenChest()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);

        if(Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            Chest chest = hit.collider.GetComponent<Chest>();
            if (chest != null)
            {
                chest.OpenChest(inventory);
            }
        }
    }
}
