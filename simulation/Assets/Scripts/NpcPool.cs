using UnityEngine;
using System.Collections.Generic;
using System;

public class NpcPool : MonoBehaviour
{
    public static NpcPool SharedInstance;
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

    public void NpcUpdate(Vector2[] targetLineSegment, float speed)
    {
        List<GameObject> activeObjects = GetPooledObjects(true);

        for (int i = 0; i < activeObjects.Count; i++)
        {
            GameObject obj = activeObjects[i];
            Vector2 currentPosition = obj.transform.position;
            Vector2 targetPosition = Utils.GetClosestPointOnTarget(targetLineSegment, currentPosition);

            // if target position lies to the right of the object: target reached -> deactivate
            bool targetReached = targetPosition.x > currentPosition.x;
            if (targetReached)
            {
                obj.SetActive(false);
            }

            // else: move in the direction of target
            else
            {
                Rigidbody2D rigidBody = obj.GetComponent<Rigidbody2D>();
                Vector2 exactDirection = Vector3.Normalize(targetPosition - currentPosition);
                Vector2 randomDirection = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                rigidBody.AddForce((exactDirection + randomDirection) * speed, ForceMode2D.Force);
            }
        }
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
            tmp.name = "npc_" + i;
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public List<GameObject> GetPooledObjects(bool active = false)
    {
        List<GameObject> objects = new List<GameObject>();

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy == active) // if object's activity status matches desired activity, proceed
            {
                objects.Add(pooledObjects[i]);
            }
        }
        return objects;
    }

    public static NpcPool GetInstance()
    {
        return SharedInstance;
    }
}
