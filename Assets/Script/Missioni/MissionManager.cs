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
    public bool plasticaCompleta = false;
    public bool organicoCompleto = false;
    public bool cartaCompleta = false;
    public bool vetroCompleto = false;
    public bool metalloCompleto = false;

    [Header("UI Missioni")]
    public TextMeshProUGUI missionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateMissionUI();
    }

    public void AggiornaMissioniUI()
    {
        UpdateMissionUI();
    }

    // NUOVA FUNZIONE: Calcola quanti oggetti mancano per completare la missione di quella categoria
    public int GetMancanti(RIFIUTIType type)
    {
        switch (type)
        {
            case RIFIUTIType.Plastica: return plasticaCompleta ? 0 : Mathf.Max(0, plasticaTarget - plasticaSmaltite);
            case RIFIUTIType.Organico: return organicoCompleto ? 0 : Mathf.Max(0, organicoTarget - organicoSmaltiti);
            case RIFIUTIType.Carta: return cartaCompleta ? 0 : Mathf.Max(0, cartaTarget - cartaSmaltita);
            case RIFIUTIType.Vetro: return vetroCompleto ? 0 : Mathf.Max(0, vetroTarget - vetroSmaltito);
            case RIFIUTIType.Metallo: return metalloCompleto ? 0 : Mathf.Max(0, metalloTarget - metalloSmaltito);
            default: return 0;
        }
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
            //  MODIFICATO: Mathf.Min blocca la visualizzazione grafica al valore massimo del Target
            int plasticaVisualizzata = Mathf.Min(plasticaSmaltite, plasticaTarget);
            int organicoVisualizzato = Mathf.Min(organicoSmaltiti, organicoTarget);
            int cartaVisualizzata = Mathf.Min(cartaSmaltita, cartaTarget);

            missionText.text =
                $"Missioni Zona 1:\n" +
                $"Plastica: {plasticaVisualizzata}/{plasticaTarget} {(plasticaCompleta ? "[V]" : "")}\n" +
                $"Organico: {organicoVisualizzato}/{organicoTarget} {(organicoCompleto ? "[V]" : "")}\n" +
                $"Carta: {cartaVisualizzata}/{cartaTarget} {(cartaCompleta ? "[V]" : "")}";
        }
        else if (XPManager.Instance.currentLevel == 2)
        {
            int vetroVisualizzato = Mathf.Min(vetroSmaltito, vetroTarget);
            int metalloVisualizzato = Mathf.Min(metalloSmaltito, metalloTarget);

            missionText.text =
                $"Missioni Zona 2:\n" +
                $"Vetro: {vetroVisualizzato}/{vetroTarget} {(vetroCompleto ? "[V]" : "")}\n" +
                $"Metallo: {metalloVisualizzato}/{metalloTarget} {(metalloCompleto ? "[V]" : "")}\n" +
                $"Cestini: {cestiniSmaltiti}/1";
        }
        else
        {
            int cestiniVisualizzati = Mathf.Min(cestiniSmaltiti, cestiniTarget);
            missionText.text =
                $"Missioni Zona 3:\n" +
                $"Cestini smaltiti: {cestiniVisualizzati}/{cestiniTarget}";
        }
    }
}