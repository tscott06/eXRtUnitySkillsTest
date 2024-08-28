using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    [SerializeField] float fireDelay = .5f;
    [SerializeField] int poolSize = 30;

    protected List<BasicBullet> bulletPool = new();
    //private Queue<BasicBullet> chamber = new Queue<BasicBullet>();

    [SerializeField] protected float delayCountDown = 0;
    protected bool cantFire;

    public UnityEvent OnBulletFired;
    public UnityEvent OnFireFail;

    protected virtual void Start()
    {
        InstantiateBullets(poolSize);
    }

    protected virtual void Update()
    {
        FireDelay();
    }

    protected virtual void FireDelay()
    {
        if (!cantFire)
            return;

        delayCountDown -= Time.deltaTime;

        if (delayCountDown <= 0)
        {
            delayCountDown = 0;
            cantFire = false;
            return;
        }
              
    }

    protected virtual void InstantiateBullets(int number)
    {
        for (int i = 0; i < number; i++) 
        {
            GameObject goInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            goInstance.SetActive(false);

            BasicBullet bullet = goInstance.GetComponent<BasicBullet>();

            bulletPool.Add(bullet);
        }
    }

    public virtual void TryPullTrigger()
    {
        if(cantFire)
        {
            //Just adebug message for now
            //Debug.LogError("CAN'T FIRE NOW - COOLDOWN IN EFFECT!");
            OnFireFail.Invoke();
            return;
        }

        BasicBullet bulletToFire = GetUnfiredBulletFromPool();
        FireBullet(bulletToFire);
        //Should using pooling really

    }

    protected virtual void FireBullet(BasicBullet bulletToFire)
    {
        bulletToFire.FireBulletFromPoint(firePoint);
        ApplyFireDelay();

        OnBulletFired.Invoke();
        Debug.Log("PEW!");
    }

    public virtual BasicBullet GetUnfiredBulletFromPool()
    {
        BasicBullet bullet = bulletPool.Find(b => !b.IsFired);

        if(bullet == null)
        {
            Debug.LogError("Could not find unfired bullet in pool! Increasing pool!");
            InstantiateBullets(poolSize / 4);
            bullet = bulletPool.Find(b => !b.IsFired);
        }

        return bullet;
    }

    protected virtual void ApplyFireDelay()
    {
        cantFire = true;
        delayCountDown = fireDelay;
    }



    protected virtual void OnValidate()
    {
        if(bulletPrefab != null)
        {
            BasicBullet bullet = bulletPrefab.GetComponentInChildren<BasicBullet>();

            if (bullet == null)
            {
                Debug.LogError("Bullet prefab should have Bullet component");
            }
        }
    }
}
