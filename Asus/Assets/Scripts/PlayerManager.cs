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
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.F))
        {
            Tag();
        }

    }

    void InitCharacter()
    {
        GameManager.instance.character1 = mainCharacter;
        GameManager.instance.character2 = subCharacter;
    }


    void Tag()
    {
        if(isTag)
        {
            mainCameraController.focus = subCharacter.transform;


            isTag = false;
        }
        else
        {
            mainCameraController.focus = mainCharacter.transform;

            isTag = true;
        }
    }
}
