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

    public Statue LastStatue { get => lastStatue; set => lastStatue = value; }
    public GameObject Player { get => player; set => player = value; }
    public int PlayerScore { get => playerScore; set => playerScore = value; }
    public int PlayerSkullCount { get => playerSkullCount; set => playerSkullCount = value; }

    [SerializeField] string playerWakeUpDialoge;

    

    [SerializeField]  GameObject rbound;
    [SerializeField]  GameObject lBound;

    [SerializeField]  GameObject enemy;

    private int playerScore = 0;
    private int playerSkullCount = 0;

    [SerializeField] GameObject player;

    private Statue lastStatue;
    [SerializeField] Transform initialSpawnPoint;
    private Transform currentSpawnPoint;

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
        //DontDestroyOnLoad(gameObject);

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

    }

    private void Start()
    {
        AudioManager.Instance.LoadSoundData();

        LoadGameData();

        UIManager.Instance.UpdatePlayerSkullText(PlayerSkullCount);

        //StartCoroutine(OnGameStartDialogue());

    }

    public void LoadGameData()
    {
        GameData gameData = SaveSystem.LoadGameData();
        if(gameData != null)
        {
            playerSkullCount = gameData.skullCount;
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
    public void SetPlayerLocationOnRespawn()
    {
        if (lastStatue)
        {
            currentSpawnPoint = lastStatue.GetStatueRespawnPoint();
            
        }
        else
        {
            currentSpawnPoint = initialSpawnPoint;
        }

        player.transform.position = currentSpawnPoint.position;
    }

    

    public void StopTime(float duration)
    {
        StartCoroutine(HitStopCoroutine(duration));
    }
    
    private IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration); // Wait using real-time (ignores time scale)
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

    IEnumerator CallFunctionByDelay(MyFunction function, float sec)
    {
        yield return new WaitForSeconds(sec);
        function();
    }

    public void EndPlayerDistortion()
    {
        playerCamera.GetComponent<PlayerCamera>().OnPlayerEndDistorted();
        
    }
}
