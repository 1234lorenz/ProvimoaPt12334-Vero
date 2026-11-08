using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli")]
    public GameObject pannelloMenu;
    public GameObject pannelloOpzioni;
    public GameObject pannelloCrediti;
    public GameObject pannelloCaricamento;

    [Header("Riferimenti")]
    public CaricamentoUI caricamentoUI;
    private FadeManager fadeManager;

    void Start()
    {
        fadeManager = FindObjectOfType<FadeManager>();
        pannelloMenu.SetActive(true);
        pannelloOpzioni.SetActive(false);
        pannelloCrediti.SetActive(false);
        pannelloCaricamento.SetActive(false);
    }

    public void Gioca()
    {
        Time.timeScale = 1f;
        fadeManager.FadeToScene("SampleScene");
    }

    
    public void CaricaSlot(int numeroSlot)
    {
        Time.timeScale = 1f;

        // 1. Diciamo al SaveManager quale slot vogliamo che il GameLoader carichi dopo
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.slotDaCaricare = numeroSlot;
        }

        // 2. Avviamo il caricamento con il FADE (schermo nero)
        if (fadeManager != null)
        {
            fadeManager.FadeToScene("SampleScene");
        }
        else
        {
            // Sicurezza nel caso non trovasse il FadeManager
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void ApriOpzioni()
    {
        pannelloMenu.SetActive(false);
        pannelloOpzioni.SetActive(true);
    }

    public void ApriCrediti()
    {
        pannelloMenu.SetActive(false);
        pannelloOpzioni.SetActive(false); // Sicurezza
        pannelloCrediti.SetActive(true);
    }

    public void ApriCaricamento()
    {
        pannelloMenu.SetActive(false);
        pannelloCaricamento.SetActive(true);
        caricamentoUI.AggiornaSlotsUI();
    }

    public void Indietro()
    {
        pannelloMenu.SetActive(true);
        pannelloOpzioni.SetActive(false);
        pannelloCrediti.SetActive(false);
        pannelloCaricamento.SetActive(false);
    }

    public void Esci()
    {
        Debug.Log("Uscita dal gioco...");

        // Chiude il gioco se è avviato come applicazione (.exe)
        Application.Quit();

        // Chiude il gioco se lo stai provando dentro l'Editor di Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}