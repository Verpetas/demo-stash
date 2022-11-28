using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CocoHarvest : MonoBehaviour
{

    public Camera cam;
    public float reach = 5f;
    public int ammoRewardSize = 20;

    [SerializeField] private Text harvestText;

    RaycastHit cocoHit;
    Seeder seederGun;

    // Start is called before the first frame update
    void Start()
    {
        harvestText.enabled = false;
        seederGun = gameObject.transform.Find("SeederGun").gameObject.GetComponent<Seeder>();
    }

    // Update is called once per frame
    void Update()
    {

        cocoHit = Collision(cam.transform.position, cam.transform.forward, reach);

        if (cocoHit.collider)
        {
            //Debug.Log(cocoHit.collider.tag);
            if (cocoHit.collider.tag == "Coconut")
            {
                harvestText.enabled = true;
            }
            else harvestText.enabled = false;
        }
        else harvestText.enabled = false;

        if(Input.GetKeyDown(KeyCode.E) && harvestText.enabled)
        {
            Destroy(cocoHit.collider.gameObject);

            int addAmmo = (int)Random.Range(ammoRewardSize * 0.75f, ammoRewardSize * 1.25f);

            seederGun.ammo += addAmmo;
        }
    }



    static RaycastHit Collision(Vector3 origin, Vector3 direction, float maxDistance)
    {
        RaycastHit hitInfo;

        Physics.Raycast(origin, direction, out hitInfo, maxDistance);

        return hitInfo;
    }

}
