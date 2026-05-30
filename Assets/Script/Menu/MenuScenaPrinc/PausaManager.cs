using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PausaManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausaPanel;
    public GameObject opzioniPausaPanel;

    private bool isPaused = false;
    private PlayerInputHandler inputHandler;
    private FirstPersonController playerController;

    void Start()
    {
        inputHandler = FindObjectOfType<PlayerInputHandler>();
        playerController = FindObjectOfType<FirstPersonController>();
        pausaPanel.SetActive(false);
    }

    void Update()
    {
        // premi ESC per aprire/chiudere la pausa
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
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.enabled = true;
    }
    public void ApriOpzioni()
    {
        pausaPanel.SetActive(false);
        opzioniPausaPanel.SetActive(true);
    }

    public void IndietroOpzioni()
    {
        pausaPanel.SetActive(true);
        opzioniPausaPanel.SetActive(false);
    }
    public void EsciAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipale");
    }
}