using Assets.Scripts;
using System.Linq;
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



    void Start()
    {
        EnemyText.text = "Enemies: " + GameObject.FindGameObjectsWithTag("Enemy").Count().ToString();

        healthBar = transform.Find("Health").gameObject;
        //origHbWidth = healthBar.transform.Find("Bar/HP").GetComponent<RectTransform>().sizeDelta.x;

        playerHealth = Player.GetComponent<PlayerHealth>();

        //Player.GetComponent<PlayerHealth>().HealthChanged += HealthChanged;
        //HealthChanged();
        RectTransform bar = healthBar.transform.Find("Bar/HP").GetComponent<RectTransform>();
        origHbWidth = bar.rect.width;
        bar.sizeDelta = new(-1 * origHbWidth * (1 - PlayerHealthPercent), bar.sizeDelta.y);
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


    }

    private void HealthChanged()
    {
        //RectTransform bar = healthBar.transform.Find("Bar/HP").GetComponent<RectTransform>();
        //bar.sizeDelta = new(origHbWidth * PlayerHealthPercent, bar.sizeDelta.y);
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
