using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedProjectile : MonoBehaviour
{
    //public PlantType plantType = new PlantType();

    //public enum PlantType
    //{
    //    Palm,
    //    Vine,
    //    Flower
    //};

    public string plantType;
    private bool collided;

    public GameObject palmSeed;
    public GameObject flowerSeed;
    public GameObject vineSeed;

    void Start()
    {
        Destroy(gameObject, 2);
    }
    
    private void OnCollisionEnter(Collision c)
    {

        if (c.gameObject.tag != "Bullet" && !collided && c.gameObject.tag != "Enemy")
        {
            collided = true;
            Destroy(gameObject, 0.5f);

            // SAPWNS ROSE
            if (plantType.ToString() == "Flower")
            {
                SpawnFlower(c);
            }

            // SPAWNS PALM
            else if (plantType.ToString() == "Palm")
            {
                SpawnPalm(c);
            }

            // SPAWNS VINE
            else if (plantType.ToString() == "Vine")
            {
                SpawnVine(c);
            }
        }
        if (c.gameObject.tag == "Enemy" && !collided)
        {
            collided = true;
            Destroy(gameObject, 0.1f);
        }
    }

    public void SetSeedType(string seedType)
    {
        plantType = seedType;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !collided)
        {
            collided = true;
            Destroy(gameObject, 0.1f);
        }
    }

    void SpawnFlower(Collision collision)
    {
        Instantiate(flowerSeed, collision.contacts[0].point, Quaternion.identity);
    }

    void SpawnPalm(Collision collision)
    {
        GameObject palmSeedUnit = Instantiate(palmSeed, collision.contacts[0].point, Quaternion.identity);
        palmSeedUnit.GetComponent<PalmSeed>().Plant();
    }

    void SpawnVine(Collision collision)
    {
        GameObject vineSeedUnit = Instantiate(vineSeed, collision.contacts[0].point, Quaternion.identity);
        vineSeedUnit.GetComponent<VineSeed>().SpawnVine(collision.contacts[0].point, collision.contacts[0].normal);
    }

}
