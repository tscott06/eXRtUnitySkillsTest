using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] HealthBase health;
    [SerializeField] PlayerCameraController playerCameraController;
    [SerializeField] PlayerInput input;

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
        Debug.Log("Player died");

        playerCameraController.enabled = false;
        input.DeactivateInput();
        GameManager.Instance.DeathScreen();
    }
}
