using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuManager : MonoBehaviour
{
    [SerializeField] GameObject creditsPannel;
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

    private void OpenPannel(GameObject pannel)
    {
        pannel.SetActive(true);
    }

    private void ClosePannel(GameObject pannel)
    {
        pannel.SetActive(false);
    }

    public void OnCreditButtonClickOpen()
    {
        OpenPannel(creditsPannel);
    }

    public void OnCreditButtonClickClose()
    {
        ClosePannel(creditsPannel);
    }
}
