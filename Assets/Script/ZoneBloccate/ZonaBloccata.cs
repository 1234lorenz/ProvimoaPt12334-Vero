using UnityEngine;
using TMPro;
using System.Collections;

public class ZonaBloccata : MonoBehaviour
{
    [Header("Zona")]
    public int zonaNumber = 2;

    [Header("Oggetti 3D")]
    public GameObject barriera;
    public GameObject overlay;
    public GameObject lucchetto;
    public TextMeshPro testoZona; // TMP 3D non UI!

    private bool isUnlocked = false;

    void Start()
    {
        // tutto attivo all'inizio — zona bloccata
        barriera.SetActive(true);
        overlay.SetActive(true);
        lucchetto.SetActive(true);
        testoZona.gameObject.SetActive(true);
    }

    public void Sblocca()
    {
        if (isUnlocked) return;
        StartCoroutine(AnimazioneSblocco());
    }

    private IEnumerator AnimazioneSblocco()
    {
        // cambia testo
        testoZona.text = $"Zona {zonaNumber} sbloccata!";

        // animazione lucchetto
        float tempo = 0f;
        float durata = 1f;
        Vector3 scalaIniziale = lucchetto.transform.localScale;

        while (tempo < durata)
        {
            tempo += Time.deltaTime;
            float t = tempo / durata;
            lucchetto.transform.Rotate(0, 360 * Time.deltaTime, 0);
            lucchetto.transform.localScale = Vector3.Lerp(scalaIniziale, Vector3.zero, t);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // rimuovi tutto
        barriera.SetActive(false);
        overlay.SetActive(false);
        lucchetto.SetActive(false);
        testoZona.gameObject.SetActive(false);

        isUnlocked = true;
    }
}