using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CocoonScript : MonoBehaviour
{
    public GameObject spider;
    public Transform spawnPoint;
    public float spawnTime = 5000;

    [SerializeField] private Image loader;

    public float i = 1;
    
    void Update()
    {
        loader.fillAmount = i / spawnTime;

        if (!PauseMenuScript.gameIsPaused)
        {
            if (i >= spawnTime)
            {
                InstantiateSpider(spawnPoint);
                i = 0;
            }
            i++;
        }
    }
    private void InstantiateSpider(Transform spawnPoint) //create bee at spawnpoint
    {
        var projectileObj = Instantiate(spider, spawnPoint.position, Quaternion.identity) as GameObject;
    }
}
