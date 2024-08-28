using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] Gun gun;

    //Raycast from arbitrary point
    [SerializeField] Transform currentTarget;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float aimDistance;

    [SerializeField] string playerTag = "Player";

    [SerializeField] LayerMask gunRaycastLayerMask;

    [SerializeField] Eyes eyes;

    private Vector3 currentPoint;


    private void Start()
    {
        //For simpicity's sake, enermy will always seek player
        if(currentTarget == null)
            currentTarget = GameObject.FindWithTag(playerTag).GetComponent<Transform>();

        eyes.currentTarget = currentTarget;

    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardPlayer();

        bool hasAim = TryAimAtPlayer();
        if(hasAim)
            TryFire();

    }

    private void MoveTowardPlayer()
    {
        
        agent.SetDestination(currentTarget.transform.position);
    }

    private bool TryAimAtPlayer()
    {
        //gun.transform.LookAt(currentTarget); //Ideally would smooth this out

        var gunTransform = gun.transform;

        Vector3 directionToPlayer = (currentTarget.position - gunTransform.position).normalized;
        Ray ray = new Ray(gun.firePoint.position, directionToPlayer);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, aimDistance, gunRaycastLayerMask))
        {
            GameObject go = hitInfo.collider.gameObject;

            Debug.Log("Ray cast");

            if (go.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("Can see player");
                gunTransform.LookAt(go.transform);
                return true;
            }
        }
        Debug.Log("Can't see player");

        return false;
    }

    private void TryFire()
    {
        if (gun.FireCooldownActive)
            return;

        gun.TryPullTrigger();
    }

}
