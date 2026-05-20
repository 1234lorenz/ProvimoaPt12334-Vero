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
        if (capacity != null)
        {
            if (capacity.IsFull())
            {
                Debug.Log("Cestino pieno!");
                return false;
            }

            for (int i = 0; i < quantity; i++)
            {
                if (!capacity.AddItem()) break;
            }
        }

        inventory.RemoveTrash(type, quantity);
        MissionManager.Instance.OnSmaltimento(type, quantity);
        return true;
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