using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int healthValue;
    public int fullHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Update()
    {
        if (healthValue == 0)
        {
            Debug.Log("fail screen");
        }

        if (healthValue > fullHealth)
        {
            healthValue = fullHealth;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < healthValue)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < fullHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
