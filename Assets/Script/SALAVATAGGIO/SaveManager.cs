using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;
    private const int NUM_SLOTS = 6;
    public int slotDaCaricare = -1;

    [HideInInspector] public SaveData datiCaricati;

    // seed dei rifiuti della partita in corso: lo imposta GeneratoreZona
    [HideInInspector] public int seedRifiuti;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        savePath = Application.persistentDataPath + "/save";
    }

    public void Salva(int slot)
    {
        SaveData data = new SaveData();

        data.currentXP = XPManager.Instance.currentXP;
        data.currentLevel = XPManager.Instance.currentLevel;

        data.plasticaSmaltite = MissionManager.Instance.plasticaSmaltite;
        data.organicoSmaltiti = MissionManager.Instance.organicoSmaltiti;
        data.cartaSmaltita = MissionManager.Instance.cartaSmaltita;
        data.plasticaCompleta = MissionManager.Instance.plasticaCompleta;
        data.organicoCompleto = MissionManager.Instance.organicoCompleto;
        data.cartaCompleta = MissionManager.Instance.cartaCompleta;
        data.vetroSmaltito = MissionManager.Instance.vetroSmaltito;
        data.metalloSmaltito = MissionManager.Instance.metalloSmaltito;
        data.vetroCompleto = MissionManager.Instance.vetroCompleto;
        data.metalloCompleto = MissionManager.Instance.metalloCompleto;
        data.cestiniSmaltiti = MissionManager.Instance.cestiniSmaltiti;
        data.seccoSmaltito = MissionManager.Instance.seccoSmaltito;
        data.seccoCompleto = MissionManager.Instance.seccoCompleto;

        if (TutorialManager.Instance != null)
        {
            data.tutorialCompleto = (TutorialManager.Instance.faseAttuale == TutorialManager.Fase.Finito);
        }
        else
        {
            data.tutorialCompleto = true;
        }

        data.rifiutiRaccolti = RifiutiManager.Instance.GetRifiutiRaccolti();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
            data.playerZ = player.transform.position.z;
        }

        CounterMenagerRifiuti counter = FindObjectOfType<CounterMenagerRifiuti>();
        if (counter != null)
        {
            data.quantitaZaino = new List<int>();
            foreach (var slotRifiuto in counter.slots)
            {
                data.quantitaZaino.Add(slotRifiuto.currentCount);
            }
        }

        // SEED dei rifiuti: serve a rigenerare la stessa mappa quando carichi
        data.seedRifiuti = seedRifiuti;

        // BARRIERE CON MINIGIOCO: salviamo i nomi di quelle gia' sbloccate
        data.barriereSbloccate = new List<string>();
        foreach (var gate in FindObjectsOfType<GateMinigameTrigger>(true))
        {
            if (gate.IsUnlocked) data.barriereSbloccate.Add(gate.GateId);
        }

        data.dataOra = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        data.slotOccupato = true;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath + slot + ".dat");
        bf.Serialize(file, data);
        file.Close();
        Debug.Log($"Salvato nello slot {slot}!");
    }

    public void Carica(int slot)
    {
        string path = savePath + slot + ".dat";
        if (!File.Exists(path)) return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();

        datiCaricati = data;

        // 1. CARICHIAMO LIVELLO E XP
        XPManager.Instance.currentXP = data.currentXP;
        XPManager.Instance.currentLevel = data.currentLevel;

        // 2. CARICHIAMO TUTTE LE MISSIONI PRIMA DI OGNI ALTRA COSA
        MissionManager.Instance.plasticaSmaltite = data.plasticaSmaltite;
        MissionManager.Instance.organicoSmaltiti = data.organicoSmaltiti;
        MissionManager.Instance.cartaSmaltita = data.cartaSmaltita;
        MissionManager.Instance.plasticaCompleta = data.plasticaCompleta;
        MissionManager.Instance.organicoCompleto = data.organicoCompleto;
        MissionManager.Instance.cartaCompleta = data.cartaCompleta;
        MissionManager.Instance.vetroSmaltito = data.vetroSmaltito;
        MissionManager.Instance.metalloSmaltito = data.metalloSmaltito;
        MissionManager.Instance.vetroCompleto = data.vetroCompleto;
        MissionManager.Instance.metalloCompleto = data.metalloCompleto;
        MissionManager.Instance.cestiniSmaltiti = data.cestiniSmaltiti;
        MissionManager.Instance.seccoSmaltito = data.seccoSmaltito;
        MissionManager.Instance.seccoCompleto = data.seccoCompleto;

        // 3. ORA APPLICHIAMO IL LIVELLO (Così XPManager legge che le missioni sono complete e imposta giocoFinito = true)
        XPManager.Instance.ApplicaLivelloCaricato();
        MissionManager.Instance.AggiornaMissioniUI();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 posizioneSalvata = new Vector3(data.playerX, data.playerY, data.playerZ);

            FirstPersonController fpc = player.GetComponent<FirstPersonController>();
            if (fpc != null)
            {
                fpc.Teletrasporta(posizioneSalvata);
            }
            else
            {
                player.transform.position = posizioneSalvata;
            }
        }

        CounterMenagerRifiuti counter = FindObjectOfType<CounterMenagerRifiuti>();
        if (counter != null && data.quantitaZaino != null)
        {
            for (int i = 0; i < counter.slots.Length && i < data.quantitaZaino.Count; i++)
            {
                counter.slots[i].currentCount = data.quantitaZaino[i];
            }
            counter.AggiornaTuttoUI();
        }

        RifiutiManager.Instance.ApplicaRifiutiRaccolti(data.rifiutiRaccolti);

        // BARRIERE CON MINIGIOCO: nei salvataggi vecchi la lista non esiste (e' null)
        if (data.barriereSbloccate != null)
        {
            foreach (var gate in FindObjectsOfType<GateMinigameTrigger>(true))
            {
                if (data.barriereSbloccate.Contains(gate.GateId)) gate.ForceUnlock();
            }
        }

        Debug.Log($"Caricato slot {slot}!");
    }

    public void Elimina(int slot)
    {
        string path = savePath + slot + ".dat";
        if (File.Exists(path)) File.Delete(path);
    }

    public bool SlotOccupato(int slot)
    {
        return File.Exists(savePath + slot + ".dat");
    }

    public SaveData LeggiSlot(int slot)
    {
        string path = savePath + slot + ".dat";
        if (!File.Exists(path)) return null;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();
        return data;
    }
}