using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeInEffect());
    }

    private IEnumerator FadeInEffect()
    {
        // parte nero
        fadePanel.color = new Color(0, 0, 0, 1);

        float tempo = 0f;
        while (tempo < fadeDuration)
        {
            tempo += Time.unscaledDeltaTime;
            fadePanel.color = new Color(0, 0, 0, 1 - tempo / fadeDuration);
            yield return null;
        }

        // completamente trasparente
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.gameObject.SetActive(false); // Disattiva per sicurezza
    }
}