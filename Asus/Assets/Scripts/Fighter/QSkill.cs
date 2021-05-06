using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSkill : MonoBehaviour
{
    [SerializeField] Transform qSkillPos = null;
    [SerializeField] GameObject obj1 = null;
    [SerializeField] GameObject obj2 = null;
    [SerializeField] GameObject obj3 = null;


    float curScale;
    float maxScale;

    void Start()
    {
        curScale = 1;
        maxScale = 2;
        StartCoroutine(DestroyObject());
    }

    void Update()
    {
        gameObject.transform.position = GameObject.Find("Fighter").GetComponent<Fighter>().qSkillPos.position;

        if (curScale < maxScale)
        {
            curScale += Time.deltaTime;
            transform.localScale = new Vector3(curScale, curScale, curScale);
            obj1.transform.localScale = new Vector3(curScale, curScale, curScale);
            obj2.transform.localScale = new Vector3(curScale, curScale, curScale);
            obj3.transform.localScale = new Vector3(curScale, curScale, curScale);
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
