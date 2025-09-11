using System.Collections;
using TMPro;
using UnityEngine;

public class BossTimer : MonoBehaviour
{
    private TextMeshProUGUI CText;

    private void Awake()
    {
        CText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void StartBossTimer(float timer)
    {
        StartCoroutine(BossTimerCoroutine(timer));
    }
    private IEnumerator BossTimerCoroutine(float timer)
    {
        
        while(timer > 0 )
        {
            int minutes = (int)(timer / 60);
            int seconds = (int)(timer % 60);

            // Format with leading zeros
            string minText = minutes.ToString("00");
            string secText = seconds.ToString("00");

            CText.text = $"{minText}:{secText}";

            timer -= Time.deltaTime;
            yield return null;
        }
        CText.text = "00:00";
    }
}
