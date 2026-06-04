using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CaricamentoUI : MonoBehaviour
{
    [Header("UI")]
    public Transform slotContainer;
    public GameObject slotPrefabCaricamento;

    private const int NUM_SLOTS = 6;

    public void AggiornaSlotsUI()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < NUM_SLOTS; i++)
        {
            int slotIndex = i + 1;
            GameObject slotObj = Instantiate(slotPrefabCaricamento, slotContainer);

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

            var bottoni = slotObj.GetComponentsInChildren<Button>();
            int capturedSlot = slotIndex;

            // bottone CARICA
            bottoni[0].onClick.AddListener(() =>
            {
                SaveManager.Instance.slotDaCaricare = capturedSlot;
                SceneManager.LoadScene("SampleScene");
            });

            // bottone ELIMINA
            if (bottoni.Length > 1)
            {
                bottoni[1].onClick.AddListener(() =>
                {
                    SaveManager.Instance.Elimina(capturedSlot);
                    AggiornaSlotsUI();
                });

                bottoni[1].gameObject.SetActive(SaveManager.Instance.SlotOccupato(slotIndex));
            }

            // disabilita carica se slot vuoto
            bottoni[0].interactable = SaveManager.Instance.SlotOccupato(slotIndex);
        }
    }
}