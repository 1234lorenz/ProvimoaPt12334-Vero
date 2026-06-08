using System.Collections;
using UnityEngine;
using TMPro;

public class CestinoCapacity : MonoBehaviour
{
    [Header("Impostazioni")]
    public int maxCapacity;
    public int itemsPerSack;
    public float distanzaInterazione = 4f; //Distanza massima per poter svuotare il cestino

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
    private System.Collections.Generic.List<GameObject> sacks = new System.Collections.Generic.List<GameObject>();
    private Transform player; // NUOVO: Riferimento al giocatore

    void Start()
    {
        // Trova il giocatore all'avvio
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    public bool AddItem()
    {
        if (isFull) return false;

        currentCount++;

        // ogni itemsPerSack oggetti spawna un sacco
        if (currentCount % itemsPerSack == 0)
            SpawnSack();

        // cestino pieno?
        if (currentCount >= maxCapacity)
        {
            isFull = true;
            //  NON accendiamo più la scritta qui, ci pensa l'Update se sei vicino!
        }

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
        // NUOVO: Controllo distanza per la UI e l'Input
        if (isFull && player != null)
        {
            // Calcola la distanza tra il cestino e il Player
            float distanza = Vector3.Distance(transform.position, player.position);

            if (distanza <= distanzaInterazione)
            {
                // Mostra la scritta solo se sei vicino
                if (fullMessage != null && !fullMessage.gameObject.activeSelf)
                {
                    fullMessage.gameObject.SetActive(true);
                    // Prova a recuperare il tipo di cestino (se hai uno script Cestino attaccato)
                    Cestino cestino = GetComponent<Cestino>();
                    string tipo = (cestino != null) ? cestino.acceptedType.ToString() : "";
                    fullMessage.text = $"Cestino {tipo} pieno!\nPremi {smaltimentoKey} per smaltire";
                }

                // Permetti lo smaltimento solo se sei vicino
                if (Input.GetKeyDown(smaltimentoKey))
                    StartCoroutine(Smaltimento());
            }
            else
            {
                // Se ti allontani troppo, nascondi la scritta
                if (fullMessage != null && fullMessage.gameObject.activeSelf)
                {
                    fullMessage.gameObject.SetActive(false);
                }
            }
        }
    }

    private IEnumerator Smaltimento()
    {
        if (smaltimentoFX != null)
            smaltimentoFX.Play();

        // Spegni subito la scritta appena premi il tasto
        if (fullMessage != null)
            fullMessage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        MissionManager.Instance.OnCestinoSmaltito();

        foreach (var sack in sacks)
            Destroy(sack);
        sacks.Clear();

        currentCount = 0;
        isFull = false;
    }

    public bool IsFull()
    {
        return isFull;
    }
}