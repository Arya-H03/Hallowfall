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

    public Action<BaseSkillSO> OnSkillChosen;
    private BaseSkillSO selectedSkillOption;
    private Material material;

    private bool isSkillSelected = false;
    private bool isStatueUnlocked = false;

    public BaseSkillSO SelectedSkillOption { get => selectedSkillOption; set => selectedSkillOption = value; }

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

    private void HandleChoosingSkill(BaseSkillSO skill)
    {
        selectedSkillOption = skill;
        skillSelectBtn.SetActive(true);
    }
    public void OnSelectSkillBtnClicked()
    {
        if (!selectedSkillOption) return;
        isSkillSelected = true;
  
        PlayerSkillManager.Instance.UnlockPlayerSkill(selectedSkillOption);

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
