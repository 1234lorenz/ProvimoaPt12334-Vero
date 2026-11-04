using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SortBin
{
    public RectTransform area;        // la zona del cestino (un'Image UI)
    public WasteCategory category;    // che rifiuti accetta
}

// Minigioco 2D: i rifiuti scorrono su un nastro, trascinali col mouse nel cestino giusto.
// Usa WasteItem e WasteCategory definiti in FallingWasteMinigame.cs.
public class ConveyorSortMinigame : MinigameBase
{
    [Header("Riferimenti UI")]
    public RectTransform playArea;   // il pannello a schermo intero (di solito questo stesso oggetto)
    public RectTransform belt;       // la striscia del nastro: serve per l'altezza a cui scorrono i rifiuti
    public GameObject itemPrefab;    // prefab UI con un componente Image (puoi riusare quello dell'altro minigioco)
    public SortBin[] bins;           // un cestino per ogni categoria che usi
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Rifiuti (ogni categoria deve avere un cestino)")]
    public WasteItem[] items;

    [Header("Regole")]
    public int scoreToWin = 6;
    public int lives = 3;
    public float timeLimit = 40f;
    public float spawnEvery = 1.4f;
    public float beltSpeed = 160f;
    public float speedRampPerSecond = 5f;   // il nastro accelera col tempo
    public bool missedItemCostsLife = true; // se un rifiuto arriva in fondo senza essere smistato

    [Header("Testi (modificabili dall'Inspector)")]
    public string scorePrefix = "Punti: ";
    public string livesPrefix = "Vite: ";
    public string winMessage = "Bravo! Cancello sbloccato";
    public string loseMessage = "Riprova!";

    class BeltItem
    {
        public RectTransform rt;
        public WasteItem data;
    }

    readonly List<BeltItem> onBelt = new List<BeltItem>();
    BeltItem dragged;
    int score, livesLeft;
    float timeLeft, spawnTimer, elapsed;
    bool running;
    bool wasHeld;

    public override void StartGame()
    {
        foreach (var b in onBelt)
            if (b.rt) Destroy(b.rt.gameObject);
        onBelt.Clear();
        dragged = null;

        score = 0;
        livesLeft = lives;
        timeLeft = timeLimit;
        spawnTimer = 0.3f;
        elapsed = 0f;
        wasHeld = PointerHeld();
        if (resultText) resultText.text = "";

        UpdateTexts();
        running = true;
    }

    void Update()
    {
        if (!running) return;
        float dt = Time.unscaledDeltaTime; // il gioco 3D e' in pausa, quindi niente Time.deltaTime
        elapsed += dt;

        HandlePointer();
        if (!running) return;

        timeLeft -= dt;
        spawnTimer -= dt;
        if (spawnTimer <= 0f) { Spawn(); spawnTimer = spawnEvery; }

        float speed = beltSpeed + speedRampPerSecond * elapsed;
        for (int i = onBelt.Count - 1; i >= 0; i--)
        {
            BeltItem it = onBelt[i];
            if (it == dragged) continue; // quello che stai trascinando non segue il nastro

            it.rt.localPosition += Vector3.right * speed * dt;

            if (it.rt.localPosition.x > playArea.rect.xMax + it.rt.rect.width * 0.5f)
            {
                Remove(i);
                if (missedItemCostsLife)
                {
                    livesLeft--;
                    if (livesLeft <= 0) { End(false); return; }
                }
            }
        }

        UpdateTexts();
        if (timeLeft <= 0f) End(false);
    }

    void HandlePointer()
    {
        Vector2 screen = GetPointerPosition();
        bool held = PointerHeld();
        bool down = held && !wasHeld;
        bool up = !held && wasHeld;
        wasHeld = held;

        if (down && dragged == null)
        {
            for (int i = onBelt.Count - 1; i >= 0; i--)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(onBelt[i].rt, screen, null))
                {
                    dragged = onBelt[i];
                    dragged.rt.SetAsLastSibling(); // lo disegna sopra a tutto
                    break;
                }
            }
        }

        if (dragged != null)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(playArea, screen, null, out Vector2 local))
                dragged.rt.localPosition = new Vector3(local.x, local.y, 0f);

            if (up) Drop(screen);
        }
    }

    void Drop(Vector2 screen)
    {
        BeltItem it = dragged;
        dragged = null;

        foreach (var b in bins)
        {
            if (b.area && RectTransformUtility.RectangleContainsScreenPoint(b.area, screen, null))
            {
                onBelt.Remove(it);
                Destroy(it.rt.gameObject);

                if (it.data.category == b.category)
                {
                    score++;
                    if (score >= scoreToWin) End(true);
                }
                else
                {
                    livesLeft--;
                    if (livesLeft <= 0) End(false);
                }
                UpdateTexts();
                return;
            }
        }

        // lasciato fuori dai cestini: torna sul nastro
        it.rt.localPosition = new Vector3(it.rt.localPosition.x, belt.localPosition.y, 0f);
    }

    void Spawn()
    {
        if (items == null || items.Length == 0) return;
        WasteItem data = items[Random.Range(0, items.Length)];

        GameObject go = Instantiate(itemPrefab, playArea);
        RectTransform rt = (RectTransform)go.transform;
        rt.localPosition = new Vector3(playArea.rect.xMin - rt.rect.width * 0.5f, belt.localPosition.y, 0f);

        Image img = go.GetComponent<Image>();
        if (img)
        {
            img.sprite = data.sprite;
            img.color = data.sprite ? Color.white : data.tint;
        }

        onBelt.Add(new BeltItem { rt = rt, data = data });
    }

    void Remove(int index)
    {
        if (onBelt[index].rt) Destroy(onBelt[index].rt.gameObject);
        onBelt.RemoveAt(index);
    }

    Vector2 GetPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        return UnityEngine.InputSystem.Mouse.current != null
            ? UnityEngine.InputSystem.Mouse.current.position.ReadValue()
            : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    bool PointerHeld()
    {
#if ENABLE_INPUT_SYSTEM
        return UnityEngine.InputSystem.Mouse.current != null
            && UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }

    void UpdateTexts()
    {
        if (scoreText) scoreText.text = scorePrefix + score + "/" + scoreToWin;
        if (livesText) livesText.text = livesPrefix + livesLeft;
        if (timerText) timerText.text = Mathf.CeilToInt(Mathf.Max(0f, timeLeft)).ToString();
    }

    void End(bool won)
    {
        if (!running) return;
        running = false;
        StartCoroutine(EndRoutine(won));
    }

    IEnumerator EndRoutine(bool won)
    {
        if (resultText) resultText.text = won ? winMessage : loseMessage;
        yield return new WaitForSecondsRealtime(1.2f);
        Finish(won);
    }
}