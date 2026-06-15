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

    public void SetOutline(bool active)
    {
        // crea l'outline solo la prima volta che serve davvero
        if (active && outline == null)
        {
            // gestione particelle: spegniamo temporaneamente per non farle "registrare" nell'outline
            ParticleSystem[] particelle = GetComponentsInChildren<ParticleSystem>();
            List<GameObject> disattivati = new List<GameObject>();

            foreach (ParticleSystem ps in particelle)
            {
                if (ps.gameObject.activeSelf)
                {
                    ps.gameObject.SetActive(false);
                    disattivati.Add(ps.gameObject);
                }
            }

            outline = gameObject.AddComponent<Outline>();
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineWidth;

            foreach (GameObject go in disattivati)
                go.SetActive(true);
        }

        if (outline != null && outline.enabled != active)
            outline.enabled = active;
    }
}

public enum RIFIUTIType { Plastica, Metallo, Carta, Vetro, Organico, Secco }