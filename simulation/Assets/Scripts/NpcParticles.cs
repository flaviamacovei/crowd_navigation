using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class NpcParticles : MonoBehaviour
{
    public static NpcParticles SharedInstance;
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

    public void CreateParticleEmitter(Rectangle location, SpriteRenderer spriteRenderer, Collider2D collider)
    {
        GameObject emitterObject = new GameObject("emitter");
        ParticleSystem emitterSystem = emitterObject.AddComponent<ParticleSystem>();

        Bounds bounds = location.GetWorldBounds();

        // set location
        Vector2 centre = bounds.center;
        emitterObject.transform.Translate(centre);

        // set scale
        var sh = emitterSystem.shape;
        sh.shapeType = ParticleSystemShapeType.Box;
        sh.scale = bounds.size;

        // set material
        ParticleSystemRenderer particleRenderer = emitterSystem.GetComponent<ParticleSystemRenderer>();
        particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        particleRenderer.material.mainTexture = spriteRenderer.sprite.texture;

        ParticleSystem.CollisionModule collision = emitterSystem.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.colliderForce = 1.0f;

        collision.SetPlane(0, collider.transform);
    }

    public void CreateParticleAttractor(Rectangle location)
    {
        
    }

    public static NpcParticles GetInstance()
    {
        return SharedInstance;
    }
}
