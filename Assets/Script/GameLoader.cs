using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;

        // FORZIAMO il nero subito, prima ancora che il gioco disegni il primo frame
        if (FadeManager.Instance != null && FadeManager.Instance.fadePanel != null)
        {
            FadeManager.Instance.fadePanel.gameObject.SetActive(true);
            FadeManager.Instance.fadePanel.color = new Color(0, 0, 0, 1); // Nero opaco al 100%
        }

        if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
        {
            StartCoroutine(CaricaConRitardo());
        }
        else
        {
            if (FadeManager.Instance != null) FadeManager.Instance.StartFadeIn();
        }
    }

    private IEnumerator CaricaConRitardo()
    {
        // Aspettiamo un attimo prima di caricare per dare tempo al sistema di inizializzare
        yield return new WaitForSeconds(0.1f);

        SaveManager.Instance.Carica(SaveManager.Instance.slotDaCaricare);
        SaveManager.Instance.slotDaCaricare = -1;

        // Aspettiamo che il teletrasporto e il posizionamento siano completati
        yield return new WaitForSeconds(0.4f);

        // Ora che tutto è al posto giusto, togliamo il nero
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.StartFadeIn();
        }
    }
}