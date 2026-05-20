using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance;

    [Header("Livelli")]
    public int currentLevel = 1;
    public int currentXP = 0;

    [Header("Punti Eco per salire di livello")]
    public int xpToLevel2 = 300;
    public int xpToLevel3 = 800;
    public int xpToWin = 1500;

    [Header("Punti Eco per azione")]
    public int xpRaccolta = 10;
    public int xpSmaltimento = 30;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public Slider xpSlider;
    public GameObject levelUpPanel;
    public TextMeshProUGUI levelUpText;

    [Header("Zone da sbloccare")]
    public ZonaBloccata zonaBloccata2;
    public ZonaBloccata zonaBloccata3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        levelUpPanel.SetActive(false);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP < 0) currentXP = 0; // non andare sotto zero
        UpdateUI();
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (currentLevel == 1 && currentXP >= xpToLevel2)
        {
            if (MissionManager.Instance.Level1Complete())
                LevelUp(2);
        }
        else if (currentLevel == 2 && currentXP >= xpToLevel3)
        {
            if (MissionManager.Instance.Level2Complete())
                LevelUp(3);
        }
        else if (currentLevel == 3 && currentXP >= xpToWin)
        {
            if (MissionManager.Instance.Level3Complete())
                Win();
        }
    }

    private void LevelUp(int newLevel)
    {
        currentLevel = newLevel;
        currentXP = 0;
        MissionManager.Instance.ResetMissioni();
        StartCoroutine(ShowLevelUp(newLevel));

        if (newLevel == 2 && zonaBloccata2 != null)
            zonaBloccata2.Sblocca();
        if (newLevel == 3 && zonaBloccata3 != null)
            zonaBloccata3.Sblocca();

        UpdateUI();
    }

    private IEnumerator ShowLevelUp(int level)
    {
        levelUpPanel.SetActive(true);
        levelUpText.text = level == 0 ? "Hai vinto! Spiaggia pulita!"
                                      : $"Zona {level} sbloccata!";
        yield return new WaitForSeconds(3f);
        levelUpPanel.SetActive(false);
    }

    private void Win()
    {
        StartCoroutine(ShowLevelUp(0));
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = $"Livello {currentLevel}";

        if (xpText != null)
        {
            int xpNext = currentLevel == 1 ? xpToLevel2 :
                         currentLevel == 2 ? xpToLevel3 : xpToWin;
            xpText.text = $"{currentXP}/{xpNext} Punti Eco";
        }

        if (xpSlider != null)
        {
            int xpPrev = currentLevel == 1 ? 0 :
                         currentLevel == 2 ? xpToLevel2 : xpToLevel3;
            int xpNext = currentLevel == 1 ? xpToLevel2 :
                         currentLevel == 2 ? xpToLevel3 : xpToWin;
            xpSlider.value = (float)(currentXP - xpPrev) / (xpNext - xpPrev);
        }
    }
}