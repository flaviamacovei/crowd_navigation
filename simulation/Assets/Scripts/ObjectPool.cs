using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceObjects(GameObject objectToPlace, List<Vector2> positions)
    {
        int numObjects = positions.Count;
        PoolObjects(numObjects, objectToPlace);

        for (int i = 0; i < numObjects; i++)
        {
            Vector2 position = positions[i];
            GameObject obj = GetPooledObject();
            obj.transform.position = position;
            obj.SetActive(true);
        }
    }

    private void PoolObjects(int numObjects, GameObject objectToPool)
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < numObjects; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public static ObjectPool GetInstance()
    {
        return SharedInstance;
    }
}
