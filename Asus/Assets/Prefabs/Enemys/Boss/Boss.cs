using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    [SerializeField] [Range(0f, 10f)] float spownDistance = 5f;
    [SerializeField] [Range(10f, 30f)] float detectDistance = 15f;
    [SerializeField] [Range(10f, 20f)] float shootDistance = 10f;
    [SerializeField] GameObject bullet = null;
    [SerializeField] Transform bulletLeftStartPoint;
    [SerializeField] Transform bulletRightStartPoint;

    float straightTime = 0.2f;
    public float shootCooltime = 3.0f;
    public float spinSpeed = 50.0f;

    bool shootable = true;
    bool movable = true;
    bool alive = true;

    float playerDistance;
    GameObject mainCharacter;
    Animator anim;
    NavMeshAgent nav;
    Vector3 startPoint;

    public int maxHp = 200;
    public int currentHp;

    // Start is called before the first frame update
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
        // ÃÑ¾Ë »ý¼º
        Instantiate(bullet, bulletLeftStartPoint.position, bulletLeftStartPoint.rotation);
        movable = false;
        shootable = false;
        //anim.SetBool("isAttack", true);
        //ÁÂ¿ì 2¹ß
        yield return new WaitForSeconds(straightTime);
        Instantiate(bullet, bulletRightStartPoint.position, bulletRightStartPoint.rotation);
        yield return new WaitForSeconds(straightTime);
        Instantiate(bullet, bulletLeftStartPoint.position, bulletLeftStartPoint.rotation);
        yield return new WaitForSeconds(straightTime);
        Instantiate(bullet, bulletRightStartPoint.position, bulletRightStartPoint.rotation);
        //ÄðÅ¸ÀÓ
        movable = true;
        yield return new WaitForSeconds(shootCooltime);
        shootable = true;
    }
}
