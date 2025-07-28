using System.Collections;
using UnityEngine;

public class CCoroutineRunner : MonoBehaviour
{
    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void EndCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
}
