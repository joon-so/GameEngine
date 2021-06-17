using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject msehObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    public int damage = 70;

    void Start()
    {
        StartCoroutine(Explosion());
    }
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3.0f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        effectObj.SetActive(true);
        msehObj.SetActive(false);

        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy1>().HitJadeGrenade();
        }
    }
}
