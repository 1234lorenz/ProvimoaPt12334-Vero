using UnityEngine;
using System.Collections;

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

    IEnumerator Start()
    {
        DisattivaTuttiIPannelli();

        // 1. Stiamo caricando una partita?
        if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
        {
            Debug.Log("TutorialManager: Sto aspettando che il GameLoader carichi il salvataggio...");

            yield return new WaitUntil(() => SaveManager.Instance.slotDaCaricare == -1);
            yield return new WaitForSeconds(0.1f);
        }

        // 2. Controlliamo se il tutorial era già completato
        if (SaveManager.Instance != null && SaveManager.Instance.datiCaricati != null)
        {
            if (SaveManager.Instance.datiCaricati.tutorialCompleto)
            {
                Debug.Log(" [TUTORIAL] Salvataggio rilevato con tutorial completato. Mi distruggo.");
                faseAttuale = Fase.Finito;
                BloccaGiocatore(false);
                Destroy(gameObject);
                yield break;
            }
        }

        // 3. Nuova partita o tutorial non completato
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
                BloccaGiocatore(true);
                panelIntro.SetActive(true);
                break;
            case Fase.Movimento:
                BloccaGiocatore(false);
                panelMovimento.SetActive(true);
                break;
            case Fase.CercaRifiuto:
                BloccaGiocatore(false);
                break;
            case Fase.SpiegaRifiuto:
                BloccaGiocatore(true);
                panelRifiuto.SetActive(true);
                break;
            case Fase.Raccogli:
                BloccaGiocatore(false);
                break;
            case Fase.ApriInventario:
                BloccaGiocatore(true);
                panelInventario.SetActive(true);
                break;
            case Fase.ApriMissioni:
                panelMissioni.SetActive(true);
                break;
            case Fase.Finito:
                BloccaGiocatore(false);
                break;
        }
    }

    private void BloccaGiocatore(bool blocca)
    {
        var player = FindObjectOfType<FirstPersonController>();
        if (player != null) player.enabled = !blocca;

        if (blocca)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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