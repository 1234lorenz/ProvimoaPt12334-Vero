using UnityEngine;

public class AmbienceZone : MonoBehaviour
{
    public TipoAmbiente tipoDiQuestoAmbiente;

    private void OnTriggerEnter(Collider other)
    {
        // Quando il Player entra nella zona, cambia la musica d'ambiente
        if (other.CompareTag("Player"))
        {
            AmbienceManager.Instance.CambiaAmbiente(tipoDiQuestoAmbiente);
        }
    }
}