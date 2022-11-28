using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolSwitch : MonoBehaviour

{
    public static int selectedWeapon = 0;
    public Vector3 endPosition = new Vector3(0.7f, -1.2f, 0.9f);
    public Vector3 startPosition = new Vector3(0.7f, -0.7f, 0.9f);
    public float duration = 0.4f;
    private float elapsedTime;
    private bool weaponDown = false;

    private int previousWeapon = 1;

    [SerializeField] private Image ammoBar;
    [SerializeField] private Image ammoBarBackground;

    [SerializeField] private Text seedTypeText;
    [SerializeField] private Text ammoText;

    [SerializeField] private Image infiniteAmmo;

    [SerializeField] private Text fillText;

    void Start()
    {
        fillText.enabled = false;
    }

    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) //switching with scroll wheel
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) //switching with number keys
        {
            selectedWeapon = 0; //seeder
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1; //watering can
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2; //rocket launcher
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3; //map
        }

        if (previousWeapon != selectedWeapon || weaponDown) //do switching if player switched or the switching hasn't ended yet
        {
            if (!weaponDown)
            {
                StartSwitch();
            }
            if (weaponDown)
            {
                EndSwitch();
            }
        }


    }

    void manageUI()
    {
        float percentage = elapsedTime / duration;
        if (selectedWeapon == 0)
        {
            ammoText.enabled = true;
            seedTypeText.enabled = true;
        }
        else
        {
            ammoText.enabled = false;
            seedTypeText.enabled = false;
        }

        if (selectedWeapon == 1)
        {
            ammoBar.enabled = true;
            ammoBarBackground.enabled = true;
        }
        else
        {
            ammoBar.enabled = false;
            ammoBarBackground.enabled = false;
        }

        if (selectedWeapon == 2)
        {
            infiniteAmmo.enabled = true;
        }
        else
        {
            infiniteAmmo.enabled = false;
        }
    }

    void SelectWeapon()
    {
        previousWeapon = selectedWeapon;
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
    void StartSwitch()
    {
        elapsedTime += Time.deltaTime;
        float percentage = elapsedTime / duration;

        transform.localPosition = Vector3.Lerp(startPosition, endPosition, percentage); //lower weapon while switching

        if(percentage >= 1) //change weapon if lowering animation ended
        {
            elapsedTime = 0;
            weaponDown = true;
            SelectWeapon();
            manageUI();
        }
    }
    void EndSwitch()
    {
        elapsedTime += Time.deltaTime;
        float percentage = elapsedTime / duration;

        transform.localPosition = Vector3.Lerp(endPosition, startPosition, percentage); //switching in animation

        if (percentage >= 1) //stop switching if animation ended
        {
            elapsedTime = 0;
            weaponDown = false;
            manageUI();
        }
    }
}
