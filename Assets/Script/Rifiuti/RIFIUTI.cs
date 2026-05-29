using UnityEngine;

public class RIFIUTI : MonoBehaviour
{
    // Tipo di rifiuto (Plastica, Metallo, Carta, Vetro) — si imposta dall'Inspector
    public RIFIUTIType type;

    [Header("Outline")]
    // Colore del bordo luminoso
    public Color outlineColor = Color.cyan;
    // Spessore del bordo (da 0 a 0.1)
    [Range(0f, 0.1f)] public float outlineWidth = 0.02f;

    // Il materiale dell'outline — creato una volta sola e riutilizzato sempre
    private Material outlineMat;

    // Contiene tutti i Renderer dell'oggetto (corpo, tappo, etichetta ecc.)
    private Renderer[] renderers;

    // Salva il numero originale di materiali per ogni renderer
    private int[] originalMaterialCounts;

    // Semaforo: true = outline attivo, false = outline spento
    private bool outlined = false;

    void Awake()
    {
        // Raccoglie tutti i Renderer dell'oggetto e dei suoi figli
        renderers = GetComponentsInChildren<Renderer>();

        // Salva il numero originale di materiali per ogni renderer
        originalMaterialCounts = new int[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterialCounts[i] = renderers[i].sharedMaterials.Length;
    }

    // Chiamata da Collector.cs — active=true accende il bordo, active=false lo spegne
    public void SetOutline(bool active)
    {
        // Evita di rieseguire il codice se lo stato è già quello desiderato
        if (outlined == active) return;
        outlined = active;

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            var mats = new System.Collections.Generic.List<Material>(r.materials);

            if (active)
            {
                // Crea il materiale outline solo la prima volta
                if (outlineMat == null)
                {
                    outlineMat = new Material(Shader.Find("Custom/OutlineURP"));
                    outlineMat.SetColor("_OutlineColor", outlineColor);
                    outlineMat.SetFloat("_OutlineWidth", outlineWidth);
                }
                // Aggiunge l'outline in fondo alla lista
                mats.Add(outlineMat);
            }
            else
            {
                // Rimuovi fino a tornare al numero originale di materiali
                while (mats.Count > originalMaterialCounts[i])
                    mats.RemoveAt(mats.Count - 1);
            }

            // Applica la lista aggiornata al Renderer
            r.materials = mats.ToArray();
        }
    }
}

public enum RIFIUTIType { Plastica, Metallo, Carta, Vetro, Organico }