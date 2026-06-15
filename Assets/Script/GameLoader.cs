using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    [Header("Fisica")]
    [Tooltip("Quanti secondi di caduta simulare prima di mostrare la scena")]
    public float secondiDaSimulare = 1.5f;

    void Start()
    {
        Time.timeScale = 1f;

        // FORZIAMO il nero subito
        if (FadeManager.Instance != null && FadeManager.Instance.fadePanel != null)
        {
            FadeManager.Instance.fadePanel.gameObject.SetActive(true);
            FadeManager.Instance.fadePanel.color = new Color(0, 0, 0, 1);
        }

        if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
        {
            StartCoroutine(CaricaConRitardo());
        }
        else
        {
            StartCoroutine(SimulaCadutaENuovaPartita());
        }
    }

    // NUOVA PARTITA — fai cadere i rifiuti mentre è nero, poi fade in
    private IEnumerator SimulaCadutaENuovaPartita()
    {
        yield return SimulaFisica();

        if (FadeManager.Instance != null) FadeManager.Instance.StartFadeIn();
    }

    // CARICAMENTO SALVATAGGIO — carica i dati, poi simula la caduta (per i rifiuti rimasti), poi fade in
    private IEnumerator CaricaConRitardo()
    {
        yield return new WaitForSeconds(0.1f);

        SaveManager.Instance.Carica(SaveManager.Instance.slotDaCaricare);
        SaveManager.Instance.slotDaCaricare = -1;

        yield return new WaitForSeconds(0.4f);

        yield return SimulaFisica();

        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.StartFadeIn();
        }
    }

    // Fa avanzare la fisica "a velocità accelerata" mentre lo schermo è nero
    private IEnumerator SimulaFisica()
    {
        Physics.simulationMode = SimulationMode.Script;

        float passi = secondiDaSimulare / Time.fixedDeltaTime;
        for (int i = 0; i < passi; i++)
        {
            Physics.Simulate(Time.fixedDeltaTime);
            yield return null; // lascia respirare un frame per non bloccare l'editor
        }

        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
}