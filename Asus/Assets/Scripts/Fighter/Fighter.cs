using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fighter : SubAI
{
    [SerializeField] GameObject leftStaffEffect = null;
    [SerializeField] GameObject rightStaffEffect = null;

    [SerializeField] GameObject qSkill = null;
    public Transform qSkillPos = null;

    [SerializeField] GameObject wLeftEffect = null;
    [SerializeField] GameObject wRightEffect = null;

    public float moveSpeed = 5.0f;
    public float dodgeCoolTime = 4.0f;
    public float followDistance = 5.0f;

    public float qSkillCoolTime = 12.0f;
    public float wSkillCoolTime = 10.0f;

    public static int attackDamage = 10;
    public static int qSkillDamage = 60;
    public static int wSkillDamage = 10;

    bool canMove;
    bool canDodge;
    bool canAttack;
    bool canSkill;
    bool speedUp;

    public static bool onDodge;
    bool onQSkill;
    bool onWSkill;

    bool doingAttack;
    bool motionEndCheck;
    bool comboContinue;

    float curFireDelay;
    float subFireDelay = 1.5f;

    Vector3 vecTarget;

    Animator anim;

    float distanceWithPlayer;

    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip dodgeClip;
    [SerializeField] AudioClip qSkillClip;
    [SerializeField] AudioClip wSkillClip;

    void Start()
    {
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        canMove = false;
        canDodge = false;
        canAttack = false;
        canSkill = false;
        FindEnemys();

        onDodge = true;
        onQSkill = true;
        onWSkill = true;
        speedUp = false;

        doingAttack = false;
        motionEndCheck = true;
        comboContinue = true;

        UIManager.instance.character1DodgeCoolDown.fillAmount = 0;
        UIManager.instance.character1QSkillCoolDown.fillAmount = 0;
        UIManager.instance.character1WSkillCoolDown.fillAmount = 0;

        StartCoroutine(StartMotion());
    }

    void Update()
    {
        if (gameObject.transform.tag == "MainCharacter")
        {
            if (canMove)
                Move();
            if (canAttack)
                Attack();
            if (canDodge)
                Dodge();
            if (canSkill)
            {
                Q_Skill();
                W_Skill();
            }
            Stop();
        }
        else if (gameObject.transform.tag == "SubCharacter")
        {
            curFireDelay += Time.deltaTime;
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
                    moveSpeed = 0f;
                    anim.SetBool("Run", false);
                    anim.SetTrigger("Throwing");
                    vecTarget = transform.position;

                    curFireDelay = 0;
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

        if (doingAttack)
        {
            anim.SetBool("Run", false);
            vecTarget = transform.position;
        }
    }
    void Stop()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveSpeed = 0f;
            vecTarget = transform.position;
            anim.SetBool("Run", false);
        }
    }
    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onDodge)
        {
            UIManager.instance.character1DodgeCoolDown.fillAmount = 1;
            SoundManager.instance.SFXPlay("dodge", dodgeClip);

            onDodge = false;

            canAttack = false;
            canMove = false;
            canSkill = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 nextVec = hit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }

            moveSpeed *= 2;
            anim.SetTrigger("Dodge");

            StartCoroutine(DodgeDelay());
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("salto2SS"))
        {
            transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            vecTarget = transform.position;
            anim.SetBool("Run", false);
        }
    }

    void Attack()
    {
        if (doingAttack)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f
                && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
            {
                if (Input.GetMouseButtonDown(0))
                    if (comboContinue)
                        comboContinue = false;
                motionEndCheck = false;
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && !motionEndCheck)
            {
                if (!comboContinue)
                {
                    anim.SetTrigger("nextCombo");
                    comboContinue = true;
                }
                else if (comboContinue)
                {
                    doingAttack = false;
                    anim.SetBool("Attack", doingAttack);

                }
                motionEndCheck = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            canMove = false;
            anim.SetBool("Run", canMove);

            SoundManager.instance.SFXPlay("attack", attackClip);

            if ((doingAttack && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f
                 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
                 || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle1SS")
                 || anim.GetCurrentAnimatorStateInfo(0).IsName("runSS"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 nextVec = hit.point - transform.position;
                    nextVec.y = 0;
                    transform.LookAt(transform.position + nextVec);
                }
                vecTarget = transform.position;
            }

            moveSpeed = 0f;
            doingAttack = true;
            anim.SetBool("Attack", doingAttack);
        }

        if (doingAttack && Input.GetMouseButtonDown(1))
        {
            doingAttack = false;
            anim.SetBool("Attack", doingAttack);
            canMove = true;
            anim.SetBool("Run", canMove);
        }
    }

    void Q_Skill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && onQSkill)
        {
            if (GameManager.instance.character1Ep <= 0)
                return;

            UIManager.instance.character1QSkillCoolDown.fillAmount = 1;
            GameManager.instance.character1Ep -= 20;

            onQSkill = false;
            anim.SetBool("Run", false);

            canAttack = false;
            canMove = false;
            canDodge = false;
            canSkill = false;

            StartCoroutine(BigAttack());
        }
    }

    void W_Skill()
    {
        if (Input.GetKeyDown(KeyCode.W) && onWSkill)
        {
            if (GameManager.instance.character1Ep <= 0)
                return;

            UIManager.instance.character1WSkillCoolDown.fillAmount = 1;
            GameManager.instance.character1Ep -= 40;

            onWSkill = false;
            anim.SetBool("Run", false);

            canAttack = false;
            canMove = false;
            canDodge = false;
            canSkill = false;

            StartCoroutine(StraightAttack());
        }
    }

    void CoolTime()
    {
        if(GameManager.instance.character1Ep < 150)
            GameManager.instance.character1Ep += Time.deltaTime;

        if (!onDodge)
        {
            UIManager.instance.character1DodgeCoolDown.fillAmount -= 1 / dodgeCoolTime * Time.deltaTime;

            if (UIManager.instance.character1DodgeCoolDown.fillAmount <= 0)
            {
                UIManager.instance.character1DodgeCoolDown.fillAmount = 0;
                onDodge = true;
            }
        }

        if (!onQSkill)
        {
            UIManager.instance.character1QSkillCoolDown.fillAmount -= 1 / qSkillCoolTime * Time.deltaTime;

            if (UIManager.instance.character1QSkillCoolDown.fillAmount <= 0)
            {
                UIManager.instance.character1QSkillCoolDown.fillAmount = 0;
                onQSkill = true;
            }
        }

        if (!onWSkill)
        {
            UIManager.instance.character1WSkillCoolDown.fillAmount -= 1 / wSkillCoolTime * Time.deltaTime;

            if (UIManager.instance.character1WSkillCoolDown.fillAmount <= 0)
            {
                UIManager.instance.character1WSkillCoolDown.fillAmount = 0;
                onWSkill = true;
            }
        }
    }

    IEnumerator StartMotion()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("StartMotion");
        yield return new WaitForSeconds(1.5f);
        leftStaffEffect.SetActive(true);
        rightStaffEffect.SetActive(true);
        canMove = true;
        canAttack = true;
        canDodge = true;
        canSkill = true;

        vecTarget = transform.position;
    }

    IEnumerator DodgeDelay()
    {
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
        canMove = true;
        canSkill = true;
    }

    IEnumerator BigAttack()
    {
        leftStaffEffect.SetActive(false);
        rightStaffEffect.SetActive(false);

        SoundManager.instance.SFXPlay("QSkill", qSkillClip);

        anim.SetTrigger("QSkill");
        anim.SetFloat("Speed", 0.2f);
        yield return new WaitForSeconds(0.5f);
        Instantiate(qSkill, qSkillPos.position, qSkillPos.rotation);
        anim.SetFloat("Speed", 0.0f);
        yield return new WaitForSeconds(1.0f);
        anim.SetFloat("Speed", 1.0f);
        yield return new WaitForSeconds(1.0f);

        leftStaffEffect.SetActive(true);
        rightStaffEffect.SetActive(true);

        vecTarget = transform.position;
        anim.SetBool("Run", false);

        canAttack = true;
        canMove = true;
        canDodge = true;
        canSkill = true;
    }

    IEnumerator StraightAttack()
    {
        leftStaffEffect.SetActive(false);
        rightStaffEffect.SetActive(false);

        anim.SetTrigger("WSkill");
        wLeftEffect.SetActive(true);
        wRightEffect.SetActive(true);

        SoundManager.instance.SFXPlay("WSkill", wSkillClip);

        yield return new WaitForSeconds(2.8f);

        wLeftEffect.SetActive(false);
        wRightEffect.SetActive(false);

        leftStaffEffect.SetActive(true);
        rightStaffEffect.SetActive(true);

        vecTarget = transform.position;
        anim.SetBool("Run", false);

        canAttack = true;
        canMove = true;
        canDodge = true;
        canSkill = true;
    }

    void Follow()
    {
        //distanceWithPlayer = Vector3.Distance(tagCharacter.transform.position, transform.position);

        //if (distanceWithPlayer > followDistance)
        //{
        //    nav.SetDestination(tagCharacter.transform.position);
        //    isRun = true;
        //    anim.SetBool("isRun", isRun);
        //}
        //else
        //{
        //    isRun = false;
        //    nav.SetDestination(transform.position);
        //    anim.SetBool("isRun", isRun);
        //}
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
