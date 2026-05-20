using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collector : MonoBehaviour
{
    // 
    // VARIABILI
    // 
    [Header("Cestino")]
    public LayerMask cestinoLayer; // crea un layer "Cestino" in Unity

    [Header("Cestino Prompt")]
    [SerializeField] private GameObject cestinoPromptUI;//e comando del cestino(Q)
    [SerializeField] private TextMeshProUGUI cestinoPromptText;//e il testo

    [Header("Settings")]
    
    public float detectionRadius = 3f;// raggio della sfera invisibile intorno al player

    public LayerMask collectableLayer; // filtro per l'OverlapSphere cerca solo oggetti sul layer "Collectable"

    [Header("UI Prompt")]
    
    [SerializeField] private GameObject collectPromptUI;// pannello UI "[ E ] Raccogli" visibile nell'Inspector ma privato
    
    [SerializeField] private TextMeshProUGUI promptText;// testo dentro il pannello UI

    [SerializeField] private GameObject panelCestinoPrompt;//il apanello del cestino(Q)

    
    private CounterMenagerRifiuti counterRifiuti;//riferimento al contatore trovato con GetComponent in Start() perché è sullo stesso GameObject

   
    [SerializeField] private PlayerInputHandler inputHandler; // riferimento all'input handler  è su un GameObject separato quindi lo assegniamo dall'Inspector

   
    private List<RIFIUTI> itemsInRange = new List<RIFIUTI>(); // La memoria del frame precedente.


    private RIFIUTI closestItem;// il rifiuto più vicino al player  quello che verrà raccolto premendo E

    private CestinoUI cestinoUI;
    private Cestino nearestCestino;//cestino piu vicino 

    //
    // START
    //

    private void Start()
    {
        // trova il contatore sullo stesso GameObject
        counterRifiuti = GetComponent<CounterMenagerRifiuti>();
        // nasconde il prompt UI all'avvio  non deve apparire prima di avvicinarsi a qualcosa
        collectPromptUI.SetActive(false);

        cestinoUI = FindObjectOfType<CestinoUI>();//cerca nella scena un qualsiasi oggetto ceh abbia lo script CestinoUI
        cestinoPromptUI.SetActive(false);
        panelCestinoPrompt.SetActive(false);
    }

   

    private void Update()
    {
        // ogni frame aggiorna il radar  chi è nel raggio, outline, prompt UI

        DetectNearby();//gestisce l'interazione tra il giocatore e i rifiuti 
        DetectNearbyCestino();//gestisce l'interazione tra il giocatore e i cestini 

        // se il giocatore preme E E c'è un rifiuto vicino  raccogli
        // closestItem != null è una sicurezza — senza questo andrebbe in errore premendo E nel vuoto
        if (inputHandler.CollectTrigger && closestItem != null)
            Collect(closestItem);
    }

    // 
    // DETECTNEARBY  il radar
    // 

    private void DetectNearbyCestino()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, cestinoLayer);//qui stiamo parlando di un array di collider 
        //qui unity usera il collider[] per meeterci dentro tutti i corpi solidi (I Collider) che la bolla insivile risucira a toccare in quel millisecondo 
        //hits = e il nome della variabile e chidice "Qui dentro ci metto la lista di cose che ho colpito con il radar".

        if (hits.Length > 0)//qui dicmoa ceh se la lunghezza dell'arrayi e maggiore di 0 quidn se dentro l'arrayc e qualcosa 
        {
            Cestino cestino = hits[0].GetComponent<Cestino>();//qui diciamo che quel colldier ha effettivamnte quel codice ovveoro il codice cesti o 
            if (cestino != null && cestino != nearestCestino)//Questa riga è un filtro antirripetizione.
            {
                nearestCestino = cestino;
                cestinoUI.SetNearCestino(cestino);
            }

            // mostra prompt
            panelCestinoPrompt.SetActive(true);
            cestinoPromptUI.SetActive(true);
            cestinoPromptText.text = $"      [Q]\nButta rifiuti";
        }
        else
        {
            if (nearestCestino != null)
            {
                nearestCestino = null;
                cestinoUI.ClearNearCestino();
            }

            // nascondi prompt
            panelCestinoPrompt.SetActive(false);
            cestinoPromptUI.SetActive(false);
        }
    }
    private void DetectNearby()
    {
        //A decidere quando accenderlo o spegnerlo riguarod la setOutline che sta nel codice rifiuti 


        // crea una sfera invisibile centrata sul player e raccoglie tutti i Collider
        // con layer "Collectable" che ci sono dentro
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, collectableLayer);

        // lista vuota per i rifiuti trovati in questo frame  riparte da zero ogni frame
        List<RIFIUTI> newInRange = new List<RIFIUTI>();

        // scorre tutti i Collider trovati dalla sfera
        foreach (var hit in hits)
        {
            // cerca lo script RIFIUTI.cs sull'oggetto di quel Collider
            var item = hit.GetComponent<RIFIUTI>();
            // se ha lo script RIFIUTI  aggiungilo alla lista, altrimenti ignoralo
            if (item != null) newInRange.Add(item);
        }

        // confronta la lista vecchia con quella nuova
        // chi era nel raggio prima ma non c'è più adesso  spegni il suo outline
        foreach (var old in itemsInRange)
            if (old != null && !newInRange.Contains(old))
                old.SetOutline(false);

        // sostituisce la lista vecchia con quella nuova  pronta per il frame successivo
        itemsInRange = newInRange;

        // azzera il più vicino  si ricalcola da zero ogni frame
        closestItem = null;
        // parte dal numero più grande possibile  così il primo oggetto trovato vince sempre il primo confronto
        float minDist = float.MaxValue;

        foreach (var item in itemsInRange)
        {
            // accende il bordino su tutti i rifiuti nel raggio
            item.SetOutline(true);
            // calcola la distanza tra il player e questo rifiuto
            float dist = Vector3.Distance(transform.position, item.transform.position);
            // se questo rifiuto è più vicino del record attuale  diventa il nuovo più vicino
            if (dist < minDist) { minDist = dist; closestItem = item; }
        }

        // aggiorna il prompt UI
        if (closestItem != null)
        {
            // c'è un rifiuto vicino  mostra il prompt con il tipo di rifiuto
            collectPromptUI.SetActive(true);
            promptText.text = $" [E] Raccogli {closestItem.type}";
        }
        else
        {
            // nessun rifiuto vicino  nascondi il prompt
            collectPromptUI.SetActive(false);
        }
    }

    // 
    // COLLECT  raccoglie il rifiuto
    //

    private void Collect(RIFIUTI item)
    {
        
        // se inventario pieno non raccogliere
        bool added = counterRifiuti.AddTrash(item.type);
        if (!added) return;

        item.SetOutline(false);
        itemsInRange.Remove(item);
        closestItem = null;
        Destroy(item.gameObject);
        MissionManager.Instance.OnRaccolta();//aggiorno delle missioni 
    }

    // disegna il raggio della sfera nell'editor per debug visivo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

/*
 
 LOGICA GENERALE DI COLLECTOR

 
 Collector è il cervello della raccolta differenziata.
 Ogni frame fa tre cose:

 1) RADAR (DetectNearby)
    Crea una sfera invisibile intorno al player.
    Raccoglie tutti i rifiuti dentro la sfera in una lista.
    Confronta la lista di questo frame con quella del frame prima:
    - chi è entrato nel raggio:  accendi outline
    - chi è uscito dal raggio: spegni outline
    Tra tutti quelli nel raggio trova il più vicino (closestItem)
    e mostra il prompt UI con il tipo di rifiuto.

 2) INPUT (Update)
    Se il giocatore preme E e c'è un rifiuto vicino  raccogli.

 3) RACCOLTA (Collect)
    Spegne outline  aggiorna contatore  rimuove dalla lista  distrugge oggetto.







Inizio gioco:
 itemsInRange = []   dichiarata vuota in cima

Frame 1:
 newInRange   = [ bottiglia, lattina ]   costruita da OverlapSphere
 itemsInRange = []      ancora vuota
 confronto    =  niente da spegnere
 itemsInRange = newInRange     ORA diventa [ bottiglia, lattina ]

Frame 2:
 newInRange   = [ bottiglia ]   lattina è uscita
 itemsInRange = [ bottiglia, lattina ]   memoria del frame prima!
 confronto  = lattina non c'è più  SetOutline(false)
 itemsInRange = newInRange               ORA diventa [ bottiglia ]
*/