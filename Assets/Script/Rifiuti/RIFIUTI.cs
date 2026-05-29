using UnityEngine;

public class RIFIUTI : MonoBehaviour
{
    public RIFIUTIType type;

    [Header("Outline")]
    public Color outlineColor = Color.cyan;
    [Range(0f, 10f)] public float outlineWidth = 5f;

    private Outline outline;

    void Awake()
    {
        // aggiunge il componente Outline automaticamente
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false; // disattivato all'inizio
    }

    public void SetOutline(bool active)
    {
        if (outline != null)
            outline.enabled = active;
    }
}

public enum RIFIUTIType { Plastica, Metallo, Carta, Vetro, Organico }