using UnityEngine;
using UnityEngine.InputSystem.HID;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpStr;
    [SerializeField] int jumpMax;
    [SerializeField] float grav;

    [SerializeField] int shootDmg;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    Vector3 moveDir;
    Vector3 playerVel;
    int jumpCount;
    float shootTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();

        sprint();
    }

    void movement()
    {
        //increment shoot timer
        shootTimer += Time.deltaTime;

        //reset jumps
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0; 
        }
        //BASIC MOVEMENT
        //getting movement input
        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        //move player
        controller.Move(moveDir * speed * Time.deltaTime);

        //JUMP LOGIC
        jump();
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= grav * Time.deltaTime;

        //SHOOT LOGIC       
        if(Input.GetButton("Fire1") && shootRate <= shootTimer)
        {
            shoot();
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
            speed *= sprintMod;
        else if (Input.GetButtonUp("Sprint"))
            speed /= sprintMod;
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpStr;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name); 
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDmg);
            }
        }
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        //add feedback here

        if( HP <= 0 )
        {
            GameManager.instance.youLose();
        }
    }
}
