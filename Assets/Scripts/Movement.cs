using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Component that controls broad mopvement of a character
/// </summary>
public class Movement : MonoBehaviour
{
    [SerializeField] CharacterController characterController;

    [SerializeField] private float maxMoveSpeed = 1.0f;
    [SerializeField] private float maxRotationSpeed = 2f;

    private float rotation;

    private Vector2 turnAmount;
    private Vector2 moveAmount;

    private bool ignoreInput = false;

    private void Update()
    {
        if (ignoreInput)
            return;

        //if(rb == null)
        //    rb = GetComponent<Rigidbody>();

        Turn(turnAmount);
        Move(moveAmount);
    }


    public void MoveInput(Vector2 moveAmount) => this.moveAmount = moveAmount;
    public void TurnInput(Vector2 turnAmount) => this.turnAmount = turnAmount;

    public void OnTurn(InputAction.CallbackContext context)
    {
        turnAmount = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveAmount = context.ReadValue<Vector2>();
    }


    private void Turn(Vector2 rotate)
    {
        if (rotate.sqrMagnitude < 0.01)
            return;

        var scaledRotateSpeed = maxRotationSpeed * Time.deltaTime;

        rotation += rotate.x * scaledRotateSpeed;

        //Quaternion newRotation = Quaternion.Euler(new Vector3(transform.position.x, rotation, transform.position.y));
        //rb.MoveRotation(newRotation);

        
        transform.localEulerAngles = new Vector3(transform.position.x, rotation, transform.position.y);
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01)
            return;

        var scaledMoveSpeed = maxMoveSpeed * Time.deltaTime;

        var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
        //rb.MovePosition(transform.position + move * scaledMoveSpeed);
        characterController.Move(move * scaledMoveSpeed);

        //transform.position += move * scaledMoveSpeed;
    }
}
