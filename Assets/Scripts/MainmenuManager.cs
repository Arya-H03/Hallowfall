using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuManager : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject skillPanel;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip backgroundMusic;

    private void Start()
    {
        GameData gameData = SaveSystem.LoadGameData();
        
        AudioManager.Instance.LoadSoundData();
        AudioManager.Instance.PlayMusic(audioSource, backgroundMusic, 1 );  
    }
    private void OnGUI()
    {
        audioSource.volume = AudioManager.Instance.MasterVolumeMultiplier * AudioManager.Instance.MusicVolumeMultiplier;
    }
    public void OnButtonStartClick()
    {
        AudioManager.Instance.SaveSoundData();  
        SceneManager.LoadScene("Game");
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

    public void OnSkillPanelClick()
    {
        SaveSystem.UpdatePlayerSkulls(100);
        GameData gameData = SaveSystem.LoadGameData();
        foreach (int node in gameData.skillTreeNodes)
        {
            Debug.Log(node);
        }
        OpenPanel(skillPanel);
        skillPanel.GetComponent<SkillTreeManager>().UpdateSkullsText();
        skillPanel.GetComponent<SkillTreeManager>().ApplySavedSkillTree(gameData.skillTreeNodes);
        
        
    }
    public void OnSkillPanelCloseClick()
    {
        ClosePanel(skillPanel);
    }
}
