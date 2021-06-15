using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateNet : MonoBehaviour
{
    public float rotateSpeed = 10;
    void Update()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
