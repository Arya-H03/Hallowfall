using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillStatue : MonoBehaviour, IInteractable
{

    [SerializeField] GameObject statue;

    [SerializeField] GameObject interactionIcon;
    [SerializeField] GameObject skillOptionsFrame;
    [SerializeField] GameObject skillDescriptionFrame;
    [SerializeField] GameObject skillSelectBtn;

    [SerializeField] TextMeshProUGUI skillDescriptionTextComp;
    [SerializeField] TextMeshProUGUI skillNameTextComp;

    [SerializeField] SkillOption option1;
    [SerializeField] SkillOption option2;
    [SerializeField] SkillOption option3;

    [SerializeField] Color defaultStatueOutlineColor;
    [SerializeField] Color interactedStatueOutlineColor;

    [SerializeField] AudioClip[] skillSelectionSFX;

    public Action<BaseSkillSO,SkillOption> OnSkillChosen;
    private BaseSkillSO selectedSkill;
    private SkillOption selectedOption;
    private Material material;

    private bool isSkillSelected = false;
    private bool isStatueUnlocked = false;

    public BaseSkillSO SelectedSkill { get => selectedSkill; set => selectedSkill = value; }
    public SkillOption SelectedOption { get => selectedOption; set => selectedOption = value; }

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
        material.SetColor("_OutlineColor", defaultStatueOutlineColor);
    }
    public void OnIntercationBegin()
    {
        if(!isStatueUnlocked)
        {
            interactionIcon.SetActive(true);
            PlayerInputHandler.Instance.InputActions.Guardian.Interact.performed += Interact;

        }
        else if(!isSkillSelected)
        {
            skillOptionsFrame.SetActive(true);
        }      
    }
    public void OnIntercationEnd()
    {
        if (!isStatueUnlocked)
        {
            interactionIcon.SetActive(false);
        }
        else if (!isSkillSelected)
        {
            DeactivateSkillFrame();
        }
      
    }
    public void Interact(InputAction.CallbackContext ctx)
    {
        isStatueUnlocked = true;
        interactionIcon.SetActive(false);

        skillOptionsFrame.SetActive(true);
        InitAllOptions();

        PlayerInputHandler.Instance.InputActions.Guardian.Interact.performed -= Interact;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
        BaseSkillSO [] skills = PlayerSkillManager.Instance.GetThreeRandomSkills();
        option1.Init(skills[0], this, skillDescriptionFrame, skillNameTextComp, skillDescriptionTextComp);
        option2.Init(skills[1], this, skillDescriptionFrame, skillNameTextComp, skillDescriptionTextComp);
        option3.Init(skills[2], this, skillDescriptionFrame, skillNameTextComp, skillDescriptionTextComp);
    }

    private void HandleChoosingSkill(BaseSkillSO skill,SkillOption option)
    {
        SkillOption lastSelectedOption = selectedOption;
        selectedOption = option;
        selectedSkill = skill;
        skillSelectBtn.SetActive(true);
        lastSelectedOption?.ResetHighlightOption();
    }
    public void OnSelectSkillBtnClicked()
    {
        if (!selectedSkill) return;
        isSkillSelected = true;
  
        PlayerSkillManager.Instance.UnlockPlayerSkill(selectedSkill);

        DeactivateSkillFrame();

        material.SetColor("_OutlineColor", interactedStatueOutlineColor);

        AudioManager.Instance.PlaySFX(skillSelectionSFX, this.transform.position, 1);
    }

    private void DeactivateSkillFrame()
    {
        skillOptionsFrame.SetActive(false);
        skillDescriptionFrame.SetActive(false);
        skillSelectBtn.SetActive(false);
    }
}
