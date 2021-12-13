using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;
    [SerializeField] private float vol;

    [Header("Levels")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject exitPopup = null;
    [SerializeField] private GameObject creditsPopup = null;

    [Header("Menus")]
    [SerializeField] private GameObject settingsMenu = null;
    [SerializeField] private GameObject lvlsMenu = null;
    [SerializeField] private GameObject mainMenu = null;
    public string mainMenuForLoad;

    public void StartNewGame()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadLevel()
    {
        //TO DO
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(mainMenuForLoad);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool fromLvlToSetFlag;

    public void GoToSettingsFromLevels()
    {
        fromLvlToSetFlag = true;
    }
    public void GoBackFromSettingsLevel()
    {
        if (fromLvlToSetFlag)
        {
            settingsMenu.SetActive(false);
            mainMenu.SetActive(false);
            lvlsMenu.SetActive(true);
            fromLvlToSetFlag = false;
        }  
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("quited the game");
    }
    public void CloseExitPopup()
    {
        exitPopup.SetActive(false);
    }

    //TO DO: make one closepopup method
    public void CloseCreditsPopup()
    {
        creditsPopup.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        vol = AudioListener.volume;
    }

    //TO DO: audio listener
    //TO DO: mute method
    //TO DO: SFX toddle controller
}

