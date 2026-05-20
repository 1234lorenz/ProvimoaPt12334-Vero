using System.Collections;
using UnityEngine;
using TMPro;

public class CestinoCapacity : MonoBehaviour
{
    [Header("Impostazioni")]
    public int maxCapacity = 30;        // limite massimo dall'Inspector
    public int itemsPerSack = 10;       // ogni quanti oggetti appare un sacco

    [Header("Riferimenti")]
    public GameObject sackPrefab;       // prefab del sacco
    public Transform spawnPoint;        // punto dove spawna il primo sacco
    public Transform fullCollider;      // collider in cima al cestino
    public TextMeshProUGUI fullMessage; // messaggio "Cestino pieno!"
    public ParticleSystem smaltimentoFX; // effetto particelle smaltimento

    [Header("Input")]
    public KeyCode smaltimentoKey = KeyCode.F;

    private int currentCount = 0;
    private bool isFull = false;
    private System.Collections.Generic.List<GameObject> sacks = new System.Collections.Generic.List<GameObject>();

    public bool AddItem()
    {
        if (isFull) return false;

        currentCount++;

        // ogni itemsPerSack oggetti spawna un sacco
        if (currentCount % itemsPerSack == 0)//ogni multiplo di 10 spunta un sacco 
            SpawnSack();

        // cestino pieno?
        if (currentCount >= maxCapacity)
        {
            isFull = true;
            if (fullMessage != null)
            {
                fullMessage.gameObject.SetActive(true);
                fullMessage.text = $"Cestino {GetComponent<Cestino>().acceptedType} pieno!\nPremi F per smaltire";
            }
        }

        return true;
    }

    private void SpawnSack()
    {
        if (sackPrefab == null || spawnPoint == null) return;

        // calcola posizione — ogni sacco è più in alto del precedente
        Vector3 pos = spawnPoint.position + Vector3.up * (sacks.Count * 0.3f);//poszione del sacco = posizionde di partenza + direzione Vertical * altezza totale
        GameObject sack = Instantiate(sackPrefab, pos, Quaternion.Euler(0,Random.Range(0,360),0), transform);
        sacks.Add(sack);
    }

    void Update()
    {
        // smaltimento
        if (Input.GetKeyDown(smaltimentoKey) && isFull)
            StartCoroutine(Smaltimento());
    }

    private IEnumerator Smaltimento()
    {
        // effetto particelle
        if (smaltimentoFX != null)
            smaltimentoFX.Play();

        yield return new WaitForSeconds(0.5f);
        MissionManager.Instance.OnCestinoSmaltito();
        // distruggi tutti i sacchi
        foreach (var sack in sacks)
            Destroy(sack);
        sacks.Clear();

        // reset
        currentCount = 0;//l contatore dei rifiuti interni torna a zero.
        isFull = false;//Questa è la chiave! Sblocca la barriera iniziale dell' AddItem.
                       //Ora il cestino è ufficialmente vuoto e l'interruttore torna a dare il "via libera" per accettare nuovi rifiuti.

        if (fullMessage != null)
            fullMessage.gameObject.SetActive(false);
       
    }

    public bool IsFull()
    {
        return isFull;
    }
}