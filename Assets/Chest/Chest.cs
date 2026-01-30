using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("宝箱設定")]
    public int chestID;
    public int itemIDToGive;
    public bool isOpened = false;

    [Header("見た目")]
    public Sprite closedSprite;
    public Sprite openedSprite;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void OpenChest(Inventory inventory)
    {
        if (isOpened) return;

        if (inventory == null)
        {
            Debug.LogError("Inventory が null です");
            return;
        }

        isOpened = true;
        UpdateVisual();

        Debug.Log("宝箱が開きました！ アイテムID: " + itemIDToGive);
        inventory.AddItem(itemIDToGive);
    }

    void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = isOpened ? openedSprite : closedSprite;
    }
}
