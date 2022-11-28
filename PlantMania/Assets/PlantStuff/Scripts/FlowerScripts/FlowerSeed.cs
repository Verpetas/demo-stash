using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSeed : MonoBehaviour
{
    FlowerGrow_v2 flower;

    // Start is called before the first frame update
    void Start()
    {
        flower = gameObject.GetComponent<FlowerGrow_v2>();
        flower.spawnFlower(transform.position);
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Water")
        {
            flower.Water();
        }
    }
}
