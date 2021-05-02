using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;

    public Transform pathHolder;
    public float speed = 888;
    public float turnSpeed = 90;
    public float breakeTime = 2;

    public bool patroling = true;

    public float timeToSpotPlayer;
    public float playerVisibleTimer;

    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    Color originalSpotlightColor;

    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        originalSpotlightColor = spotlight.color;

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position + new Vector3 (0, transform.position.y, 0);
        }

        StartCoroutine(PatrolWaypoints(waypoints));   
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            //StartCoroutine(TimeToEscape());
            playerVisibleTimer += Time.deltaTime;
        }
        else{
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (Mathf.Abs(angleBetweenGuardAndPlayer) < viewAngle /2)
            {
                if(!Physics.Linecast(transform.position, player.position, viewMask))
                {                    
                    return true;
                }
            }
        }

        return false;
    }


    IEnumerator TimeToEscape()
    {
        spotlight.color = Color.red;

        yield return new WaitForSeconds(timeToSpotPlayer);

        if (CanSeePlayer())
        {
            Debug.Log("Game Over!!");
            // Game Over
        }      
    }

    IEnumerator PatrolWaypoints(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (patroling)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(breakeTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;  // Zaczekanie jedną klatke aby ruch był płynny
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 direction = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    //IEnumerator Move(Vector3 destination, float speed)
    //{
    //    while (transform.position != destination)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    //        yield return null;
    //    }
    //}

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }

        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
