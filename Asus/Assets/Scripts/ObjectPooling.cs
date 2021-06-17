using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance;
    [SerializeField] private GameObject poolingObject;
    private Queue<AssaultRifleBullet> objectQueue = new Queue<AssaultRifleBullet>();

    private void Awake()
    {
        Instance = this;

        Initialize(5);
    }
    private AssaultRifleBullet CreateObject()
    {
        var newObject = Instantiate(poolingObject, transform).GetComponent<AssaultRifleBullet>();
        newObject.gameObject.SetActive(false);
        return newObject;
    }
    private void Initialize(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            objectQueue.Enqueue(CreateObject());
        }
    }
    public static AssaultRifleBullet GetObject()
    {
        if (Instance.objectQueue.Count > 0)
        {
            var obj = Instance.objectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObject = Instance.CreateObject();
            newObject.transform.SetParent(null);
            newObject.gameObject.SetActive(true);
            return newObject;
        }
    }
    public static void ReturnObject(AssaultRifleBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(Instance.transform);
        Instance.objectQueue.Enqueue(bullet);
    }
}