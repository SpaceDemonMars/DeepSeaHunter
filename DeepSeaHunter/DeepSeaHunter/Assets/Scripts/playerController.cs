using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class playerController : MonoBehaviour, IDamage, ITangle
{
    [SerializeField] int HP;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] public int speed;
    //[SerializeField] int sprintMod;
    [SerializeField] int pushResolve;
    [SerializeField] public int dashStr;
    [SerializeField] int dashMax;
    [SerializeField] float dashRechargeTimer;
    [SerializeField] float dashDuration;

    [SerializeField] public int jumpStr;
    [SerializeField] int jumpMax;
    [SerializeField] float grav;

    [SerializeField] int knifeDmg;
    [SerializeField] float knifeRate;
    [SerializeField] int knifeDist;
    [SerializeField] int shootDmg;
    [SerializeField] float shootRate;
    [SerializeField] float shootMin;
    [SerializeField] float shootMax;

    int HPOrig;
    Vector3 moveDir;
    public Vector3 pushDir;
    Vector3 playerVel;
    int dashCount;
    int jumpCount;
    float knifeTimer;
    float shootTimer;
    public float shootDist;
    public bool isTangled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
        shootDist = shootMin;
    }

    // Update is called once per frame
    void Update()
    {
        pushDir = Vector3.Lerp(pushDir, Vector3.zero, Time.deltaTime * pushResolve);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();

        if (shootRate <= shootTimer && GameManager.instance.isPaused == false)
            harpoon();
        //sprint();
        updateReloadUI();
    }

    void movement()
    {
        //increment shoot timer
        knifeTimer += Time.deltaTime;
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
        controller.Move((moveDir + pushDir) * speed * Time.deltaTime);

        //JUMP/DASH LOGIC
        jump();
        dash();
        //this needs to reduce x and z to zero 0 overtime
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= grav * Time.deltaTime;

        //SHOOT LOGIC           
        if (Input.GetButton("Fire1") && knifeRate <= knifeTimer && GameManager.instance.isPaused == false)
        {
            knife();
        }
        //TANGLED TESTING
        /*if (Input.GetButtonDown("Fire3"))
        {
            toggleTangled(2);
        }*/
    }

    void harpoon()
    {
        if (Input.GetButton("Fire2") && shootDist < shootMax) //start charging
        {
            shootDist += Time.deltaTime * 5;
            updateChargeUI();
        }
        else if (Input.GetButtonUp("Fire2")) //fire
        {
            shoot();
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
            /*playerVel.x = dashStr * moveDir.x; //x = r cos theta; dashStr = r //z = r sin theta; theta = moveDir
            playerVel.z = dashStr * moveDir.z; //movedir might already has the cart coords*/
            if (moveDir.normalized == Vector3.zero) //if no moveDir
                pushDir = transform.forward * dashStr;
            else
                pushDir = moveDir * dashStr;
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

    void knife()
    {
        knifeTimer = 0;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, knifeDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(knifeDmg);
            }
        }
    }

    void shoot()
    {
        shootTimer = 0;
        knifeTimer = 0; //so you cant use your knife while using harpoon gun
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
        shootDist = shootMin;//reset shoot dist
        updateChargeUI();
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());
        //add feedback here

        if( HP <= 0 )
        {
            GameManager.instance.youLose();
        }
    }

    IEnumerator flashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
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

    public void toggleTangled(int tangleMod)
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
        GameManager.instance.playerSlowScreen.SetActive(isTangled);
    }
    //

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        updateChargeUI();
        updateReloadUI();
    }

    void updateChargeUI()
    {
        float charge = shootDist - shootMin;
        float maxCharge = shootMax - shootMin;
        GameManager.instance.harpoonChargeBar.fillAmount = charge / maxCharge;
    }

    void updateReloadUI()
    {
        GameManager.instance.knifeReloadBar.fillAmount = knifeTimer / knifeRate;
        GameManager.instance.harpoonReloadBar.fillAmount = shootTimer / shootRate;
    }
}
