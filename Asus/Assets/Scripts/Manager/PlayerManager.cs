using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Camera focusCharacter;
    [SerializeField] GameObject mainCharacter;
    [SerializeField] GameObject subCharacter;

    private CameraController mainCameraController;
    private bool isTag;

    void Start()
    {
        mainCameraController = focusCharacter.GetComponent<CameraController>();
        isTag = true;

        InitCharacter();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Tag();
        }

    }

    void InitCharacter()
    {
        GameManager.instance.character1 = mainCharacter;
        GameManager.instance.character2 = subCharacter;

        GameManager.instance.mainPlayerHp = 500;
        GameManager.instance.mainPlayerEp = 150;
        GameManager.instance.subPlayerHp = 400;
        GameManager.instance.subPlayerEp = 200;
    }

    void Tag()
    {
        if(isTag)
        {
            mainCameraController.focus = subCharacter.transform;
            GameManager.instance.Tag1();
            isTag = false;
        }
        else
        {
            mainCameraController.focus = mainCharacter.transform;
            GameManager.instance.Tag2();
            isTag = true;
        }
    }
}
