using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SalvataggioUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject salvataggioPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;

    private const int NUM_SLOTS = 6;

    public void ApriSalvataggio()
    {
        salvataggioPanel.SetActive(true);
        AggiornaSlotsUI();
    }

    public void ChiudiSalvataggio()
    {
        salvataggioPanel.SetActive(false);
    }

    private void AggiornaSlotsUI()
    {
        // pulisci slot vecchi
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        // crea uno slot per ogni salvataggio
        for (int i = 0; i < NUM_SLOTS; i++)
        {
            int slotIndex = i + 1;
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);

            // testo dello slot
            var testo = slotObj.GetComponentInChildren<TextMeshProUGUI>();

            if (SaveManager.Instance.SlotOccupato(slotIndex))
            {
                SaveData data = SaveManager.Instance.LeggiSlot(slotIndex);
                testo.text = $"Slot {slotIndex} — Livello {data.currentLevel} | {data.currentXP} XP | {data.dataOra}";
            }
            else
            {
                testo.text = $"Slot {slotIndex} — Vuoto";
            }

            // bottone SALVA
            var bottoni = slotObj.GetComponentsInChildren<Button>();
            int capturedSlot = slotIndex;

            bottoni[0].onClick.AddListener(() =>
            {
                SaveManager.Instance.Salva(capturedSlot);
                AggiornaSlotsUI();
            });

            // bottone ELIMINA
            if (bottoni.Length > 1)
            {
                bottoni[1].onClick.AddListener(() =>
                {
                    SaveManager.Instance.Elimina(capturedSlot);
                    AggiornaSlotsUI();
                });

                // nascondi elimina se slot vuoto
                bottoni[1].gameObject.SetActive(SaveManager.Instance.SlotOccupato(slotIndex));
            }
        }
    }
}