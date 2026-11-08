using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum WasteCategory { Carta, Plastica, Vetro, Organico, Indifferenziato, Metallo }

[System.Serializable]
public class WasteItem
{
    public string itemName;
    public WasteCategory category;
    public Sprite sprite;             // opzionale
    public Color tint = Color.white;  // se non hai lo sprite viene usato questo colore
}


public class FallingWasteMinigame : MinigameBase
{
    [Header("Riferimenti UI")]
    public RectTransform playArea;   // il pannello a schermo intero (puo' essere questo stesso oggetto)
    public RectTransform bin;        // il cestino, figlio di playArea
    public GameObject itemPrefab;    // prefab UI con un componente Image
    public TMP_Text targetText;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Rifiuti (servono almeno 2 categorie diverse)")]
    public WasteItem[] items;

    [Header("Regole")]
    public int scoreToWin = 8;
    public int lives = 3;
    public float timeLimit = 30f;
    public float retargetEvery = 10f;
    public float spawnEvery = 0.9f;
    public Vector2 fallSpeed = new Vector2(250f, 400f);
    [Range(0f, 1f)] public float correctChance = 0.5f;

    [Header("Testi (modificabili dall'Inspector)")]
    public string targetPrefix = "Prendi solo: ";
    public string scorePrefix = "Punti: ";
    public string livesPrefix = "Vite: ";
    public string winMessage = "Bravo! Cancello sbloccato";
    public string loseMessage = "Riprova!";

    class Falling
    {
        public RectTransform rt;
        public WasteItem data;
        public float speed;
    }

    readonly List<Falling> falling = new List<Falling>();
    WasteCategory target;
    int score, livesLeft;
    float timeLeft, spawnTimer, retargetTimer;
    bool running;

    public override void StartGame()
    {
        foreach (var f in falling)
            if (f.rt) Destroy(f.rt.gameObject);
        falling.Clear();

        score = 0;
        livesLeft = lives;
        timeLeft = timeLimit;
        spawnTimer = 0f;
        retargetTimer = 0f;
        if (resultText) resultText.text = "";

        PickTarget();
        UpdateTexts();
        running = true;
    }

    void Update()
    {
        if (!running) return;
        float dt = Time.unscaledDeltaTime; // il gioco 3D e' in pausa, quindi niente Time.deltaTime

        MoveBin();

        timeLeft -= dt;
        retargetTimer += dt;
        if (retargetTimer >= retargetEvery) { retargetTimer = 0f; PickTarget(); }

        spawnTimer -= dt;
        if (spawnTimer <= 0f) { Spawn(); spawnTimer = spawnEvery; }

        float bottom = playArea.rect.yMin - 80f;
        for (int i = falling.Count - 1; i >= 0; i--)
        {
            Falling f = falling[i];
            f.rt.localPosition += Vector3.down * f.speed * dt;

            if (Caught(f.rt))
            {
                OnCaught(f);
                Remove(i);
                if (!running) return;
            }
            else if (f.rt.localPosition.y < bottom)
            {
                Remove(i);
            }
        }

        UpdateTexts();
        if (timeLeft <= 0f) End(false);
    }

    void MoveBin()
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(playArea, GetPointerPosition(), null, out Vector2 local))
        {
            float half = bin.rect.width * 0.5f;
            float x = Mathf.Clamp(local.x, playArea.rect.xMin + half, playArea.rect.xMax - half);
            bin.localPosition = new Vector3(x, bin.localPosition.y, 0f);
        }
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

    void Spawn()
    {
        bool wantCorrect = Random.value < correctChance;
        WasteItem data = RandomItem(wantCorrect);
        if (data == null) return;

        GameObject go = Instantiate(itemPrefab, playArea);
        RectTransform rt = (RectTransform)go.transform;
        float half = rt.rect.width * 0.5f;
        float x = Random.Range(playArea.rect.xMin + half, playArea.rect.xMax - half);
        rt.localPosition = new Vector3(x, playArea.rect.yMax + half, 0f);

        Image img = go.GetComponent<Image>();
        if (img)
        {
            img.sprite = data.sprite;
            img.color = data.sprite ? Color.white : data.tint;
        }

        falling.Add(new Falling { rt = rt, data = data, speed = Random.Range(fallSpeed.x, fallSpeed.y) });
    }

    WasteItem RandomItem(bool correct)
    {
        var pool = new List<WasteItem>();
        foreach (var it in items)
            if ((it.category == target) == correct) pool.Add(it);
        return pool.Count > 0 ? pool[Random.Range(0, pool.Count)] : null;
    }

    bool Caught(RectTransform item)
    {
        Vector3 p = item.localPosition;
        Vector3 b = bin.localPosition;
        float halfW = (bin.rect.width + item.rect.width) * 0.5f;
        float halfH = (bin.rect.height + item.rect.height) * 0.5f;
        return Mathf.Abs(p.x - b.x) < halfW && Mathf.Abs(p.y - b.y) < halfH;
    }

    void OnCaught(Falling f)
    {
        if (f.data.category == target)
        {
            score++;
            if (score >= scoreToWin) End(true);
        }
        else
        {
            livesLeft--;
            if (livesLeft <= 0) End(false);
        }
    }

    void Remove(int index)
    {
        if (falling[index].rt) Destroy(falling[index].rt.gameObject);
        falling.RemoveAt(index);
    }

    void PickTarget()
    {
        target = items[Random.Range(0, items.Length)].category;
        if (targetText) targetText.text = targetPrefix + target.ToString().ToUpper();
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