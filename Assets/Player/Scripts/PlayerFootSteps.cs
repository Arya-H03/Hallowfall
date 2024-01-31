using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] GameObject runEffect;
    private ParticleSystem footstepPS;

    private void Awake()
    {
        footstepPS = GetComponent<ParticleSystem>();
    }
    public void OnStartPlayerFootstep()
    {
        footstepAudioSource.Play();
        footstepPS.Play();
        //runEffect.SetActive(true);
    }

    public void OnEndPlayerFootstep()
    {
        footstepAudioSource.Stop();
        footstepPS.Stop();
        //runEffect.SetActive(true);
    }
}
