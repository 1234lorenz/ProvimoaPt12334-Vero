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
    public int xpToLevel3 = 390;
    public int xpToWin = 330;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public Slider xpSlider;
    public GameObject levelUpPanel;
    public TextMeshProUGUI levelUpText;

    [Header("Zone da sbloccare")]
    public ZonaBloccata zonaBloccata2;
    public ZonaBloccata zonaBloccata3;
    public ZonaBloccata zonaBloccata3_2;

    [Header("Pannello Vittoria")]
    public GameObject vittoriaPanel;
    public TextMeshProUGUI vittoriaText;

    // Traccia se la storia è stata già completata per evitare di riaprire il pannello
    [HideInInspector] public bool giocoFinito = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (vittoriaPanel != null) vittoriaPanel.SetActive(false);

        // Sblocco automatico cancelli per i test dai livelli avanzati
        if (currentLevel >= 2 && zonaBloccata2 != null)
            zonaBloccata2.Sblocca();

        if (currentLevel >= 3)
        {
            if (zonaBloccata3 != null) zonaBloccata3.Sblocca();
            if (zonaBloccata3_2 != null) zonaBloccata3_2.Sblocca();
        }
    }

    public void ApplicaLivelloCaricato()
    {
        UpdateUI();

        if (currentLevel >= 2 && zonaBloccata2 != null)
        {
            zonaBloccata2.Sblocca();
        }

        if (currentLevel >= 3)
        {
            if (zonaBloccata3 != null) zonaBloccata3.Sblocca();
            if (zonaBloccata3_2 != null) zonaBloccata3_2.Sblocca();

            // Riconosce se il gioco era già stato vinto nel salvataggio caricato
            if (currentXP >= xpToWin && MissionManager.Instance.Level3Complete())
            {
                giocoFinito = true;
            }
        }
    }

    public void AggiornaUI()
    {
        UpdateUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP < 0) currentXP = 0;

        // Blocco dei Punti Eco alla soglia target se le missioni della zona non sono ancora complete
        if (currentLevel == 1 && !MissionManager.Instance.Level1Complete() && currentXP > xpToLevel2)
        {
            currentXP = xpToLevel2;
        }
        else if (currentLevel == 2 && !MissionManager.Instance.Level2Complete() && currentXP > xpToLevel3)
        {
            currentXP = xpToLevel3;
        }
        else if (currentLevel == 3 && !MissionManager.Instance.Level3Complete() && currentXP > xpToWin)
        {
            currentXP = xpToWin;
        }

        UpdateUI();
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        // Se la vittoria è già stata mostrata/registrata, non riaprire mai più la finestra
        if (giocoFinito) return;

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
            {
                giocoFinito = true;
                Win();
            }
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

        if (newLevel == 3)
        {
            if (zonaBloccata3 != null) zonaBloccata3.Sblocca();
            if (zonaBloccata3_2 != null) zonaBloccata3_2.Sblocca();
        }

        UpdateUI();
    }

    private IEnumerator ShowLevelUp(int level)
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            if (levelUpText != null) levelUpText.text = $"Zona {level} sbloccata!";
            yield return new WaitForSeconds(3f);
            levelUpPanel.SetActive(false);
        }
    }

    private void Win()
    {
        StartCoroutine(ShowVittoria());
    }

    private IEnumerator ShowVittoria()
    {
        yield return new WaitForSeconds(1f);

        if (vittoriaPanel != null)
            vittoriaPanel.SetActive(true);

        int totaleRifiuti = MissionManager.Instance.plasticaSmaltite +
                            MissionManager.Instance.organicoSmaltiti +
                            MissionManager.Instance.cartaSmaltita +
                            MissionManager.Instance.vetroSmaltito +
                            MissionManager.Instance.metalloSmaltito +
                            MissionManager.Instance.seccoSmaltito;

        if (vittoriaText != null)
        {
            vittoriaText.text =
                $"Hai completato tutte le missioni!\n\n" +
                $"Rifiuti smaltiti: {totaleRifiuti}\n" +
                $"Punti Eco totali: {currentXP}\n\n" +
                $"L'isola è più pulita grazie a te!\n" +
                $"Puoi continuare a raccogliere altri rifiuti.";
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinuaAGiocare()
    {
        if (vittoriaPanel != null)
            vittoriaPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Aggiorna l'interfaccia delle missioni nascondendola a fine gioco
        MissionManager.Instance.AggiornaMissioniUI();
    }

    public void TornaAlMenuDaVittoria()
    {
        Time.timeScale = 1f;
        FindObjectOfType<FadeManager>().FadeToScene("MenuPrincipale");
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = $"Livello {currentLevel}";

        int xpNext = currentLevel == 1 ? xpToLevel2 :
                     currentLevel == 2 ? xpToLevel3 : xpToWin;

        if (xpText != null)
        {
            xpText.text = $"{currentXP}/{xpNext} Punti Eco";
        }

        if (xpSlider != null && xpNext > 0)
        {
            xpSlider.value = (float)currentXP / xpNext;
        }
    }
}