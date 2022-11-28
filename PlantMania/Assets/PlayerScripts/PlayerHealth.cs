using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Image HealthBar;
    public float pHealth = 100f;
    public float maxHealth = 100f;
    public float hitDamage = 8f;
    public float immuneTime = 0.5f;
    public float timer = 0;

    public GameObject bloodVfx;

    public GameObject deathScreen;
    public GameObject playerUI;

    public Text finalScore;
    public Text finalScoreInUI;

    private void Update()
    {
        HealthBar.fillAmount = pHealth / maxHealth;

        if(timer != 0)
        {
            timer += Time.deltaTime;
        }
        if(timer >= immuneTime)
        {
            timer = 0;
        }


        if(pHealth <= 0)
        {
            playerDeath();  
        }
    }

    //Kodas kai prigauna priesas
    void OnTriggerEnter(Collider other)
    {

        if (timer >= immuneTime || timer == 0)
        {
            if (other.gameObject.tag == "Enemy")
            {
                pHealth -= hitDamage;
                GameObject spVfx = Instantiate(bloodVfx, transform.position, transform.rotation); //creating and destroying explosion
                Destroy(spVfx, 1);
            }
            timer = 0.00001f;
        }
        
    }

    private void playerDeath()
    {
        deathScreen.SetActive(true);

        finalScore.text = finalScoreInUI.text;

        playerUI.SetActive(false);

        Time.timeScale = 0f;
        PauseMenuScript.gameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pHealth = 100f;

    }
}
