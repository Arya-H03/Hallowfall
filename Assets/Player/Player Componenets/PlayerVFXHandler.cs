using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVFXHandler : MonoBehaviour, IInitializeable<PlayerController>
{
    SpriteRenderer spriteRenderer;
    private GameObject imagePrefab;
    private float imageLifeTime;
    private PlayerController playerController;
    private Material playerMat;

    int flashID = Shader.PropertyToID("_Flash");
    int dissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    private Coroutine afterImageCoroutine;
    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        spriteRenderer = playerController.SpriteRenderer;
        imagePrefab = playerController.PlayerConfig.afterImagePefab;
        imageLifeTime = playerController.PlayerConfig.afterImageLifeTime;
        playerMat = spriteRenderer.material;

        playerController.PlayerSignalHub.OnAfterImageStart += StartAfterImageEffect;
        playerController.PlayerSignalHub.OnAfterImageStop += StopAfterImageEffect;
        playerController.PlayerSignalHub.OnSpawnVFX += SpawnVFX;
        playerController.PlayerSignalHub.OnSpawnScaledVFX += SpawnVFX;
        playerController.PlayerSignalHub.OnMaterialFlash += FlashMaterial;
        playerController.PlayerSignalHub.OnDissolveEffect += DissolveEffect;
        playerController.PlayerSignalHub.OnScaleEffect += ScaleEffect;
        playerController.PlayerSignalHub.RequestSpawnedVFX += ReturnSpawnedVFX;
    }

    private void OnDisable()
    {
        playerController.PlayerSignalHub.OnAfterImageStart -= StartAfterImageEffect;
        playerController.PlayerSignalHub.OnAfterImageStop -= StopAfterImageEffect;
        playerController.PlayerSignalHub.OnSpawnVFX -= SpawnVFX;
        playerController.PlayerSignalHub.OnSpawnScaledVFX -= SpawnVFX;
        playerController.PlayerSignalHub.OnMaterialFlash -= FlashMaterial;
        playerController.PlayerSignalHub.OnDissolveEffect -= DissolveEffect;
        playerController.PlayerSignalHub.OnScaleEffect -= ScaleEffect;
        playerController.PlayerSignalHub.RequestSpawnedVFX -= ReturnSpawnedVFX;

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

    private void SpawnVFX(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {
        GameObject obj = Instantiate(prefab, position, rotation);
        Destroy(obj, lifetime + 0.05f);
    }

    private GameObject ReturnSpawnedVFX(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime)
    {
        GameObject obj = Instantiate(prefab, position, rotation);
        Destroy(obj, lifetime + 0.05f);
        return obj;
    }

    private void SpawnVFX(GameObject prefab, Vector3 position, Quaternion rotation, float lifetime, Vector3 scale)
    {
        GameObject obj = Instantiate(prefab, position, rotation);
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
            playerMat.SetFloat(flashID, value);
          
            yield return null;
        }
        playerMat.SetFloat(flashID, 2);

        timer = 0;

        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float value = Mathf.Lerp(2, 0, t);
            playerMat.SetFloat(flashID, value);
            yield return null;
        }
        playerMat.SetFloat(flashID, 0);
    }
    private void DissolveEffect(GameObject effect, float duration)
    {
        StartCoroutine(DissolveEffectCoroutine(effect,duration));
    }
    private IEnumerator DissolveEffectCoroutine(GameObject effect, float duration)
    {
        Material effectMat = effect.GetComponent<SpriteRenderer>().material;
        float timer = 0;
        while(timer < duration)
        {
            if (effect == null) yield break;
            timer += Time.deltaTime;
            float t = timer / duration;
            float value = Mathf.Lerp(0, 1, t);
            effectMat.SetFloat(dissolveAmountID, value);

            yield return null;
        }
        if (effect == null) yield break;
        effectMat.SetFloat(dissolveAmountID, 1.1f);

    }
    private void ScaleEffect(GameObject effect, Vector3 targetScale, float duration)
    {
        StartCoroutine(ScaleEffectCoroutine(effect, targetScale, duration));
    }
    private IEnumerator ScaleEffectCoroutine(GameObject effect, Vector3 targetScale, float duration)
    {
        float timer = 0;
        Vector3 startScale = effect.transform.localScale;

        while(timer < duration)
        {
            if (effect == null) yield break;
            timer += Time.deltaTime;
            float t = timer / duration;
            effect.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        if (effect == null) yield break;
        effect.transform.localScale = targetScale;

    }
}
