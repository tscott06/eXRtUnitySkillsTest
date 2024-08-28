using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Simple bullet behaviour. Treats Z are forward axis. 
public class BasicBullet : MonoBehaviour, IDamageInflictor
{
    [SerializeField] private Rigidbody rb;
    //[SerializeField] private LayerMask layerMask;

    [SerializeField] private float fireForce = 100;
    [SerializeField] private float bulletDamage = 25;
    [SerializeField] private float lifeTime = 5;

    [SerializeField] UnityEvent OnFired;
    [SerializeField] UnityEvent OnHit;

    private float firedTime = 0;

    public bool IsFired;

    public float FiredTime { get => firedTime;  }
    public float FireForce { get => fireForce;  }
    public float BulletDamage { get => bulletDamage;  }
    public float LifeTime { get => lifeTime; }

    private void Update()
    {      
        if (!IsFired)
            return;

        if(firedTime >= lifeTime)
        {
            firedTime = 0;
            DeactivateBullet();
        }

        firedTime += Time.deltaTime;
    }


    public virtual void FireBullet()
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);

        rb.velocity = Vector3.zero;

        rb.AddForce(transform.forward * fireForce, ForceMode.VelocityChange);

        IsFired = true;

        OnFired.Invoke();
    }

    public virtual void FireBulletFromPoint(Transform point)
    {
        transform.position = point.position;
        transform.rotation = point.rotation;

        FireBullet();
    }

    public virtual void InflictDamage(IHealth healthHaver, float damage)
    {
        healthHaver.TakeDamage(this, damage);
    }

    public virtual void DeactivateBullet()
    {
        IsFired = false;
        gameObject.SetActive(false);
        firedTime = 0;
        rb.velocity = Vector3.zero;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        bool hitHealthHaver = collision.collider.TryGetComponent(out IHealth healthHaver);

        //if (!hitHealthHaver)
        //    return;
        if (hitHealthHaver)
            InflictDamage(healthHaver, bulletDamage);

        OnHit.Invoke();

        DeactivateBullet();
        //Destroy if collides with anything
    }
}
