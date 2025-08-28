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


    private PlayerController playerController;
    private GameObject player;

    private Coroutine currentCameraShakeCoroutine;

    private bool isShaking = false;

    private Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    private float followSpeed = 2.5f;


    public Volume Volume { get => volume; }
    public Vignette Vignette { get => vignette; }
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

        playerController.PlayerSignalHub.OnCameraShake += ShakeCamera;
        playerController.PlayerSignalHub.OnVignetteFlash += FlashVignette;
    }

    private void OnDisable()
    {
        playerController.PlayerSignalHub.OnCameraShake -= ShakeCamera;
        playerController.PlayerSignalHub.OnVignetteFlash -= FlashVignette;
    }
    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (!player || playerController.IsDead || isShaking) return;

        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
    private IEnumerator ShakeCameraCoroutine(float duration, float magnitude)
    {
        Vector3 posBeforeShake = transform.position;
        float timer = 0f;
        isShaking = true;

        float seedX = Random.Range(0f, 100f);
        float seedY = Random.Range(0f, 100f);

        float frequency = 20f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float time = timer * frequency;

            float x = (Mathf.PerlinNoise(seedX, time) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(seedY, time) - 0.5f) * 2f;

            transform.position = posBeforeShake + new Vector3(x, y, 0) * magnitude;
            yield return null;
        }
        ResetCameraAfterShake();
    }

    private void ResetCameraAfterShake()
    {
        isShaking = false;
    }
    private void ShakeCamera(float duration, float magnitude)
    {
        if (currentCameraShakeCoroutine != null)
        {
            StopCoroutine(currentCameraShakeCoroutine);
            ResetCameraAfterShake();
        }

        currentCameraShakeCoroutine = StartCoroutine(ShakeCameraCoroutine(duration, magnitude));
    }

    private void FlashVignette(float duration, float intensity, Color newColor)
    {
        StartCoroutine(FlashVignetteCoroutine(duration, intensity, newColor));
    }
    private IEnumerator FlashVignetteCoroutine(float duration,float intensity,Color newColor)
    {
        float timer = 0;

        while(timer < duration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            vignette.intensity.Override(Mathf.Lerp(0, intensity, t));
            vignette.color.Override(Color.Lerp(Color.clear, newColor, t));
            yield return null;
        }
        vignette.intensity.Override(intensity);
        vignette.color.Override(newColor);


        timer = 0;

        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            vignette.intensity.Override(Mathf.Lerp(intensity, 0, t));
            vignette.color.Override(Color.Lerp(newColor, Color.clear, t));
            yield return null;
        }
        vignette.color.Override(Color.clear);
        vignette.intensity.Override(0f);
    }

    public void OnPlayerDistorted()
    {
        ChromaticAberration.intensity.Override(1);
        Vignette.intensity.Override(0.75f);
    }

    public void OnPlayerEndDistorted()
    {
        ChromaticAberration.intensity.Override(0);
        Vignette.intensity.Override(0);
    }

    public Vector3 GetRandomOffScreenPos(float offSet)
    {
        Vector3 result = new();
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        float minX = bottomLeft.x;
        float maxX = topRight.x;

        float minY = bottomLeft.y;
        float maxY = topRight.y;

        int side = Random.Range(1, 5); //1 top, 2 right, 3 left, 4 bottom

        switch (side)
        {
            case 1:
                result = new Vector3(Random.Range(minX,maxX),maxY + offSet, 0);
                break;
            case 2:
                result = new Vector3(maxX + offSet,Random.Range(minY,maxY), 0);
                break;
            case 3:
                result = new Vector3(minX - offSet, Random.Range(minY, maxY), 0);
                break;
            case 4:
                result = new Vector3(Random.Range(minX, maxX), minY - offSet, 0);
                break;
        }

        return result;
    }
}
