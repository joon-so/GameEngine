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
    [SerializeField] GameObject dropEffect = null;
    [SerializeField] Transform bulletLeftStartPoint;
    [SerializeField] Transform bulletRightStartPoint;

    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip deadSound;

    float straightTime = 0.2f;
    public float shootCooltime = 3.0f;
    public float spinSpeed = 50.0f;

    bool shootable = true;
    bool movable = true;
    bool born = false;
    bool drop = false;

    float playerDistance;
    GameObject mainCharacter;
    Animator anim;
    NavMeshAgent nav;
    Rigidbody rigid;
    Vector3 startPoint;

    protected List<GameObject> targets;

    public int maxHp = 200;
    public int currentHp;
    public HpBar hpBar;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();

        targets = GameObject.Find("Enemys").GetComponent<EnemyList>().Enemys;
        transform.position = new Vector3(transform.position.x, 45f, transform.position.z);
        startPoint = transform.position;
        rigid.useGravity = false;
        nav.enabled = false;

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
        if(!born)
        {
            Waiting();
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
                //movable = false;
                SoundManager.instance.SFXPlay("Explosion", deadSound);

                shootable = false;
                GetComponent<TriangleExplosion>().ExplosionMesh();
                targets.Remove(gameObject);
            }
        }
    }
    void Waiting()
    {
        mainCharacter = GameObject.FindGameObjectWithTag("MainCharacter");

        playerDistance = Vector3.Distance(
            new Vector3(mainCharacter.transform.position.x, 0, mainCharacter.transform.position.z),
            new Vector3(transform.position.x, 0, transform.position.z));

        if (playerDistance < spownDistance && !drop)
        {
            drop = true;
            StartCoroutine(DropAndExplosion());
        }
    }
    IEnumerator Attack()
    {
        // ÃÑ¾Ë »ý¼º
        SoundManager.instance.SFXPlay("Explosion", attackSound);
        Instantiate(bullet, bulletLeftStartPoint.position, bulletLeftStartPoint.rotation);
        movable = false;
        shootable = false;
        //anim.SetBool("isAttack", true);
        //ÁÂ¿ì 2¹ß
        yield return new WaitForSeconds(straightTime);
        SoundManager.instance.SFXPlay("Explosion", attackSound);
        Instantiate(bullet, bulletRightStartPoint.position, bulletRightStartPoint.rotation);
        yield return new WaitForSeconds(straightTime);
        SoundManager.instance.SFXPlay("Explosion", attackSound);
        Instantiate(bullet, bulletLeftStartPoint.position, bulletLeftStartPoint.rotation);
        yield return new WaitForSeconds(straightTime);
        SoundManager.instance.SFXPlay("Explosion", attackSound);
        Instantiate(bullet, bulletRightStartPoint.position, bulletRightStartPoint.rotation);
        //ÄðÅ¸ÀÓ
        movable = true;
        yield return new WaitForSeconds(shootCooltime);
        shootable = true;
    }
    IEnumerator DropAndExplosion()
    {
        rigid.useGravity = true;
        yield return new WaitForSeconds(3f);
        Instantiate(dropEffect, transform.position, transform.rotation);
        this.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        nav.enabled = true;
        born = true;
        movable = true;
        yield return new WaitForSeconds(1.9f);
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
        if (collision.gameObject.tag == "FighterAttack")
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
