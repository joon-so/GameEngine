using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : MonoBehaviour
{
    [SerializeField] [Range(30f, 100f)] float detectDistance;
    [SerializeField] [Range(30f, 100f)] float shootDistance = 30f;
    [SerializeField] GameObject bullet = null;
    [SerializeField] Transform bulletPos;

    public float shootCooltime = 3.0f;

    bool shootable = true;
    bool movable = true;
    bool alive = true;

    NavMeshAgent nav;
    float playerDistance;
    GameObject mainCharacter;
    Animator anim;
    Vector3 startPoint;
    Rigidbody rigid;

    public int maxHp = 200;
    public int currentHp;

    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        //InvokeRepeating("Find", 0f, 0.5f);
        startPoint = transform.position;

        currentHp = maxHp;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shootable)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        // �Ѿ� ����
        Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        movable = false;
        shootable = false;
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.2f);
        Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        //�߻�ð� ���
        movable = true;
        //��Ÿ�� ���� �� 
        yield return new WaitForSeconds(shootCooltime);
        shootable = true;
    }
}
