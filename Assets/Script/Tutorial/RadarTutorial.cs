using UnityEngine;

public class RadarTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        

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