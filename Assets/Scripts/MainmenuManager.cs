using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuManager : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject settingsPanel;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip backgroundMusic;

    private void Start()
    {
        AudioManager.Instance.LoadSoundData();
        AudioManager.Instance.PlayMusic(audioSource, backgroundMusic);  
    }
    private void OnGUI()
    {
        audioSource.volume = AudioManager.Instance.MasterVolumeMultiplier * AudioManager.Instance.MusicVolumeMultiplier;
    }
    public void OnButtonStartClick()
    {
        AudioManager.Instance.SaveSoundData();  
        SceneManager.LoadScene("Cemetery");
    }

    public void OnButtonExitClick()
    {
        AudioManager.Instance.SaveSoundData();
        Application.Quit();
    }

    public void OnButtonSettingsClick()
    {
        
    }

    private void OpenPanel(GameObject pannel)
    {
        pannel.SetActive(true);
    }

    private void ClosePanel(GameObject pannel)
    {
        pannel.SetActive(false);
    }

    public void OnCreditButtonOpenClick()
    {
        OpenPanel(creditsPanel);
    }

    public void OnCreditButtonCloseClick()
    {
        ClosePanel(creditsPanel);
    }

    public void OnSettingstButtonOpenClick()
    {
        OpenPanel(settingsPanel);
    }

    public void OnSettingsButtonCloseClick()
    {
        ClosePanel(settingsPanel);
    }
}
