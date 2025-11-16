using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text EnemyText;
    public GameObject WinScreen;

    public GameObject Player;

    private PlayerHealth playerHealth;
    private float PlayerHealthPercent => (float)playerHealth.health / playerHealth.maxHealth;

    private GameObject healthUI;
    private float origHbWidth;

    private PlayerWalkControls playerWalkControls;
    private GameObject staminaUI;
    private float origSbWidth;

    private PlayerLookControls playerLookControls;

    private GameObject weaponUI;
    private PlayerWeaponManager playerWeaponManager;

    private GameObject crosshairUI;
    private GameObject hitmarkerUI;

    private GameObject objectivesUI;

    // turn objectives into a custom class instead of a tuple eventually hopefully we can just have a list of objectives that gets converted into the ui elements automatically idk
    private readonly List<(GameObject title, GameObject value)> objectives = new();

    void Start()
    {
        playerHealth = Player.GetComponent<PlayerHealth>();

        healthUI = transform.Find("Health").gameObject;
        RectTransform hpBar = healthUI.transform.Find("Bar/HP").GetComponent<RectTransform>();
        origHbWidth = hpBar.rect.width;
        hpBar.sizeDelta = new(-1 * origHbWidth * (1 - PlayerHealthPercent), hpBar.sizeDelta.y);
        playerHealth.HealthChanged += HealthChanged;
        HealthChanged();


        playerWalkControls = Player.GetComponent<PlayerWalkControls>();

        staminaUI = transform.Find("Stamina").gameObject;
        RectTransform stBar = staminaUI.transform.Find("Bar/ST").GetComponent<RectTransform>();
        origSbWidth = stBar.rect.width;
        stBar.sizeDelta = new(-1 * origSbWidth * (1 - playerWalkControls.stamina), stBar.sizeDelta.y);

        playerLookControls = Player.GetComponent<PlayerLookControls>();

        weaponUI = transform.Find("Weapon").gameObject;
        playerWeaponManager = Player.GetComponent<PlayerWeaponManager>();
        playerWeaponManager.WeaponChanged += WeaponUpdated;
        playerWeaponManager.AmmoChanged += WeaponUpdated;
        WeaponUpdated();
        playerWeaponManager.OnHit += () => OnWeaponHit();
        playerWeaponManager.OnKill += () => OnWeaponHit(true);

        crosshairUI = transform.Find("Crosshair").gameObject;
        hitmarkerUI = crosshairUI.transform.Find("Hitmarkers").gameObject;

        objectivesUI = transform.Find("Objectives").gameObject;
        foreach (Transform child in objectivesUI.transform.Find("ObjectiveList").transform)
            objectives.Add((child.gameObject, child.Find("Value").gameObject));
    }

    private void Update()
    {
        objectives[0].value.GetComponent<TextMeshProUGUI>().text = GameObject.FindGameObjectsWithTag("Enemy").Count().ToString();

        // health bar
        {
            Transform hpBar = healthUI.transform.Find("Bar/HP");
            hpBar.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(hpBar.GetComponent<RectTransform>().sizeDelta, new(-1 * origHbWidth * (1 - PlayerHealthPercent), hpBar.GetComponent<RectTransform>().sizeDelta.y), Time.deltaTime * 16);
            if (PlayerHealthPercent < 0.25)
                hpBar.GetComponent<Image>().color = Color.Lerp(new(0.85f, 0.05f, 0.05f), new(0.5f, 0.075f, 0.075f), Utils.SineTime(2.5));
            else
                hpBar.GetComponent<Image>().color = Color.white;
        }

        // stamina bar
        {
            Transform stBar = staminaUI.transform.Find("Bar/ST");
            stBar.GetComponent<RectTransform>().sizeDelta = new(-1 * origSbWidth * (1 - playerWalkControls.stamina), stBar.GetComponent<RectTransform>().sizeDelta.y);
            //stBar.GetComponent<Image>().color = Color.Lerp(new(1f, 1f, 1f), new(0.75f, 0.75f, 0.75f), 1 - (playerWalkControls.stamina * 2)); // change color based on stamina
            if (playerWalkControls.stamina >= 1 || playerLookControls.VehicleController != null)
                staminaUI.transform.localScale = new(0, 0, 0);
            else
                staminaUI.transform.localScale = new(1, 1, 1);
        }


        UpdateHits();
    }

    private void HealthChanged() => healthUI.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = playerHealth.health.ToString();

    private void WeaponUpdated()
    {
        TextMeshProUGUI weaponName = weaponUI.transform.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ammoCount = weaponUI.transform.Find("AmmoCount").GetComponent<TextMeshProUGUI>();
        weaponName.text = playerWeaponManager.ActiveWeapon.weaponName;

        if (playerWeaponManager.ActiveWeapon.ammoUsedPerShot > 0)
            ammoCount.text = "<mspace=0.333em>" + playerWeaponManager.ActiveWeapon.ammunition.ToString().PadLeft(3, '0');
        else
            ammoCount.text = string.Empty;

        weaponUI.GetComponent<RectTransform>().sizeDelta = new(weaponName.preferredWidth + 8, weaponUI.GetComponent<RectTransform>().sizeDelta.y);
        weaponName.GetComponent<RectTransform>().sizeDelta = new(weaponName.preferredWidth, weaponUI.GetComponent<RectTransform>().sizeDelta.y);
    }

    private void OnWeaponHit(bool killed = false)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject square = new("UIHitmarker");
            square.transform.SetParent(hitmarkerUI.transform, false);
            square.transform.localEulerAngles = new Vector3(0, 0, 45 + (i * 90));

            RectTransform rectTransform = square.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(8, 3f);
            rectTransform.anchoredPosition = Vector2.zero;

            Image image = square.AddComponent<Image>();
            if (killed)
            {
                rectTransform.sizeDelta = new Vector2(8.5f, 3.25f);
                image.color = new(0.4f, 0.025f, 0.01f);
            }
            else
            {
                image.color = Color.gray;
            }

            Destroy(square, 0.2f);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject square = new("UIHitmarker");
            square.transform.SetParent(hitmarkerUI.transform, false);
            square.transform.localEulerAngles = new Vector3(0, 0, 45 + (i * 90));

            RectTransform rectTransform = square.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(7, 2f);
            rectTransform.anchoredPosition = Vector2.zero;

            Image image = square.AddComponent<Image>();
            if (killed)
            {
                rectTransform.sizeDelta = new Vector2(7.5f, 2.25f);
                image.color = new(0.9f, 0.05f, 0.025f);
            }
            else
            {
                image.color = Color.white;
            }

            Destroy(square, 0.2f);
        }
    }

    private void UpdateHits()
    {
        foreach (Transform child in hitmarkerUI.transform)
        {
            if (child == null || child.name != "UIHitmarker")
                continue;

            float angle = child.transform.localEulerAngles.z * Mathf.Deg2Rad;
            Vector2 direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += 180 * Time.deltaTime * direction;

            float newX = Mathf.Pow(1.015f, Time.deltaTime * 140);
            float newY = Mathf.Pow(0.975f, Time.deltaTime * 140);
            rectTransform.sizeDelta *= new Vector2(newX, newY);
        }
    }

    public void RestartGame()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLookControls>().enabled = true;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
