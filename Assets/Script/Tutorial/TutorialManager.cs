using UnityEngine;
using System.Collections; //  AGGIUNTO per usare le attese (Coroutine)

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Pannelli Tutorial (Trascinali qui)")]
    public GameObject panelIntro;
    public GameObject panelMovimento;
    public GameObject panelRifiuto;
    public GameObject panelInventario;
    public GameObject panelMissioni;

    public enum Fase { Intro, Movimento, CercaRifiuto, SpiegaRifiuto, Raccogli, ApriInventario, ApriMissioni, Finito }
    public Fase faseAttuale = Fase.Intro;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //  MODIFICA: Ora Start è un IEnumerator, così può ASPETTARE!
    IEnumerator Start()
    {
        DisattivaTuttiIPannelli(); // Spegniamo la UI per sicurezza mentre aspettiamo

        // 1. Stiamo caricando una partita? (slotDaCaricare è diverso da -1 se stiamo caricando)
        if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
        {
            Debug.Log("TutorialManager: Sto aspettando che il GameLoader carichi il salvataggio...");

            // Aspetta finché il SaveManager non finisce il caricamento (e rimette lo slot a -1)
            yield return new WaitUntil(() => SaveManager.Instance.slotDaCaricare == -1);

            // Aspettiamo un altro micro-secondo per sicurezza
            yield return new WaitForSeconds(0.1f);
        }

        // 2. ORA il GameLoader ha sicuramente finito di caricare i dati! Controlliamo:
        if (SaveManager.Instance != null && SaveManager.Instance.datiCaricati != null)
        {
            if (SaveManager.Instance.datiCaricati.tutorialCompleto)
            {
                Debug.Log(" [TUTORIAL] Salvataggio rilevato con tutorial completato. Mi distruggo.");
                faseAttuale = Fase.Finito;
                Time.timeScale = 1f; // Assicura che il tempo non resti congelato
                Destroy(gameObject); // Elimina il tutorial
                yield break; // Ferma questo script qui, non andare oltre
            }
        }

        // 3. Se arriviamo qui, significa che è una Nuova Partita o il tutorial non era finito.
        AvviaFase(Fase.Intro);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (faseAttuale == Fase.Intro) AvviaFase(Fase.Movimento);
            else if (faseAttuale == Fase.Movimento) AvviaFase(Fase.CercaRifiuto);
            else if (faseAttuale == Fase.SpiegaRifiuto) AvviaFase(Fase.Raccogli);
        }
    }

    public void AvviaFase(Fase nuovaFase)
    {
        faseAttuale = nuovaFase;
        DisattivaTuttiIPannelli();

        switch (faseAttuale)
        {
            case Fase.Intro:
                Time.timeScale = 0f;
                panelIntro.SetActive(true);
                break;
            case Fase.Movimento:
                panelMovimento.SetActive(true);
                break;
            case Fase.CercaRifiuto:
                Time.timeScale = 1f;
                break;
            case Fase.SpiegaRifiuto:
                Time.timeScale = 0f;
                panelRifiuto.SetActive(true);
                break;
            case Fase.Raccogli:
                Time.timeScale = 1f;
                break;
            case Fase.ApriInventario:
                Time.timeScale = 0f;
                panelInventario.SetActive(true);
                break;
            case Fase.ApriMissioni:
                panelMissioni.SetActive(true);
                break;
            case Fase.Finito:
                Time.timeScale = 1f;
                break;
        }
    }

    private void DisattivaTuttiIPannelli()
    {
        panelIntro.SetActive(false);
        panelMovimento.SetActive(false);
        panelRifiuto.SetActive(false);
        panelInventario.SetActive(false);
        panelMissioni.SetActive(false);
    }

    public void OnRifiutoAvvicinato() { if (faseAttuale == Fase.CercaRifiuto) AvviaFase(Fase.SpiegaRifiuto); }
    public void OnRifiutoRaccolto() { if (faseAttuale == Fase.Raccogli) AvviaFase(Fase.ApriInventario); }
    public void OnInventarioAperto() { if (faseAttuale == Fase.ApriInventario) AvviaFase(Fase.ApriMissioni); }
    public void OnMissioniAperte() { if (faseAttuale == Fase.ApriMissioni) AvviaFase(Fase.Finito); }
}