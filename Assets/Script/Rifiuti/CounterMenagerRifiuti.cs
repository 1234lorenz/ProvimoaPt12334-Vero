using UnityEngine;
using TMPro;
using System.Collections;
/*
 questo script fa il lavoro da "contabile": 
tiene il conto di quanti rifiuti hai addosso, stabilisce il limite massimo di spazio nello zaino e fa apparire i messaggi di errore se provi a raccogliere troppa spazzatura.
 
 */
public class CounterMenagerRifiuti : MonoBehaviour
{
    [System.Serializable]//è un vero e proprio traduttore automatico tra il codice puro e l'interfaccia grafica.
    public class TrashSlot
    {
        public RIFIUTIType type;
        public int maxCount;
        public TextMeshProUGUI countText;
        [HideInInspector] public int currentCount;
    }

    [Header("Inventario")]
    public TrashSlot[] slots;

    [Header("UI Messaggio Pieno")]
    public GameObject fullMessagePanel;
    public TextMeshProUGUI fullMessage;  // trascini il testo dall'Inspector
    public float messageDuration = 2f;  // secondi prima che sparisce

    private Coroutine messageCoroutine; // tiene traccia della coroutine attiva

    public bool AddTrash(RIFIUTIType type, int quantity = 1)
    {
        foreach (var slot in slots)
        {
            if (slot.type == type)
            {
                // Controlliamo in anticipo se la somma supera il massimo!
                if (slot.currentCount + quantity > slot.maxCount)
                {
                    // mostra messaggio inventario pieno
                    ShowFullMessage(type);
                    return false;
                }

                // --- SE SEI ARRIVATO QUI, SIGNIFICA CHE C'È SPAZIO! ---
                slot.currentCount += quantity;
                slot.countText.text = $"{slot.currentCount}/{slot.maxCount}"; // aggiornamento grafica

                // NUOVA RIGA: Riproduce il suono specifico per il tipo di rifiuto raccolto!
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPickupSound(type);
                }

                return true;
            }
        }
        return false;
    }

    private void ShowFullMessage(RIFIUTIType type)
    {
        // se c'è già un messaggio attivo fermalo
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        // avvia la coroutine che mostra e nasconde il messaggio
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(type));
    }

    private IEnumerator ShowMessageCoroutine(RIFIUTIType type)
    {
        // attiva il pannello E il testo
        fullMessagePanel.SetActive(true);
        fullMessage.gameObject.SetActive(true);
        fullMessage.text = $"Inventario {type} pieno!";

        yield return new WaitForSeconds(messageDuration);

        // disattiva tutto
        fullMessagePanel.SetActive(false);
        messageCoroutine = null;

    }
    public void AggiornaTuttoUI()
    {
        foreach (var slot in slots)
            slot.countText.text = $"{slot.currentCount}/{slot.maxCount}";
    }

    public void RemoveTrash(RIFIUTIType type, int quantity)//questa funzione serv a svuotare l'inventario
    {
        foreach (var slot in slots)
        {
            if (slot.type == type)
            {
                // non andare sotto zero
                slot.currentCount = Mathf.Max(0, slot.currentCount - quantity);
                slot.countText.text = $"{slot.currentCount}/{slot.maxCount}";
                return;
            }
        }
    }
}

/* 
 * =====================================================================================
 * SYSTEMA DI GESTIONE E CONTEGGIO INVENTARIO RIFIUTI (CounterManagerRifiuti.cs)
 * =====================================================================================
 * 
 * COSA FA QUESTO SCRIPT:
 * Agisce come il "contabile" dello zaino del giocatore. Tiene traccia di quanti rifiuti
 * abbiamo addosso, aggiorna l'interfaccia grafica (UI) e blocca la raccolta se siamo pieni.
 * 
 * STRUTTURA DEL CODICE:
 * 1. CLASSE 'TrashSlot': Funziona come una struct personalizzata. Modella il singolo "cassetto"
 *    dello zaino contenente: Tipo di rifiuto, Limite Massimo, Conteggio Corrente e Testo UI.
 *    Il tag [System.Serializable] la traduce graficamente per poterla configurare nell'Inspector.
 * 
 * 2. GESTIONE TIMER (messageCoroutine): Una variabile d'appoggio che funge da "telecomando"
 *    per controllare il timer del messaggio d'errore ed evitare che si sovrapponga se l'utente
 *    clicca furiosamente.
 * 
 * FUNZIONI PRINCIPALI:
 * 
 * • AddTrash(type): Viene chiamata quando raccogliamo un oggetto. Gira tutti gli slot via 'foreach':
 *   - Se lo slot è PIENO: attiva l'avviso di errore e restituisce 'false' (impedendo di distruggere l'oggetto a terra).
 *   - Se c'è SPAZIO: incrementa il contatore, aggiorna il testo a schermo (es. "4/10") e restituisce 'true'.
 * 
 * • ShowFullMessage(type): Gestisce la sicurezza dell'avviso. Se c'è già un timer attivo a schermo,
 *   lo interrompe bruscamente (StopCoroutine) e ne fa partire uno nuovo, resettando il tempo visivo.
 * 
 * • ShowMessageCoroutine(type): La Coroutine vera e propria. 
 *   - Fase 1: Accende il pannello e scrive il testo dinamico ("Inventario Plastica pieno!").
 *   - Fase 2: Sfrutta 'yield return new WaitForSeconds' per CONGELARE questa funzione per X secondi,
 *             lasciando che il resto del gioco continui a girare normalmente.
 *   - Fase 3: Al risveglio, spegne il pannello UI e svuota la variabile (la rimette a 'null').
 * 
 * • RemoveTrash(type, quantity): Viene chiamata (ad esempio dal cestino) per svuotare lo zaino.
 *   Usa 'Mathf.Max(0, ...)' come cintura di sicurezza matematica: confronta il risultato della sottrazione
 *   con lo zero e tiene il valore più alto. In questo modo impedisce all'inventario di andare in negativo (es. -2)
 *   anche in caso di bug. Infine aggiorna la grafica.
 * =====================================================================================
 */