using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Obsolete("Was getting bugs with this approach, using unity's player input instead")]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] InputActionProperty movementInput;
    [SerializeField] InputActionProperty rotationInput;
    [SerializeField] InputActionProperty fireInput;

    [SerializeField] Movement movement;
    [SerializeField] PlayerCameraController playerCamera;
    [SerializeField] Gun gun;


    private void Start()
    {
        if(movement == null)
            movement = GetComponent<Movement>();

        if(gun == null)
            gun = GetComponentInChildren<Gun>();

        movementInput.action.actionMap.Enable();
    }

    private void OnEnable()
    {
        movementInput.action.started += MoveInputHandler;
        movementInput.action.started += playerCamera.LookInputHandler;
        rotationInput.action.started += TurnInputHandler;
        fireInput.action.started += FireInputHandler;
    }

    private void OnDisable()
    {
        movementInput.action.started -= MoveInputHandler;
        movementInput.action.started -= playerCamera.LookInputHandler;
        rotationInput.action.started -= TurnInputHandler;
        fireInput.action.started -= FireInputHandler;
    }


    public void TurnInputHandler(InputAction.CallbackContext context)
    {
        if (!CheckCorrectInputType(context.valueType, typeof(Vector2)))
            return;

        movement.TurnInput(context.ReadValue<Vector2>());
    
    }

    public void MoveInputHandler(InputAction.CallbackContext context)
    {
        if (!CheckCorrectInputType(context.valueType, typeof(Vector2)))
            return;

        movement.MoveInput(context.ReadValue<Vector2>());
    }

    public void FireInputHandler(InputAction.CallbackContext context) 
    {
        if (!CheckCorrectInputType(context.valueType, typeof(Single)))
            return;

        gun.TryPullTrigger();
    }


    private bool CheckCorrectInputType(System.Type inputType, System.Type expectedType)
    {
        if (inputType != expectedType)
        {
            Debug.LogError($"Incorrect type of input! Input type recieved {expectedType}. Type received {inputType}");
            return false;
        }

        return true;
    }
}
