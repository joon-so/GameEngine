using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] GameObject effectLeftStaff = null;
    [SerializeField] GameObject effectRightStaff = null;
    [SerializeField] GameObject effectQSkillStaff1 = null;
    [SerializeField] GameObject effectQSkillStaff2 = null;

    public float moveSpeed = 5.0f;
    public float dodgeCoolTime = 5.0f;
    float curDodgeCoolTime = 0;

    bool doAttack = false;
    bool motionEndCheck = true;
    bool comboContinue = true;
    bool isRun = false;

    Vector3 vecTarget;

    Animator anim;
    GameObject tagCharacter;

    float distanceWithPlayer;
    public float followDistance = 20.0f;

    public float qskillCoolTime = 5.0f;
    float curQskillCoolTime = 0f;
    public float wskillCoolTime = 5.0f;
    float curWskillCoolTime = 0f;

    bool canDodge;
    bool canMove;
    bool canQSkill;
    bool canWSkill;
    bool doingDodge;
    bool doingSkill;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        vecTarget = transform.position;

        curDodgeCoolTime = dodgeCoolTime;
        curQskillCoolTime = qskillCoolTime;
        curWskillCoolTime = wskillCoolTime;

        canDodge = false;
        canMove = false;
        canQSkill = false;
        canWSkill = false;

        doingDodge = false;
        doingSkill = false;
        StartCoroutine(StartMotion());
    }

    void Update()
    {
        if (gameObject.transform.tag == "MainCharacter")
        {
            if (canMove)
            {
                Move();
                Stop();
                Attack();
            }
            if (canDodge)
                Dodge();
            if (doingDodge)
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                vecTarget = transform.position;
            }
            AttackRange();
            CoolTime();
            if (!doingSkill && !doingDodge)
            {
                Q_Skill();
                W_Skill();
            }
        }
        else
        {
            // Follow();
        }
    }

    void Move()
    {
        if (Input.GetMouseButton(1))
        {
            moveSpeed = 5.0f;
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

        isRun = vecTarget != transform.position;
        anim.SetBool("Run", isRun);

        if (doAttack)
        {
            isRun = false;
            anim.SetBool("Run", isRun);
            vecTarget = transform.position;
        }
    }

    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDodge)
        {
            StartCoroutine(dodge());
        }
    }

    void Stop()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveSpeed = 0f;
            isRun = false;
            anim.SetBool("Run", isRun);
            vecTarget = transform.position;
        }
    }

    void Attack()
    {
        if (doAttack)
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
                    doAttack = false;
                    anim.SetBool("Attack", doAttack);

                }
                motionEndCheck = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            isRun = false;
            anim.SetBool("Run", isRun);

            if ((doAttack && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f
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
            doAttack = true;
            anim.SetBool("Attack", doAttack);
        }

        if (doAttack && Input.GetMouseButtonDown(1))
        {
            doAttack = false;
            anim.SetBool("Attack", doAttack);
        }
    }

    void AttackRange()
    {
        //attackRange.transform.position = transform.position;
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    attackRange.SetActive(true);
        //}
        //else if (Input.GetKeyUp(KeyCode.A))
        //{
        //    attackRange.SetActive(false);
        //}
    }

    void CoolTime()
    {
        if (curDodgeCoolTime < dodgeCoolTime)
        {
            curDodgeCoolTime += Time.deltaTime;
            canDodge = false;
        }
        else
        {
            canDodge = true;
        }

        if (curQskillCoolTime < qskillCoolTime)
        {
            curQskillCoolTime += Time.deltaTime;
            canQSkill = false;
        }
        else
        {
            canQSkill = true;
        }

        if (curWskillCoolTime < wskillCoolTime)
        {
            curWskillCoolTime += Time.deltaTime;
            canWSkill = false;
        }
        else
        {
            canWSkill = true;
        }
    }

    void Q_Skill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (canQSkill)
            {
                StartCoroutine(BigAttack());
            }
        }
    }

    void W_Skill()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (canWSkill)
            {
                StartCoroutine(StraightAttack());
            }
        }
    }

    void E_Skill()
    {

    }

    IEnumerator StartMotion()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("StartMotion");
        yield return new WaitForSeconds(1.5f);
        canDodge = true;
        canMove = true;
        canQSkill = true;
        canWSkill = true;
    }
    IEnumerator BigAttack()
    {
        effectLeftStaff.SetActive(false);
        effectRightStaff.SetActive(false);
        anim.SetTrigger("QSkill");
        doingSkill = true;
        canMove = false;
        curQskillCoolTime = 0.0f;
        effectQSkillStaff1.SetActive(true);
        yield return new WaitForSeconds(0.25f);

        effectQSkillStaff2.SetActive(true);
        yield return new WaitForSeconds(0.65f);

        effectQSkillStaff1.SetActive(false);
        effectQSkillStaff2.SetActive(false);
        effectLeftStaff.SetActive(true);
        effectRightStaff.SetActive(true);

        vecTarget = transform.position;
        canMove = true;
        isRun = false;
        doingSkill = false;
        anim.SetBool("Run", isRun);
    }

    IEnumerator StraightAttack()
    {
        anim.SetTrigger("WSkill");
        canMove = false;
        doingSkill = true;
        curWskillCoolTime = 0.0f;
        yield return new WaitForSeconds(2.8f);

        vecTarget = transform.position;
        canMove = true;
        isRun = false;
        doingSkill = false;
        anim.SetBool("Run", isRun);
    }

    IEnumerator dodge()
    {
        curDodgeCoolTime = 0.0f;
        anim.SetTrigger("Dodge");
        canMove = false;
        doingDodge = true;
        moveSpeed = 3.0f;
        yield return new WaitForSeconds(0.3f);
        moveSpeed = 10.0f;
        yield return new WaitForSeconds(0.8f);
        moveSpeed = 3.0f;
        yield return new WaitForSeconds(0.5f);
        moveSpeed = 5.0f;
        canMove = true;
        doingDodge = false;
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


    void OnCollisionEnter(Collision collision)
    {
    }
}
