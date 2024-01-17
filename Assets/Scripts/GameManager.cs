using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    [SerializeField] GameObject dialogeBox;

    [SerializeField] string playerWakeUpDialoge;

    private GameObject playerCamera;

    public delegate void MyFunction();


    private void Awake()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        
    }

    private void Start()
    {
       
        OnPlayerWakeUp();


        MyFunction func = EndPlayerDistortion;
        StartCoroutine(CallFunctionByDelay(func, 4));
   
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
