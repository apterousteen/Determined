using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading;
using System;
using System.Drawing;

public class MenuController : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;
    [SerializeField] private float vol;
    AudioManager audioManager;

    [Header("Levels")]
    public string _newGameLevel;

    [Header("Popups")]
    [SerializeField] private GameObject exitPopup = null;
    [SerializeField] private GameObject creditsPopup = null;
    [SerializeField] private GameObject winPopup = null;
    [SerializeField] private GameObject failPopup = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject hintPopup = null;

    [Header("Menus")]
    [SerializeField] private GameObject settingsMenu = null;
    [SerializeField] private GameObject lvlsMenu = null;
    [SerializeField] private GameObject mainMenu = null;

    [Header("Hint Types")]
    [SerializeField] private Sprite doubleMatrix = null;
    [SerializeField] private Sprite triangle = null;
    [SerializeField] private Sprite leibniz = null;

    public ResultBoard resultBoard;


    private void Awake()
    {
        resultBoard = FindObjectOfType<ResultBoard>();
        audioManager = FindObjectOfType<AudioManager>();
        SetVolume(defaultVolume);
    }

    public void ShowHint()
    {
        //if (resultBoard.typeOfResult == DeterminantType.DoubleMatrix)
        //    hintPopup.GetComponentInChildren<Image>().sprite = doubleMatrix;
       // else if (resultBoard.typeOfResult == DeterminantType.Triangles)
        //    hintPopup.GetComponentInChildren<Image>().sprite = triangle;
       // else if (resultBoard.typeOfResult == DeterminantType.Leibniz)
            hintPopup.GetComponentInChildren<Image>().sprite = leibniz;
    }

    public static bool GameIsPaused = false;

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void SelectLevel (string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("MainMenuTemp");
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

    public void ControlAudio()
    {
        if (audioManager.muted)
        {
            Unmute();
            audioManager.muted = false;
        }
        else
        {
            Mute();
            audioManager.muted = true;
        }
    }

    private void Mute()
    {
        AudioListener.volume = 0;
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    private void Unmute()
    {
        AudioListener.volume = vol;
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void OpenWinPopup()
    {
        if (resultBoard.levelWasWon)
        {
            FindObjectsOfType<Button>().Where(x => x.gameObject.tag == "Result Button").First().enabled = false;
            StartCoroutine(WaitAndShow(winPopup, 2.0f));
        }//4 secs
    }

    IEnumerator WaitAndShow(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(true);
    }

    //TO DO: audio listener
    //TO DO: mute method
}

