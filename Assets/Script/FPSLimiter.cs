using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [Header("Limite FPS")]
    public int targetFPS = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0; // disattiva V-Sync per poter impostare un FPS custom
        Application.targetFrameRate = targetFPS;
    }
}