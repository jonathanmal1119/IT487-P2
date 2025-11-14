using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPistol : MonoBehaviour
{
    public Transform bulletSpawnSource;
    public GameObject bulletPrefab;
    public GameObject gunModel;
    public Text ammoCounter;
    public Animator animator;

    InputAction shootAction;
    //InputAction reloadAction;

    public float timeBetweenShots = 0.2f;
    public float switchToWeaponTime = 0.5f;
    //public float reloadTime = 1.5f;
    float nextShot = 0f;

    public bool holdToAutomaticallyShoot = false;

    public int ammunition = 999;

    private void Awake()
    {
        shootAction = InputSystem.actions.FindAction("Player/Attack");
        //reloadAction = InputSystem.actions.FindAction("Player/Reload");
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

    }
    private void OnDisable()
    {
        gunModel.SetActive(false);
        if (ammoCounter != null)
        {
            ammoCounter.text = "";
        }
    }

    void Start()
    {
        nextShot = Time.time + switchToWeaponTime;
        gunModel.SetActive(true);
    }

    void Update()
    {
        if(ammoCounter != null)
        {
            ammoCounter.text = ammunition.ToString();
        }

        if(ammunition > 0)
        {
            if(Time.time >= nextShot)
            {
                //press button to shoot.
                if (shootAction.WasPressedThisFrame())
                {
                    Shoot();
                }
                //hold button to continuously shoot, if enabled.
                else if (holdToAutomaticallyShoot && shootAction.IsPressed())
                {
                    Shoot();
                }
            }
        }
    }

    void Shoot()
    {
        if(animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        Instantiate(bulletPrefab, bulletSpawnSource.position, bulletSpawnSource.rotation);
        nextShot = Time.time + timeBetweenShots;
        ammunition -= 1;
    }
}
