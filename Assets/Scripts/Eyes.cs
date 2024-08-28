using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    public Transform leftEye;
    public Transform rightEye;

    public Transform currentTarget;
    private bool isLookingAhead;

    private void Update()
    {
        if (currentTarget == null && !isLookingAhead)
        {
            LookAhead();
        }
      
        LookAtTarget(currentTarget);
        
    }

    private void LookAhead()
    {
        leftEye.rotation = Quaternion.Euler(leftEye.forward);
        rightEye.rotation = Quaternion.Euler(leftEye.forward);

        isLookingAhead = true;
    }


    private void LookAtTarget(Transform target)
    {
        leftEye.LookAt(target);
        rightEye.LookAt(target);

    }
}
