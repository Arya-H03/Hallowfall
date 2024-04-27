using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuManager : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;
   public void OnButtonStartClick()
    {
        SceneManager.LoadScene("RealmBeyond");
    }

    public void OnButtonExitClick()
    {
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
}
