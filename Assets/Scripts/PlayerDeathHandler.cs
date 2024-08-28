using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] HealthBase health;
    [SerializeField] PlayerInputController input;
    [SerializeField] Rigidbody rb;

    private void OnEnable()
    {
        health.OnZeroHealth.AddListener(Die);
    }

    private void OnDisable()
    {
        health.OnZeroHealth.RemoveListener(Die);
    }

    private void Die(ZeroHealthArgs args)
    {
        input.enabled = false;
        rb.constraints = RigidbodyConstraints.None;
    }
}
