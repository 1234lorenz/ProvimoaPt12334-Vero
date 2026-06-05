using UnityEngine;

public class EffettoAcqua : MonoBehaviour
{
    [Header("Impostazioni Camminata")]
    [SerializeField] private float lentezzaInAcqua = 0.4f;

    [Header("Nebbia di Sfondo (Subacquea)")]
    [SerializeField] private Color coloreNebbiaSubacquea = new Color(0f, 0.2f, 0.4f); // Blu scuro per l'orizzonte
    [SerializeField] private float distanzaFineNebbia; // Più è bassa, meno  si vede la mappa in lontananza

    private bool nebbiaIniziale;
    private Color coloreNebbiaIniziale;
    private float fineNebbiaIniziale;
    private FogMode modalitaNebbiaIniziale;

    void Start()
    {
        // Salviamo la nebbia originale del mondoo

        nebbiaIniziale = RenderSettings.fog;
        coloreNebbiaIniziale = RenderSettings.fogColor;
        fineNebbiaIniziale = RenderSettings.fogEndDistance;
        modalitaNebbiaIniziale = RenderSettings.fogMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                player.moltiplicatoreVelocita = lentezzaInAcqua;

                // Attiva la nebbia per nascondere la mappa 
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogColor = coloreNebbiaSubacquea;
                RenderSettings.fogStartDistance = 0f;
                RenderSettings.fogEndDistance = distanzaFineNebbia;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FirstPersonController player = other.GetComponent<FirstPersonController>();
            if (player != null)
            {
                player.moltiplicatoreVelocita = 1f;

                // Ripristina il cielo e la visuale normale della mappa
                RenderSettings.fog = nebbiaIniziale;
                RenderSettings.fogColor = coloreNebbiaIniziale;
                RenderSettings.fogEndDistance = fineNebbiaIniziale;
                RenderSettings.fogMode = modalitaNebbiaIniziale;
            }
        }
    }
}