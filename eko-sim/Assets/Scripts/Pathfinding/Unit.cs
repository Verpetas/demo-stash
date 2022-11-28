using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public float speed = 1750;
    public float turnSpeed = 1;

    public Transform target;
    Vector3[] path;
    int targetIndex;

    Rigidbody seekerRB;

    public LayerMask groundMask;

    private void Start()
    {
        seekerRB = GetComponent<Rigidbody>();
        StartCoroutine(UpdatePath());
        //PathRequestManager.RequestPath(transform, target, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;

            if(path.Length != 0)
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            PathRequestManager.RequestPath(transform, target, OnPathFound);
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator FollowPath()
    {

        Vector3 currentWaypoint = path[0];
        float turnAmount = 0;
        Vector3 oldForce = DirectForce(path[0]);

        while (true)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) < 1f)
            {
                Debug.Log("within");

                oldForce = DirectForce(currentWaypoint);

                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
                turnAmount = 0;
            }

            if (turnAmount < 1)
            {
                turnAmount += turnSpeed / 1000;
            }
            else
            {
                turnAmount = 1;
            }

            Vector3 newForce = DirectForce(currentWaypoint);

            Vector3 forceDirection = Vector3.Lerp(oldForce, newForce, turnAmount).normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(forceDirection, transform.up), transform.up);

            seekerRB.AddForce(forceDirection * speed * Time.deltaTime);

            Debug.DrawRay(transform.position, transform.forward * 5, Color.red, 0.01f);
            //Debug.DrawRay(transform.position + Vector3.up * 0.25f, seekerRB.velocity, Color.blue, 0.01f);

            yield return null;
        }

    }

    Vector3 DirectForce(Vector3 waypoint)
    {
        Vector3 bodyUp = transform.up;
        RaycastHit hit;
        Physics.Raycast(transform.position, -bodyUp, out hit, 500f, groundMask);

        Vector3 bodyDirection = Vector3.ProjectOnPlane(waypoint, bodyUp);
        Vector3 forceDirection = (Quaternion.FromToRotation(bodyUp, hit.normal) * bodyDirection).normalized;

        return forceDirection;
    }

    Quaternion RotateOnSphere(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 forward = Vector3.ProjectOnPlane(direction, transform.up);
        Quaternion forwardRotation = Quaternion.LookRotation(forward, transform.up);

        return forwardRotation;
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
}
