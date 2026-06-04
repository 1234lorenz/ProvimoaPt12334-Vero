using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
        {
            StartCoroutine(CaricaConRitardo());
        }
    }

    private IEnumerator CaricaConRitardo()
    {
        // aspetta un frame che tutti gli script siano inizializzati
        yield return null;
        yield return null;

        SaveManager.Instance.Carica(SaveManager.Instance.slotDaCaricare);
        SaveManager.Instance.slotDaCaricare = -1;
    }
}