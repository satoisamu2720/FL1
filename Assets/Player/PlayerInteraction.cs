using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInteraction : MonoBehaviour
{
    private Inventory inventory; //�v���C���[�̃C���x���g���Q��
    private Chest chest; //�󔠂̃A�Z�b�g


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        // �󔠂ƐڐG���ŁAE�L�[�������ꂽ��J��
        if (chest != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("�󔠂��J�����I");
            chest.OpenChest(inventory);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            chest = other.GetComponent<Chest>();
            Debug.Log("�󔠂̑O�ɗ�����");
        }
    }
    // �󔠂��痣�ꂽ�Ƃ�
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            if (chest == other.GetComponent<Chest>())
                chest = null;
        }
    }
}
