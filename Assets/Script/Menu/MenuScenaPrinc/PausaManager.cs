using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PausaManager : MonoBehaviour
{
    [Header("UI Pausa")]
    public GameObject pausaPanel;
    public GameObject opzioniPausaPanel;

    [Header("Slider Opzioni Pausa")]
    public Slider sliderMusicaPausa;
    public Slider sliderEffettiPausa;
    public Slider sliderSensibilitaPausa;
    public SalvataggioUI salvataggioUI;

    private bool isPaused = false;
    private PlayerInputHandler inputHandler;
    private FirstPersonController playerController;
    private FadeManager fadeManager;

    void Start()
    {
        inputHandler = FindObjectOfType<PlayerInputHandler>();
        playerController = FindObjectOfType<FirstPersonController>();
        fadeManager = FindObjectOfType<FadeManager>();
        pausaPanel.SetActive(false);
        if (opzioniPausaPanel != null)
            opzioniPausaPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Riprendi();
            else Pausa();
        }
    }

    public void Pausa()
    {
        isPaused = true;
        pausaPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerController.enabled = false;
    }

    public void Riprendi()
    {
        isPaused = false;
        pausaPanel.SetActive(false);
        if (opzioniPausaPanel != null)
            opzioniPausaPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.enabled = true;
    }

    public void ApriOpzioni()
    {
        pausaPanel.SetActive(false);
        opzioniPausaPanel.SetActive(true);

        if (sliderMusicaPausa != null)
            sliderMusicaPausa.value = PlayerPrefs.GetFloat("VolMusica", 0.75f);
        if (sliderEffettiPausa != null)
            sliderEffettiPausa.value = PlayerPrefs.GetFloat("VolEffetti", 0.75f);
        if (sliderSensibilitaPausa != null)
            sliderSensibilitaPausa.value = PlayerPrefs.GetFloat("Sensibilita", 0.1f);
    }

    public void ApriSalvataggio()
    {
        pausaPanel.SetActive(false);
        salvataggioUI.ApriSalvataggio();
    }

    public void IndietroOpzioni()
    {
        pausaPanel.SetActive(true);
        opzioniPausaPanel.SetActive(false);
    }

    public void ChiudiSalvataggio()
    {
        salvataggioUI.ChiudiSalvataggio();
        pausaPanel.SetActive(true);
    }
    public void EsciAlMenu()
    {
        Time.timeScale = 1f;
        if (fadeManager != null)
            fadeManager.FadeToScene("MenuPrincipale");
        else
            SceneManager.LoadScene("MenuPrincipale");
    }
}