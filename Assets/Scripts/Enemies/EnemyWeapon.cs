using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EnemyWeapon : MonoBehaviour
{
    public EnemyController EnemyController;

    public Transform bulletSpawnSource;
    public GameObject bulletPrefab;
    public GameObject gunModel;
    public Animator animator;
    private bool isFiring = false;

    InputAction shootAction;
    //InputAction reloadAction;

    public float timeBetweenShots = 0.2f;
    public float switchToWeaponTime = 0.5f;
    //public float reloadTime = 1.5f;
    float nextShot = 0f;

    private void Awake()
    {
        //shootAction = InputSystem.actions.FindAction("Player/Attack");
        //reloadAction = InputSystem.actions.FindAction("Player/Reload");
        EnemyController = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        //shootAction.Enable();

        nextShot = Time.time + switchToWeaponTime;
        //reloadAction.Enable();
        gunModel.SetActive(true);
        if (animator != null)
        {
            animator.SetTrigger("Equip");
        }

    }
    /*private void OnDisable()
    {
        gunModel.SetActive(false);
        if (ammoCounter != null)
        {
            ammoCounter.text = "";
        }
    }*/

    void Start()
    {
        nextShot = Time.time + switchToWeaponTime;
        gunModel.SetActive(true);
    }

    void Update()
    {
        if (isFiring == false && EnemyController.playerDetected == true)
        {
            StartCoroutine(FireWeapon());
            isFiring = true;
        }
    }

    void Shoot()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        Instantiate(bulletPrefab, bulletSpawnSource.position, bulletSpawnSource.rotation);
        nextShot = Time.time + timeBetweenShots;
    }

    private IEnumerator FireWeapon()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        Shoot();
        isFiring = false;
    }
}
