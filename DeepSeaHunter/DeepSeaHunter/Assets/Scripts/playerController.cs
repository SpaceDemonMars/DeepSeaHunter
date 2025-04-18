using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, ITangle, IPickup
{
    [Header("General")]
    public int HP;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
    int HPOrig;
    float speedOrig;
    Vector3 moveDir;

    [Header("<----- Stats ----->")]
    [SerializeField] public float speed;
    [Header("Dash Stats")]
    [SerializeField] int pushResolve;
    [SerializeField] public float dashStr;
    [SerializeField] int dashMax;
    [SerializeField] float dashRechargeTimer;
    [SerializeField] float dashDuration;
    Vector3 pushDir;
    int dashCount;
    [Header("Jump Stats")]
    [SerializeField] float jumpStr;
    [SerializeField] int jumpMax;
    [SerializeField] float grav;
    Vector3 playerVel;
    int jumpCount;

    [Header("<----- Weapons ----->")]
    [SerializeField] GameObject flashLight;

    [Header("<----- Weapons ----->")]
    [Header("Knife")]
    [SerializeField] GameObject weaponModel;
    [SerializeField] meleeStats meleeCurr;
    int knifeDmg;
    float knifeRate;
    int knifeDist;
    float knifeTimer;
    [Header("Harpoon")]
    [SerializeField] LineRenderer grappleLine;
    [SerializeField] Transform linePos;
    [SerializeField] rangedStats rangedCurr;
    float grappleDist;
    float grappleSpeed;

    [Header("<----- Audio ----->")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audDashVol;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audHurt;
    bool isPlayingSteps;
    bool isDashing;
    bool hasPlayedGrapple;
    //STATUS
    bool isTangled;
    GameObject grappledTarget;

    //OLD MEMBER VARS
    /*[SerializeField] int sprintMod;
    [SerializeField] int shootDmg;
    [SerializeField] float shootRate;
    [SerializeField] float shootMin;
    [SerializeField] float shootMax;                                //UNCOMMENT: THIS IS FOR THE START POS OF LINE RENDERER
    [SerializeField] List<meleeStats> meleeList = new List<meleeStats>(); //player can only have 1 weap >> list not needed
    [SerializeField] List<rangedStats> rangedList = new List<rangedStats>();

    int meleeListPos;
    int rangedListPos;


    float shootTimer;
    float harpoonChargeSpeed;*/

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
        //increment shoot timer
        knifeTimer += Time.deltaTime;
        pushDir = Vector3.Lerp(pushDir, Vector3.zero, Time.deltaTime * pushResolve);
        if (meleeCurr != null)
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * meleeCurr.meleeDist, Color.red);

        if (GameManager.instance.isPaused == false)
        {
            isGrappling();
            //movement();

            //harpoon();
        }
        //sprint();

        //Light
        Light();

        updateReloadUI();
        HandleCluePickup();
    }

    void HandleCluePickup()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3f))
            {
                Clue clue = hit.collider.GetComponent<Clue>();
                if (clue != null)
                {
                    clue.Pickup(); 
                }
            }
        }
    }

    void Light()
    {
        if (Input.GetButtonDown("Light"))
        {
            flashLight.SetActive(!flashLight.activeSelf);
        }
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (isDashing)
            yield return new WaitForSeconds(.3f);
        else
            yield return new WaitForSeconds(.5f);
        isPlayingSteps = false;
    }

    void movement()
    {
        //shootTimer += Time.deltaTime;

        //reset jumps
        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.3f && !isPlayingSteps)
                StartCoroutine(playSteps());
            playerVel.y = 0;
            jumpCount = 0;
        }
        //BASIC MOVEMENT
        //getting movement input
        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        //move player
        controller.Move((moveDir + pushDir) * speed * Time.deltaTime);
        if (pushDir.magnitude < .3f)
            isDashing = false; //turns off fast foot steps

        //JUMP/DASH LOGIC
        jump();
        dash();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= grav * Time.deltaTime;

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
            isDashing = true;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audDashVol);
            //this needs math to get the angle right; look at polar to cart coords
            /*playerVel.x = dashStr * moveDir.x; //x = r cos theta; dashStr = r //z = r sin theta; theta = moveDir
            playerVel.z = dashStr * moveDir.z; //movedir might already has the cart coords*/
            pushDir = moveDir * dashStr;
            if (moveDir.normalized == Vector3.zero) //if no moveDir
                pushDir = transform.forward * dashStr;
            else
                pushDir = moveDir * dashStr;
            Debug.Log("Dashed");
            //StartCoroutine(endDash());
            StartCoroutine(rechargeDash());
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpStr;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void knife()
    {
        knifeTimer = 0;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, knifeDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            aud.PlayOneShot(meleeCurr.hitSound[Random.Range(0, meleeCurr.hitSound.Length)], meleeCurr.hitVol); //only play on hit
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
            if (hit.collider.CompareTag("GrapplePoint") || hit.collider.CompareTag("Level Boss")) //allows grappling to bosses despite needed special tag
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
            if (!hasPlayedGrapple)
            {
                hasPlayedGrapple = true;
                aud.PlayOneShot(rangedCurr.hitSound[Random.Range(0, rangedCurr.hitSound.Length)], rangedCurr.hitVol);
            }
        }
        else
        {
            grappleLine.enabled = false;
            hasPlayedGrapple = false;
            movement();
        }
    }

    /*public void harpoonPull()
    {//get help
        controller.Move(harpoonDir * harpoonPullSpeed * Time.deltaTime);
    }*/

    public void takeDamage(int damage)
    {
        if (HP == HPOrig && damage < 0)
            return; //if healing and max health >> return
        HP -= damage;
        if (HP > HPOrig) //if overhealed
            HP = HPOrig; //reset
        updatePlayerUI();
        StartCoroutine(showDamageNum(damage));
        if (damage > 0) //if damaging
        {
            StartCoroutine(flashDamageScreen());
            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        }
        else if (damage < 0) //if healing; not else bc of small chance dmg = 0, where we do nothing
            StartCoroutine(flashHealScreen());
        //add feedback here
        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    IEnumerator showDamageNum(int damage)
    {
        damage *= -1; //flip  sign
        GameManager.instance.damageText.SetText(damage.ToString());
        yield return new WaitForSeconds(.5f);
        GameManager.instance.damageText.text = "";
    }

    IEnumerator flashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    IEnumerator flashHealScreen()
    {
        GameManager.instance.playerHealScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerHealScreen.SetActive(false);
    }
    IEnumerator rechargeDash()
    {
        yield return new WaitForSeconds(dashRechargeTimer);
        dashCount--;
        Debug.Log("Dash Recharged");
    }
    /*IEnumerator endDash()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        playerVel.x = 0;
        playerVel.z = 0;
    }*/

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
            if (rweapon.rank > 0) //not starting weapon
                StartCoroutine(weaponUpgradePopup(rweapon.name));
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
            if (mweapon.rank > 0) //not starting weapon
                StartCoroutine(weaponUpgradePopup(mweapon.name));
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

    IEnumerator weaponUpgradePopup(string weaponName)
    {
        GameManager.instance.weaponUpgradeText.text = weaponName + " Obtained!";
        GameManager.instance.weaponUpgradePopup.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.instance.weaponUpgradePopup.SetActive(false);
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

/*    Leanne's Grapple code if she wants to keep working on it
 *  IEnumerator PullPlayerToPoint(Vector3 targetPoint)
    {//to lock onto points and pull smoothly
        while (Vector3.Distance(transform.position, targetPoint) > 1.0f)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;
            controller.Move(direction * grappleSpeed * Time.deltaTime);

            grappleLine.SetPosition(0, linePos.position);
            grappleLine.SetPosition(1, targetPoint);

            yield return null;
        }
        grappleLine.enabled = false;
    }

    IEnumerator PullPlayerToTarget()
    {//to lock on enemy and  pull smoothly
        while (grappledTarget != null)
        {
            if (Input.GetButtonUp("Fire2"))
            {
                grappledTarget = null;
                grappleLine.enabled = false;
                yield break;
            }
            Vector3 direction = (grappledTarget.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, grappledTarget.transform.position);

            if (distance < 1.0f)
            {
                grappledTarget = null;
                grappleLine.enabled = false;
                yield break;
            }

            controller.Move(direction * grappleSpeed * Time.deltaTime);
            grappleLine.SetPosition(0, linePos.position);
            grappleLine.SetPosition(1, grappledTarget.transform.position);

            yield return null;
        }
        grappleLine.enabled = false;
    }
*/