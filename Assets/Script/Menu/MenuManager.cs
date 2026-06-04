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

    public void ApriOpzioni()
    {
        pannelloMenu.SetActive(false);
        pannelloOpzioni.SetActive(true);
    }

    public void ApriCrediti()
    {
        pannelloMenu.SetActive(false);
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
        Application.Quit();
        Debug.Log("Uscita dal gioco");
    }
}