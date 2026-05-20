using UnityEngine;

public class RIFIUTI : MonoBehaviour
{
    // Tipo di rifiuto (Plastica, Metallo, Carta, Vetro) — si imposta dall'Inspector
    public RIFIUTIType type;

    [Header("Outline")]
    // Colore del bordo luminoso
    public Color outlineColor = Color.cyan;
    // Spessore del bordo (da 0 a 0.1)
    [Range(0f, 0.1f)] public float outlineWidth = 0.01f;

    // Il materiale dell'outline — creato una volta sola e riutilizzato sempre
    private Material outlineMat;

    // Contiene tutti i Renderer dell'oggetto (corpo, tappo, etichetta ecc.)
    // Serve perché un oggetto 3D può essere fatto di più mesh separate
    private Renderer[] renderers;

    // Semaforo: true = outline attivo, false = outline spento
    // Evita di fare lavoro inutile se lo stato non è cambiato
    private bool outlined = false;

    void Awake()
    {
        // Raccoglie tutti i Renderer dell'oggetto e dei suoi figli
        // GetComponentsInChildren parte dall'oggetto su cui è attaccato lo script
        renderers = GetComponentsInChildren<Renderer>();
    }

    // Chiamata da Collector.cs — active=true accende il bordo, active=false lo spegne
    public void SetOutline(bool active)
    {
        // Se lo stato è già quello che vogliamo, esci subito — nessun lavoro inutile
        if (outlined == active) return;

        // Aggiorna il semaforo
        outlined = active;

        // Scorre ogni pezzo dell'oggetto (corpo, tappo, etichetta ecc.)
        foreach (var r in renderers)
        {
            // Copia i materiali attuali in una List modificabile
            // Non si può aggiungere/rimuovere direttamente da sharedMaterials
            var mats = new System.Collections.Generic.List<Material>(r.sharedMaterials);

            if (active)
            {
                // Crea il materiale outline solo la prima volta (risparmio memoria)
                if (outlineMat == null)
                {
                    // Trova lo shader nel progetto tramite il suo nome
                    outlineMat = new Material(Shader.Find("Custom/OutlineURP"));
                    // Passa colore e spessore allo shader
                    // I nomi _OutlineColor e _OutlineWidth devono corrispondere
                    // esattamente alle Properties nel file .shader
                    outlineMat.SetColor("_OutlineColor", outlineColor);
                    outlineMat.SetFloat("_OutlineWidth", outlineWidth);
                }
                // Aggiunge l'outline in fondo alla lista — sopra il materiale originale 
                /*
                 ggiunge questo materiale dell'outline in fondo alla lista dei materiali dell'oggetto. 
                Essendo l'ultimo, Unity lo disegnerà "sopra" a quello normale, creando il contorno luminoso. Infine, applica la nuova lista al modello 3D (r.materials = mats.ToArray()).
                 */
                mats.Add(outlineMat);
            }
            else
            {
                // Controlla che ci sia almeno 2 materiali prima di rimuovere
                // (sicurezza: non cancellare il materiale originale per errore)
                if (mats.Count > 1)
                    // Rimuove l'ultimo elemento = sempre l'outline
                    // perché lo abbiamo aggiunto per ultimo con mats.Add
                    mats.RemoveAt(mats.Count - 1);
            }
            r.materials = mats.ToArray();//sostituzione con il nuovo array con i nuovi materiali
        }
    }
}

// Enum = lista di valori predefiniti, tipo un menù a tendina nell'Inspector
// Usare un enum evita errori di battitura rispetto alle stringhe ("Plastica" ecc.)
public enum RIFIUTIType { Plastica, Metallo, Carta, Vetro,Organico }

/*
 
 Abbiamo creato una lista di materiali modificabili perché gli array originali di Unity hanno una dimensione fissa e 
non permettono di aggiungere o rimuovere elementi al volo. Inoltre, creando una copia locale e riapplicandola tramite r.materials, 
siamo sicuri di accendere il bordo luminoso solo ed esclusivamente sul rifiuto specifico vicino al giocatore, 
senza rischiare di far illuminare per errore tutti gli altri rifiuti della mappa che condividono lo stesso materiale."
 
  di base sto codice dei rifiuti si occupa solo di raggruppare materiali in un array e attiv are il contorno 
 */