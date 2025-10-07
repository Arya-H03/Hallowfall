using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SkillOption : MonoBehaviour
{
    private SkillStatue skillStatue;
    private BaseSkillSO currentSkill;

    private GameObject descriptionFrame;
    private TextMeshProUGUI skillNameTextComp;
    private TextMeshProUGUI skillDescriptionTextComp;

    [SerializeField] GameObject iconGO;
    private Image optionImage;

    private Vector3 originalLocalScale;
    
    [SerializeField] float hightlightScale = 1.25f;
    private void Awake()
    {
        optionImage = iconGO.GetComponent<Image>();
        originalLocalScale = transform.localScale;
    }

    public void Init(BaseSkillSO skillSO,SkillStatue skillStatue,GameObject infoFrame, TextMeshProUGUI skillNameTextComp, TextMeshProUGUI skillDescriptionTextComp)
    {
        currentSkill = skillSO;
        this.skillStatue = skillStatue;
        optionImage.sprite = currentSkill.icon;
        this.descriptionFrame = infoFrame;
        this.skillNameTextComp = skillNameTextComp;
        this.skillDescriptionTextComp = skillDescriptionTextComp;

    }

    public void HighlightOption()
    {
        StartCoroutine(HighlightOptionCoroutine());
        descriptionFrame.SetActive(true);

        skillNameTextComp.text = currentSkill.abilityName;
        skillDescriptionTextComp.text = currentSkill.GetDescription();
    }

    public void ResetHighlightOption()
    {
        StartCoroutine(ResetHighlightOptionCoroutine());
        if (skillStatue.SelectedSkillOption == currentSkill) return;

        if (skillStatue.SelectedSkillOption != null && skillStatue.SelectedSkillOption != currentSkill)
        {
            skillNameTextComp.text = skillStatue.SelectedSkillOption.abilityName;
            skillDescriptionTextComp.text = skillStatue.SelectedSkillOption.GetDescription();
           
            return;
        }

        skillNameTextComp.text = "";
        skillDescriptionTextComp.text = "";

        descriptionFrame.SetActive(false);
    }

    private IEnumerator HighlightOptionCoroutine()
    {
        float timer = 0;
        Vector3 targetScale = this.transform.localScale * hightlightScale;
        while (timer < 0.25f)
        {
            float t = timer / 0.25f;
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale,t);
            timer += Time.deltaTime;
            yield return null;
        }
        this.transform.localScale = targetScale;
    }

    private IEnumerator ResetHighlightOptionCoroutine()
    {
        float timer = 0;
        Vector3 targetScale = originalLocalScale;
        while (timer < 0.25f)
        {
            float t = timer / 0.25f;
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, targetScale, t);
            timer += Time.deltaTime;
            yield return null;
        }
        this.transform.localScale = targetScale;
    }

    public void OnSkillOptionClicked()
    {
        if(skillStatue.SelectedSkillOption != currentSkill) skillStatue.OnSkillChosen?.Invoke(currentSkill);
    }

}
