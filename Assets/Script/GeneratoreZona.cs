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

    [ContextMenu("Genera Rifiuti In Questa Zona")]
    public void Spargi()
    {
        transform.localScale = Vector3.one;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        int generati = 0;

        for (int i = 0; i < quantita; i++)
        {
            float x = Random.Range(-larghezzaX, larghezzaX);
            float z = Random.Range(-lunghezzaZ, lunghezzaZ);

            Vector3 puntoInCielo = new Vector3(transform.position.x + x, transform.position.y + 500f, transform.position.z + z);

            if (Physics.Raycast(puntoInCielo, Vector3.down, out RaycastHit colpo, Mathf.Infinity, stratoDaColpire, QueryTriggerInteraction.Ignore))
            {
                GameObject prefabScelto = prefabsRifiuti[Random.Range(0, prefabsRifiuti.Length)];

                Vector3 posizioneAria = colpo.point + (Vector3.up * altezzaCaduta);

                // 1. Rotazione visiva di partenza totalmente imprevedibile
                Quaternion rotazioneCasuale = Random.rotation;

                GameObject rifiuto = Instantiate(prefabScelto, posizioneAria, rotazioneCasuale);
                rifiuto.transform.parent = this.transform;
                generati++;

                // 2. MAGIA DELLA FISICA EXTRA: Diamo una spinta reale al Rigidbody se presente
                Rigidbody rb = rifiuto.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Crea una direzione di lancio a caso (escludendo l'asse Y per non spararli in alto)
                    Vector3 direzioneLancio = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                    float forzaLancio = Random.Range(0f, spintaCasualeMassima);

                    // Applica la spinta laterale iniziale
                    rb.AddForce(direzioneLancio * forzaLancio, ForceMode.Impulse);

                    // Crea un effetto trottola casuale su tutti gli assi
                    Vector3 coppiaRotazione = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(0f, rotazioneFisicaMassima);

                    // Applica la rotazione fisica interna
                    rb.AddTorque(coppiaRotazione, ForceMode.Impulse);
                }
            }
        }

        Debug.Log("Fatto! Generati " + generati + " oggetti con fisica ultra-caotica.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(larghezzaX * 2, 100, lunghezzaZ * 2));
    }
}