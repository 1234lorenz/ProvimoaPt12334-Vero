using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OpzioniManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Slider Volume")]
    public Slider sliderMusica;
    public Slider sliderEffetti;

    [Header("Slider Sensibilità")]
    public Slider sliderSensibilita;

    [Header("Testi valori")]
    public TextMeshProUGUI testoMusica;
    public TextMeshProUGUI testoEffetti;
    public TextMeshProUGUI testoSensibilita;

    void Start()
    {
        // carica valori salvati o usa default
        sliderMusica.value = PlayerPrefs.GetFloat("VolMusica", 0.75f);
        sliderEffetti.value = PlayerPrefs.GetFloat("VolEffetti", 0.75f);
        sliderSensibilita.value = PlayerPrefs.GetFloat("Sensibilita", 0.1f);

        // applica subito i valori
        SetVolMusica(sliderMusica.value);
        SetVolEffetti(sliderEffetti.value);
        SetSensibilita(sliderSensibilita.value);
    }

    public void SetVolMusica(float value)
    {
        // converti da 0-1 a decibel (-80 a 0)
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        audioMixer.SetFloat("VolMusica", dB);
        PlayerPrefs.SetFloat("VolMusica", value);
        if (testoMusica != null)
            testoMusica.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void SetVolEffetti(float value)
    {
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        audioMixer.SetFloat("VolEffetti", dB);
        PlayerPrefs.SetFloat("VolEffetti", value);
        if (testoEffetti != null)
            testoEffetti.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void SetSensibilita(float value)
    {
        PlayerPrefs.SetFloat("Sensibilita", value);
        if (testoSensibilita != null)
            testoSensibilita.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}