using UnityEngine;
using Cinemachine;

public class ResetFreeLook : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    void Start()
    {
        // resetta la posizione della camera all'avvio
        freeLook.m_YAxis.Value = 0.5f;
        freeLook.m_XAxis.Value = 0f;
    }
}