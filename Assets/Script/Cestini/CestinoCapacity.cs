using System.Collections;
using UnityEngine;
using TMPro;

public class CestinoCapacity : MonoBehaviour
{
    [Header("Impostazioni")]
    public int maxCapacity;
    public int itemsPerSack;
    public float distanzaInterazione = 10f; // Alzata per bilanciare la grandezza del cestino

    [Header("Riferimenti")]
    public GameObject sackPrefab;
    public Transform spawnPoint;
    public Transform fullCollider;
    public TextMeshProUGUI fullMessage;
    public ParticleSystem smaltimentoFX;

    [Header("Input")]
    public KeyCode smaltimentoKey = KeyCode.F;

    public int currentCount = 0;
    private bool isFull = false;
    private bool isSvuotando = false; // Impedisce alla UI di glitchare durante lo smaltimento
    private System.Collections.Generic.List<GameObject> sacks = new System.Collections.Generic.List<GameObject>();
    private Transform player;
    private Cestino cestino;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        cestino = GetComponent<Cestino>();
    }

    public bool AddItem()
    {
        if (isFull) return false;

        currentCount++;

        if (currentCount % itemsPerSack == 0)
            SpawnSack();

        if (currentCount >= maxCapacity)
            isFull = true;

        return true;
    }

    private void SpawnSack()
    {
        if (sackPrefab == null || spawnPoint == null) return;

        Vector3 pos = spawnPoint.position + Vector3.up * (sacks.Count * 0.3f);
        GameObject sack = Instantiate(sackPrefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
        sacks.Add(sack);
    }

    void Update()
    {
        if (player == null) return;

        // Se il cestino NON è pieno, o se stiamo già facendo l'animazione di svuotamento,
        // fermiamo lo script per questo cestino. NON spegniamo la UI qui!
        if (!isFull || isSvuotando) return;

        // Arrivati a questo punto, sappiamo che QUESTO cestino è pieno.
        // Calcoliamo la distanza dal giocatore:
        float distanza = Vector3.Distance(transform.position, player.position);

        if (distanza <= distanzaInterazione)
        {
            // Il giocatore è vicino: Accendiamo la scritta
            if (fullMessage != null)
            {
                fullMessage.gameObject.SetActive(true);
                string tipo = (cestino != null) ? cestino.acceptedType.ToString() : "";
                fullMessage.text = $"Cestino {tipo} pieno!\nPremi {smaltimentoKey} per smaltire";
            }

            // Controlliamo il tasto F (ora è istantaneo!)
            if (Input.GetKeyDown(smaltimentoKey))
            {
                StartCoroutine(Smaltimento());
            }
        }
        else
        {
            // Il giocatore si è allontanato da questo cestino pieno. Spegniamo la scritta,
            // ma solo se stava mostrando il messaggio di QUESTO tipo di cestino.
            if (fullMessage != null && fullMessage.gameObject.activeSelf)
            {
                string tipo = (cestino != null) ? cestino.acceptedType.ToString() : "";
                if (fullMessage.text.Contains(tipo))
                {
                    fullMessage.gameObject.SetActive(false);
                }
            }
        }
    }

    private IEnumerator Smaltimento()
    {
        isSvuotando = true; // Blocca l'Update ed evita che la scritta si riaccenda per errore

        if (smaltimentoFX != null)
            smaltimentoFX.Play();

        // Spegne definitivamente la scritta appena premiamo F
        if (fullMessage != null)
            fullMessage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        MissionManager.Instance.OnCestinoSmaltito();

        foreach (var sack in sacks)
            Destroy(sack);

        sacks.Clear();
        currentCount = 0;
        isFull = false;

        isSvuotando = false; // Il cestino è di nuovo pronto a ricevere rifiuti
    }

    public bool IsFull()
    {
        return isFull;
    }
}