using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillStatue : MonoBehaviour, IInteractable
{

    [SerializeField] GameObject statue;

    [SerializeField] GameObject skillOptionsFrame;
    [SerializeField] GameObject skillDescriptionFrame;
    [SerializeField] GameObject skillSelectBtn;

    [SerializeField] TextMeshProUGUI skillDescriptionTextComp;
    [SerializeField] TextMeshProUGUI skillNameTextComp;

    [SerializeField] SkillOption option1;
    [SerializeField] SkillOption option2;
    [SerializeField] SkillOption option3;

    [SerializeField] BaseSkillSO skill1;
    [SerializeField] BaseSkillSO skill2;
    [SerializeField] BaseSkillSO skill3;

    [SerializeField] AudioClip[] skillSelectionSFX; 

    public Action <BaseSkillSO> OnSkillChosen;
    private BaseSkillSO selectedSkillOption;
    private Material material;

    private bool isSkillSelected = false;

    public BaseSkillSO SelectedSkillOption { get=> selectedSkillOption; set => selectedSkillOption = value; }

    private void OnEnable()
    {
        OnSkillChosen += HandleChoosingSkill;
    }

    private void OnDisable()
    {
        OnSkillChosen -= HandleChoosingSkill;
    }
    private void Awake()
    {
        material = statue.GetComponent<SpriteRenderer>().material;
    }
    public void Interact(InputAction.CallbackContext ctx)
    {

    }

    public void OnIntercationBegin()
    {
        skillOptionsFrame.SetActive(true);
        InitAllOptions();
    }

    public void OnIntercationEnd()
    {
        skillOptionsFrame.SetActive(false);
        skillSelectBtn.SetActive(false);
        skillDescriptionFrame.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isSkillSelected)
        {
            OnIntercationBegin();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isSkillSelected)
        {
            OnIntercationEnd();
        }
    }

    private void InitAllOptions()
    {
        option1.Init(skill1, this, skillDescriptionFrame, skillNameTextComp,skillDescriptionTextComp);
        option2.Init(skill2, this, skillDescriptionFrame, skillNameTextComp, skillDescriptionTextComp);
        option3.Init(skill3, this, skillDescriptionFrame, skillNameTextComp, skillDescriptionTextComp);
    }

    private void HandleChoosingSkill(BaseSkillSO skill)
    {
        selectedSkillOption = skill;
        skillSelectBtn.SetActive(true);
    }
    public void OnSelectSkillBtnClicked()
    {
        if (!selectedSkillOption) return;
        isSkillSelected = true;
        skillOptionsFrame.SetActive(false);
        skillDescriptionFrame.SetActive(false);
        skillSelectBtn.SetActive(false);
        selectedSkillOption.Init(GameManager.Instance.PlayerController);
        material.SetFloat("_OutlineThickness", 0);
        AudioManager.Instance.PlaySFX(skillSelectionSFX, this.transform.position, 1);
    }
}
