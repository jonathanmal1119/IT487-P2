using Assets.Scripts;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    private GameObject healthBar;
    private float origHbWidth;

    private PlayerWalkControls playerWalkControls;
    private GameObject staminaBar;
    private float origSbWidth;



    void Start()
    {
        EnemyText.text = "Enemies: " + GameObject.FindGameObjectsWithTag("Enemy").Count().ToString();

        playerHealth = Player.GetComponent<PlayerHealth>();

        healthBar = transform.Find("Health").gameObject;
        RectTransform hpBar = healthBar.transform.Find("Bar/HP").GetComponent<RectTransform>();
        origHbWidth = hpBar.rect.width;
        hpBar.sizeDelta = new(-1 * origHbWidth * (1 - PlayerHealthPercent), hpBar.sizeDelta.y);
        playerHealth.HealthChanged += HealthChanged;
        HealthChanged();


        playerWalkControls = Player.GetComponent<PlayerWalkControls>();

        staminaBar = transform.Find("Stamina").gameObject;
        RectTransform stBar = staminaBar.transform.Find("Bar/ST").GetComponent<RectTransform>();
        origSbWidth = stBar.rect.width;
        hpBar.sizeDelta = new(-1 * origSbWidth * (1 - playerWalkControls.stamina), hpBar.sizeDelta.y);
    }

    private void Update()
    {
        EnemyText.text = "Enemies: " + GameObject.FindGameObjectsWithTag("Enemy").Count().ToString();

        Transform hpBar = healthBar.transform.Find("Bar/HP");
        hpBar.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(hpBar.GetComponent<RectTransform>().sizeDelta, new(-1 * origHbWidth * (1 - PlayerHealthPercent), hpBar.GetComponent<RectTransform>().sizeDelta.y), Time.deltaTime * 16);
        if (PlayerHealthPercent < 0.25)
            hpBar.GetComponent<Image>().color = Color.Lerp(new(0.85f, 0.05f, 0.05f), new(0.5f, 0.075f, 0.075f), Utils.SineTime(2.5));
        else
            hpBar.GetComponent<Image>().color = Color.white;

        Transform stBar = staminaBar.transform.Find("Bar/ST");
        stBar.GetComponent<RectTransform>().sizeDelta = new(-1 * origSbWidth * (1 - playerWalkControls.stamina), stBar.GetComponent<RectTransform>().sizeDelta.y);
        //stBar.GetComponent<Image>().color = Color.Lerp(new(1f, 1f, 1f), new(0.75f, 0.75f, 0.75f), 1 - (playerWalkControls.stamina * 2)); // change color based on stamina
        if (playerWalkControls.stamina >= 1)
            staminaBar.transform.localScale = new(0, 0, 0);
        else
            staminaBar.transform.localScale = new(1, 1, 1);
    }

    private void HealthChanged()
    {
        healthBar.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = playerHealth.health.ToString();
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
