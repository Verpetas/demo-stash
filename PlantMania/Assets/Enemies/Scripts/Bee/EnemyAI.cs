using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    Transform player;

    GameObject beeObject;
    GameObject bucket;
    GameObject blade;

    public GameObject blood;
    public GameObject deathExplosion;

    public float rotationSpeed = 8f;
    public float moveSpeed = 3f;
    public float viewDistance = 20f;
    public LayerMask playerMask;

    [SerializeField] private Image enemyHealthBar; 
    public float enemyHealth = 100f;
    public float maxHealth = 100f;
    public float damage = 10f;

    public float chooseTime = 2f;
    public float elapsedTime;

    public bool canFindPlayer;

    public float percentage;
    public Quaternion rotation;
    public Quaternion old;

    [SerializeField]
    private Texture defaultTexture;
    [SerializeField]
    private Texture angryTexture;
    [SerializeField]
    private Renderer beeRenderer; //for texture change

    RaycastHit hit;
    public float flyHeight = 4f;
    public float chaseHeight = 2f;
    public float heightSpeed = 6f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;


        bucket = transform.GetChild(0).GetChild(0).gameObject;
        blade = transform.GetChild(0).GetChild(1).gameObject;

    }
    void Update()
    {

        manageHealth();

        canFindPlayer = Physics.CheckSphere(transform.position, viewDistance, playerMask);
        if (canFindPlayer)
        {
            beeRenderer.material.mainTexture = angryTexture; //change texture and go to player if able to see him
            moveToPlayer();
            ground(chaseHeight);
        }
        else
        {
            beeRenderer.material.mainTexture = defaultTexture; //change texture and wander if cannot see player
            wander();
            ground(flyHeight);
        }
            
    }

    private void manageHealth()
    {
        enemyHealthBar.fillAmount = enemyHealth / maxHealth;
        if(enemyHealth <= 0)
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
            player.GetComponent<Rigidbody>().AddExplosionForce(3000, transform.position - new Vector3(0f, 3f, 0f), 20);
        }
        if(other.gameObject.name == "WorldTerrain")
        {
            transform.position += transform.up * moveSpeed * heightSpeed * Time.deltaTime;
        }
        if(other.gameObject.tag == "Enemy")
        {
            transform.position = Vector2.MoveTowards(transform.position, other.transform.position, -1 * Time.deltaTime);
        }
        if (other.gameObject.tag == "Bullet")
        {
            enemyHealth -= damage;

            GameObject bloodVfx = Instantiate(blood, transform.position, rotation); //creating and destroying blood;
            Destroy(bloodVfx, 5);
        }
    }
    void moveToPlayer()
    {
        if (bucket.activeSelf)
        {
            bucket.SetActive(false);
            blade.SetActive(true);
        }
        //rotation to player
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position + new Vector3(0f, 1f, 0f)), Time.deltaTime * rotationSpeed);

        //moving to player
        transform.position += transform.forward * moveSpeed * 3 * Time.deltaTime;
        old = transform.rotation;
        elapsedTime = 0;
    }

    void ground(double height)
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.cyan);

            if(hit.distance < height - 0.4) //making the height less exact by adding or subtracting 0.2 to avoid jitter
            {
                transform.position += transform.up * heightSpeed * Time.deltaTime;
            }
            if (hit.distance > height + 0.4)
            {
                transform.position -= transform.up * heightSpeed * Time.deltaTime;
            }



        }
    }

    void wander()
    {
        if (blade.activeSelf)
        {
            bucket.SetActive(true);
            blade.SetActive(false);
        }

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
