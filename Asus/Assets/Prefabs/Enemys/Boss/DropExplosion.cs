using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropExplosion : MonoBehaviour
{
    float destroyTime = 1.4f;
    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0)
            Destroy(gameObject);
    }
}
