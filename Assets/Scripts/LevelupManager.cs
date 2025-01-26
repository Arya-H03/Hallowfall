using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelupManager : MonoBehaviour
{
    private static LevelupManager instance;
    private PlayerController playerController;

    [SerializeField] GameObject abilityWindow;
    [SerializeField] GameObject abilityCard1; //left
    [SerializeField] GameObject abilityCard2; //Middle
    [SerializeField] GameObject abilityCard3; //right
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
        DontDestroyOnLoad(gameObject);

       
        
    }
    

    private void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        if (!playerController)
        {
            Debug.LogWarning("Levelup manager doesn't have ref to player controller");
        }
        PlayerDeathState.PlayerRespawnEvent += ResetAttonement;
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


        float ratio = (float)playerController.PlayerInfo.CurrentAtonement / playerController.PlayerInfo.AtonementToLevel;
        GameManager.Instance.atonementBar.localScale = new Vector3(ratio, 1, 1);
    }

    public void ResetAttonement()
    {
        playerController.PlayerInfo.CurrentAtonement = 0;
        playerController.PlayerInfo.AtonementToLevel = 0;
        playerController.PlayerInfo.AtonementToLevel = 3;

        float ratio = (float)playerController.PlayerInfo.CurrentAtonement / playerController.PlayerInfo.AtonementToLevel;
        GameManager.Instance.atonementBar.localScale = new Vector3(ratio, 1, 1);
        GameManager.Instance.atonementLvlText.GetComponent<TextMeshProUGUI>().text = 0.ToString();
    }

    private void OnLevelUp()
    {
        playerController.PlayerInfo.AtonementLvl++;
        playerController.PlayerInfo.CurrentAtonement = 0;
        playerController.PlayerInfo.AtonementToLevel += 2;

        GameManager.Instance.atonementLvlText.GetComponent<TextMeshProUGUI>().text = playerController.PlayerInfo.AtonementLvl.ToString();

        abilityWindow.SetActive(true);
        InputManager.Instance.OnDisable();
        Time.timeScale = 0;
    }
}
