using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WateringCan : MonoBehaviour
{
    public Camera cam;
    public GameObject water;
    public Transform firePoint;
    public Transform splashPoint;
    public Transform fillPoint;

    public LayerMask mask;
    public float reach = 4f;
    [SerializeField] private Text fillText;

    public float speed = 10;
    public float fireRate = 20;


    private float timeToFire;
    private Vector3 destination;

    PlayerMovement pm;
    private bool wasGrounded = true;
    public GameObject splashVfx;
    public GameObject fillVfx;

    public float ammo = 100f;
    [SerializeField] private Image ammoBar;

    private void Start()
    {
        pm = FindObjectOfType<PlayerMovement>(); //refference to get isGrounded from PlayerMovement
    }

    private void Update()
    {
        manageFilling();

        ammoBar.fillAmount = ammo / 100;
        
        if (wasGrounded == false && pm.isGrounded == true)
        {
            splash();
            ammo -= 1;
        }
        wasGrounded = pm.isGrounded;


        if (Input.GetButton("Fire1") && Time.time >= timeToFire && ammo > 0 && !PauseMenuScript.gameIsPaused)
        {
            timeToFire = Time.time + 1 / fireRate; 
            Shoot();
        }
    }

    private void splash()
    {
        GameObject spVfx = Instantiate(splashVfx, splashPoint.position, splashPoint.rotation); //creating and destroying explosion
        Destroy(spVfx, 1);
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            destination = hit.point;
        }
        else
        {
            destination = ray.GetPoint(1000);
        }
        InstantiateWater(firePoint);
        ammo -= 1;
    }

    private void manageFilling()
    {

        Ray fillRay = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit fillHit;

        if (Physics.Raycast(fillRay, out fillHit, reach, mask) && ammo < 100)
        {
            fillText.enabled = true;

            if (Input.GetKeyDown(KeyCode.E))
            {

                ammo = 100;
                GameObject fVfx = Instantiate(fillVfx, fillPoint.position, fillPoint.rotation);
                Destroy(fVfx, 2);
            }

        }
        else fillText.enabled = false;
    }

    private void InstantiateWater(Transform firePoint)
    {
        var projectileObj = Instantiate(water, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * speed;
    }
}
