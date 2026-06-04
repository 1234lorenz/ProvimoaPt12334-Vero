using UnityEngine;

public class RIFIUTI : MonoBehaviour
{
    public RIFIUTIType type;

    [Header("ID Univoco")]
    public string rifiutoID; // imposta dall'Inspector su ogni rifiuto

    [Header("Quantità")]
    public int quantita = 1;

    [Header("Outline")]
    public Color outlineColor = Color.cyan;
    [Range(0f, 10f)] public float outlineWidth = 5f;

    private Outline outline;

    void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = outlineColor;
        outline.OutlineWidth = outlineWidth;
        outline.enabled = false;
    }

    public void SetOutline(bool active)
    {
        if (outline != null)
            outline.enabled = active;
    }
}

public enum RIFIUTIType { Plastica, Metallo, Carta, Vetro, Organico,Secco }