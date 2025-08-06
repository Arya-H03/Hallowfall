using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class PlayerVFXHandler : MonoBehaviour, IInitializeable<PlayerController>
{
    SpriteRenderer spriteRenderer;
    private GameObject imagePrefab;
    private float imageLifeTime;
    private PlayerController playerController;
    private Material material;

    int flashID = Shader.PropertyToID("_Flash");
    private Coroutine afterImageCoroutine;
    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        spriteRenderer = playerController.SpriteRenderer;
        imagePrefab = playerController.PlayerConfig.afterImagePefab;
        imageLifeTime = playerController.PlayerConfig.afterImageLifeTime;
        material = spriteRenderer.material;

        playerController.PlayerSignalHub.OnAfterImageStart += StartAfterImageEffect;
        playerController.PlayerSignalHub.OnAfterImageStop += StopAfterImageEffect;
        playerController.PlayerSignalHub.OnSpawnVFX += SpawnVFX;
        playerController.PlayerSignalHub.OnSpawnScaledVFX += SpawnVFX;
        playerController.PlayerSignalHub.OnMaterialFlash += FlashMaterial;
    }

    private void OnDisable()
    {
        playerController.PlayerSignalHub.OnAfterImageStart -= StartAfterImageEffect;
        playerController.PlayerSignalHub.OnAfterImageStop -= StopAfterImageEffect;
        playerController.PlayerSignalHub.OnSpawnVFX -= SpawnVFX;
        playerController.PlayerSignalHub.OnSpawnScaledVFX -= SpawnVFX;
        playerController.PlayerSignalHub.OnMaterialFlash -= FlashMaterial;
    }

    private void StartAfterImageEffect()
    {
        if (afterImageCoroutine == null) afterImageCoroutine = StartCoroutine(SpawnImages());

    }

    private void StopAfterImageEffect()
    {
        if (afterImageCoroutine != null)
        {
            StopCoroutine(afterImageCoroutine);
            afterImageCoroutine = null;
        }
    }
    public IEnumerator SpawnImages()
    {
        while (true)
        {
            GameObject afterImageGO = Instantiate(imagePrefab, playerController.GetPlayerPos(), Quaternion.identity);
            afterImageGO.GetComponent<AfterImage>().InitializeImage(spriteRenderer.sprite, spriteRenderer.flipX, imageLifeTime, spriteRenderer.color, this.transform.localScale.x);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SpawnVFX(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        Destroy(obj, lifetime);
    }

    public void SpawnVFX(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.localScale = scale;   
        Destroy(obj, lifetime);
    }

    private void FlashMaterial(float duration)
    {
        StartCoroutine(FlashMaterialCoroutine(duration));
    }
    private IEnumerator FlashMaterialCoroutine(float duration)
    {
        float timer = 0;

        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float value = Mathf.Lerp(0, 2, t);
            material.SetFloat("_Flash", value);
          
            yield return null;
        }
        material.SetFloat("_Flash", 2);
        timer = 0;

        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float value = Mathf.Lerp(2, 0, t);
            material.SetFloat("_Flash", value);
            yield return null;
        }
        material.SetFloat("_Flash", 0);
    }
}
