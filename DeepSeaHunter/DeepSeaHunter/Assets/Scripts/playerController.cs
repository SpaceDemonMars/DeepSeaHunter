using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, ITangle, IHarpoon, IPickup
{
    public int HP;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] public float speed;
    //[SerializeField] int sprintMod;
    [SerializeField] int pushResolve;
    [SerializeField] public float dashStr;
    [SerializeField] int dashMax;
    [SerializeField] float dashRechargeTimer;
    [SerializeField] float dashDuration;

    [SerializeField] public float jumpStr;
    [SerializeField] int jumpMax;
    [SerializeField] float grav;

    [SerializeField] int knifeDmg;
    [SerializeField] float knifeRate;
    [SerializeField] int knifeDist;
    [SerializeField] int shootDmg;
    [SerializeField] float shootRate;
    [SerializeField] float shootMin;
    [SerializeField] float shootMax;


    [SerializeField] List<meleeStats> meleeList = new List<meleeStats>();
    [SerializeField] List<rangedStats> rangedList = new List<rangedStats>();

    [SerializeField] GameObject weaponModel;

    int meleeListPos;
    int rangedListPos;

    int HPOrig;
    float speedOrig;
    Vector3 moveDir;
    public Vector3 pushDir;
    Vector3 playerVel;
    Vector3 harpoonDir;
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
        speedOrig = speed;
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
        selectMeleeWeapon();
        selectRangedWeapon();
        
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
            shootDist += Time.deltaTime * harpoonChargeSpeed;
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
            IHarpoon pull = hit.collider.GetComponent<IHarpoon>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDmg);
            }
            if (pull != null)
                pull.harpoonPull();
            else
            {
                harpoonPull();
                harpoonDir = hit.point - transform.position;
            }
        }
        shootDist = shootMin;//reset shoot dist
        updateChargeUI();
    }

    public void harpoonPull()
    {//get help
        controller.Move(harpoonDir * harpoonPullSpeed * Time.deltaTime); 
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

    public void stateTangled(int tangleMod)
    {
        isTangled = true;
        speed /= tangleMod; //
        jumpStr /= tangleMod;
        dashStr /= tangleMod;
        dashDuration /= tangleMod;
        shootRate *= tangleMod;
        GameManager.instance.playerSlowScreen.SetActive(isTangled);
    }

    public void stateUntangled(int tangleMod) 
    {
        speed *= tangleMod; //
        jumpStr *= tangleMod;
        dashStr *= tangleMod;
        dashDuration *= tangleMod;
        shootRate /= tangleMod;
        if (speed == speedOrig) //if there are multiple sources of tangled, this (should) ensure that player is fully untangled before being set to false
            isTangled = false;
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
    public void getRangedStats(rangedStats rweapon)
    {
        rangedList.Add(rweapon);
        rangedListPos = rangedList.Count - 1;
        changeRangedWeapon();
    }
    void selectRangedWeapon()
    {
        if (Input.GetAxis("AltMouse ScrollWheel;") > 0 && rangedListPos < rangedList.Count - 1)
        {
            rangedListPos++;
            changeRangedWeapon();
        }
        else if (Input.GetAxis("AltMouse ScrollWheel;") < 0 && rangedListPos > 0)
        {
            rangedListPos--;
            changeRangedWeapon();
        }
    }
    void changeRangedWeapon()
    {
        shootDmg = rangedList[rangedListPos].shootDamage;
        shootDist = rangedList[rangedListPos].shootDist;
        shootRate = rangedList[rangedListPos].shootRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = rangedList[rangedListPos].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = rangedList[rangedListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void getMeleeStats(meleeStats mweapon)
    {
        meleeList.Add(mweapon);
        meleeListPos = meleeList.Count - 1;
        changeMeleeWeapon();
    }
    void selectMeleeWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel;") > 0 && meleeListPos < meleeList.Count - 1)
        {
            meleeListPos++;
            changeMeleeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel;") < 0 && meleeListPos > 0)
        {
            meleeListPos--;
            changeMeleeWeapon();
        }
    }
    void changeMeleeWeapon()
    {
        knifeDmg = meleeList[meleeListPos].meleeDmg;
        knifeDist = meleeList[meleeListPos].meleeDist;
        knifeRate = meleeList[meleeListPos].meleeRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = meleeList[meleeListPos].model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = meleeList[meleeListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }
}