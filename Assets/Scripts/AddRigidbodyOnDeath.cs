using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRigidbodyOnDeath : MonoBehaviour
{
    [SerializeField] HealthBase healthBase;

    private void OnEnable()
    {
        healthBase.OnZeroHealth.AddListener(DeathHandler);
    }

    private void DeathHandler(ZeroHealthArgs arg0)
    {
        AddRigidbody();
    }

    private void AddRigidbody()
    {
        var rb = gameObject.AddComponent<Rigidbody>();

        rb.AddForce(-gameObject.transform.forward * 5);

        Destroy(this);
    }
}
