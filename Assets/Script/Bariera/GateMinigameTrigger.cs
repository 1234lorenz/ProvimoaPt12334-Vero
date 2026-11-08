using System.Collections;
using UnityEngine;

// Va su un oggetto vuoto con uno Sphere/Box Collider "Is Trigger" PIU' GRANDE della barriera

// Se vinci la barriera si rimpicciolisce e sparisce, se perdi puoi riprovare uscendo e rientrando.
public class GateMinigameTrigger : MonoBehaviour
{
    [Header("Barriera (la sfera)")]
    public Transform barrier;
    public float openDuration = 1f;

    [Header("Minigiochi")]
    public MinigameBase[] minigames;

    [Header("Mentre il minigioco e' aperto")]
    [Tooltip("Trascina qui gli script che muovono il player e la camera (es. PlayerInputHendler)")]
    public MonoBehaviour[] disableWhilePlaying;
    [Tooltip("Oggetti da nascondere durante il minigioco (es. la scritta 'Premi T')")]
    public GameObject[] hideWhilePlaying;
    [Tooltip("Ferma il tempo del gioco 3D (nemici compresi) durante il minigioco")]
    public bool pauseWorld = true;

    [Header("Dopo una sconfitta")]
    public float retryDelay = 1.5f;

    bool unlocked;
    bool[] hiddenWasActive;

    [Header("Salvataggio")]
    [Tooltip("Nome unico di questa barriera nel salvataggio. Se lo lasci vuoto usa il nome dell'oggetto.")]
    public string gateId = "";

    public string GateId => string.IsNullOrEmpty(gateId) ? gameObject.name : gateId;
    public bool IsUnlocked => unlocked;

    // Chiamato dal caricamento: la barriera risulta gia' superata, senza rifare il minigioco.
    public void ForceUnlock()
    {
        unlocked = true;
        if (barrier) barrier.gameObject.SetActive(false);
    }
    bool running;
    float retryTime;
    float prevTimeScale = 1f;
    CursorLockMode prevLock;
    bool prevCursorVisible;

    void OnTriggerEnter(Collider other)
    {
        if (unlocked || running || Time.time < retryTime) return;
        if (!other.CompareTag("Player")) return;
        if (minigames == null || minigames.Length == 0) return;

        running = true;
        MinigameBase game = minigames[Random.Range(0, minigames.Length)];

        foreach (var m in disableWhilePlaying)
            if (m) m.enabled = false;

        hiddenWasActive = new bool[hideWhilePlaying != null ? hideWhilePlaying.Length : 0];
        for (int i = 0; i < hiddenWasActive.Length; i++)
        {
            GameObject go = hideWhilePlaying[i];
            hiddenWasActive[i] = go && go.activeSelf;
            if (go) go.SetActive(false);
        }
        MinigameBase.AnyRunning = true;

        prevLock = Cursor.lockState;
        prevCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        prevTimeScale = Time.timeScale;
        if (pauseWorld) Time.timeScale = 0f;

        game.OnFinished = HandleResult;
        game.gameObject.SetActive(true);
        game.StartGame();
    }

    void HandleResult(bool won)
    {
        running = false;

        Time.timeScale = prevTimeScale;
        Cursor.lockState = prevLock;
        Cursor.visible = prevCursorVisible;

        foreach (var m in disableWhilePlaying)
            if (m) m.enabled = true;

        for (int i = 0; hiddenWasActive != null && i < hiddenWasActive.Length; i++)
            if (hiddenWasActive[i] && hideWhilePlaying[i]) hideWhilePlaying[i].SetActive(true);
        MinigameBase.AnyRunning = false;

        if (won)
        {
            unlocked = true;
            if (barrier) StartCoroutine(OpenBarrier());
        }
        else
        {
            retryTime = Time.time + retryDelay;
        }
    }

    IEnumerator OpenBarrier()
    {
        Vector3 start = barrier.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, openDuration);
            barrier.localScale = Vector3.Lerp(start, Vector3.zero, t * t);
            yield return null;
        }
        barrier.gameObject.SetActive(false); // col GameObject sparisce anche il suo collider
    }
}