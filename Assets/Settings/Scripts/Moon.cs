using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    GameObject mainCamera;
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    
    void Update()
    {
        this.transform.position = new Vector3(mainCamera.transform.position.x, this.transform.position.y,this.transform.position.z);
    }
}
