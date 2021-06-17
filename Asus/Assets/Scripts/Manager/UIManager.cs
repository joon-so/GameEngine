using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image character1Hp;
    [SerializeField] Image character1Ep;
    [SerializeField] Image character2Hp;
    [SerializeField] Image character2Ep;

    [SerializeField] Text textCharacter1Hp;
    [SerializeField] Text textCharacter1Ep;
    [SerializeField] Text textCharacter2Hp;
    [SerializeField] Text textCharacter2Ep;

    [SerializeField] GameObject character1Mask_F;
    [SerializeField] GameObject character1Mask_S;
    [SerializeField] GameObject character2Mask_F;
    [SerializeField] GameObject character2Mask_S;

    [SerializeField] GameObject character1QSkill;
    [SerializeField] GameObject character1WSkill;
    [SerializeField] GameObject character2QSkill;
    [SerializeField] GameObject character2WSkill;

    private bool isTag;         

    void Start()
    {
        character1Mask_F.SetActive(true);
        character1Mask_S.SetActive(false);
        character2Mask_F.SetActive(false);
        character2Mask_S.SetActive(true);

        character1QSkill.SetActive(true);
        character1WSkill.SetActive(true);
        character2QSkill.SetActive(false);
        character2WSkill.SetActive(false);

        isTag = true;
    }

    void Update()
    {
        UpdateHpEp();

        if (Input.GetKeyDown(KeyCode.F))
        {
            Tag();
        }
    }

    void UpdateHpEp()
    {
        character1Hp.fillAmount = GameManager.instance.character1Hp / GameManager.instance.character1MaxHp;
        character1Ep.fillAmount = GameManager.instance.character1Ep / GameManager.instance.character1MaxEp;
        character2Hp.fillAmount = GameManager.instance.character2Hp / GameManager.instance.character2MaxHp;
        character2Ep.fillAmount = GameManager.instance.character2Ep / GameManager.instance.character2MaxEp;


        textCharacter1Hp.text = string.Format("{0}/{1}", GameManager.instance.character1Hp, GameManager.instance.character1MaxHp);
        textCharacter1Ep.text = string.Format("{0}/{1}", GameManager.instance.character1Ep, GameManager.instance.character1MaxEp);
        textCharacter2Hp.text = string.Format("{0}/{1}", GameManager.instance.character2Hp, GameManager.instance.character2MaxHp);
        textCharacter2Ep.text = string.Format("{0}/{1}", GameManager.instance.character2Ep, GameManager.instance.character2MaxEp);
    }

    void Tag()
    {
        float tempHp = GameManager.instance.character1Hp;
        GameManager.instance.character1Hp = GameManager.instance.character2Hp;
        GameManager.instance.character2Hp = tempHp;

        float tempEp = GameManager.instance.character1Ep;
        GameManager.instance.character1Ep = GameManager.instance.character2Ep;
        GameManager.instance.character2Ep = tempEp;

        float tempMaxHp = GameManager.instance.character1MaxHp;
        GameManager.instance.character1MaxHp = GameManager.instance.character2MaxHp;
        GameManager.instance.character2MaxHp = tempMaxHp;

        float tempMaxEp = GameManager.instance.character1MaxEp;
        GameManager.instance.character1MaxEp = GameManager.instance.character2MaxEp;
        GameManager.instance.character2MaxEp = tempMaxEp;

        if (isTag)
        {
            character1Mask_F.SetActive(false);
            character1Mask_S.SetActive(true);
            character2Mask_F.SetActive(true);
            character2Mask_S.SetActive(false);

            character1QSkill.SetActive(false);
            character1WSkill.SetActive(false);
            character2QSkill.SetActive(true);
            character2WSkill.SetActive(true);

            isTag = false;
        }
        else
        {
            character1Mask_F.SetActive(true);
            character1Mask_S.SetActive(false);
            character2Mask_F.SetActive(false);
            character2Mask_S.SetActive(true);

            character1QSkill.SetActive(true);
            character1WSkill.SetActive(true);
            character2QSkill.SetActive(false);
            character2WSkill.SetActive(false);

            isTag = true;
        }
    }

    //void C1_QSkillCoolDownUI()
    //{
    //    if (Input.GetKey(KeyCode.Q) && c1_QSkillCoolDown == false)
    //    {
    //        c1_QSkillCoolDown = true;
    //        c1_QSkillImg.fillAmount = 1;
    //    }

    //    if (c1_QSkillCoolDown)
    //    {
    //        c1_QSkillImg.fillAmount -= 1 / c1_QSkillcoolDown * Time.deltaTime;

    //        if (c1_QSkillImg.fillAmount <= 0)
    //        {
    //            c1_QSkillImg.fillAmount = 0;
    //            c1_QSkillCoolDown = false;
    //        }
    //    }
    //}
}