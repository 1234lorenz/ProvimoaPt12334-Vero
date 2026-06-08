using UnityEngine;
using TMPro;
using System.Collections;

public class Cestino : MonoBehaviour
{
    [Header("Tipo cestino")]
    public RIFIUTIType acceptedType;

    [Header("Errore cestino sbagliato")]
    public int puntiEcoPenalita = 10;
    public TextMeshProUGUI errorMessage;// testo "Cestino sbagliato!"
    public float errorDuration = 2f;

    private CameraSheke cameraShake;
    private Coroutine errorCoroutine;

    void Start()
    {
        cameraShake = FindObjectOfType<CameraSheke>();
    }

    public bool TryDeposit(RIFIUTIType type, int quantity, CounterMenagerRifiuti inventory)
    {
        if (type != acceptedType)
        {
            // cestino sbagliato!
            StartCoroutine(ShowError());
            cameraShake.StartCoroutine(cameraShake.Shake());
            XPManager.Instance.AddXP(-puntiEcoPenalita); // togli punti eco
            Debug.Log($"Cestino sbagliato! -{puntiEcoPenalita} Punti Eco");
            return false;
        }

        CestinoCapacity capacity = GetComponent<CestinoCapacity>();
        int oggettiDepositatiEffettivamente = 0; // NUOVO: Teniamo il conto di quanti ne entrano

        if (capacity != null)
        {
            if (capacity.IsFull())
            {
                Debug.Log("Cestino pieno!");
                return false;
            }

            // Proviamo a buttare la quantità richiesta
            for (int i = 0; i < quantity; i++)
            {
                if (!capacity.AddItem())
                {
                    break; // Il cestino si è riempito a metà dell'opera, ci fermiamo!
                }
                oggettiDepositatiEffettivamente++; // È entrato, aumentiamo il conto!
            }
        }

        // Se siamo riusciti a buttare almeno un oggetto
        if (oggettiDepositatiEffettivamente > 0)
        {
            // Rimuoviamo dall'inventario SOLO il numero di oggetti che sono entrati davvero!
            inventory.RemoveTrash(type, oggettiDepositatiEffettivamente);
            MissionManager.Instance.OnSmaltimento(type, oggettiDepositatiEffettivamente);
            return true;
        }

        return false;
    }

    private IEnumerator ShowError()
    {
        if (errorCoroutine != null)
            StopCoroutine(errorCoroutine);

        if (errorMessage != null)
        {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Cestino sbagliato!";
            yield return new WaitForSecondsRealtime(errorDuration); 
            errorMessage.gameObject.SetActive(false);
        }
    }
}