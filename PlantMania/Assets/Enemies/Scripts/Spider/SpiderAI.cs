using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderAI : MonoBehaviour
{
    Transform player;

    public GameObject blood;
    public GameObject deathExplosion;

    public float rotationSpeed = 4f;
    public float moveSpeed = 6f;
    public float viewDistance = 30f;
    public LayerMask playerMask;

    [SerializeField] private Image enemyHealthBar;
    public float enemyHealth = 50f;
    public float maxHealth = 50f;
    public float damage = 15f;

    public float chooseTime = 2f;
    public float elapsedTime;

    public bool canFindPlayer;

    public float percentage;
    public Quaternion rotation;
    public Quaternion old;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    void Update()
    {

        manageHealth();

        canFindPlayer = Physics.CheckSphere(transform.position, viewDistance, playerMask);
        if (canFindPlayer)
        {
            moveToPlayer();
        }
        else
        {
            wander();
        }
    }

    private void manageHealth()
    {
        enemyHealthBar.fillAmount = enemyHealth / maxHealth;
        if (enemyHealth <= 0)
        {
            GameObject deathVfx = Instantiate(deathExplosion, transform.position, rotation); //creating and destroying blood;
            Destroy(deathVfx, 5);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerBody")
        {
            player.GetComponent<Rigidbody>().AddExplosionForce(3000, transform.position - new Vector3(0f, 1f, 0f), 20);
        }
        if (other.gameObject.tag == "Bullet")
        {
            enemyHealth -= damage;
            GameObject bloodVfx = Instantiate(blood, transform.position + new Vector3(0f, 1f, 0f), rotation); //creating and destroying blood;
            Destroy(bloodVfx, 5);
        }
    }

    void moveToPlayer()
    {
        //rotation to player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position + new Vector3(0f, 1f, 0f)), Time.deltaTime * rotationSpeed);

        //moving to player
        transform.position += transform.forward * moveSpeed * 3 * Time.deltaTime;
        old = transform.rotation;
        elapsedTime = 0;
    }

    void wander()
    {
        elapsedTime += Time.deltaTime;
        percentage = elapsedTime / chooseTime;

        transform.localRotation = Quaternion.Slerp(old, rotation, percentage); //rotate randomly every fame
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if (percentage > 1) //if ended rotation start new rotation
        {
            old = transform.localRotation;
            rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            elapsedTime = 0;
        }
    }

}
