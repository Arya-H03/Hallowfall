using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuManager : MonoBehaviour
{
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
}
