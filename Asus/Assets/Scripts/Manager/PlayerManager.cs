using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Camera focusCharacter;
    [SerializeField] GameObject character1;
    [SerializeField] GameObject character2;

    private CameraController mainCameraController;
    private bool isTag;

    void Start()
    {
        mainCameraController = focusCharacter.GetComponent<CameraController>();
        isTag = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Tag();
        }
        GameManager.instance.mainPlayerMark.transform.position = new Vector3(GameManager.instance.character1.transform.position.x, 0.3f, GameManager.instance.character1.transform.position.z);
        GameManager.instance.subPlayerMark.transform.position = new Vector3(GameManager.instance.character2.transform.position.x, 0.3f, GameManager.instance.character2.transform.position.z);
    }

    void Tag()
    {
        if(isTag)
        {
            mainCameraController.focus = character2.transform;
            GameManager.instance.Tag1();
            GameManager.instance.character1.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            GameManager.instance.character2.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            isTag = false;
        }
        else
        {
            mainCameraController.focus = character1.transform;
            GameManager.instance.Tag2();
            GameManager.instance.character1.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            GameManager.instance.character2.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            isTag = true;
        }
    }
}
