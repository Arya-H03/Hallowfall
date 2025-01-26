using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] public Transform atonementBar;

   

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


        DontDestroyOnLoad(gameObject);

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<DialogueBox>();

    }

    private void Start()
    {
        AudioManager.Instance.LoadSoundData();
        //StartCoroutine(OnGameStartDialogue());

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
        MobManager.Instance.ResetLookingForPlayersForAllEnemies();
        Player.GetComponent<PlayerController>().PlayerDeathState.OnPlayerRespawn();
        
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
        dialogueBox.StartDialogue("Find your way forward",5f);
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
