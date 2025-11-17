using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts;

public class PlayerThrowGrenade : PlayerPistol
{
    float fuseEndTime;
    bool cookingGrenade = false;

    public GameObject grenadeExplosion;
    
    //the actual grenade uses the inherited bulletPrefab variable

    public GameObject fuseDisplay;
    public float grenadeFuseLength = 3f;

    public PlayerHealth health;
    //damage dealt to the player after cooking their grenade for too long
    public int grenadeOvercookExplosionDamage = 50;

    //---NOTE: these have been commented out because the parent class already does these. Also, Unity was throwing errors about managing the input stuff twice for some reason.-----
    /*
    private void Awake()
    {
        shootAction = InputSystem.actions.FindAction("Player/Attack");
        //reloadAction = InputSystem.actions.FindAction("Player/Reload");
    }
    */

    /*
    private void OnEnable()
    {
        shootAction.Enable();

        nextShot = Time.time + switchToWeaponTime;
        //reloadAction.Enable();
        gunModel.SetActive(true);
        if (animator != null)
        {
            animator.SetTrigger("Equip");
        }

    }
    */
    
    private void OnDisable()
    {
        gunModel.SetActive(false);
        cookingGrenade = false;
        AmmoChanged?.Invoke();
    }

    void Start()
    {
        nextShot = Time.time + switchToWeaponTime;
        gunModel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (ammunition > 0 || ammoUsedPerShot <= 0)
        {
            if (gunModel.activeSelf == false && hideOnNoAmmo)
            {
                gunModel.SetActive(true);
            }

            //will probably be the pin in the grenade model
            if(fuseDisplay != null)
            {
                if (cookingGrenade) { fuseDisplay.SetActive(false); }
                else { fuseDisplay.SetActive(true); }
            }

            if (Time.time >= nextShot)
            {
                //begin cooking the grenade
                if(shootAction.IsPressed() && cookingGrenade == false)
                {
                    cookingGrenade = true;
                    fuseEndTime = Time.time + grenadeFuseLength;
                }
                //throw a grenade when releasing the shoot button
                else if (shootAction.WasReleasedThisFrame())
                {
                    //throw a cooked grenade
                    if (cookingGrenade)
                    {
                        ThrowGrenade(fuseEndTime - Time.time);
                    }
                    //throw an uncooked grenade. This is just a possible edge case
                    else
                    {
                        ThrowGrenade(grenadeFuseLength);
                    }
                    cookingGrenade = false;
                }
                //grenade explodes in the players face when held too long
                else if (cookingGrenade && Time.time > fuseEndTime)
                {
                    if(health != null)
                    {
                        health.TakeDamage(grenadeOvercookExplosionDamage);
                    }

                    Instantiate(grenadeExplosion, bulletSpawnSource.position, bulletSpawnSource.rotation);
                    if (animator != null)
                    {
                        //for grenades, the "shoot" trigger plays an animation of pulling out a new grenade
                        animator.SetTrigger("Shoot");
                    }
                    cookingGrenade = false;
                    nextShot = Time.time + timeBetweenShots;
                    ammunition -= ammoUsedPerShot;
                    AmmoChanged?.Invoke();
                }
            }
        }
        else if (hideOnNoAmmo)
        {
            gunModel.SetActive(false);
        }
    }

    public void ThrowGrenade(float fuseLength)
    {
        //for grenades, the "shoot" trigger plays an animation of pulling out a new grenade
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        GameObject pb = Instantiate(bulletPrefab, bulletSpawnSource.position, bulletSpawnSource.rotation);
        pb.transform.Rotate(UnityEngine.Random.Range(EffectiveSpread.x * -1, EffectiveSpread.x), UnityEngine.Random.Range(EffectiveSpread.y * -1, EffectiveSpread.y), 0f);
        if (pb.GetComponent<PlayerGrenade>() != null)
        {
            pb.GetComponent<PlayerGrenade>().Owner = GetComponent<PlayerWeaponManager>().N();
            pb.GetComponent<PlayerGrenade>().fuseTime = fuseLength;
        }

        nextShot = Time.time + timeBetweenShots;
        ammunition -= ammoUsedPerShot;
        AmmoChanged?.Invoke();
    }
}
