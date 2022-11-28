using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explodeScript : MonoBehaviour
{
    public GameObject explosion;
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Bullet")
            {
                GameObject bloodVfx = Instantiate(explosion, transform.position, transform.rotation); //creating and destroying blood;
                Destroy(bloodVfx, 5);
                Destroy(gameObject);
            }
        }
}
