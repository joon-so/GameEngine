using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : MonoBehaviour
{
    [SerializeField] [Range(1f, 30f)] float detectDistance = 15f;
    [SerializeField] [Range(1f, 30f)] float shootDistance = 10f;
    [SerializeField] GameObject bullet = null;
    [SerializeField] Transform bulletPos;

    public float shootCooltime = 3.0f;
    public float spinSpeed = 50.0f;

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
        mainCharacter = GameObject.FindGameObjectWithTag("MainCharacter");

        if (mainCharacter == null)
        {
            return;
        }
        else
        {
            playerDistance = Vector3.Distance(mainCharacter.transform.position, transform.position);

            //Attack
            if (playerDistance < shootDistance)
            {
                if (movable)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(mainCharacter.transform.position - transform.position);
                    Vector3 euler = Quaternion.RotateTowards(transform.rotation, lookRotation, spinSpeed * Time.deltaTime).eulerAngles;
                    transform.rotation = Quaternion.Euler(0, euler.y, 0);
                }
                nav.SetDestination(transform.position);

                if (shootable)
                    StartCoroutine(Attack());
            }
            //Detect
            else if (playerDistance < detectDistance)
            {
                if (movable)
                    nav.SetDestination(mainCharacter.transform.position);
            }
            else
            {
                if (Vector3.Distance(startPoint, transform.position) < 0.1f)
                {
                    //anim.SetBool("isRun", false);
                }
                else
                {
                    // anim.SetBool("isRun", true);
                }

                if (movable)
                    nav.SetDestination(startPoint);
            }

            if (currentHp <= 0)
            {
                GetComponent<TriangleExplosion>().ExplosionMesh();
            }
        }
    }

    IEnumerator Attack()
    {
        // 총알 생성
        Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        movable = false;
        shootable = false;
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.2f);
        Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        //발사시간 대기
        movable = true;
        //쿨타임 지난 후 
        yield return new WaitForSeconds(shootCooltime);
        shootable = true;
    }
}
