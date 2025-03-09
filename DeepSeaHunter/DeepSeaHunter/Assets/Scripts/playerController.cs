using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] int speed;
    //[SerializeField] int sprintMod;
    [SerializeField] int dashStr;
    [SerializeField] int dashMax;
    [SerializeField] float dashRechargeTimer;
    [SerializeField] float dashDuration;

    [SerializeField] int jumpStr;
    [SerializeField] int jumpMax;
    [SerializeField] float grav;

    [SerializeField] int shootDmg;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [SerializeField] int tangleMod;
    Vector3 moveDir;
    Vector3 playerVel;
    int dashCount;
    int jumpCount;
    float shootTimer;
    public bool isTangled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();

        //sprint();
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

        //JUMP/DASH LOGIC
        jump();
        dash();
        //this needs to reduce x and z to zero 0 overtime
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= grav * Time.deltaTime;

        //SHOOT LOGIC       
        if(Input.GetButton("Fire1") && shootRate <= shootTimer)
        {
            shoot();
        }
        //TANGLED TESTING
        if (Input.GetButtonDown("Fire2"))
        {
            toggleTangled();
        }
    }

    /*void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
            speed *= sprintMod;
        else if (Input.GetButtonUp("Sprint"))
            speed /= sprintMod;
    }*/

    void dash()
    {
        if (Input.GetButtonDown("Dash") && dashCount < dashMax)
        {
            dashCount++;
            //this needs math to get the angle right; look at polar to cart coords
            playerVel.x = dashStr * moveDir.x; //x = r cos theta; dashStr = r //z = r sin theta; theta = moveDir
            playerVel.z = dashStr * moveDir.z; //movedir might already has the cart coords
            Debug.Log("Dashed");
            StartCoroutine(endDash());
            StartCoroutine(rechargeDash());
        }
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

    IEnumerator rechargeDash()
    {
        yield return new WaitForSeconds(dashRechargeTimer);
        dashCount--;
        Debug.Log("Dash Recharged");
    }
    IEnumerator endDash()
    {
        yield return new WaitForSeconds(dashDuration);
        playerVel.x = 0;
        playerVel.z = 0;
    }

    public void toggleTangled()
    {
        isTangled = !isTangled;
        if (isTangled)
        {
            speed /= tangleMod; //
            jumpStr /= tangleMod;
            dashStr /= tangleMod;
            dashDuration /= tangleMod;
            shootRate *= tangleMod;
        }
        else
        {
            speed *= tangleMod; //
            jumpStr *= tangleMod;
            dashStr *= tangleMod;
            dashDuration *= tangleMod;
            shootRate /= tangleMod;
        }
    }
}
