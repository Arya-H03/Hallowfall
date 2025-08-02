using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField] DialogueBox dialogueBox;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject abilityWindow;

    [SerializeField] private Transform chargebar1;
    [SerializeField] private Transform chargebar2;

    [SerializeField] Transform healthBar;
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] Transform atonementBar;
    [SerializeField] TextMeshProUGUI atonementLvlText;

    [SerializeField] TextMeshProUGUI playerScoreText;

    [SerializeField] TextMeshProUGUI abilityDescription;

    [SerializeField] TextMeshProUGUI skullCountText;

    [SerializeField] TextMeshProUGUI fpsCountText;

    private PlayerController playerController;

    public List<GameObject> listOfFreeAbilitySlots;
    private Dictionary<BaseAbility, GameObject> dictionaryOfAbilitySlots ;
    private Dictionary<BaseAbility, int> dictionaryOfAbilityLvls ;


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

    public GameObject PauseMenu { get => pauseMenu; }
    public GameObject DeathMenu { get => deathMenu;  }
    public GameObject SettingsPanel { get => settingsPanel;  }
    public GameObject AbilityWindow { get => abilityWindow;  }
    public TextMeshProUGUI AbilityDescription { get => abilityDescription; }
    public DialogueBox DialogueBox { get => dialogueBox;  }
    public TextMeshProUGUI FpsCountText { get => fpsCountText; }
    public Transform HealthBar { get => healthBar;}
    public TextMeshProUGUI HealthText { get => healthText;}
    public Transform Chargebar1 { get => chargebar1;}
    public Transform Chargebar2 { get => chargebar2; }

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
        dictionaryOfAbilitySlots = new Dictionary<BaseAbility, GameObject> ();
        dictionaryOfAbilityLvls = new Dictionary<BaseAbility, int> ();

    }
    private void Update()
    {
        if (playerController)
        {
            UpdateEssenceUI(playerController);
        }
    }

    public void UpdatePlayerScoreText(int value)
    {
        playerScoreText.text = " Score: " + value.ToString();
    }

    public void UpdatePlayerSkullText(int value)
    {
        skullCountText.text = value.ToString();
    }

   

    private void UpdateEssenceUI(PlayerController playerController)
    {

        float ratio = (float)playerController.CurrentEssence / playerController.AtonementToLevel;
        atonementBar.localScale = new Vector3(ratio, 1, 1);
        atonementLvlText.text = playerController.AtonementLvl.ToString();
    }

    public void UpdateFPSTracker(int  value)
    {
        fpsCountText.text = value.ToString();
    }
    public void OpenDeathMenu()
    {
        SaveSystem.UpdatePlayerSkulls(GameManager.Instance.PlayerSkullCount);
        deathMenu.SetActive(true);
    }

    public void CloseDeathMenu()
    {
        deathMenu.SetActive(false);
    }

    public void OnReplayButtonClick()
    {
        CloseDeathMenu();
        SceneManager.LoadScene("Game");
        //MobManager.Instance.ResetLookingForPlayersForAllEnemies();
        //PlayerGO.GetComponent<PlayerController>().PlayerDeathState.OnPlayerRespawn();


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
        playerController.PlayerInputHandler.OnDisable();
        SaveSystem.UpdatePlayerSkulls(GameManager.Instance.PlayerSkullCount);
    }

    public void OnGameUnPause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        playerController.PlayerInputHandler.OnEnable();
    }

    public void OnOpenSettingsMenu()
    {
        settingsPanel.SetActive(true);


    }

    public void OnCloseSettingsMenu()
    {
        AudioManager.Instance.SaveSoundData();
        settingsPanel.SetActive(false);
    }

    public void AddAbilitySlot(BaseAbility ability)
    {
        if (!dictionaryOfAbilitySlots.ContainsKey(ability))
        {
            GameObject abilitySlot = listOfFreeAbilitySlots[0];
            dictionaryOfAbilitySlots.Add(ability, abilitySlot);
            

            listOfFreeAbilitySlots.Remove(abilitySlot);
            dictionaryOfAbilitySlots[ability].GetComponent<Image>().sprite = ability.icon;

            if (ability.canLevel)
            {
                dictionaryOfAbilityLvls.Add(ability, 1);
                dictionaryOfAbilitySlots[ability].GetComponentInChildren<TextMeshProUGUI>().text = "1";
            }

            dictionaryOfAbilitySlots[ability].SetActive(true);




        }
        else if(ability.canLevel)
        {
            dictionaryOfAbilityLvls[ability] += 1;
            dictionaryOfAbilitySlots[ability].GetComponentInChildren<TextMeshProUGUI>().text = dictionaryOfAbilityLvls[ability].ToString();
        }

    }

  

}
