using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField] DialogueBox dialogueBox;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject abilityWindow;

    [SerializeField] Transform healthBar;
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] Transform atonementBar;
    [SerializeField] TextMeshProUGUI atonementLvlText;

    [SerializeField] TextMeshProUGUI abilityDescription;

    private PlayerController playerController;


    public static UIManager Instance
    {
        get
        {
            if (!instance)
            {
                GameObject go = new GameObject("UIManager");
                instance = go.AddComponent<UIManager>();
            }
            return instance;
        }
    }

    public GameObject PauseMenu { get => pauseMenu; set => pauseMenu = value; }
    public GameObject DeathMenu { get => deathMenu; set => deathMenu = value; }
    public GameObject SettingsPanel { get => settingsPanel; set => settingsPanel = value; }
    public GameObject AbilityWindow { get => abilityWindow; set => abilityWindow = value; }
    public TextMeshProUGUI AbilityDescription { get => abilityDescription; set => abilityDescription = value; }
    public DialogueBox DialogueBox { get => dialogueBox; set => dialogueBox = value; }

    private void Awake()
    {
        if(instance !=this && instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;


    }

    private void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();    
    }
    private void Update()
    {
        if (playerController)
        {
            UpdateHealthUI(playerController);
            UpdateAttonementUI(playerController);
        }
    }

    private void UpdateHealthUI(PlayerController playerController)
    {
        float ratio = (float)playerController.PlayerInfo.CurrentHealth / playerController.PlayerInfo.MaxHealth;
        healthBar.localScale = new Vector3(ratio, 1, 1);

        healthText.text = playerController.PlayerInfo.CurrentHealth.ToString() + "/" + playerController.PlayerInfo.MaxHealth.ToString();
    }

    private void UpdateAttonementUI(PlayerController playerController)
    {

        float ratio = (float)playerController.PlayerInfo.CurrentAtonement / playerController.PlayerInfo.AtonementToLevel;
        atonementBar.localScale = new Vector3(ratio, 1, 1);
        atonementLvlText.text = playerController.PlayerInfo.AtonementLvl.ToString();
    }

    public void OpenDeathMenu()
    {
        deathMenu.SetActive(true);
    }

    public void CloseDeathMenu()
    {
        deathMenu.SetActive(false);
    }

    public void OnReplayButtonClick()
    {
        CloseDeathMenu();
        SceneManager.LoadScene("Cemetery");
        //MobManager.Instance.ResetLookingForPlayersForAllEnemies();
        //Player.GetComponent<PlayerController>().PlayerDeathState.OnPlayerRespawn();


    }

    public void OnMainmenuButtonClick()
    {
        OnGameUnPause();
        AudioManager.Instance.SaveSoundData();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnExitButtonClick()
    {

        AudioManager.Instance.SaveSoundData();
        Application.Quit();
    }

    public void OnGamePause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        InputManager.Instance.OnDisable();
    }

    public void OnGameUnPause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        InputManager.Instance.OnEnable();
    }

    public void OnOpenSettingsMenu()
    {
        settingsPanel.SetActive(true);


    }

    public void OnCloseSettingsMenu()
    {
        settingsPanel.SetActive(false);
    }
}
