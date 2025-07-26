using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
   

    private float runSpeed = 0;

    private AudioClip[] groundRunSFX;
    private AudioClip[] grassRunSFX;
    private AudioClip[] stoneRunSFX;


    public float RunSpeed { get => runSpeed; set => runSpeed = value; }

    public PlayerRunState()
    {
        this.stateEnum = PlayerStateEnum.Run;
    }

    public override void InitState(PlayerConfig config)
    {
        runSpeed = config.runSpeed;
        groundRunSFX = config.groundSFX;
        grassRunSFX = config.grassSFX;
        stoneRunSFX = config.stoneSFX;
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

    public void PlayStepSound()
    {
 
        switch (playerController.CurrentFloorType)
        {
            case FloorTypeEnum.Ground:
                AudioManager.Instance.PlaySFX(groundRunSFX,playerController.transform.position, 0.1f);
                break;
            case FloorTypeEnum.Grass:
                AudioManager.Instance.PlaySFX(grassRunSFX, playerController.transform.position, 0.1f);
                break;
            case FloorTypeEnum.Stone:
                AudioManager.Instance.PlaySFX(stoneRunSFX, playerController.transform.position, 0.1f);
                break;
        }
    }

    private void StartRunning()
    {
        playerController.PlayerMovementHandler.MoveSpeed = RunSpeed;
        playerController.AnimationController.SetBoolForAnimations("isRunning", true);
        //StartRunningSFX();
        //playerFootSteps.OnStartPlayerFootstep();

    }

    private void StopRunning()
    {
        playerController.AnimationController.SetBoolForAnimations("isRunning", false);
        //StopRunningSFX();
        //playerFootSteps.OnEndPlayerFootstep();

    }

   
    public void StartRunningSFX()
    {
        //switch (playerController.CurrentFloorType)
        //{
        //    case FloorTypeEnum.Ground:
        //        AudioManager.Instance.PlaySFX(audioSource, groundRunSFX);
        //        break;
        //    case FloorTypeEnum.Grass:
        //        AudioManager.Instance.PlaySFX(audioSource, grassRunSFX);
        //        break;
        //    case FloorTypeEnum.Stone:
        //        AudioManager.Instance.PlaySFX(audioSource, woodRunSFX);
        //        break;
        //}

    }

    public void StopRunningSFX()
    {
        //AudioManager.Instance.StopAudioSource(audioSource);
    }

    public void PauseRunningSFX()
    {
        //if (audioSource.isPlaying)
        //{
        //    audioSource.Pause();
            
        //}
    }

    public void ResumeRunningSFX()
    {
        //if (!audioSource.isPlaying)
        //{
        //    audioSource.UnPause();
            
        //}
    }
}
