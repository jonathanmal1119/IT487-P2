using System.Collections.Generic;
using UnityEngine;

public class PlayerShotgun : PlayerPistol
{
    public int bulletsPerShot = 5;

    public float recoilForceMultiplier = 1.25f;
    public float recoilRestMultiplier = 1.5f;

    public List<Vector2> pattern = new();

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

    // Update is called once per frame
    void Update()
    {
        if (playerHealth?.IsAlive == false)
            return;

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
                    ShotgunShot(bulletsPerShot);
                }
            }
        }
        else if (hideOnNoAmmo)
        {
            gunModel.SetActive(false);
        }
    }

    void ShotgunShot(int bulletcount)
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }


        // if set pattern is set, use pattern, else use random spread
        if (pattern.Count > 1)
        {
            foreach (Vector2 point in pattern)
            {
                Vector2 deviatedPoint = point * Random.Range(1.25f, 0.75f); // add slight deviation so its not perfect

                GameObject pb = Instantiate(bulletPrefab, bulletSpawnSource.position, bulletSpawnSource.rotation);
                pb.transform.Rotate(deviatedPoint.y * EffectiveSpread.x * 2, deviatedPoint.x * EffectiveSpread.y * 2, 0f);
                if (pb.GetComponent<PlayerBullet>() != null)
                    pb.GetComponent<PlayerBullet>().Owner = GetComponent<PlayerWeaponManager>();

            }
        }
        else
        {
            for (int i = 0; i < bulletcount; i++)
            {
                GameObject pb = Instantiate(bulletPrefab, bulletSpawnSource.position, bulletSpawnSource.rotation);
                pb.transform.Rotate(Random.Range(EffectiveSpread.x * -1, EffectiveSpread.x), Random.Range(EffectiveSpread.y * -1, EffectiveSpread.y), 0f);
                if (pb.GetComponent<PlayerBullet>() != null)
                    pb.GetComponent<PlayerBullet>().Owner = GetComponent<PlayerWeaponManager>();
                if (pb.GetComponent<PlayerGrenade>() != null)
                    pb.GetComponent<PlayerGrenade>().Owner = GetComponent<PlayerWeaponManager>();
            }
        }

        if (shootFX != null && Muzzle != null)
            Instantiate(shootFX, Muzzle);

        nextShot = Time.time + timeBetweenShots;
        ammunition -= ammoUsedPerShot;
        AmmoChanged?.Invoke();

        playerLookControls.AddCameraRecoil(verticalRecoil * 0.65f, randomHorizontalRecoil * 0.65f);

        if (soundSource != null && attackSound != null)
        {
            soundSource.PlayOneShot(attackSound, attackSoundVolume);
        }
    }
}
