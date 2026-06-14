using UnityEngine;
using UnityEngine.Audio;

public enum SuperficieType { Erba, Sabbia, Roccia, Acqua, Ponte }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Configurazione Mixer")]
    public AudioMixerGroup effettiGroup;

    [System.Serializable]
    public class TrashAudio
    {
        public RIFIUTIType type;
        public AudioClip clip;
    }

    [Header("Suoni Raccolta Rifiuti")]
    public TrashAudio[] pickupSounds;

    [Header("Suoni Passi Divisi per Terreno")]
    public AudioClip[] passiErba;
    public AudioClip[] passiSabbia;
    public AudioClip[] passiRoccia;
    public AudioClip[] passiAcqua;
    public AudioClip[] passiPonte;

    [Header("Suoni Cancello")]
    [Tooltip("Trascina qui il suono del cancello che si apre")]
    public AudioClip suonoAperturaCancello;
    [Tooltip("Trascina qui il suono del cancello che si chiude")]
    public AudioClip suonoChiusuraCancello;

    // Due sorgenti separate per non accavallare o distorcere i pitch
    private AudioSource sfxSource;
    private AudioSource passiSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Sorgente dedicata a rifiuti, interfacce e oggetti interattivi (cancello)
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = effettiGroup;

        // Sorgente dedicata esclusivamente ai passi
        passiSource = gameObject.AddComponent<AudioSource>();
        passiSource.outputAudioMixerGroup = effettiGroup;
    }

    public void PlayPickupSound(RIFIUTIType type)
    {
        sfxSource.pitch = 1.0f;
        foreach (var trashAudio in pickupSounds)
        {
            if (trashAudio.type == type && trashAudio.clip != null)
            {
                sfxSource.PlayOneShot(trashAudio.clip);
                return;
            }
        }
    }

    public void PlayFootstep(SuperficieType terreno)
    {
        AudioClip[] listaSelezionata = passiErba;

        switch (terreno)
        {
            case SuperficieType.Sabbia: listaSelezionata = passiSabbia; break;
            case SuperficieType.Roccia: listaSelezionata = passiRoccia; break;
            case SuperficieType.Acqua: listaSelezionata = passiAcqua; break;
            case SuperficieType.Ponte: listaSelezionata = passiPonte; break;
        }

        if (listaSelezionata.Length == 0) return;

        int index = Random.Range(0, listaSelezionata.Length);

        // Modifica il pitch solo sul canale dei passi
        passiSource.pitch = Random.Range(0.85f, 1.15f);
        float volumeCasuale = Random.Range(0.35f, 0.45f);

        passiSource.PlayOneShot(listaSelezionata[index], volumeCasuale);
    }

    public void PlayCancelloApri()
    {
        if (suonoAperturaCancello != null && sfxSource != null)
        {
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(suonoAperturaCancello);
        }
    }

    public void PlayCancelloChiudi()
    {
        if (suonoChiusuraCancello != null && sfxSource != null)
        {
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(suonoChiusuraCancello);
        }
    }
}