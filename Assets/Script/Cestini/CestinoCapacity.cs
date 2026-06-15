using System.Collections;
using UnityEngine;
using TMPro;

public class CestinoCapacity : MonoBehaviour
{
    [Header("Impostazioni")]
    public int maxCapacity;
    public int itemsPerSack;
    public float distanzaInterazione = 4f;

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
    private bool isSvuotando = false; // NUOVO: Impedisce alla UI di glitchare durante lo smaltimento
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

        // Se il cestino non è pieno, o se stiamo già facendo l'animazione di svuotamento,
        // spegniamo la scritta (se attiva) e usciamo subito.
        if (!isFull || isSvuotando)
        {
            if (fullMessage != null && fullMessage.gameObject.activeSelf)
                fullMessage.gameObject.SetActive(false);
            return;
        }

        // Controlliamo la distanza ogni frame per rendere l'interazione fluidissima
        float distanza = Vector3.Distance(transform.position, player.position);

        if (distanza <= distanzaInterazione)
        {
            // Mostra la scritta solo se non è già visibile
            if (fullMessage != null && !fullMessage.gameObject.activeSelf)
            {
                fullMessage.gameObject.SetActive(true);
                string tipo = (cestino != null) ? cestino.acceptedType.ToString() : "";
                fullMessage.text = $"Cestino {tipo} pieno!\nPremi {smaltimentoKey} per smaltire";
            }

            // ORA il tasto F risponde istantaneamente ad ogni singola pressione!
            if (Input.GetKeyDown(smaltimentoKey))
            {
                StartCoroutine(Smaltimento());
            }
        }
        else
        {
            // Se il giocatore si allontana, spegne la scritta
            if (fullMessage != null && fullMessage.gameObject.activeSelf)
                fullMessage.gameObject.SetActive(false);
        }
    }

    private IEnumerator Smaltimento()
    {
        isSvuotando = true; // Blocca l'Update ed evita che la scritta si riaccenda

        if (smaltimentoFX != null)
            smaltimentoFX.Play();

        if (fullMessage != null)
            fullMessage.gameObject.SetActive(false); // Spegne definitivamente la scritta

        yield return new WaitForSeconds(0.5f);

        MissionManager.Instance.OnCestinoSmaltito();

        foreach (var sack in sacks)
            Destroy(sack);

        sacks.Clear();
        currentCount = 0;
        isFull = false;

        isSvuotando = false; // Sblocco finale: il cestino torna a funzionare normalmente
    }

    public bool IsFull()
    {
        return isFull;
    }
}