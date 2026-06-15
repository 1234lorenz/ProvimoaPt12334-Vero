using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [Header("Cestino")]
    public LayerMask cestinoLayer;

    [Header("Cestino Prompt")]
    [SerializeField] private GameObject cestinoPromptUI;
    [SerializeField] private TextMeshProUGUI cestinoPromptText;

    [Header("Settings")]
    public float detectionRadius = 3f;
    public LayerMask collectableLayer;

    [Header("Performance")]
    [SerializeField] private float checkInterval = 0.1f;
    private float checkTimer = 0f;

    [Header("UI Prompt")]
    [SerializeField] private GameObject collectPromptUI;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject panelCestinoPrompt;

    private CounterMenagerRifiuti counterRifiuti;

    [SerializeField] private PlayerInputHandler inputHandler;

    private List<RIFIUTI> itemsInRange = new List<RIFIUTI>();

    private RIFIUTI closestItem;

    private CestinoUI cestinoUI;
    private Cestino nearestCestino;

    private void Start()
    {
        counterRifiuti = GetComponent<CounterMenagerRifiuti>();
        collectPromptUI.SetActive(false);

        cestinoUI = FindObjectOfType<CestinoUI>();
        cestinoPromptUI.SetActive(false);
        panelCestinoPrompt.SetActive(false);
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            DetectNearby();
            DetectNearbyCestino();
        }

        // l'input va controllato ogni frame per non perdere la pressione del tasto
        if (inputHandler.CollectTrigger && closestItem != null)
            Collect(closestItem);
    }

    private void DetectNearbyCestino()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, cestinoLayer);

        if (hits.Length > 0)
        {
            Cestino cestino = hits[0].GetComponent<Cestino>();
            if (cestino != null && cestino != nearestCestino)
            {
                nearestCestino = cestino;
                cestinoUI.SetNearCestino(cestino);
            }

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

            panelCestinoPrompt.SetActive(false);
            cestinoPromptUI.SetActive(false);
        }
    }

    private void DetectNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, collectableLayer);

        List<RIFIUTI> newInRange = new List<RIFIUTI>();

        foreach (var hit in hits)
        {
            var item = hit.GetComponent<RIFIUTI>();
            if (item != null) newInRange.Add(item);
        }

        foreach (var old in itemsInRange)
            if (old != null && !newInRange.Contains(old))
                old.SetOutline(false);

        itemsInRange = newInRange;

        closestItem = null;
        float minDist = float.MaxValue;

        foreach (var item in itemsInRange)
        {
            item.SetOutline(true);
            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist < minDist) { minDist = dist; closestItem = item; }
        }

        if (closestItem != null)
        {
            collectPromptUI.SetActive(true);
            promptText.text = $" [E] Raccogli {closestItem.type}";
        }
        else
        {
            collectPromptUI.SetActive(false);
        }
    }

    private void Collect(RIFIUTI item)
    {
        bool added = counterRifiuti.AddTrash(item.type, item.quantita);
        if (!added) return;

        if (!string.IsNullOrEmpty(item.rifiutoID))
            RifiutiManager.Instance.RegistraRaccolta(item.rifiutoID);

        item.SetOutline(false);
        itemsInRange.Remove(item);
        closestItem = null;
        MissionManager.Instance.OnRaccolta();
        Destroy(item.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}