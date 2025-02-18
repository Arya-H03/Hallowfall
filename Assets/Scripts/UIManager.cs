using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] Transform healthBar;
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] Transform atonementBar;
    [SerializeField] TextMeshProUGUI atonementLvlText;

    [SerializeField] TextMeshProUGUI abilityDescription;

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
        dictionaryOfAbilitySlots = new Dictionary<BaseAbility, GameObject> ();
        dictionaryOfAbilityLvls = new Dictionary<BaseAbility, int> ();

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
        float ratio = (float)playerController.CurrentHealth / playerController.MaxHealth;
        healthBar.localScale = new Vector3(ratio, 1, 1);

        healthText.text = playerController.CurrentHealth.ToString() + "/" + playerController.MaxHealth.ToString();
    }

    private void UpdateAttonementUI(PlayerController playerController)
    {

        float ratio = (float)playerController.CurrentAtonement / playerController.AtonementToLevel;
        atonementBar.localScale = new Vector3(ratio, 1, 1);
        atonementLvlText.text = playerController.AtonementLvl.ToString();
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
        SceneManager.LoadScene("Game");
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
