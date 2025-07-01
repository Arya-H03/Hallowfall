using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;


    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
            }
            return instance;
        }
    }

  
    public GameObject Player { get => playerGO; set => playerGO = value; }
    public int PlayerScore { get => playerScore; set => playerScore = value; }
    public int PlayerSkullCount { get => playerSkullCount; set => playerSkullCount = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }

    public Dictionary<int, SkillSO> skillsDictonary;
    [SerializeField] SkillSO [] skillSORefs;

    [SerializeField] string playerWakeUpDialoge;

    

    [SerializeField]  GameObject rbound;
    [SerializeField]  GameObject lBound;

    [SerializeField]  GameObject enemy;

    private int playerScore = 0;
    private int playerSkullCount = 0;

    [SerializeField] GameObject playerGO;
    private PlayerController playerController;

   

    private GameObject playerCamera;

    public delegate void MyFunction();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
     

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        PlayerController = playerGO.GetComponent<PlayerController>();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        AudioManager.Instance.LoadSoundData();
        playerSkullCount = SaveSystem.LoadGameData().skullCount;
        UIManager.Instance.UpdatePlayerSkullText(PlayerSkullCount);
        //StartCoroutine(OnGameStartDialogue());

        

    }

    public void InitSkillsFromSkillTree()
    {
        skillsDictonary = new Dictionary<int, SkillSO>();
        foreach (SkillSO so in skillSORefs)
        {
            skillsDictonary.Add(so.id, so);
        }

        int [] skillindices = SaveSystem.LoadGameData().skillTreeNodes;
        for (int i = 0; i < skillindices.Length; i++)
        {
            if(skillindices[i] == 1)
            {
                skillsDictonary[i].ApplySkill(PlayerController);
            }
        }
    }
    public void AddToPlayerScore(int value)
    {
        playerScore += value;
        UIManager.Instance.UpdatePlayerScoreText(playerScore);
    }

    public void AddToPlayerSkulls(int value)
    {
        PlayerSkullCount += value;
        UIManager.Instance.UpdatePlayerSkullText(PlayerSkullCount);
    }
    
    public void StopTime(float duration)
    {
        StartCoroutine(HitStopCoroutine(duration));
    }
    
    private IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }



    IEnumerator OnGameStartDialogue()
    {
        InputManager.Instance.OnDisable();
        DistortCamera();
        yield return new WaitForSeconds(2f);
        UIManager.Instance.DialogueBox.StartDialogue("Find the Statue",10f);
        yield return new WaitForSeconds(1f);
        EndPlayerDistortion();
        InputManager.Instance.OnEnable();

    }

    public void DistortCamera()
    {
        playerCamera.GetComponent<PlayerCamera>().OnPlayerDistorted();
    }

   
    public void EndPlayerDistortion()
    {
        playerCamera.GetComponent<PlayerCamera>().OnPlayerEndDistorted();
        
    }
}
