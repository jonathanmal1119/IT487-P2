using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerLookControls))]
public class PlayerWeaponManager : MonoBehaviour
{
    public PlayerPistol[] weapons;
    public int activeWeapon = 0;

    InputAction scrollAction, nextAction, previousAction;

    public PlayerPistol ActiveWeapon => weapons[activeWeapon];

    public Action? WeaponChanged;
    public Action? AmmoChanged;
    public Action? OnHit;
    public Action? OnKill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetActiveWeapon(activeWeapon);
    }

    private void Awake()
    {
        nextAction = InputSystem.actions.FindAction("Player/Next");
        previousAction = InputSystem.actions.FindAction("Player/Previous");
        scrollAction = InputSystem.actions.FindAction("Player/Scrollwheel");
    }
    private void OnEnable()
    {
        nextAction.Enable();
        previousAction.Enable();
        scrollAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if ((scrollAction.ReadValue<float>() > 0 || nextAction.WasPressedThisFrame()) && GetComponent<PlayerLookControls>().EnableMouse)
        {
            SetActiveWeapon(activeWeapon + 1);
        }
        else if ((scrollAction.ReadValue<float>() < 0 || previousAction.WasPressedThisFrame()) && GetComponent<PlayerLookControls>().EnableMouse)
        {
            SetActiveWeapon(activeWeapon - 1);
        }
    }

    public void SetActiveWeapon(int i)
    {
        if(i >= weapons.Length) { i = 0; }
        if(i < 0) { i = weapons.Length - 1; }
        activeWeapon = i;
        foreach(PlayerPistol w in weapons)
        {
            w.enabled = false;
        }
        weapons[i].enabled = true;
        WeaponChanged?.Invoke();
    }

    public bool AddAmmo(int amount, int index)
    {
        if(index < 0 || index >= weapons.Length) { return false; }

        if(weapons[index] == null) { return false; }

        weapons[index].ammunition += amount;

        return true;
    }
}
