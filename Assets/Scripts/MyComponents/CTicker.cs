using System;
using UnityEngine;

public class CTicker : MonoBehaviour
{
    [SerializeField] private float tickInterval = 0.2f;
    private float tickTimer = 0;

    private bool canTick = false;

    public event Action OnTickEvent;

    public bool CanTick { get => canTick; set => canTick = value; }

   

    private void Update()
    {
        if(!canTick) return;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {   
            tickTimer -=tickInterval;
            OnTickEvent?.Invoke();
        }
        
    }

    public void TickNow()
    {
        OnTickEvent?.Invoke();
    }
}
