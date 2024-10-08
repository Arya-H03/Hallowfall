using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCamera : MonoBehaviour
{
    private GameObject player;
    private Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private float followSpeed = 2f;

    private PlayerController playerController;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();    
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chromaticAberration);
    }

    
    void Update()
    {
        if (player && !playerController.IsDead)
        {

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);

            transform.position = Vector3.Lerp(currentPosition, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void OnPlayerDistorted( )
    {
        chromaticAberration.intensity.Override(1);
        vignette.intensity.Override(0.75f);
    }

    public void OnPlayerEndDistorted()
    {
        chromaticAberration.intensity.Override(0);
        vignette.intensity.Override(0);
    }
}
