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

    [SerializeField] Canvas canvas;

    private DialogueBox dialogueBox;
   
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] string playerWakeUpDialoge;

    [SerializeField] public Transform healthBar;
    [SerializeField] public TextMeshProUGUI healthText;
    [SerializeField] public TextMeshProUGUI abilityDescription;
    [SerializeField] public Transform atonementBar;
    [SerializeField] public GameObject atonementLvlText;

    [SerializeField]  GameObject rbound;
    [SerializeField]  GameObject lBound;

    [SerializeField]  GameObject enemy;

   

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
        dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<DialogueBox>();

    }

    private void Start()
    {
        AudioManager.Instance.LoadSoundData();
        StartCoroutine(OnGameStartDialogue());

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

    public IEnumerator SpawnEnemies()
    {
        int waveCount = 1; // Start from wave 1
        float firstWaveDelay = 3f;
        float subsequentWaveDelay = 10f;

        // Wait for the first wave
        yield return new WaitForSeconds(firstWaveDelay);

        while (true)
        {
            // Determine the number of enemies to spawn in this wave
            int spawnCount = GetSpawnCount(waveCount);

            // Spawn the wave
            SpawnWave(spawnCount);

            // Increment wave count
            waveCount++;

            // Wait for the next wave
            yield return new WaitForSeconds(subsequentWaveDelay);
        }
    }

    private int GetSpawnCount(int waveCount)
    {
        // Wave enemy counts: 1, 2, 4, 6, 8 (capped at 8)
        if (waveCount == 1) return 1;
        if (waveCount == 2) return 2;
        if (waveCount == 3) return 4;
        if (waveCount == 4) return 6;
        return 8; // Cap at 8 for waves 5+
    }

    private void SpawnWave(int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // Calculate a random spawn position within the bounds
            Vector2 spawnPoint = new Vector2(Random.Range(lBound.transform.position.x, rbound.transform.position.x), -3);

            // Optionally stagger enemy spawns within the wave
            StartCoroutine(SpawnEnemyWithDelay(spawnPoint, i * 0.2f));
        }
    }

    private IEnumerator SpawnEnemyWithDelay(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(enemy, position, Quaternion.identity);
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

    

    IEnumerator OnGameStartDialogue()
    {
        InputManager.Instance.OnDisable();
        DistortCamera();
        yield return new WaitForSeconds(2f);
        dialogueBox.StartDialogue("Find the Statue",10f);
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
