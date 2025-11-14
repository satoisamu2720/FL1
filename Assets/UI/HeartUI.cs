using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Sprite[] heartSprites; // 0=‹ó, 1=1/4, 2=1/2, 3=3/4, 4=–žƒ^ƒ“

    private List<Image> hearts = new List<Image>();
    private int maxHearts = 0;

    void Start()
    {
        maxHearts = Status.Instance.MaxHP / 4;
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            hearts.Add(newHeart.GetComponent<Image>());
        }
    }

    void Update()
    {
       

    }

    void UpdateHearts()
    {
        if (Status.Instance == null) return;

        float hp = Status.Instance.PlayerHP;
        int fullHearts = Mathf.FloorToInt(hp / 4);
        int remainder = Mathf.FloorToInt(hp % 4);

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < fullHearts)
            {
                hearts[i].sprite = heartSprites[4];
            }
            else if (i == fullHearts)
            {
                hearts[i].sprite = heartSprites[remainder];
            }
            else
            {
                hearts[i].sprite = heartSprites[0];
            }
        }
    }
}
