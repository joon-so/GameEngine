using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : SubAI
{
    [SerializeField] GameObject attackRange = null;
    [SerializeField] GameObject useAssaultRifle = null;
    [SerializeField] GameObject useMissileLauncher = null;
    [SerializeField] GameObject backAssaultRifle = null;
    [SerializeField] GameObject backMissileLauncher = null;

    [SerializeField] Transform assaultRifleBulletPos = null;
    [SerializeField] GameObject assaultRifleBullet = null;

    [SerializeField] Transform missileBulletPos = null;
    [SerializeField] GameObject missileBullet = null;
    [SerializeField] GameObject missileRange = null;
    [SerializeField] GameObject missileEffect = null;

    [SerializeField] Transform grenadePos = null;
    [SerializeField] GameObject Grenade = null;

    public float moveSpeed = 5.0f;

    public float fireCoolTime = 0.5f;
    public float subFireDelay = 1.5f;
    public float followDistance = 5.0f;

    public float dodgeCoolTime = 7.0f;
    public float qSkillCoolTime = 13.0f;
    public float wSkillCoolTime = 6.0f;

    public static int attackDamage = 30;
    public static int qSkillDamage = 80;
    public static int wSkillDamage = 40;

    bool onDodge;
    bool onQSkill;
    bool onWSkill;

    bool canMove;
    bool canAttack;
    bool canDodge;
    bool canSkill;
    bool speedUp;

    float curFireDelay;

    bool doingDodge;

    Vector3 vecTarget;

    float fireDelay;

    Animator anim;

    float distanceWithPlayer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        vecTarget = transform.position;
        onDodge = true;
        onQSkill = true;
        onWSkill = true;

        FindEnemys();

        nav = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();

        canAttack = false;
        canMove = false;
        canDodge = false;
        canSkill = false;
        speedUp = false;
        doingDodge = false;

        UIManager.instance.character2DodgeCoolDown.fillAmount = 0;
        UIManager.instance.character2QSkillCoolDown.fillAmount = 0;
        UIManager.instance.character2WSkillCoolDown.fillAmount = 0;

        StartCoroutine(DrawAssaultRifle());
    }

    void Update()
    {
        curFireDelay += Time.deltaTime;
        if (gameObject.transform.tag == "MainCharacter")
        {
            fireDelay += Time.deltaTime;
            if (canMove)
                Move();
            if (canAttack)
                Attack();
            if (canDodge)
                Dodge();
            if (doingDodge)
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                vecTarget = transform.position;
            }

            if (canSkill)
            {
                Q_Skill();
                W_Skill();
            }
            Stop();
            AttackRange();
        }
        else if (gameObject.transform.tag == "SubCharacter")
        {
            distance = Vector3.Distance(tagCharacter.transform.position, transform.position);
            if (currentState == characterState.trace)
            {
                MainCharacterTrace();
                anim.SetBool("Run", true);
                curFireDelay = 1f;
            }
            else if (currentState == characterState.attack)
            {
                SubAttack();

                if (target)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                    Vector3 euler = Quaternion.RotateTowards(transform.rotation, lookRotation, spinSpeed * Time.deltaTime).eulerAngles;
                    transform.rotation = Quaternion.Euler(0, euler.y, 0);

                }
                if (curFireDelay > subFireDelay && target != null)
                {
                    GameObject instantBullet = Instantiate(assaultRifleBullet, assaultRifleBulletPos.position, assaultRifleBulletPos.rotation);
                    Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
                    bulletRigid.velocity = assaultRifleBulletPos.forward;

                    moveSpeed = 0f;
                    anim.SetBool("Run", false);
                    vecTarget = transform.position;

                    anim.SetTrigger("shootAssaultRifle");
                    curFireDelay = 0;

                    StartCoroutine(AttackDelay());
                }
            }
            else if (currentState == characterState.idle)
            {
                Idle();
                anim.SetBool("Run", false);
                curFireDelay = 1f;
            }
        }
        CoolTime();
        Tag();
        SpeedUp();
    }
    void Move()
    {
        if (Input.GetMouseButton(1))
        {
            if (!speedUp)
            {
                moveSpeed = 5.0f;
            }
            else
            {
                moveSpeed = 10.0f;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                vecTarget = hit.point;
                vecTarget.y = transform.position.y;

                Vector3 nextVec = hit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, vecTarget, moveSpeed * Time.deltaTime);
        anim.SetBool("Run", vecTarget != transform.position);
    }
    void Stop()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveSpeed = 0f;
            anim.SetBool("Run", false);
            vecTarget = transform.position;
        }
    }
    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onDodge)
        {
            StartCoroutine(dodge());
        }
    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            canMove = false;
            canDodge = false;
            canSkill = false;

            if (fireDelay > fireCoolTime)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 nextVec = hit.point - transform.position;
                    nextVec.y = 0;
                    transform.LookAt(transform.position + nextVec);
                }

                GameObject instantBullet = Instantiate(assaultRifleBullet, assaultRifleBulletPos.position, assaultRifleBulletPos.rotation);
                Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
                bulletRigid.velocity = assaultRifleBulletPos.forward;

                moveSpeed = 0f;
                anim.SetBool("Run", false);
                vecTarget = transform.position;

                anim.SetTrigger("shootAssaultRifle");
                fireDelay = 0;

                StartCoroutine(AttackDelay());
            }
        }
    }
    void AttackRange()
    {
        attackRange.transform.position = transform.position;
        if (Input.GetKeyDown(KeyCode.A))
        {
            attackRange.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            attackRange.SetActive(false);
        }
    }

    void Q_Skill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && onQSkill)
        {
            UIManager.instance.character2QSkillCoolDown.fillAmount = 1;

            onQSkill = false;
            anim.SetBool("Run", false);

            canAttack = false;
            canMove = false;
            canDodge = false;
            canSkill = false;

            StartCoroutine(ShootMissile());
        }
    }
    void W_Skill()
    {
        if (Input.GetKeyDown(KeyCode.W) && onWSkill)
        {
            UIManager.instance.character2WSkillCoolDown.fillAmount = 1;

            onWSkill = false;
            anim.SetBool("Run", false);

            canAttack = false;
            canMove = false;
            canDodge = false;
            canSkill = false;

            StartCoroutine(ShootGrenade());
        }
    }
    void CoolTime()
    {
        if (!onDodge)
        {
            UIManager.instance.character2DodgeCoolDown.fillAmount -= 1 / dodgeCoolTime * Time.deltaTime;

            if (UIManager.instance.character2DodgeCoolDown.fillAmount <= 0)
            {
                UIManager.instance.character2DodgeCoolDown.fillAmount = 0;
                onDodge = true;
            }
        }

        if (!onQSkill)
        {
            UIManager.instance.character2QSkillCoolDown.fillAmount -= 1 / qSkillCoolTime * Time.deltaTime;

            if (UIManager.instance.character2QSkillCoolDown.fillAmount <= 0)
            {
                UIManager.instance.character2QSkillCoolDown.fillAmount = 0;
                onQSkill = true;
            }
        }

        if (!onWSkill)
        {
            UIManager.instance.character2WSkillCoolDown.fillAmount -= 1 / wSkillCoolTime * Time.deltaTime;

            if (UIManager.instance.character2WSkillCoolDown.fillAmount <= 0)
            {
                UIManager.instance.character2WSkillCoolDown.fillAmount = 0;
                onWSkill = true;
            }
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.2f);
        canMove = true;
        canDodge = true;
        canSkill = true;
    }
    IEnumerator dodge()
    {
        UIManager.instance.character2DodgeCoolDown.fillAmount = 1;

        //0.9sec
        canMove = false;
        canAttack = false;
        canSkill = false;
        onDodge = false;

        anim.SetTrigger("Dodge");
        doingDodge = true;
        moveSpeed = 8.0f;
        yield return new WaitForSeconds(0.6f);
        moveSpeed = 2.0f;
        yield return new WaitForSeconds(0.25f);
        moveSpeed = 5.0f;
        canMove = true;
        canAttack = true;
        canSkill = true;
        doingDodge = false;
    }
    IEnumerator DrawAssaultRifle()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("drawAssaultRifle");
        yield return new WaitForSeconds(0.5f);
        backAssaultRifle.SetActive(false);
        useAssaultRifle.SetActive(true);
        canMove = true;
        canAttack = true;
        canDodge = true;
        canSkill = true;
    }
    IEnumerator ShootMissile()
    {
        vecTarget = transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
        {
            Vector3 nextVec = rayHit.point - transform.position;
            nextVec.y = 0;
            transform.LookAt(transform.position + nextVec);

            anim.SetTrigger("drawMissileLauncher");
            yield return new WaitForSeconds(0.5f);
            useAssaultRifle.SetActive(false);
            useMissileLauncher.SetActive(true);

            anim.SetBool("AimMissile", true);
            yield return new WaitForSeconds(0.5f);
            missileEffect.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            anim.SetBool("AimMissile", false);
            missileEffect.SetActive(false);

            anim.SetTrigger("shootMissileLauncher");
            GameObject instantMissile = Instantiate(missileBullet, missileBulletPos.position, missileBulletPos.rotation);
            Rigidbody missileRigid = instantMissile.GetComponent<Rigidbody>();
            missileRigid.velocity = missileBulletPos.forward;

            yield return new WaitForSeconds(1.0f);

            anim.SetTrigger("drawAssaultRifle");
            yield return new WaitForSeconds(0.5f);
            useMissileLauncher.SetActive(false);
            useAssaultRifle.SetActive(true);

            yield return new WaitForSeconds(0.3f);
            canAttack = true;
            canMove = true;
            canDodge = true;
            canSkill = true;
        }
    }
    IEnumerator ShootGrenade()
    {
        vecTarget = transform.position;
        anim.SetTrigger("shootGrenade");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
        {
            Vector3 nextVec = rayHit.point - transform.position;
            if (Vector3.Distance(nextVec, transform.position) > 6)
                nextVec = nextVec.normalized * 6;

            transform.LookAt(transform.position + nextVec);
            nextVec.y = 5;
            GameObject instantGrenade = Instantiate(Grenade, grenadePos.position, grenadePos.rotation);
            Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
            rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
            rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.3f);
        canAttack = true;
        canMove = true;
        canDodge = true;
        canSkill = true;
    }

    void Tag()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            vecTarget = transform.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy1Bullet")
        {
            if (GameManager.instance.character1Hp > 0)
                GameManager.instance.character1Hp -= Enemy1.damage;
        }
    }

    void SpeedUp()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!speedUp)
            {
                speedUp = true;
                nav.speed *= 2;
            }
            else
            {
                speedUp = false;
                nav.speed /= 2;
            }
        }
    }
}