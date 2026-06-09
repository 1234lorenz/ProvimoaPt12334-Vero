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
    public AudioClip[] passiPonte; // AGGIUNTO: Ora questa casella apparirà nell'Inspector!

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = effettiGroup;
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
        // L'erba rimane il suono di riserva (default)
        AudioClip[] listaSelezionata = passiErba;

        switch (terreno)
        {
            case SuperficieType.Sabbia: listaSelezionata = passiSabbia; break;
            case SuperficieType.Roccia: listaSelezionata = passiRoccia; break;
            case SuperficieType.Acqua: listaSelezionata = passiAcqua; break;
            case SuperficieType.Ponte: listaSelezionata = passiPonte; break; // AGGIUNTO: Ora il codice sa di dover usare i suoni del ponte!
        }

        if (listaSelezionata.Length == 0) return;

        int index = Random.Range(0, listaSelezionata.Length);

        // Variazione di Pitch e Volume per rendere realistici i passi
        sfxSource.pitch = Random.Range(0.85f, 1.15f);
        float volumeCasuale = Random.Range(0.35f, 0.45f);

        sfxSource.PlayOneShot(listaSelezionata[index], volumeCasuale);
    }
}