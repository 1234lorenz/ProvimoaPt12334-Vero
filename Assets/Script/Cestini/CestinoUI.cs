using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
/*
 è il cuore dell'interfaccia di interazione. Serve a gestire la schermata che compare quando apri un bidone della spazzatura,
 
 */

public class CestinoUI : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject cestinoUIPanel;       // pannello principale inventario
    public Transform slotContainer;         // contenitore degli slot
    public GameObject slotPrefab;           // prefab bottone slot
    public GameObject quantityPanel;        // pannello "quanta roba vuoi buttare?"
    public TMP_InputField quantityInput;    // campo dove scrivi il numero
    public TextMeshProUGUI quantityLabel;   // "Quanti Plastica vuoi buttare?"

    private CounterMenagerRifiuti inventory;
    private PlayerInputHandler inputHandler;
    private Cestino currentCestino;
    private RIFIUTIType selectedType;
    private bool isOpen = false;
    private bool wasOpenBinPressed = false;
    private FirstPersonController playerController;

    [Header("Prompt Uscita")]
    public TextMeshProUGUI escPromptText; // trascini dall'Inspector

    [Header("Icone categorie")]
    public Sprite plasticaIcon;
    public Sprite vetroIcon;
    public Sprite metalloIcon;
    public Sprite cartaIcon;
    public Sprite organicoIcon;

    void Start()
    {
        inventory = FindObjectOfType<CounterMenagerRifiuti>();
        inputHandler = FindObjectOfType<PlayerInputHandler>();
        playerController = FindObjectOfType<FirstPersonController>();
        cestinoUIPanel.SetActive(false);
        quantityPanel.SetActive(false);
    }

    void Update()
    {
        bool isPressed = inputHandler.OpenBinTrigger;

        // simula GetKeyDown  reagisce solo al primo frame della pressione
        if (isPressed && !wasOpenBinPressed && currentCestino != null)// con  questo contorllo siamo sicuri che la UI si apra o si chiuda una sola volta per ogni singola pressione del tasto, evitando che la schermata si apra e si chiuda all'infinito."
        {
            if (isOpen) CloseUI();//se prmeo esc si attiva closeUi 
            else OpenUI();
        }

        wasOpenBinPressed = isPressed;

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            CloseUI();
    }

    // chiamato da Collector quando sei vicino a un cestino
    public void SetNearCestino(Cestino cestino)
    {
        currentCestino = cestino;
    }

    // chiamato da Collector quando esci dal raggio del cestino
    public void ClearNearCestino()
    {
        currentCestino = null;
        if (isOpen) CloseUI();
    }

    void OpenUI()
    {
        isOpen = true;
        cestinoUIPanel.SetActive(true);
        quantityPanel.SetActive(false);
        AggiornaSlotsUI();

        // blocca tutto
        playerController.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        escPromptText.gameObject.SetActive(true);
        escPromptText.text = "Premi ESC per uscire";
    }

    void CloseUI()
    {
        isOpen = false;
        cestinoUIPanel.SetActive(false);
        quantityPanel.SetActive(false);

        // riprendi tutto
        playerController.enabled = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        escPromptText.gameObject.SetActive(false);
    }

    void AggiornaSlotsUI()
    {
        slotContainer.gameObject.SetActive(true);//Per prima cosa, si assicura che il contenitore principale degli slot (la griglia o il pannello nel Canvas della UI) sia acceso e visibile.

        foreach (Transform child in slotContainer)//questo distrugge 
            Destroy(child.gameObject);
                 /*
                  Questo è un sistema di pulizia totale. Dice: "Prendi ogni singolo oggetto figlio (child) che si trova dentro il contenitore e distruggilo".

                  Perché lo fa? Perché se aprissi il cestino la seconda volta senza pulire, Unity lascerebbe i vecchi bottoni e ne creerebbe di nuovi sotto, duplicando tutto e creando un macello visivo. Si riparte sempre da una pagina bianca.
                  */

        foreach (var slot in inventory.slots)//questo ricrea .. e quello che vediamo nel cestinopanel
        {
            

            GameObject slotObj = Instantiate(slotPrefab, slotContainer);

            // aggiorna icona
            var icon = slotObj.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = GetIcon(slot.type);

            // aggiorna testo quantità
            var text = slotObj.transform.Find("CountText").GetComponent<TextMeshProUGUI>();
            text.text = slot.currentCount.ToString();

            // bottone
            var btn = slotObj.GetComponent<Button>();
            RIFIUTIType capturedType = slot.type;
            btn.onClick.AddListener(() => OnSlotClicked(capturedType));
        }
    }

    Sprite GetIcon(RIFIUTIType type)
    {
        switch (type)
        {
            case RIFIUTIType.Plastica: return plasticaIcon;
            case RIFIUTIType.Vetro: return vetroIcon;
            case RIFIUTIType.Metallo: return metalloIcon;
            case RIFIUTIType.Carta: return cartaIcon;
            case RIFIUTIType.Organico: return organicoIcon;
            default: return null;
        }
    }

    void OnSlotClicked(RIFIUTIType type)
    {
        selectedType = type;
        quantityPanel.SetActive(true);
        slotContainer.gameObject.SetActive(false); // nascondi gli slot
        quantityInput.text = "";
        quantityLabel.text = $"[Quanti {type} vuoi buttare?]";
        quantityInput.ActivateInputField();//comodità per scrivere direttamente la quantita 
    }

    // collegato al bottone Conferma nel pannello quantità dall'Inspector
    public void OnConfirmQuantity()
    {
        if (!int.TryParse(quantityInput.text, out int quantity)) return;
        if (quantity <= 0) return;

        var slot = System.Array.Find(inventory.slots, s => s.type == selectedType);
        if (slot == null) return;

        // se il numero è maggiore di quello che hai messaggio errore
        if (quantity > slot.currentCount)
        {
            quantityLabel.text = $"Hai solo {slot.currentCount} {selectedType}!";
            return;
        }

        bool success = currentCestino.TryDeposit(selectedType, quantity, inventory);

        if (success)
        {
            quantityPanel.SetActive(false);
            AggiornaSlotsUI();
        }
    }
}