using UnityEngine;
using System.Collections.Generic;

public class RIFIUTI : MonoBehaviour
{
    public RIFIUTIType type;

    [Header("ID Univoco")]
    public string rifiutoID;

    [Header("Quantità")]
    public int quantita = 1;

    [Header("Outline")]
    public Color outlineColor = Color.cyan;
    [Range(0f, 10f)] public float outlineWidth = 5f;

    private Outline outline;

    void Awake()
    {
        // 1. Troviamo le particelle
        ParticleSystem[] particelle = GetComponentsInChildren<ParticleSystem>();
        List<GameObject> disattivatiInAwake = new List<GameObject>();

        // 2. Spegniamo le particelle SOLO all'avvio (Awake viene letto 1 sola volta)
        foreach (ParticleSystem ps in particelle)
        {
            if (ps.gameObject.activeSelf)
            {
                ps.gameObject.SetActive(false);
                disattivatiInAwake.Add(ps.gameObject);
            }
        }

        // 3. Creiamo l'outline sulla lisca, che ora "registrerà" solo i modelli 3D e non il fumo
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;

        // 4. Riaccendiamo le particelle per sempre
        foreach (GameObject go in disattivatiInAwake)
        {
            go.SetActive(true);
        }
    }

    public void SetOutline(bool active)
    {
        // Se l'outline esiste e sta cambiando stato rispetto a prima
        if (outline != null && outline.enabled != active)
        {
            // Lo accendiamo o spegniamo normalmente senza più toccare le particelle!
            outline.enabled = active;
        }
    }
}

public enum RIFIUTIType { Plastica, Metallo, Carta, Vetro, Organico, Secco }