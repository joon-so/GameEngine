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
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deadSound;

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

    protected List<GameObject> targets;

    public int maxHp = 200;
    public int currentHp;
    public static int damage = 10;
    public HpBar hpBar;

    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        targets = GameObject.Find("Enemys").GetComponent<EnemyList>().Enemys;

        //InvokeRepeating("Find", 0f, 0.5f);
        startPoint = transform.position;

        currentHp = maxHp;
        hpBar.SetMaxHp(maxHp);
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
                SoundManager.instance.SFXPlay("Explosion", deadSound);
                //movable = false;
                shootable = false;
                GetComponent<TriangleExplosion>().ExplosionMesh();
                targets.Remove(gameObject);
            }
        }
    }

    IEnumerator Attack()
    {
        // 총알 생성
        Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        SoundManager.instance.SFXPlay("attack", attackSound);
        movable = false;
        shootable = false;
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.SFXPlay("attack", attackSound);
        Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        //발사시간 대기
        movable = true;
        //쿨타임 지난 후 
        yield return new WaitForSeconds(shootCooltime);
        shootable = true;
    }

    public void HitJadeGrenade()
    {
        currentHp -= Soldier.wSkillDamage;
        hpBar.SetHp(currentHp);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FighterAttack")
        {
            currentHp -= Fighter.attackDamage;
            hpBar.SetHp(currentHp);
        }

        if (other.gameObject.tag == "FighterWSkill")
        {
            currentHp -= Fighter.wSkillDamage;
            hpBar.SetHp(currentHp);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "FighterAttack")
        {
            currentHp -= Fighter.attackDamage;
            hpBar.SetHp(currentHp);
        }
        if (collision.gameObject.tag == "FighterQSkill")
        {
            currentHp -= Fighter.qSkillDamage;
            hpBar.SetHp(currentHp);
        }
        if (collision.gameObject.tag == "FighterWSkill")
        {
            currentHp -= Fighter.wSkillDamage;
            hpBar.SetHp(currentHp);
        }
        if (collision.gameObject.tag == "SoldierAttack")
        {
            currentHp -= Soldier.attackDamage;
            hpBar.SetHp(currentHp);
        }
        if (collision.gameObject.tag == "SoldierQSkill")
        {
            currentHp -= Soldier.qSkillDamage;
            hpBar.SetHp(currentHp);
        }
        if (collision.gameObject.tag == "SoldierWSkill")
        {
            currentHp -= Soldier.wSkillDamage;
            hpBar.SetHp(currentHp);
        }
    }
}
