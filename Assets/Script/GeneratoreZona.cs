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

    [Header("REGOLAZIONE ALTEZZA")]
    [Tooltip("Alza gli oggetti rispetto al terreno per non farli affondare (es: 0.1 o 0.2)")]
    public float offsetAltezza = 0.1f; // <--- NUOVO: Regolalo dall'Inspector!

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

                // LA MODIFICA: Prende il punto del terreno e lo sposta un pochino verso l'alto (Vector3.up)
                Vector3 posizioneSollevata = colpo.point + (Vector3.up * offsetAltezza);

                GameObject rifiuto = Instantiate(prefabScelto, posizioneSollevata, Quaternion.Euler(0, Random.Range(0, 360), 0));

                rifiuto.transform.parent = this.transform;
                generati++;
            }
        }

        Debug.Log("Fatto! Ho posizionato correttamente a terra " + generati + " oggetti.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(larghezzaX * 2, 100, lunghezzaZ * 2));
    }
}