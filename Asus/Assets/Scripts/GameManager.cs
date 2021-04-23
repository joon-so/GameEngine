using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject character1;
    public GameObject character2;

    public float mainPlayerMaxHp;
    public float mainPlayerMaxEp;
    public float subPlayerMaxHp;
    public float subPlayerMaxEp;

    public float mainPlayerHp;
    public float mainPlayerEp;
    public float subPlayerHp;
    public float subPlayerEp;

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

    public void Tag1()
    {
        character1.gameObject.tag = "SubCharacter";
        character2.gameObject.tag = "MainCharacter";
        character1.gameObject.layer = 7;
        character2.gameObject.layer = 6;
    }
    public void Tag2()
    {
        character1.gameObject.tag = "MainCharacter";
        character2.gameObject.tag = "SubCharacter";
        character1.gameObject.layer = 6;
        character2.gameObject.layer = 7;
    }
}
