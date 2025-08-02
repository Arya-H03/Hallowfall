using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCameraHandler : MonoBehaviour
{
    private static PlayerCameraHandler instance;

    public static PlayerCameraHandler Instance
    {
        get { return instance; }
    }

  

    private GameObject player;

    private Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    private float followSpeed = 2.5f;

    private PlayerController playerController;

    public Volume Volume { get => volume;}
    public Vignette Vignette { get => vignette;}
    public ChromaticAberration ChromaticAberration { get => chromaticAberration; }
    public ColorAdjustments ColorAdjustments { get => colorAdjustments; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out colorAdjustments);
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        playerController = GameManager.Instance.PlayerController;
    }

    void Update()
    {
        if (player && !playerController.IsDead)
        {

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

            transform.position = Vector3.Lerp(currentPosition, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void OnPlayerDistorted( )
    {
        ChromaticAberration.intensity.Override(1);
        Vignette.intensity.Override(0.75f);
    }

    public void OnPlayerEndDistorted()
    {
        ChromaticAberration.intensity.Override(0);
        Vignette.intensity.Override(0);
    }
}
