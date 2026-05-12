using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class NpCParticles : MonoBehaviour
{
    public static NpCParticles SharedInstance;
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

    public void CreateParticleEmitter(Vector2[] location)
    {
        
    }

    public void CreateParticleAttractor(Rectangle extent)
    {
        
    }

    public static NpCParticles GetInstance()
    {
        return SharedInstance;
    }
}
