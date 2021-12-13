using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int healthValue;
    public int fullHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Image[] stars;
    public Sprite fullStar;
    public Sprite emptyStar;

    [Header("Menus")]
    [SerializeField] private GameObject failMenu = null;

    bool gameHasEnded;

    void Update()
    {
        if (healthValue == 0 && gameHasEnded == false)
        {
            gameHasEnded = true;
            Debug.Log("fail screen");
            failMenu.SetActive(true);
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
                stars[i].sprite = fullStar;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
                stars[i].sprite = emptyStar;
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
