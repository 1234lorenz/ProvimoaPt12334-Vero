using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance; // Per trovarlo facilmente da altri script

    public Image fadePanel;
    public float fadeDuration = 0.3f;

    void Awake()
    {
        Instance = this;
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    // Da trasparente a NERO (Uscita)
    private IEnumerator FadeOut(string sceneName)
    {
        fadePanel.gameObject.SetActive(true);
        float tempo = 0f;
        while (tempo < fadeDuration)
        {
            tempo += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, tempo / fadeDuration);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;
    }

    // --- NUOVO: Da NERO a trasparente (Entrata) ---
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float tempo = 0f;
        while (tempo < fadeDuration)
        {
            tempo += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, 1f - (tempo / fadeDuration));
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, 0); // Trasparente al 100%
        fadePanel.gameObject.SetActive(false);   // Disattiva il pannello così non blocca i click del mouse
    }
}