using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class NpcParticles : MonoBehaviour
{
    private static NpcParticles SharedInstance;
    private List<GameObject> pooledObjects;
    private GameObject emitterObject;
    private ParticleSystem emitterSystem;
    private List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
    private GameObject attractorParent;
    private ParticleSystemForceField attractorSystem;
    private BoxCollider2D attractorCollider;

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

    private void SetParticleMaterial(ParticleSystem system, SpriteRenderer spriteRenderer)
    {
        ParticleSystemRenderer particleRenderer = system.GetComponent<ParticleSystemRenderer>();
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        particleRenderer.material.mainTexture = spriteRenderer.sprite.texture;
    }

    private void SetParticleCollision(ParticleSystem system, Collider2D collider)
    {
        // collision
        ParticleSystem.CollisionModule collision = system.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.colliderForce = 1.0f;
        collision.SetPlane(0, collider.transform);
        collision.bounce = 0;

        // trigger
        ParticleSystem.TriggerModule trigger = system.trigger;
        trigger.enabled = true;
        trigger.inside = ParticleSystemOverlapAction.Ignore;
        trigger.enter = ParticleSystemOverlapAction.Callback;

        // external forces
        ParticleSystem.ExternalForcesModule externalForces = system.externalForces;
        externalForces.enabled = true;
    }

    public void CreateParticleEmitter(Rectangle location, float size, SpriteRenderer spriteRenderer)
    {
        emitterObject = new GameObject("emitter");
        emitterSystem = emitterObject.AddComponent<ParticleSystem>();

        Bounds bounds = location.GetWorldBounds();

        // set location
        Vector2 centre = bounds.center;
        emitterObject.transform.Translate(centre);

        // set scale
        var sh = emitterSystem.shape;
        sh.shapeType = ParticleSystemShapeType.Box;
        sh.scale = bounds.size;
        var main = emitterSystem.main;
        main.startSize = size;

        // set material
        SetParticleMaterial(emitterSystem, spriteRenderer);
    }

    public void CreateParticleAttractor(Rectangle location, Collider2D collider, float range)
    {
        // set emitter collision
        SetParticleCollision(emitterSystem, collider);
        
        attractorParent = new GameObject("attractor");
        attractorSystem = attractorParent.AddComponent<ParticleSystemForceField>();
        attractorCollider = attractorParent.AddComponent<BoxCollider2D>();

        ParticleCollector attractorCollector = emitterObject.AddComponent<ParticleCollector>();
        attractorCollector.SetEmitterSystem(emitterSystem);
        attractorCollector.SetParticles(particles);

        Bounds bounds = location.GetWorldBounds();

        // set location
        Vector2 centre = bounds.center;
        attractorParent.transform.Translate(centre);

        // set shape
        attractorSystem.shape = ParticleSystemForceFieldShape.Box;
        attractorSystem.endRange = range;
        attractorSystem.rotationSpeed = 0;
        attractorSystem.rotationAttraction = 0;

        attractorCollider.isTrigger = false;

        emitterSystem.trigger.SetCollider(0, attractorParent.transform);
    }

    public static NpcParticles GetInstance()
    {
        return SharedInstance;
    }
}


public class ParticleCollector : MonoBehaviour
{
    private ParticleSystem emitterSystem;
    private List<ParticleSystem.Particle> particles;
    
    private void OnParticleTrigger()
    {
        int triggeredParticles = emitterSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
        for (int i = 0; i < triggeredParticles; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            particle.remainingLifetime = 0;
            particles[i] = particle;
        }

        emitterSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }

    public void SetEmitterSystem(ParticleSystem system)
    {
        emitterSystem = system;
    }
    public void SetParticles(List<ParticleSystem.Particle> particleList)
    {
        particles = particleList;
    }
}