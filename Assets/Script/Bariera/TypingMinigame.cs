using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WasteWord
{
    public Sprite sprite;                          // l'icona del rifiuto
    public Color tint = Color.white;               // se non hai lo sprite viene usato questo colore
    [Tooltip("La prima risposta e' quella mostrata quando sbagli. Aggiungi tutte le varianti accettate.")]
    public string[] acceptedAnswers;
}


// Giusto = un punto, sbagliato o tempo scaduto = una vita in meno.
public class TypingMinigame : MinigameBase
{
    [Header("Riferimenti UI")]
    public Image itemImage;              // dove appare l'icona del rifiuto
    public TMP_InputField input;         // il campo dove scrivi (deve essere Single Line)
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text timerText;
    public TMP_Text hintText;            // opzionale: l'aiuto con la prima lettera
    public TMP_Text feedbackText;        // opzionale: "Giusto!" / "Sbagliato! Era: ..."
    public TMP_Text resultText;

    [Header("Rifiuti")]
    public WasteWord[] words;

    [Header("Regole")]
    public int scoreToWin = 5;
    public int lives = 3;
    public float timePerItem = 15f;
    public float hintAfterSeconds = 7f;  // dopo quanti secondi compare l'aiuto
    public bool allowOneTypo = true;     // accetta un errore di battitura (solo parole da 5 lettere in su)
    public float feedbackSeconds = 1f;

    [Header("Testi (modificabili dall'Inspector)")]
    public string scorePrefix = "Punti: ";
    public string livesPrefix = "Vite: ";
    public string correctMessage = "Giusto!";
    public string wrongPrefix = "Sbagliato! Era: ";
    public string timeoutPrefix = "Tempo scaduto! Era: ";
    public string winMessage = "Bravo! Cancello sbloccato";
    public string loseMessage = "Riprova!";
    public Color correctColor = new Color(0.4f, 1f, 0.4f);
    public Color wrongColor = new Color(1f, 0.4f, 0.4f);

    readonly Queue<int> order = new Queue<int>();
    WasteWord current;
    int score, livesLeft;
    float itemTimeLeft;
    bool running, waiting;

    public override void StartGame()
    {
        if (words == null || words.Length == 0 || input == null)
        {
            Debug.LogError("TypingMinigame: mancano i rifiuti (Words) o il campo Input.");
            Finish(false);
            return;
        }

        score = 0;
        livesLeft = lives;
        if (resultText) resultText.text = "";
        if (feedbackText) feedbackText.text = "";

        input.onSubmit.RemoveListener(OnSubmit);
        input.onSubmit.AddListener(OnSubmit);

        order.Clear();
        running = true;
        NextItem();
    }

    void OnDisable()
    {
        if (input) input.onSubmit.RemoveListener(OnSubmit);
    }

    void Update()
    {
        if (!running || waiting) return;

        itemTimeLeft -= Time.unscaledDeltaTime; // il gioco 3D e' in pausa, quindi niente Time.deltaTime

        if (hintText && hintText.text.Length == 0 && timePerItem - itemTimeLeft >= hintAfterSeconds)
            hintText.text = BuildHint();

        UpdateTexts();
        if (itemTimeLeft <= 0f) Resolve(false, true);
    }

    void NextItem()
    {
        if (order.Count == 0) BuildOrder();
        current = words[order.Dequeue()];

        waiting = false;
        itemTimeLeft = timePerItem;

        if (itemImage)
        {
            itemImage.sprite = current.sprite;
            itemImage.color = current.sprite ? Color.white : current.tint;
        }
        if (hintText) hintText.text = "";
        if (feedbackText) feedbackText.text = "";

        input.interactable = true;
        input.text = "";
        UpdateTexts();
        StartCoroutine(FocusInput());
    }

    void BuildOrder()
    {
        var list = new List<int>();
        for (int i = 0; i < words.Length; i++) list.Add(i);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = list[i]; list[i] = list[j]; list[j] = tmp;
        }
        foreach (int i in list) order.Enqueue(i);
    }

    IEnumerator FocusInput()
    {
        yield return null; // aspetta un frame, altrimenti il campo non prende il focus
        if (!running || waiting) yield break;
        input.Select();
        input.ActivateInputField();
    }

    void OnSubmit(string text)
    {
        if (!running || waiting) return;

        if (string.IsNullOrWhiteSpace(text))
        {
            input.ActivateInputField(); // Invio a vuoto: non conta come errore
            return;
        }
        Resolve(IsCorrect(text), false);
    }

    void Resolve(bool correct, bool timeout)
    {
        waiting = true;
        input.interactable = false;

        string official = OfficialAnswer();
        if (correct)
        {
            score++;
            ShowFeedback(correctMessage, correctColor);
        }
        else
        {
            livesLeft--;
            ShowFeedback((timeout ? timeoutPrefix : wrongPrefix) + official, wrongColor);
        }

        UpdateTexts();
        StartCoroutine(AfterFeedback());
    }

    IEnumerator AfterFeedback()
    {
        yield return new WaitForSecondsRealtime(feedbackSeconds);

        if (score >= scoreToWin) { End(true); yield break; }
        if (livesLeft <= 0) { End(false); yield break; }
        NextItem();
    }

    void ShowFeedback(string message, Color color)
    {
        if (!feedbackText) return;
        feedbackText.text = message;
        feedbackText.color = color;
    }

    string OfficialAnswer()
    {
        return (current.acceptedAnswers != null && current.acceptedAnswers.Length > 0)
            ? current.acceptedAnswers[0]
            : "?";
    }

    bool IsCorrect(string text)
    {
        if (current.acceptedAnswers == null) return false;

        string guess = Normalize(text);
        if (guess.Length == 0) return false;

        foreach (string answer in current.acceptedAnswers)
        {
            string target = Normalize(answer);
            if (target.Length == 0) continue;
            if (guess == target) return true;
            if (allowOneTypo && target.Length >= 5 && Distance(guess, target) <= 1) return true;
        }
        return false;
    }

    // minuscolo, senza accenti, senza punteggiatura, spazi singoli
    static string Normalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";

        string decomposed = s.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        bool lastSpace = false;
        foreach (char c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;

            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
                lastSpace = false;
            }
            else if (char.IsWhiteSpace(c) || c == '\'' || c == '-')
            {
                if (!lastSpace) { sb.Append(' '); lastSpace = true; }
            }
        }
        return sb.ToString().Trim();
    }

    // distanza di Levenshtein: quante modifiche servono per passare da a a b
    static int Distance(string a, string b)
    {
        int[] prev = new int[b.Length + 1];
        int[] curr = new int[b.Length + 1];
        for (int j = 0; j <= b.Length; j++) prev[j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            curr[0] = i;
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                curr[j] = Mathf.Min(Mathf.Min(curr[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
            }
            int[] tmp = prev; prev = curr; curr = tmp;
        }
        return prev[b.Length];
    }

    // "B _ _ _ _ _" : prima lettera e lunghezza della risposta
    string BuildHint()
    {
        if (current.acceptedAnswers == null || current.acceptedAnswers.Length == 0) return "";

        string answer = current.acceptedAnswers[0];
        var sb = new StringBuilder();
        for (int i = 0; i < answer.Length; i++)
        {
            char c = answer[i];
            if (c == ' ') sb.Append("   ");
            else if (i == 0) sb.Append(char.ToUpper(c)).Append(' ');
            else sb.Append("_ ");
        }
        return sb.ToString();
    }

    void UpdateTexts()
    {
        if (scoreText) scoreText.text = scorePrefix + score + "/" + scoreToWin;
        if (livesText) livesText.text = livesPrefix + livesLeft;
        if (timerText) timerText.text = Mathf.CeilToInt(Mathf.Max(0f, itemTimeLeft)).ToString();
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