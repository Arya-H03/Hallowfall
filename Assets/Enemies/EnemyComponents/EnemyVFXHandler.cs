using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class EnemyVFXHandler : MonoBehaviour,IInitializeable<EnemyController>
{
    private EnemyController enemyController;
    private EnemyConfigSO enemyConfig;
    private EnemySignalHub signalHub;

    private Coroutine squashCoroutine;
    private Vector3 originalScale;

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        this.signalHub = enemyController.SignalHub;
        originalScale = enemyController.transform.localScale;
        signalHub.OnEnemyHit += HandleEnemySquash;

        signalHub.OnEnemyDamage += PlayBloodEffect;
        signalHub.OnEnemyDamage += SpawnDamagePopUp;

    }

    private void OnDisable()
    {
        signalHub.OnEnemyHit -= HandleEnemySquash;

        signalHub.OnEnemyDamage -= PlayBloodEffect;
        signalHub.OnEnemyDamage -= SpawnDamagePopUp;
    }

    private void HandleEnemySquash(float f, HitSfxType h)
    {
        int xDir = enemyController.transform.localScale.x < 0 ? -1 : 1; 
        if (squashCoroutine == null)
        {
            squashCoroutine = StartCoroutine(SquashEnemyCoroutine(0.5f));
        }
        else
        {
            StopCoroutine(squashCoroutine);
            enemyController.transform.localScale = new Vector3(originalScale.x * xDir, originalScale.y, originalScale.z);
            StartCoroutine(SquashEnemyCoroutine(0.5f));

        }
        enemyController.transform.localScale  = new Vector3(originalScale.x * xDir, originalScale.y,originalScale.z);
    }
    private IEnumerator SquashEnemyCoroutine(float duration)
    {
        float ySquish = Random.Range(-0.075f, 0.075f);
        float xSquish = -ySquish + Random.Range(-0.035f, 0.035f);
        float halfDuration = duration / 2f;
        float timer = 0;

        Vector3 originScale = transform.localScale;
        Vector3 squashedScale = originScale - new Vector3(xSquish, ySquish, 0);


        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.localScale = Vector3.Lerp(originScale, squashedScale, t);
            yield return null;
        }

        transform.localScale = originScale;


        timer = 0;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.localScale = Vector3.Lerp(squashedScale, originScale, t);
            yield return null;
        }
        transform.localScale = originScale;
    }
    private void PlayBloodEffect(float f)
    {
        Vector3 randPos = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
        GameObject go = Instantiate(enemyConfig.bloofVFXPrefabs[Random.Range(0, enemyConfig.bloofVFXPrefabs.Length)], enemyController.GetEnemyPos() + randPos, Quaternion.identity);
        Vector3 scale = go.transform.localScale;

        int randX = (int)MyUtils.GetRandomValue<int>(new int[] { -1, 1 });
        scale.x *= randX;
        go.transform.localScale = scale;
    }

    private void SpawnDamagePopUp(float damage)
    {
        var obj = Instantiate(enemyConfig.damagePopUpPrefab, transform.position + Vector3.up, Quaternion.identity);
        obj.SetText(damage.ToString());
    }
    //public IEnumerator EnemyHitCoroutine(float damageAmount, Vector2 hitPoint, HitSfxType hitType, float knockbackForce)
    //{     
    //    //material.SetFloat("_Flash", 1);   
    //    yield return new WaitForSeconds(0.1f);
    //    //material.SetFloat("_Flash", 0);
    //}
}
