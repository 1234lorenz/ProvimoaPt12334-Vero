using UnityEngine;

public class GeneratoreZona : MonoBehaviour
{
    [Header("Rifiuti ESCLUSIVI per questa zona")]
    public GameObject[] prefabsRifiuti;

    [Header("Quanti rifiuti vuoi qui?")]
    public int quantita = 20;

    [Header("Grandezza dell'area")]
    public float larghezzaX = 30f;
    public float lunghezzaZ = 30f;

    [Header("COSA DEVE COLPIRE IL RAGGIO?")]
    public LayerMask stratoDaColpire;

    [Header("REGOLAZIONE FISICA EXTRA (NUOVO)")]
    [Tooltip("Altezza da cui far CASCARE l'oggetto")]
    public float altezzaCaduta = 2.5f;

    [Range(0f, 10f)]
    [Tooltip("Forza casuale che spinge l'oggetto di lato appena nasce per farlo rotolare di più")]
    public float spintaCasualeMassima = 5f;

    [Range(0f, 50f)]
    [Tooltip("Effetto 'trottola': quanto forte l'oggetto deve girare su se stesso a mezz'aria")]
    public float rotazioneFisicaMassima = 20f;

    // Generatore di numeri casuali "ripetibile": con lo stesso seed produce sempre la stessa sequenza.
    // Serve a rigenerare la stessa mappa quando carichi un salvataggio.
    private System.Random rng;

    // Seed della partita in corso, condiviso da tutte le zone della stessa scena
    private static int seedScenaHandle = int.MinValue;
    private static int seedValore;

    // --- NUOVO: Questo fa partire la pioggia di rifiuti da sola appena avvii il gioco! ---
    private void Awake()
    {
        Spargi(false);
    }

    // Se premiamo a mano nell'editor svuota la zona, all'avvio del gioco no (per sicurezza).
    [ContextMenu("Genera Rifiuti In Questa Zona")]
    public void SpargiEditor()
    {
        Spargi(true);
    }

    public void Spargi(bool svuotaPrima)
    {
        transform.localScale = Vector3.one;

        // Svuota i vecchi figli solo se lo abbiamo chiesto esplicitamente (utile in Editor)
        if (svuotaPrima)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                // In gioco si usa Destroy, in Editor si usa DestroyImmediate
                if (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        // Ogni zona ha il suo generatore, ma parte dallo stesso seed della partita
        int seed = ScegliSeed();
        int seedZona;
        unchecked { seedZona = seed + StableHash(gameObject.name); }
        rng = new System.Random(seedZona);

        int generati = 0;

        for (int i = 0; i < quantita; i++)
        {
            float x = R(-larghezzaX, larghezzaX);
            float z = R(-lunghezzaZ, lunghezzaZ);

            Vector3 puntoInCielo = new Vector3(transform.position.x + x, transform.position.y + 500f, transform.position.z + z);

            if (Physics.Raycast(puntoInCielo, Vector3.down, out RaycastHit colpo, Mathf.Infinity, stratoDaColpire, QueryTriggerInteraction.Ignore))
            {
                GameObject prefabScelto = prefabsRifiuti[rng.Next(0, prefabsRifiuti.Length)];

                Vector3 posizioneAria = colpo.point + (Vector3.up * altezzaCaduta);

                // 1. Rotazione visiva di partenza totalmente imprevedibile
                Quaternion rotazioneCasuale = Quaternion.Euler(R(0f, 360f), R(0f, 360f), R(0f, 360f));

                GameObject rifiuto = Instantiate(prefabScelto, posizioneAria, rotazioneCasuale);
                rifiuto.transform.parent = this.transform;
                generati++;

                // ID UNICO: nome della zona + numero del rifiuto.
                // Con lo stesso seed, il rifiuto numero i e' sempre lo stesso: serve al salvataggio.
                RIFIUTI scriptRifiuto = rifiuto.GetComponentInChildren<RIFIUTI>();
                if (scriptRifiuto != null)
                    scriptRifiuto.rifiutoID = gameObject.name + "_" + i;

                
                Rigidbody rb = rifiuto.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Crea una direzione di lancio a caso (escludendo l'asse Y per non spararli in alto)
                    Vector3 direzioneLancio = new Vector3(R(-1f, 1f), 0f, R(-1f, 1f)).normalized;
                    float forzaLancio = R(0f, spintaCasualeMassima);

                    // Applica la spinta laterale iniziale
                    rb.AddForce(direzioneLancio * forzaLancio, ForceMode.Impulse);

                    // Crea un effetto trottola casuale su tutti gli assi
                    Vector3 coppiaRotazione = new Vector3(R(-1f, 1f), R(-1f, 1f), R(-1f, 1f)) * R(0f, rotazioneFisicaMassima);

                    // Applica la rotazione fisica interna
                    rb.AddTorque(coppiaRotazione, ForceMode.Impulse);
                }
            }
        }

        Debug.Log("Fatto! Generati " + generati + " oggetti con fisica ultra-caotica in questa zona.");//debug di cobtrollo
    }

    // Numero casuale tra min e max, ma ripetibile
    private float R(float min, float max)
    {
        return (float)(rng.NextDouble() * (max - min) + min);
    }

    // Sceglie il seed della partita: quello del salvataggio se stai caricando, altrimenti uno nuovo.
    // E' lo stesso per tutte le zone della stessa scena.
    private int ScegliSeed()
    {
        // In Editor (premendo il menu a mano) basta un seed a caso
        if (!Application.isPlaying) return Random.Range(1, int.MaxValue);

        int handle = gameObject.scene.handle;
        if (handle != seedScenaHandle)
        {
            seedScenaHandle = handle;
            seedValore = 0;

            // Partita caricata: riprendi il seed salvato
            if (SaveManager.Instance != null && SaveManager.Instance.slotDaCaricare != -1)
            {
                SaveData dati = SaveManager.Instance.LeggiSlot(SaveManager.Instance.slotDaCaricare);
                if (dati != null) seedValore = dati.seedRifiuti;
            }

            // Partita nuova, o salvataggio vecchio senza seed: ne serve uno nuovo
            if (seedValore == 0) seedValore = Random.Range(1, int.MaxValue);

            // Lo passiamo al SaveManager, che lo scrive nel salvataggio
            if (SaveManager.Instance != null) SaveManager.Instance.seedRifiuti = seedValore;
        }
        return seedValore;
    }

    // Hash del nome che non cambia da una partita all'altra
    private static int StableHash(string s)
    {
        unchecked
        {
            int h = 17;
            foreach (char c in s) h = h * 31 + c;
            return h;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(larghezzaX * 2, 100, lunghezzaZ * 2));
    }
}