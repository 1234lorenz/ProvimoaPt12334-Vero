using UnityEngine;
using TMPro;
using System.Collections;

public class Cestino : MonoBehaviour
{
    [Header("Tipo cestino")]
    public RIFIUTIType acceptedType;

    [Header("Errore cestino sbagliato")]
    public int puntiEcoPenalita = 10;
    public TextMeshProUGUI errorMessage;
    public float errorDuration = 2f;

    private CameraSheke cameraShake;
    private CestinoCapacity capacity;
    private Coroutine errorCoroutine;

    void Start()
    {
        cameraShake = FindObjectOfType<CameraSheke>();
        capacity = GetComponent<CestinoCapacity>(); // preso una sola volta invece che ogni TryDeposit
    }

    public bool TryDeposit(RIFIUTIType type, int quantity, CounterMenagerRifiuti inventory)
    {
        if (type != acceptedType)
        {
            StartCoroutine(ShowError());

            if (cameraShake != null)
                cameraShake.Shake();

            XPManager.Instance.AddXP(-puntiEcoPenalita);
            Debug.Log($"Cestino sbagliato! -{puntiEcoPenalita} Punti Eco");
            return false;
        }

        int oggettiDepositatiEffettivamente = 0;

        if (capacity != null)
        {
            if (capacity.IsFull())
            {
                Debug.Log("Cestino pieno!");
                return false;
            }

            for (int i = 0; i < quantity; i++)
            {
                if (!capacity.AddItem())
                    break;

                oggettiDepositatiEffettivamente++;
            }
        }

        if (oggettiDepositatiEffettivamente > 0)
        {
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