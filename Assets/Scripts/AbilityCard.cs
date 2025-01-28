using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCard : MonoBehaviour
{
    public Image cardIcon;
    public TextMeshProUGUI cardName;
    private string cardDescription;

    public delegate void EventHandler();
    public EventHandler ApplyAbilityEvent;

    public string CardDescription { get => cardDescription; set => cardDescription = value; }

    public void OnCardClicked()
    {
        ApplyAbilityEvent?.Invoke();
    }

    public void OnCardHover()
    {
        GameManager.Instance.abilityDescription.text = cardDescription;
    }

    public void OnCardHoverClear()
    {
        GameManager.Instance.abilityDescription.text = "";
    }
}
