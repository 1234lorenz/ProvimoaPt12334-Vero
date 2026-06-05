using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CestinoUI : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject cestinoUIPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;
    public GameObject quantityPanel;
    public TMP_InputField quantityInput;
    public TextMeshProUGUI quantityLabel;

    [Header("Prompt Interazione Cestino")]
    public GameObject panelCestinoPromptQ;

    private CounterMenagerRifiuti inventory;
    private PlayerInputHandler inputHandler;
    private Cestino currentCestino;
    private RIFIUTIType selectedType;
    private bool isOpen = false;
    private bool wasOpenBinPressed = false;
    private FirstPersonController playerController;

    [Header("Icone Categorie Rifiuti")]
    public Sprite plasticaIcon;
    public Sprite vetroIcon;
    public Sprite metalloIcon;
    public Sprite cartaIcon;
    public Sprite organicoIcon;
    public Sprite seccoIcon;

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

        if (isPressed && !wasOpenBinPressed && currentCestino != null)
        {
            if (isOpen) CloseUI();
            else OpenUI();
        }

        wasOpenBinPressed = isPressed;

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            CloseUI();

        if (isOpen && panelCestinoPromptQ != null && panelCestinoPromptQ.activeSelf)
        {
            panelCestinoPromptQ.SetActive(false);
        }
    }

    public void SetNearCestino(Cestino cestino)
    {
        currentCestino = cestino;
    }

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

        if (panelCestinoPromptQ != null)
            panelCestinoPromptQ.SetActive(false);

        playerController.enabled = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseUI()
    {
        isOpen = false;
        cestinoUIPanel.SetActive(false);
        quantityPanel.SetActive(false);

        if (panelCestinoPromptQ != null && currentCestino != null)
            panelCestinoPromptQ.SetActive(true);

        playerController.enabled = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TornaAlMenuIniziale()
    {
        quantityPanel.SetActive(false);
        AggiornaSlotsUI();
    }

    void AggiornaSlotsUI()
    {
        slotContainer.gameObject.SetActive(true);

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (var slot in inventory.slots)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);

            var icon = slotObj.transform.Find("Icon").GetComponent<Image>();
            Sprite iconaTrovata = GetIcon(slot.type);

            if (iconaTrovata != null)
            {
                icon.sprite = iconaTrovata;
                icon.gameObject.SetActive(true);
            }
            else
            {
                icon.gameObject.SetActive(false);
            }

            var text = slotObj.transform.Find("CountText").GetComponent<TextMeshProUGUI>();
            text.text = slot.currentCount.ToString();

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
            case RIFIUTIType.Secco: return seccoIcon;
            default: return null;
        }
    }

    void OnSlotClicked(RIFIUTIType type)
    {
        selectedType = type;
        quantityPanel.SetActive(true);
        slotContainer.gameObject.SetActive(false);
        quantityInput.text = "";
        quantityLabel.text = $"[Quanti {type} vuoi buttare?]";
        quantityInput.ActivateInputField();
    }

    public void OnConfirmQuantity()
    {
        if (!int.TryParse(quantityInput.text, out int quantity)) return;
        if (quantity <= 0) return;

        var slot = System.Array.Find(inventory.slots, s => s.type == selectedType);
        if (slot == null) return;

        if (quantity > slot.currentCount)
        {
            quantityLabel.text = $"Hai solo {slot.currentCount} {selectedType}!";
            return;
        }

        //  NUOVA MODIFICA: Controlla quanti ne mancano alla missione ed evita gli sprechi nell'inventario
        if (MissionManager.Instance != null)
        {
            int mancanti = MissionManager.Instance.GetMancanti(selectedType);

            // Se la missione richiede ancora oggetti, e l'utente ne inserisce di più, frena l'input al necessario
            if (mancanti > 0 && quantity > mancanti)
            {
                quantity = mancanti;
            }
        }

        bool success = currentCestino.TryDeposit(selectedType, quantity, inventory);

        if (success)
        {
            quantityPanel.SetActive(false);
            AggiornaSlotsUI();
        }
    }
}