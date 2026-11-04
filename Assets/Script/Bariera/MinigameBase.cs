using System;
using UnityEngine;

// Classe base: ogni minigioco la estende, cosi' il cancello li gestisce tutti allo stesso modo.
public abstract class MinigameBase : MonoBehaviour
{
    public Action<bool> OnFinished; // true = vinto, false = perso

    // true mentre un minigioco e' aperto: gli altri script (es. quello del tasto T al cancello) possono controllarlo
    public static bool AnyRunning;

    public abstract void StartGame();

    protected void Finish(bool won)
    {
        gameObject.SetActive(false);
        OnFinished?.Invoke(won);
    }
}