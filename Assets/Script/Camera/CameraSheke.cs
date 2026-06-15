using UnityEngine;
using Cinemachine;

public class CameraSheke : MonoBehaviour
{
    [Header("Intensità Shake")]
    [SerializeField] private float intensitaShake = 0.5f;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake()
    {
        if (impulseSource != null)
            impulseSource.GenerateImpulse(intensitaShake);
    }
}