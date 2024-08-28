using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Aims gun by using a raycast from a given point and point gun at hit
/// </summary>
public class PlayerGunAimer : MonoBehaviour
{
    [SerializeField] Gun gun;

    //Raycast from camera - overrides aimFrom transform below
    [SerializeField] bool aimFromCamera; 

    //Raycast from arbitrary point
    [SerializeField] Transform aimFrom;


    [SerializeField] private float distance;

    [SerializeField] LayerMask layerMask;

    private Vector3 currentPoint;
   

    private void Start()
    {
        if(aimFromCamera)
            aimFrom = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(aimFrom.position, aimFrom.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask))
        {
            currentPoint = hitInfo.point;
        } else
        {
            currentPoint = aimFrom.TransformPoint(0,0,distance);
        }

        gun.transform.LookAt(currentPoint); //Ideally would smooth this out
    }

}
