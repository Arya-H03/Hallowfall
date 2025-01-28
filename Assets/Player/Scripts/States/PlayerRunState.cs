using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
   

    [SerializeField] private float runSpeed = 3.5f;

    private AudioSource audioSource;
    [SerializeField] private AudioClip groundRunSFX;
    [SerializeField] private AudioClip grassRunSFX;
    [SerializeField] private AudioClip woodRunSFX;

    public float RunSpeed { get => runSpeed; set => runSpeed = value; }

    public PlayerRunState()
    {
        this.stateEnum = PlayerStateEnum.Run;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
   
    public override void OnEnterState()
    {
        
        StartRunning();
    }

    public override void OnExitState()
    {
        StopRunning();
    }

    public override void HandleState()
    {


    }

    private void StartRunning()
    {
        playerController.PlayerMovementManager.MoveSpeed = RunSpeed;
        playerController.AnimationController.SetBoolForAnimations("isRunning", true);
        StartRunningSFX();
        //playerFootSteps.OnStartPlayerFootstep();

    }

    private void StopRunning()
    {
        playerController.AnimationController.SetBoolForAnimations("isRunning", false);
        StopRunningSFX();
        //playerFootSteps.OnEndPlayerFootstep();

    }

   
    public void StartRunningSFX()
    {
        switch (playerController.CurrentFloorType)
        {
            case FloorTypeEnum.Ground:
                AudioManager.Instance.PlaySFX(audioSource, groundRunSFX);
                break;
            case FloorTypeEnum.Grass:
                AudioManager.Instance.PlaySFX(audioSource, grassRunSFX);
                break;
            case FloorTypeEnum.Wood:
                AudioManager.Instance.PlaySFX(audioSource, woodRunSFX);
                break;
        }
        
    }

    public void StopRunningSFX()
    {
        AudioManager.Instance.StopAudioSource(audioSource);
    }

    public void PauseRunningSFX()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            
        }
    }

    public void ResumeRunningSFX()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.UnPause();
            
        }
    }
}
