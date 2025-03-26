using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, ITangle, IPickup
{
    public int HP;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
   
    [Header("<----- Stats ----->")]
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

    [Header("<----- Weapons ----->")]
    [SerializeField] int knifeDmg;
    [SerializeField] float knifeRate;
    [SerializeField] int knifeDist;
    //[SerializeField] int shootDmg;
    //[SerializeField] float shootRate;
    //[SerializeField] float shootMin;
    //[SerializeField] float shootMax;
    [SerializeField] GameObject weaponModel;
    [SerializeField] LineRenderer grappleLine;
    [SerializeField] Transform linePos;                                       //UNCOMMENT: THIS IS FOR THE START POS OF LINE RENDERER
    //[SerializeField] List<meleeStats> meleeList = new List<meleeStats>(); //player can only have 1 weap >> list not needed
    [SerializeField] meleeStats meleeCurr;
    //[SerializeField] List<rangedStats> rangedList = new List<rangedStats>();
    [SerializeField] rangedStats rangedCurr;
    
    /*int meleeListPos;
    int rangedListPos;*/

    int HPOrig;
    float speedOrig;

    Vector3 moveDir;
    public Vector3 pushDir;
    Vector3 playerVel;

    int dashCount;
    int jumpCount;
    float knifeTimer;
    //float shootTimer;
    public float grappleDist;
    bool isTangled; //was only public for testing
    //float harpoonChargeSpeed;
    public float grappleSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        speedOrig = speed;
        //shootDist = shootMin;
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        pushDir = Vector3.Lerp(pushDir, Vector3.zero, Time.deltaTime * pushResolve);
        if (meleeCurr != null)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * meleeCurr.meleeDist, Color.red);

        if (GameManager.instance.isPaused == false)
        { 
            movement();

            //harpoon();
        }
        //sprint();
        updateReloadUI();
    }

    void movement()
    {
        //increment shoot timer
        knifeTimer += Time.deltaTime;
        //shootTimer += Time.deltaTime;

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

        isGrappling();

        //SHOOT LOGIC           
        if (Input.GetButton("Fire1") && meleeCurr != null && knifeRate <= knifeTimer && GameManager.instance.isPaused == false)
        {
            knife();
        }
        /*selectMeleeWeapon();
        selectRangedWeapon();*/

        //TANGLED TESTING
        /*if (Input.GetButtonDown("Fire3"))
        {
            toggleTangled(2);
        }*/
    }

    /*void harpoon()
    {//moved timer check to be called AFTER button check, for performance (mentioned in lec5)
        if (Input.GetButton("Fire2") && shootRate <= shootTimer && shootDist < shootMax) //start charging
        {
            shootDist += Time.deltaTime * harpoonChargeSpeed;
            updateChargeUI();
        }
        else if (Input.GetButtonUp("Fire2") && shootRate <= shootTimer) //fire
        {
            shoot();
        }
    }*/

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
            pushDir = moveDir * dashStr;
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
            Instantiate(meleeCurr.hitEffect, hit.point, Quaternion.identity);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(knifeDmg);
            }
        }
    }

    /*void shoot()
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
    }*/

    bool grapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleDist, ~ignoreLayer))
        {
            if (hit.collider.CompareTag("GrapplePoint"))
            {
                controller.Move((hit.point - transform.position).normalized * grappleSpeed * Time.deltaTime);

                grappleLine.SetPosition(0, linePos.position);
                grappleLine.SetPosition(1, hit.point);
                return true;
            }
        }
        return false;
    }

    void isGrappling()
    {
        if (Input.GetButton("Fire2") && grapple())
        {
            grappleLine.enabled = true;
        }
        else
        {
            grappleLine.enabled = false;
            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= grav * Time.deltaTime;
        }
    }

    /*public void harpoonPull()
    {//get help
        controller.Move(harpoonDir * harpoonPullSpeed * Time.deltaTime);
    }*/

    public void takeDamage(int damage)
    {
        HP -= damage;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());
        //add feedback here

        if (HP <= 0)
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
        //shootRate *= tangleMod;
        GameManager.instance.playerSlowScreen.SetActive(isTangled);
    }

    public void stateUntangled(int tangleMod)
    {
        speed *= tangleMod; //
        jumpStr *= tangleMod;
        dashStr *= tangleMod;
        dashDuration *= tangleMod;
        //shootRate /= tangleMod;
        if (speed == speedOrig) //if there are multiple sources of tangled, this (should) ensure that player is fully untangled before being set to false
            isTangled = false;
        GameManager.instance.playerSlowScreen.SetActive(isTangled);
    }
    //

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        //updateChargeUI();
        updateReloadUI();
    }

    /*void updateChargeUI(bool canGrapple)
    {
        GameManager.instance.harpoonChargeBar.enabled = canGrapple;
    }*/

    void updateReloadUI()
    {
        GameManager.instance.knifeReloadBar.fillAmount = knifeTimer / knifeRate;
        GameManager.instance.harpoonReloadBar.enabled = !grappleLine.enabled;
    }
    public void getRangedStats(rangedStats rweapon)
    {
        /*rangedList.Add(rweapon);
        rangedListPos = rangedList.Count - 1;*/
        if (rangedCurr == null || rangedCurr.rank < rweapon.rank) //if pick up is an upgrade (better then curr)
        {
            rangedCurr = rweapon; //this replaces the 2 above lines
            changeRangedWeapon();
        }
    }
    /*void selectRangedWeapon()
    {
        if (Input.GetAxis("AltMouse ScrollWheel") > 0 && rangedListPos < rangedList.Count - 1)
        {
            rangedListPos++;
            changeRangedWeapon();
        }
        else if (Input.GetAxis("AltMouse ScrollWheel") < 0 && rangedListPos > 0)
        {
            rangedListPos--;
            changeRangedWeapon();
        }
    }*/
    void changeRangedWeapon()
    {
        //shootDmg = rangedCurr.shootDamage;
        grappleDist = rangedCurr.grappleDist;
        grappleSpeed = rangedCurr.grappleSpeed;
        //shootRate = rangedCurr.shootRate;
        /*
        weaponModel.GetComponent<MeshFilter>().sharedMesh = rangedCurr.model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = rangedCurr.model.GetComponent<MeshRenderer>().sharedMaterial;*/
    }

    public void getMeleeStats(meleeStats mweapon)
    {
        /*meleeList.Add(mweapon);
        meleeListPos = meleeList.Count - 1;*/ //weapon upgrades >> dont keep old weap
        if (meleeCurr == null || meleeCurr.rank < mweapon.rank) //if no weapon OR pick up is an upgrade (better then curr)
        {
            meleeCurr = mweapon; //this replaces the 2 above lines
            changeMeleeWeapon();
        }
    }
    /*void selectMeleeWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && meleeListPos < meleeList.Count - 1)
        {
            meleeListPos++;
            changeMeleeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && meleeListPos > 0)
        {
            meleeListPos--;
            changeMeleeWeapon();
        }
    }*/
    void changeMeleeWeapon()
    {
        knifeDmg = meleeCurr.meleeDmg;
        knifeDist = meleeCurr.meleeDist;
        knifeRate = meleeCurr.meleeRate;

        weaponModel.GetComponent<MeshFilter>().sharedMesh = meleeCurr.model.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = meleeCurr.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

   /* void ITangle.toggleTangled(int tangleMod)
    {
        throw new System.NotImplementedException();
    }*/
    public void spawnPlayer()
    {
        controller.transform.position = GameManager.instance.playerSpawnPos.transform.position;
        HP = HPOrig;
        updatePlayerUI();

    }
}
