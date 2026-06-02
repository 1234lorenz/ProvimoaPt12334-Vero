using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli")]
    public GameObject pannelloMenu;      // pannello principale
    public GameObject pannelloOpzioni;   // pannello opzioni
    public GameObject pannelloCrediti;   // pannello crediti

    private FadeManager fadeManager;
    void Start()
    {
        // mostra solo il menu principale all'avvio
        fadeManager = FindObjectOfType<FadeManager>();

        pannelloMenu.SetActive(true);
        pannelloOpzioni.SetActive(false);
        pannelloCrediti.SetActive(false);
    }

    // collegato al bottone GIOCA
    public void Gioca()
    {
        Time.timeScale = 1f;
        fadeManager.FadeToScene("SampleScene");
    }

    // collegato al bottone OPZIONI
    public void ApriOpzioni()
    {
        pannelloMenu.SetActive(false);
        pannelloOpzioni.SetActive(true);
    }

    // collegato al bottone CREDITI
    public void ApriCrediti()
    {
        pannelloMenu.SetActive(false);
        pannelloCrediti.SetActive(true);
    }

    // collegato al bottone INDIETRO nei pannelli
    public void Indietro()
    {
        pannelloMenu.SetActive(true);
        pannelloOpzioni.SetActive(false);
        pannelloCrediti.SetActive(false);
    }

    // collegato al bottone ESCI
    public void Esci()
    {
        Application.Quit();
        Debug.Log("Uscita dal gioco");
    }
}