using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    [SerializeField] GameObject dialogeBox;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] string playerWakeUpDialoge;

    [SerializeField] InputManager inputManager;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemy;
    [SerializeField] Transform enemySpawnTransform;


    private GameObject playerCamera;

    public delegate void MyFunction();


    private void Awake()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        
    }

    private void OnEnable()
    {
        Player.PlayerRespawnEvent += SpawnEnemy;
    }
    private void Start()
    {

        //OnPlayerWakeUp();
        //MyFunction func = EndPlayerDistortion;
        //StartCoroutine(CallFunctionByDelay(func, 4));

        //SpawnEnemy();

    }

    private void SpawnEnemy()
    {
        if (enemy)
        {
            Destroy(enemy);
        }
        enemy = Instantiate(enemyPrefab, enemySpawnTransform.position, Quaternion.identity);
    }

  

    public void PlayAudio(AudioSource source, AudioClip clip)
    {
        source.volume = 0.5f;
        source.PlayOneShot(clip);   
    }
    public void OnReplayButtonClick()
    {
        SceneManager.LoadScene("RealmBeyond");
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
        inputManager.OnDisable();
    }

    public void OnGameUnPause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        inputManager.OnEnable();
    }
    private void CreateUpDialogeBox(string text)
    {
        //GameObject obj = Instantiate(dialogeBox, Vector3.zero, Quaternion.identity);
        //obj.transform.SetParent(canvas.transform, false);
        //obj.transform.position = new Vector3(220, 75, 0);
        dialogeBox.SetActive(true);
        dialogeBox.GetComponent<Dialoge>().StartDialoge(text);

        //obj.GetComponent<Dialoge>().text[0] = text;
        
    }   

    IEnumerator CallDialoge(float sec,string text)
    {
        yield return new WaitForSeconds(sec);
        CreateUpDialogeBox(text);
    }

    private void OnPlayerWakeUp()
    {
        playerCamera.GetComponent<PlayerCamera>().OnPlayerDistorted();

       
    }

    IEnumerator CallFunctionByDelay(MyFunction function,float sec)
    {
       yield return new WaitForSeconds(sec);
        function();
    }

    private void EndPlayerDistortion()
    {
        playerCamera.GetComponent<PlayerCamera>().OnPlayerEndDistorted();
        StartCoroutine(CallDialoge(2, playerWakeUpDialoge));
    }

    
}
