using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject character1;
    public GameObject character2;

    public float character1MaxHp;
    public float character1MaxEp;
    public float character2MaxHp;
    public float character2MaxEp;

    public float character1Hp;
    public float character1Ep;
    public float character2Hp;
    public float character2Ep;

    public GameObject mainPlayerMark;
    public GameObject subPlayerMark;

    public bool onDodge;
    public bool onQSkill;
    public bool onWSkill;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        character1Hp = character1MaxHp;
        character1Ep = character1MaxEp;
        character2Hp = character2MaxHp;
        character2Ep = character2MaxEp;

        onDodge = true;
        onQSkill = true;
        onWSkill = true;
    }

    public void Tag1()
    {
        character1.gameObject.tag = "SubCharacter";
        character2.gameObject.tag = "MainCharacter";
        character1.gameObject.layer = 7;
        character2.gameObject.layer = 6;

        GameObject temp = mainPlayerMark;
        mainPlayerMark = subPlayerMark;
        subPlayerMark = temp;
    }
    public void Tag2()
    {
        character1.gameObject.tag = "MainCharacter";
        character2.gameObject.tag = "SubCharacter";
        character1.gameObject.layer = 6;
        character2.gameObject.layer = 7;

        GameObject temp = mainPlayerMark;
        mainPlayerMark = subPlayerMark;
        subPlayerMark = temp;
    }
}
