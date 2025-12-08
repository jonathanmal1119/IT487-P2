using Assets.Scripts;
using UnityEngine;

public class ScreenFXReset : MonoBehaviour
{
    public Material ScreenDamageMaterial;

    void Start()
    {
        ScreenDamageMaterial.N()?.SetFloat("_Power", 10f);
        ScreenDamageMaterial.N()?.SetFloat("_Offset", 0.25f);
        ScreenDamageMaterial.N()?.SetFloat("_Opacity", 0f);
    }
}
