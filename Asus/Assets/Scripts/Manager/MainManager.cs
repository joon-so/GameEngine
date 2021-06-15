using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField] AudioClip ButtonClickSound;

    public void OnClickStart()
    {
        SoundManager.instance.SFXPlay("ButtonClick", ButtonClickSound);
        SceneManager.LoadScene("Stage1");
    }
    
    public void OnClickExit()
    {
        SoundManager.instance.SFXPlay("ButtonClick", ButtonClickSound);
        Application.Quit();
    }
}
