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
    public int seccoSmaltito = 0;
    public int seccoTarget = 10;
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
    public int xpBonusSecco = 50;
    public int xpBonusCestino = 60;

    // traccia missioni già completate per non dare bonus due volte
    public bool plasticaCompleta = false;
    public bool organicoCompleto = false;
    public bool cartaCompleta = false;
    public bool vetroCompleto = false;
    public bool metalloCompleto = false;
    public bool seccoCompleto = false;

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

    public int GetMancanti(RIFIUTIType type)
    {
        switch (type)
        {
            case RIFIUTIType.Plastica: return plasticaCompleta ? 0 : Mathf.Max(0, plasticaTarget - plasticaSmaltite);
            case RIFIUTIType.Organico: return organicoCompleto ? 0 : Mathf.Max(0, organicoTarget - organicoSmaltiti);
            case RIFIUTIType.Carta: return cartaCompleta ? 0 : Mathf.Max(0, cartaTarget - cartaSmaltita);
            case RIFIUTIType.Vetro: return vetroCompleto ? 0 : Mathf.Max(0, vetroTarget - vetroSmaltito);
            case RIFIUTIType.Metallo: return metalloCompleto ? 0 : Mathf.Max(0, metalloTarget - metalloSmaltito);
            case RIFIUTIType.Secco: return seccoCompleto ? 0 : Mathf.Max(0, seccoTarget - seccoSmaltito);

            default: return 0;
        }
    }

    public void OnSmaltimento(RIFIUTIType type, int quantity)
    {
        XPManager.Instance.AddXP(xpPerOggetto * quantity);

        switch (type)
        {
            case RIFIUTIType.Plastica:
                plasticaSmaltite += quantity;
                if (!plasticaCompleta && plasticaSmaltite >= plasticaTarget)
                {
                    plasticaCompleta = true;
                    XPManager.Instance.AddXP(xpBonusPlastica);
                }
                break;

            case RIFIUTIType.Organico:
                organicoSmaltiti += quantity;
                if (!organicoCompleto && organicoSmaltiti >= organicoTarget)
                {
                    organicoCompleto = true;
                    XPManager.Instance.AddXP(xpBonusOrganico);
                }
                break;

            case RIFIUTIType.Carta:
                cartaSmaltita += quantity;
                if (!cartaCompleta && cartaSmaltita >= cartaTarget)
                {
                    cartaCompleta = true;
                    XPManager.Instance.AddXP(xpBonusCarta);
                }
                break;

            case RIFIUTIType.Vetro:
                vetroSmaltito += quantity;
                if (!vetroCompleto && vetroSmaltito >= vetroTarget)
                {
                    vetroCompleto = true;
                    XPManager.Instance.AddXP(xpBonusVetro);
                }
                break;

            case RIFIUTIType.Metallo:
                metalloSmaltito += quantity;
                if (!metalloCompleto && metalloSmaltito >= metalloTarget)
                {
                    metalloCompleto = true;
                    XPManager.Instance.AddXP(xpBonusMetallo);
                }
                break;

            case RIFIUTIType.Secco:
                seccoSmaltito += quantity;
                if (!seccoCompleto && seccoSmaltito >= seccoTarget)
                {
                    seccoCompleto = true;
                    XPManager.Instance.AddXP(xpBonusSecco);
                }
                break;
        }

        UpdateMissionUI();
    }

    public void OnRaccolta()
    {
    }

    public void OnCestinoSmaltito()
    {
        // Determina il target massimo di cestini in base al livello attuale
        int targetCestiniLivello = (XPManager.Instance.currentLevel == 2) ? 1 : cestiniTarget;

        // Aumenta il conteggio e assegna XP SOLO se non abbiamo ancora raggiunto il target
        if (cestiniSmaltiti < targetCestiniLivello)
        {
            cestiniSmaltiti++;
            XPManager.Instance.AddXP(xpBonusCestino);
        }

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
        return cestiniSmaltiti >= cestiniTarget && seccoCompleto;
    }

    public void ResetMissioni()
    {
        plasticaSmaltite = 0; organicoSmaltiti = 0; cartaSmaltita = 0;
        vetroSmaltito = 0; metalloSmaltito = 0; cestiniSmaltiti = 0; seccoSmaltito = 0;

        plasticaCompleta = false; organicoCompleto = false;
        cartaCompleta = false; vetroCompleto = false; metalloCompleto = false; seccoCompleto = false;

        UpdateMissionUI();
    }

    private void UpdateMissionUI()
    {
        if (missionText == null) return;

        if (XPManager.Instance.currentLevel == 1)
        {
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
            int cestiniVisualizzati = Mathf.Min(cestiniSmaltiti, 1); // Blocca graficamente a 1

            missionText.text =
                $"Missioni Zona 2:\n" +
                $"Vetro: {vetroVisualizzato}/{vetroTarget} {(vetroCompleto ? "[V]" : "")}\n" +
                $"Metallo: {metalloVisualizzato}/{metalloTarget} {(metalloCompleto ? "[V]" : "")}\n" +
                $"Cestini: {cestiniVisualizzati}/1 {(cestiniSmaltiti >= 1 ? "[V]" : "")}";
        }
        else
        {
            int cestiniVisualizzati = Mathf.Min(cestiniSmaltiti, cestiniTarget);
            int seccoVisualizzato = Mathf.Min(seccoSmaltito, seccoTarget);

            missionText.text =
                $"Missioni Zona 3:\n" +
                $"Secco: {seccoVisualizzato}/{seccoTarget} {(seccoCompleto ? "[V]" : "")}\n" +
                $"Cestini smaltiti: {cestiniVisualizzati}/{cestiniTarget} {(cestiniSmaltiti >= cestiniTarget ? "[V]" : "")}";
        }
    }
}