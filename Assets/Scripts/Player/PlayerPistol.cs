using Assets.Scripts;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerLookControls))]
[RequireComponent(typeof(PlayerWeaponManager))]
public class PlayerPistol : MonoBehaviour
{
    public string weaponName;

    public Transform bulletSpawnSource;
    public GameObject bulletPrefab;
    public GameObject gunModel;

    public Transform Muzzle;
    public GameObject shootFX;

    public Animator animator;

    public InputAction shootAction;
    //InputAction reloadAction;

    public float timeBetweenShots = 0.2f;
    public float switchToWeaponTime = 0.5f;
    //public float reloadTime = 1.5f;
    public float nextShot = 0f;
    int continuousShots = 0;

    //random eulerAngle rotation of bullets when shooting. No Z value because that is for roll, which isn't useful in this circumstance.
    public Vector2 hipFireRandomSpread;
    public float bulletSpreadIncreasePerShot;

    public Vector2 aimFireRandomSpread;

    public float verticalRecoil = 0;
    public float randomHorizontalRecoil = 0;

    public Vector2 EffectiveSpread { 
        get {
            if (aimDownSights)
            {
                return aimFireRandomSpread;
            }
            else
            {
                float extraSpread = (float)Math.Log(continuousShots + 0.5) * bulletSpreadIncreasePerShot;
                return new(hipFireRandomSpread.x + extraSpread, hipFireRandomSpread.y + extraSpread);
            }
        } 
    }

    bool aimDownSights = false;
    
    [Header("leave weaponXOffset null if the weapon doesn't allow aim-down-sight")]
    public GameObject weaponXOffset;
    public float aimOffsetDistance = 0.3f;

    public float aimDownSightsSpeed = 15f;
    public float aimDownSightsFOVMultiplier = 1.25f;

    public bool holdToAutomaticallyShoot = false;

    [Header("Ammunition Stuff")]

    public int ammunition = 999;
    public int ammoUsedPerShot = 1;
    public bool hideOnNoAmmo = false;
    
    public Action? AmmoChanged => GetComponent<PlayerWeaponManager>().N()?.AmmoChanged;
    public bool IsAiming => aimDownSights;
    public bool HasSpread => hipFireRandomSpread != Vector2.zero || bulletSpreadIncreasePerShot > 0f || aimFireRandomSpread != Vector2.zero;

    PlayerLookControls playerLookControls;

    PlayerHealth? playerHealth;

    [Header("Sounds")]

    public AudioSource soundSource;
    public AudioClip attackSound;
    public float attackSoundVolume = 1f;
    public AudioClip equipSound;
    public float equipSoundVolume = 1f;

    private void Awake()
    {
        shootAction = InputSystem.actions.FindAction("Player/Attack");
        //reloadAction = InputSystem.actions.FindAction("Player/Reload");
        playerLookControls = GetComponent<PlayerLookControls>();
        playerHealth = GetComponent<PlayerHealth>().N();
    }

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
        if(soundSource != null && equipSound != null)
        {
            soundSource.PlayOneShot(equipSound, equipSoundVolume);
        }
    }
    private void OnDisable()
    {
        gunModel.SetActive(false);
        AmmoChanged?.Invoke();
    }

    void Start()
    {
        nextShot = Time.time + switchToWeaponTime;
        gunModel.SetActive(true);
    }

    void Update()
    {
        if (playerHealth?.IsAlive == false)
            return;

        if (weaponXOffset != null)
        {
            if (Mouse.current.rightButton.isPressed && playerLookControls.EnableMouse)
            {
                weaponXOffset.transform.localPosition = Vector3.Lerp(weaponXOffset.transform.localPosition, new(aimOffsetDistance * -1f, 0.1f, 0f), Time.deltaTime * 24);
                aimDownSights = true;
                playerLookControls.playerCam.fieldOfView = Mathf.Lerp(playerLookControls.playerCam.fieldOfView, playerLookControls.OriginalFOV / aimDownSightsFOVMultiplier, Time.deltaTime * aimDownSightsSpeed);
            }
            else
            {
                weaponXOffset.transform.localPosition = Vector3.Lerp(weaponXOffset.transform.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime * 24);
                aimDownSights = false;
                playerLookControls.playerCam.fieldOfView = Mathf.Lerp(playerLookControls.playerCam.fieldOfView, playerLookControls.OriginalFOV, Time.deltaTime * aimDownSightsSpeed);
            }
        }

        if (ammunition > 0 || ammoUsedPerShot <= 0)
        {
            if (gunModel.activeSelf == false && hideOnNoAmmo)
            {
                gunModel.SetActive(true);
            }

            if (Time.time >= nextShot)
            {
                //press button to shoot.
                if (shootAction.WasPressedThisFrame() && playerLookControls.EnableMouse)
                {
                    Shoot();
                }
                //hold button to continuously shoot, if enabled.
                else if (holdToAutomaticallyShoot && shootAction.IsPressed() && playerLookControls.EnableMouse)
                {
                    Shoot();
                }
            }
        }
        else if (hideOnNoAmmo)
        {
            gunModel.SetActive(false);
        }

        if (nextShot + 0.25f < Time.time)
            continuousShots = 0;
    }

    void Shoot()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        GameObject pb = Instantiate(bulletPrefab, bulletSpawnSource.position, bulletSpawnSource.rotation);
        pb.transform.Rotate(UnityEngine.Random.Range(EffectiveSpread.x * -1, EffectiveSpread.x), UnityEngine.Random.Range(EffectiveSpread.y * -1, EffectiveSpread.y), 0f);
        if (pb.GetComponent<PlayerBullet>() != null)
            pb.GetComponent<PlayerBullet>().Owner = GetComponent<PlayerWeaponManager>();
        if (pb.GetComponent<PlayerGrenade>() != null)
            pb.GetComponent<PlayerGrenade>().Owner = GetComponent<PlayerWeaponManager>();

        if (shootFX != null && Muzzle != null)
            Instantiate(shootFX, Muzzle);

        continuousShots++;

        nextShot = Time.time + timeBetweenShots;
        ammunition -= ammoUsedPerShot;
        AmmoChanged?.Invoke();

        if (aimDownSights)
            playerLookControls.AddCameraRecoil(verticalRecoil, randomHorizontalRecoil);
        else
            playerLookControls.AddCameraRecoil(verticalRecoil * 0.65f, randomHorizontalRecoil * 0.65f);


        if (soundSource != null && attackSound != null)
        {
            soundSource.PlayOneShot(attackSound, attackSoundVolume);
        }
    }
}
