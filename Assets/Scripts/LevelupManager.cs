using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelupManager : MonoBehaviour
{
    private static LevelupManager instance;
    private PlayerController playerController;

   
    [SerializeField] GameObject[] abilityCards;
    

    public List<BaseAbility> abilities;
    public List<BaseAbility> abilitiesToAssign;
    public static LevelupManager Instance
    {
        get
        {   if (!instance)
            {
                GameObject go = new GameObject("LevelupManager");
                go.AddComponent<LevelupManager>();

            } 
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);

       
        
    }
    

    private void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        if (!playerController)
        {
            Debug.LogWarning("Levelup manager doesn't have ref to player controller");
        }
        PlayerDeathState.PlayerRespawnEvent += ResetAttonement;

        abilitiesToAssign = new List<BaseAbility>(abilities);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            OnAtonementPickUp();
        }
    }

    public void OnAtonementPickUp()
    {
        playerController.PlayerInfo.CurrentAtonement++;
        if (playerController.PlayerInfo.CurrentAtonement >= playerController.PlayerInfo.AtonementToLevel)
        {
            OnLevelUp();
        }

    }

    public void ResetAttonement()
    {
        playerController.PlayerInfo.CurrentAtonement = 0;
        playerController.PlayerInfo.AtonementToLevel = 0;
        playerController.PlayerInfo.AtonementToLevel = 3;

    }

    private void OnLevelUp()
    {
        playerController.PlayerInfo.AtonementLvl++;
        playerController.PlayerInfo.CurrentAtonement = 0;
        playerController.PlayerInfo.AtonementToLevel += 2;
        playerController.PlayerInfo.CurrentHealth = playerController.PlayerInfo.MaxHealth;
        float ratio = (float)playerController.PlayerInfo.CurrentHealth / playerController.PlayerInfo.MaxHealth;
       
        FillAbilityCards();
        UIManager.Instance.AbilityWindow.SetActive(true);
        InputManager.Instance.OnDisable();
        GameManager.Instance.Player.GetComponentInChildren<PlayerRunState>().PauseRunningSFX();
        Time.timeScale = 0;
    }

    private void CloseAbilityWindow(AbilityCard abilitycard)
    {
        abilitiesToAssign = new List<BaseAbility>(abilities);
        InputManager.Instance.OnEnable();
        Time.timeScale = 1;
        GameManager.Instance.Player.GetComponentInChildren<PlayerRunState>().ResumeRunningSFX();
        abilitycard.ApplyAbilityEvent = null;
        UIManager.Instance.AbilityWindow.SetActive(false);
    }
    private void FillAbilityCards()
    {
        foreach (var card in abilityCards)
        {
            int index = Random.Range(0, abilitiesToAssign.Count);
            BaseAbility ability = abilitiesToAssign[index];
            abilitiesToAssign.Remove(ability);

            AbilityCard abilityCard = card.GetComponent<AbilityCard>();
            abilityCard.cardIcon.sprite = ability.icon;
            abilityCard.cardName.text = ability.abilityName;
            abilityCard.CardDescription = ability.description;
            abilityCard.ApplyAbilityEvent += ability.ApplyAbility;
            abilityCard.ApplyAbilityEvent += () => CloseAbilityWindow(abilityCard);



        }
    }
}
