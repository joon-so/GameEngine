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

    public void Tag()
    {
    }
}
