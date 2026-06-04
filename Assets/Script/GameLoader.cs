using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;

        if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
        {
            StartCoroutine(CaricaConRitardo());
        }
    }

    
    private IEnumerator CaricaConRitardo()
    {
        Time.timeScale = 1f;
        yield return null;
        yield return null;

        SaveManager.Instance.Carica(SaveManager.Instance.slotDaCaricare);
        SaveManager.Instance.slotDaCaricare = -1;

        // aggiorna UI dopo il caricamento
        yield return null;

        CounterMenagerRifiuti counter = FindObjectOfType<CounterMenagerRifiuti>();
        if (counter != null)
            counter.AggiornaTuttoUI();

        MissionManager.Instance.AggiornaMissioniUI();
        XPManager.Instance.AggiornaUI();
    }

}