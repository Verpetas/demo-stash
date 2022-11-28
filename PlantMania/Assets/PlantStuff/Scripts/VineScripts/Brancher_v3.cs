using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class Brancher_v3 : MonoBehaviour
{
    [SerializeField] GameObject vinePart;
    [SerializeField] GameObject vineLeaf;
    [SerializeField] int childProbabilityDivisor = 30;
    [SerializeField] float stepDistance = 0.2f;
    [SerializeField] float pointCountFalloff = 0.5f;
    [SerializeField] float maxTerrainAngle = 10;
    [SerializeField] int leafProbabilityDivisor = 10;
    [SerializeField] float curviness = 10;
    [SerializeField] float size = 1;

    int brokenAt = 0;

    Transform branchesStore;
    Transform leavesStore;

    private void Awake()
    {
        branchesStore = gameObject.transform.Find("Leaves");
        leavesStore = gameObject.transform.Find("Branches");
    }

    public IEnumerator SpawnNextPart(Vector3 firstPointCoords, Vector3 surfaceNormal, int pointCount, Vector3 direction)
    {
        //Add a Spline Computer component to this object
        GameObject vineGO = Instantiate(vinePart, branchesStore);
        SplineComputer vineSC = vineGO.GetComponent<SplineComputer>();

        SplinePoint[] points = new SplinePoint[pointCount];

        Vector3 upDirection = surfaceNormal;
        GameObject rotationReference = new GameObject();
        rotationReference.transform.up = upDirection;

        Vector3 currentPoint = firstPointCoords;
        Vector3 formerPoint;
        Vector3 sideVector;

        RaycastHit frontCollision;
        RaycastHit groundCollision;
        RaycastHit collision;

        float terrainAngle;
        float curveAngle;

        List<GameObject> leaves = new List<GameObject>();

        bool broken = false;

        for (int i = 0; i < pointCount; i++)
        {
            points[i].position = currentPoint - upDirection * size * 0.1f;
            points[i].normal = upDirection;
            points[i].size = size;

            formerPoint = currentPoint;

            sideVector = Quaternion.AngleAxis(-90, upDirection) * direction;

            if (Random.Range(0, leafProbabilityDivisor) == 0 && !broken)
            {
                leaves.Add(SpawnLeaf(upDirection, currentPoint, rotationReference.transform.rotation));
            }

            if (Random.Range(0, childProbabilityDivisor) == 0 && !broken)
            {
                // child spawn code here
                int newPointCount = (int)Mathf.Round((float)pointCount * pointCountFalloff);
                StartCoroutine(gameObject.GetComponent<Brancher_v3>().SpawnNextPart(currentPoint, upDirection, newPointCount, direction));
            }

            currentPoint += stepDistance * direction;

            frontCollision = Collision(currentPoint, direction, stepDistance);
            groundCollision = Collision(currentPoint, -upDirection, stepDistance * 3);

            collision = groundCollision;

            if(frontCollision.collider != null)
            {
                collision = frontCollision;
            }
            else if (groundCollision.collider != null)
            {
                collision = groundCollision;
            }
            else if (i < pointCount-1)
            {
                formerPoint = currentPoint;

                i++;

                points[i].position = currentPoint - upDirection * size * 0.1f;
                points[i].size = size;

                upDirection = Quaternion.AngleAxis(-90, sideVector) * upDirection;

                points[i].normal = upDirection;

                direction = Quaternion.AngleAxis(-90, sideVector) * direction;

                currentPoint += stepDistance * direction;

                frontCollision = Collision(currentPoint, direction, stepDistance);
                groundCollision = Collision(currentPoint, -upDirection, stepDistance * 5);

                //Debug.DrawRay(currentPoint, direction * 10, Color.red, 999);

                if (frontCollision.collider != null)
                {
                    collision = frontCollision;
                }
                else if (groundCollision.collider != null)
                {
                    collision = groundCollision;
                }
                else
                {
                    if (!broken)
                    {
                        broken = true;
                        brokenAt = i;
                    }

                    //Debug.DrawRay(currentPoint, Vector3.up * 10, Color.red, 999);
                }
            }
            else
            {
                if (!broken)
                {
                    broken = true;
                    brokenAt = i;
                }

                //Debug.DrawRay(currentPoint, Vector3.up * 10, Color.red, 999);
            }

            upDirection = collision.normal;

            if (!broken)
            {
                currentPoint = collision.point + collision.normal * size * 0.3f;
            }

            rotationReference.transform.up = upDirection;

            terrainAngle = Vector3.Angle(upDirection, points[i].normal);
            curveAngle = Random.Range(-curviness, curviness);
            direction = Quaternion.AngleAxis(terrainAngle, sideVector) * Vector3.Normalize(currentPoint - formerPoint);
            direction = Quaternion.AngleAxis(curveAngle, upDirection) * direction;

            if(pointCount / 5 != 0)
            {
                if (i % (pointCount / 5) == 0)
                {
                    yield return null;
                }
            }

        }

        vineSC.SetPoints(points);

        if (broken)
        {
            SplineMesh flowerSM = vineGO.GetComponent<SplineMesh>();
            flowerSM.GetChannel(0).clipTo = (float)brokenAt / pointCount;
        }

        StartCoroutine(SpawnLeaves(leaves));

    }

    IEnumerator SpawnLeaves(List<GameObject> leaves)
    {
        foreach (GameObject leaf in leaves)
        {
            if(leaf != null)
                leaf.GetComponent<Renderer>().enabled = true;

            yield return new WaitForSeconds(0.1f);
        }
    }

    GameObject SpawnLeaf(Vector3 up, Vector3 point, Quaternion rotation)
    {

        float angle = Random.Range(0f, 360f);
        rotation *= vineLeaf.transform.rotation;
        rotation *= Quaternion.AngleAxis(angle, Vector3.forward);

        //Debug.DrawRay(point, up * 10, Color.red, 999);

        GameObject leaf = Instantiate(vineLeaf, point, rotation, leavesStore);
        leaf.transform.position += up * size * 0.1f;
        leaf.transform.localScale *= size * 0.5f * Random.Range(0.5f, 1.5f);

        leaf.GetComponent<Renderer>().enabled = false;

        return leaf;
    }

    static RaycastHit Collision(Vector3 origin, Vector3 direction, float maxDistance)
    {
        RaycastHit hitInfo;

        Physics.Raycast(origin, direction, out hitInfo, maxDistance);

        return hitInfo;
    }

}
