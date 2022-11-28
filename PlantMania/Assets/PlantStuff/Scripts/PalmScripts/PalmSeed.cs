using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmSeed : MonoBehaviour
{

    [SerializeField] GameObject part;
    [SerializeField] private float maxAngle = 12.5f;
    [SerializeField] private int maxPartCount = 14;
    [SerializeField] private int waterRequired = 10;

    private int partCount = 1;

    int waterLevel = 0;

    GameObject nextPart;

    LeafSpawner leaf;
    Transform leaves;

    CoconutSpawner coconut;
    Transform coconuts;

    Transform parts;

    public void Plant()
    {

        Quaternion newRot = transform.rotation * Quaternion.Euler(RandomVectorYPlane(maxAngle));

        parts = gameObject.transform.Find("Parts");
        nextPart = Instantiate(part, gameObject.transform.position, newRot, parts);
        nextPart.GetComponent<PartSpawner_v2>().Initiate(0);

        leaves = gameObject.transform.Find("Leaves");
        leaf = leaves.GetComponent<LeafSpawner>();
        leaf.SpawnLeaves(gameObject.transform);
        leaves.transform.localScale = Vector3.one * (partCount + 10) / (maxPartCount + 10);

        coconuts = gameObject.transform.Find("Coconuts");
        coconut = coconuts.GetComponent<CoconutSpawner>();
    }

    void Update()
    {

        if (partCount >= maxPartCount)
        {
            coconut.SpawnCoconuts(nextPart.transform);

            Destroy(this);
        }
        else if (waterLevel > waterRequired)
        {
            nextPart = nextPart.GetComponent<PartSpawner_v2>().SpawnNextPart();

            Vector3 leavesOffset = nextPart.transform.up * -(1 - nextPart.transform.localScale.x / 100) * 2.5f;

            leaves.position = nextPart.transform.position + leavesOffset;
            leaves.rotation = nextPart.transform.rotation;
            leaves.localScale = Vector3.one * (partCount + 10) / (maxPartCount + 10);

            partCount++;
            waterLevel = 0;
        }
    }

    static Vector3 RandomVectorYPlane(float offset)
    {
        return new Vector3(Random.Range(-offset, offset), 0, Random.Range(-offset, offset));
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Water")
        {
            waterLevel += 2;
        }
    }
}
