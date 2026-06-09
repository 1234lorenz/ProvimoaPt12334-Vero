using UnityEngine;

public enum TipoAmbiente { Foresta, Spiaggia }

public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance;

    [Header("Canali Audio Loop")]
    public AudioSource forestaSource;
    public AudioSource spiaggiaSource;

    [Header("Impostazioni Sfumatura")]
    public float velocitaSfumatura = 0.8f; // Più è basso, più la sfumatura è lenta e dolce
    public float volumeMassimoAmbiente = 0.4f; // Per non coprire i passi

    private float targetVolForesta;
    private float targetVolSpiaggia;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Di base il gioco parte nella foresta
        forestaSource.loop = true;
        spiaggiaSource.loop = true;

        forestaSource.Play();
        spiaggiaSource.Play();

        targetVolForesta = volumeMassimoAmbiente;
        targetVolSpiaggia = 0f;

        forestaSource.volume = targetVolForesta;
        spiaggiaSource.volume = targetVolSpiaggia;
    }

    void Update()
    {
        // Sfuma i volumi in tempo reale verso il loro obiettivo (Target)
        forestaSource.volume = Mathf.MoveTowards(forestaSource.volume, targetVolForesta, velocitaSfumatura * Time.deltaTime);
        spiaggiaSource.volume = Mathf.MoveTowards(spiaggiaSource.volume, targetVolSpiaggia, velocitaSfumatura * Time.deltaTime);
    }

    public void CambiaAmbiente(TipoAmbiente nuovoAmbiente)
    {
        if (nuovoAmbiente == TipoAmbiente.Foresta)
        {
            targetVolForesta = volumeMassimoAmbiente;
            targetVolSpiaggia = 0f;
            Debug.Log("[AMBIENTE] Sfumo verso la Foresta...");
        }
        else if (nuovoAmbiente == TipoAmbiente.Spiaggia)
        {
            targetVolForesta = 0f;
            targetVolSpiaggia = volumeMassimoAmbiente;
            Debug.Log("[AMBIENTE] Sfumo verso il Mare...");
        }
    }
}