using UnityEngine;

public class RadarTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Debug universale: scriviamo TUTTO quello che tocchiamo
        //Debug.Log("DEBUG: Il radar ha toccato: " + other.name + " con Tag: " + other.tag);

        // Ora il controllo col Tag esatto
        if (other.CompareTag("RIFIUTO"))
        {
            //Debug.Log(" RIFIUTO RILEVATO CORRETTAMENTE!");
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.OnRifiutoAvvicinato();
            }
        }
    }
}