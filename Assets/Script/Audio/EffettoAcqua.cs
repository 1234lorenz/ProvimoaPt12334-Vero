using UnityEngine;

public class EffettoAcqua : MonoBehaviour
{
    [Header("Impostazioni Camminata")]
    [SerializeField] private float lentezzaInAcqua = 0.4f;

    [Header("Nebbia di Sfondo (Subacquea)")]
    [SerializeField] private Color coloreNebbiaSubacquea = new Color(0f, 0.2f, 0.4f); // Blu scuro per l'orizzonte
    [SerializeField] private float distanzaFineNebbia = 35f; // Distanza di visibilità in acqua

    private bool nebbiaIniziale;
    private Color coloreNebbiaIniziale;
    private float fineNebbiaIniziale;
    private FogMode modalitaNebbiaIniziale;

    void Start()
    {
        // Forziamo la nebbia a SPENTA all'avvio del gioco (così la mappa è limpida)
        RenderSettings.fog = false;
        nebbiaIniziale = false;

        // Salviamo i valori di default
        coloreNebbiaIniziale = RenderSettings.fogColor;
        fineNebbiaIniziale = RenderSettings.fogEndDistance;
        modalitaNebbiaIniziale = RenderSettings.fogMode;

        // Spegniamo l'effetto audio ovattato all'avvio
        ImpostaFiltroAudio(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlliamo se chi entra è il Player o un suo oggetto figlio
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            FirstPersonController player = other.GetComponentInParent<FirstPersonController>();
            if (player != null)
            {
                player.moltiplicatoreVelocita = lentezzaInAcqua;

                // Attiva la nebbia subacquea
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogColor = coloreNebbiaSubacquea;
                RenderSettings.fogStartDistance = 0f;
                RenderSettings.fogEndDistance = distanzaFineNebbia;

                // Attiva audio ovattato
                ImpostaFiltroAudio(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            FirstPersonController player = other.GetComponentInParent<FirstPersonController>();
            if (player != null)
            {
                player.moltiplicatoreVelocita = 1f;

                // Ripristina le impostazioni iniziali del mondo
                RenderSettings.fog = nebbiaIniziale;
                RenderSettings.fogColor = coloreNebbiaIniziale;
                RenderSettings.fogEndDistance = fineNebbiaIniziale;
                RenderSettings.fogMode = modalitaNebbiaIniziale;

                // Disattiva audio ovattato
                ImpostaFiltroAudio(false);
            }
        }
    }

    private void ImpostaFiltroAudio(bool attivo)
    {
        if (Camera.main != null)
        {
            AudioLowPassFilter filtro = Camera.main.GetComponent<AudioLowPassFilter>();
            if (filtro != null)
            {
                filtro.enabled = attivo;
            }
        }
    }
}