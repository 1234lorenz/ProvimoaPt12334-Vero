using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("Missioni Livello 1")]
    public int plasticaSmaltite = 0;
    public int organicoSmaltiti = 0;
    public int cartaSmaltita = 0;
    public int plasticaTarget = 10;
    public int organicoTarget = 5;
    public int cartaTarget = 5;

    [Header("Missioni Livello 2")]
    public int vetroSmaltito = 0;
    public int metalloSmaltito = 0;
    public int vetroTarget = 15;
    public int metalloTarget = 10;

    [Header("Missioni Livello 3")]
    public int cestiniSmaltiti = 0;
    public int cestiniTarget = 3;

    [Header("Punti Eco per oggetto smaltito")]
    public int xpPerOggetto = 10;

    [Header("Punti Eco bonus completamento missione")]
    public int xpBonusPlastica = 50;
    public int xpBonusOrganico = 25;
    public int xpBonusCarta = 25;
    public int xpBonusVetro = 40;
    public int xpBonusMetallo = 40;
    public int xpBonusCestino = 60;

    // traccia missioni già completate per non dare bonus due volte
    private bool plasticaCompleta = false;
    private bool organicoCompleto = false;
    private bool cartaCompleta = false;
    private bool vetroCompleto = false;
    private bool metalloCompleto = false;

    [Header("UI Missioni")]
    public TextMeshProUGUI missionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
      
        UpdateMissionUI(); // aggiungi questa riga
    }
    public void OnSmaltimento(RIFIUTIType type, int quantity)
    {
        // XP progressivi per ogni oggetto
        XPManager.Instance.AddXP(xpPerOggetto * quantity);

        // aggiorna contatori e controlla bonus
        switch (type)
        {
            case RIFIUTIType.Plastica:
                plasticaSmaltite += quantity;
                if (!plasticaCompleta && plasticaSmaltite >= plasticaTarget)
                {
                    plasticaCompleta = true;
                    XPManager.Instance.AddXP(xpBonusPlastica);
                    Debug.Log("Missione Plastica completata! Bonus Punti Eco!");
                }
                break;

            case RIFIUTIType.Organico:
                organicoSmaltiti += quantity;
                if (!organicoCompleto && organicoSmaltiti >= organicoTarget)
                {
                    organicoCompleto = true;
                    XPManager.Instance.AddXP(xpBonusOrganico);
                    Debug.Log("Missione Organico completata! Bonus Punti Eco!");
                }
                break;

            case RIFIUTIType.Carta:
                cartaSmaltita += quantity;
                if (!cartaCompleta && cartaSmaltita >= cartaTarget)
                {
                    cartaCompleta = true;
                    XPManager.Instance.AddXP(xpBonusCarta);
                    Debug.Log("Missione Carta completata! Bonus Punti Eco!");
                }
                break;

            case RIFIUTIType.Vetro:
                vetroSmaltito += quantity;
                if (!vetroCompleto && vetroSmaltito >= vetroTarget)
                {
                    vetroCompleto = true;
                    XPManager.Instance.AddXP(xpBonusVetro);
                    Debug.Log("Missione Vetro completata! Bonus Punti Eco!");
                }
                break;

            case RIFIUTIType.Metallo:
                metalloSmaltito += quantity;
                if (!metalloCompleto && metalloSmaltito >= metalloTarget)
                {
                    metalloCompleto = true;
                    XPManager.Instance.AddXP(xpBonusMetallo);
                    Debug.Log("Missione Metallo completata! Bonus Punti Eco!");
                }
                break;
        }

        UpdateMissionUI();
    }

    public void OnRaccolta()
    {
        // raccolta non dà XP diretti — solo smaltimento
    }

    public void OnCestinoSmaltito()
    {
        cestiniSmaltiti++;
        XPManager.Instance.AddXP(xpBonusCestino);
        UpdateMissionUI();
    }

    public bool Level1Complete()
    {
        return plasticaCompleta && organicoCompleto && cartaCompleta;
    }

    public bool Level2Complete()
    {
        return vetroCompleto && metalloCompleto && cestiniSmaltiti >= 1;
    }

    public bool Level3Complete()
    {
        return cestiniSmaltiti >= cestiniTarget;
    }

    // resetta le missioni quando si sale di livello
    public void ResetMissioni()
    {
        plasticaSmaltite = 0; organicoSmaltiti = 0; cartaSmaltita = 0;
        vetroSmaltito = 0; metalloSmaltito = 0; cestiniSmaltiti = 0;
        plasticaCompleta = false; organicoCompleto = false;
        cartaCompleta = false; vetroCompleto = false; metalloCompleto = false;
        UpdateMissionUI();
    }

    private void UpdateMissionUI()
    {
        if (missionText == null) return;

        if (XPManager.Instance.currentLevel == 1)
        {
            missionText.text =
                $"Missioni Zona 1:\n" +
                $"Plastica: {plasticaSmaltite}/{plasticaTarget} {(plasticaCompleta ? "[V]" : "")}\n" +
                $"Organico: {organicoSmaltiti}/{organicoTarget} {(organicoCompleto ? "[V]" : "")}\n" +
                $"Carta: {cartaSmaltita}/{cartaTarget} {(cartaCompleta ? "[V]" : "")}";
        }
        else if (XPManager.Instance.currentLevel == 2)
        {
            missionText.text =
                $"Missioni Zona 2:\n" +
                $"Vetro: {vetroSmaltito}/{vetroTarget} {(vetroCompleto ? "[V]" : "")}\n" +
                $"Metallo: {metalloSmaltito}/{metalloTarget} {(metalloCompleto ? "[V]" : "")}\n" +
                $"Cestini: {cestiniSmaltiti}/1";
        }
        else
        {
            missionText.text =
                $"Missioni Zona 3:\n" +
                $"Cestini smaltiti: {cestiniSmaltiti}/{cestiniTarget}";
        }
    }
}