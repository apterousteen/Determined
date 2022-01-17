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
    public AudioManager audioManager;

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
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        SetVolume(defaultVolume);
    }

    public void ShowHint()
    {
        if (resultBoard.typeOfResult == DeterminantType.DoubleMatrix)
            hintPopup.GetComponentInChildren<Image>().sprite = doubleMatrix;
        else if (resultBoard.typeOfResult == DeterminantType.Triangles)
            hintPopup.GetComponentInChildren<Image>().sprite = triangle;
        else if (resultBoard.typeOfResult == DeterminantType.Leibniz)
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuTemp");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
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
            //audioManager.toggle.isOn = true;
            //UpdateSoundUI();
        }
        else
        {
            Mute();
            audioManager.muted = true;
            //audioManager.toggle.isOn = false;
            //UpdateSoundUI();
        }
    }

    public void Mute()
    {
        AudioListener.volume = 0;
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void Unmute()
    {
        AudioListener.volume = vol;
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    private void Update()
    {
        UpdateSoundUI();
        try
        {
            OpenWinPopup();
        }
        catch (Exception)
        {
        };
    }

    public void UpdateSoundUI()
    {
        if (audioManager.muted)
        {
            try
            {
                if (SceneManager.GetActiveScene().name == "MainMenuTemp")
                {
                    FindObjectsOfType<Button>().Where(x => x.gameObject.tag == "Sound Button").First().GetComponentInChildren<Image>().sprite = audioManager.mutedSprite;
                    //audioManager.toggle.isOn = false;
                }
                else
                {
                    FindObjectsOfType<Button>().Where(x => x.gameObject.tag == "Sound Button").First().GetComponentInChildren<Image>().sprite = audioManager.mutedSpriteMini;
                }
            }
            catch (Exception)
            {
            };
        }
        else
        {
            try
            {
                if (SceneManager.GetActiveScene().name == "MainMenuTemp")
                {
                    FindObjectsOfType<Button>().Where(x => x.gameObject.tag == "Sound Button").First().GetComponentInChildren<Image>().sprite = audioManager.unmutedSprite;
                    //audioManager.toggle.isOn = true;
                }
                else
                {
                    FindObjectsOfType<Button>().Where(x => x.gameObject.tag == "Sound Button").First().GetComponentInChildren<Image>().sprite = audioManager.unmutedSpriteMini;
                }
            }
            catch (Exception)
            {
            };
        }
    }

    public void OpenWinPopup()
    {

        if (resultBoard.levelWasWon)
        {
            Debug.Log("Level Was Won");
            FindObjectsOfType<Button>().Where(x => x.gameObject.tag == "Result Button").First().enabled = false;
            StartCoroutine(WaitAndShow(winPopup, 2.0f));
            resultBoard.levelWasWon = false;
        }//4 secs
    }

    IEnumerator WaitAndShow(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(true);
    }

    public void PlayButtonSound()
    {
        audioManager.Play("Button");
    }
}