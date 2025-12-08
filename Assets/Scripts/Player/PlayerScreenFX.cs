using UnityEngine;
using Assets.Scripts;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerScreenFX : MonoBehaviour
{
    public Material ScreenDamageMaterial;
    public float TransitionSpeed = 5;

    PlayerHealth playerHealth;

    // i should be using an object instead of a tuple but im kinda drunk
    (float power, float offset, float opacity) Base = (10, 0.25f, 0);

    (float power, float offset, float opacity) Health50 = (8, 0.3f, 0.5f);
    (float power, float offset, float opacity) Health0 = (6f, 0.325f, 0.75f);

    (float power, float offset, float opacity) Dead = (4, 0.5f, 1);

    (float power, float offset, float opacity) Target;

    (float power, float offset, float opacity) Current {
        get {
            return (ScreenDamageMaterial.GetFloat("_Power"), ScreenDamageMaterial.GetFloat("_Offset"), ScreenDamageMaterial.GetFloat("_Opacity"));
        }
        set {
            ScreenDamageMaterial.SetFloat("_Power", value.power);
            ScreenDamageMaterial.SetFloat("_Offset", value.offset);
            ScreenDamageMaterial.SetFloat("_Opacity", value.opacity);
        }
    }

    void Start()
    {
        Current = Base;
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth.health <= 0)
            Target = Dead;
        else if (playerHealth.health > 50)
            Target = Base;
        else
        {
            float healthRatio = playerHealth.health.Remap(0, 50, 0, 1).Clamp(0, 1);

            Target = (
                Mathf.Lerp(Health0.power, Health50.power, healthRatio),
                Mathf.Lerp(Health0.offset, Health50.offset, healthRatio),
                Mathf.Lerp(Health0.opacity, Health50.opacity, healthRatio)
            );
        }

        Current = (
            Mathf.Lerp(Current.power, Target.power, Time.deltaTime * TransitionSpeed),
            Mathf.Lerp(Current.offset, Target.offset, Time.deltaTime * TransitionSpeed),
            Mathf.Lerp(Current.opacity, Target.opacity, Time.deltaTime * TransitionSpeed)
        );
    }

    public void TookDamage()
    {
        if (playerHealth.health > 0)
            Current = (Current.power / 2.5f, 0.4f, 1);
    }
}
