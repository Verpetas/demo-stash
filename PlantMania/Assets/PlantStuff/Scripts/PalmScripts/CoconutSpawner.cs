using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutSpawner : MonoBehaviour
{

    [SerializeField] GameObject coconut;
    [SerializeField] private float popInDuration = 0.5f;

    private int cocoCount;

    private GameObject[] coconuts;
    private Vector3[] cocoScale;

    private float startTime;
    private bool spawned = false;

    [SerializeField]
    private AnimationCurve growthCurve;

    public void SpawnCoconuts(Transform trunk)
    {

        coconuts = new GameObject[cocoCount];
        cocoScale = new Vector3[cocoCount];

        for (int i = 0; i < cocoCount; i++)
        {
            coconuts[i] = SpawnCoconut(trunk);
            coconuts[i].transform.localScale = Vector3.zero;
            cocoScale[i] = coconut.transform.localScale * Random.Range(0.75f, 1.25f);
        }

        spawned = true;
        startTime = Time.time;

    }

    private GameObject SpawnCoconut(Transform trunk)
    {
        Quaternion rotation = trunk.rotation * Quaternion.Euler(RandomVector(0f, 360f));
        float randomAngle = Random.Range(0f, 360f);
        Vector3 position = trunk.position + Quaternion.AngleAxis(randomAngle, trunk.up) * trunk.forward * 0.85f + new Vector3(0f, -0.4f, 0f);

        return Instantiate(coconut, position, rotation, gameObject.transform);
    }

    void Start()
    {
        cocoCount = Random.Range(1, 4);
    }

    void Update()
    {
        if (spawned)
        {
            float stage = (Time.time - startTime) / popInDuration;

            for (int i = 0; i < cocoCount; i++)
            {

                coconuts[i].transform.localScale = cocoScale[i] * growthCurve.Evaluate(stage);

            }

            if (stage > 1.0f)
                Destroy(this);

        }
    }

    static Vector3 RandomVector(float min, float max)
    {
        return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
    }

}
