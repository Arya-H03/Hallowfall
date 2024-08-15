using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject dialogeBox;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject deathMenu;
    [SerializeField] string playerWakeUpDialoge;
   

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
    }

    private void Start()
    {
        
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

    public void PlayAudio(AudioSource source, AudioClip clip)
    {
        source.volume = 0.5f;
        source.PlayOneShot(clip);
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
        Player.GetComponent<PlayerController>().PlayerDeathState.OnPlayerRespawn();
        
    }

    public void OnMainmenuButtonClick()
    {
        OnGameUnPause();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnExitButtonClick()
    {
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

    private void CreateUpDialogeBox(string text)
    {
        dialogeBox.SetActive(true);
        dialogeBox.GetComponent<Dialoge>().StartDialoge(text);
    }

    IEnumerator CallDialoge(float sec, string text)
    {
        yield return new WaitForSeconds(sec);
        CreateUpDialogeBox(text);
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
