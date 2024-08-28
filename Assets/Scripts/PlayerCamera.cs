using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private float maxXRotationSpeed = 2f;

    private Vector2 rotation;

    [SerializeField] private Vector3 offset = new Vector3(2, 1, -5);
    [SerializeField] private float distanceFromPlayer;
    private Vector3 currentTrackingVelocity;

    private Vector2 lookMove = Vector3.zero;
    private Vector3 lookOffset = Vector3.zero;

    private Vector3 currentTargetPosition;
    private Vector3 currentTargetRotation;


    private void Start()
    {
        if(playerCamera == null)
            playerCamera = Camera.main;

        if (targetTransform == null)
            targetTransform = transform;

        playerCamera.transform.parent = targetTransform;
        
        
        playerCamera.transform.position = targetTransform.transform.TransformPoint(offset);

    }

    private void Update()
    {
        Look(lookMove);       
    }

    public void LookInputHandler(InputAction.CallbackContext context)
    {
        lookMove = context.ReadValue<Vector2>();
    }


    private void Look(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;

        var scaledRotateSpeed = maxXRotationSpeed * Time.deltaTime;

        rotation.x = Mathf.Clamp(rotation.x - rotate.y * scaledRotateSpeed, -10, 45);
        targetTransform.eulerAngles = new Vector3(rotation.x, targetTransform.eulerAngles.y, targetTransform.eulerAngles.z);

    } 
}
