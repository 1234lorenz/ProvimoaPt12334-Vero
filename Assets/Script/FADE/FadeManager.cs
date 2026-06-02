using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1f;

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeOut(string sceneName)
    {
        // schermo diventa nero
        float tempo = 0f;
        while (tempo < fadeDuration)
        {
            tempo += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, tempo / fadeDuration);
            yield return null;
        }

        // carica la scena
        SceneManager.LoadScene(sceneName);
    }
}