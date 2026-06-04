using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;
    private const int NUM_SLOTS = 6;
    public int slotDaCaricare = -1;

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

    // salva nello slot scelto
    public void Salva(int slot)
    {
        SaveData data = new SaveData();

        // XP e livello
        data.currentXP = XPManager.Instance.currentXP;
        data.currentLevel = XPManager.Instance.currentLevel;

        // Missioni
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

        // Posizione player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            data.playerX = player.transform.position.x;
            data.playerY = player.transform.position.y;
            data.playerZ = player.transform.position.z;
        }

        // Info slot
        data.dataOra = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        data.slotOccupato = true;

        // salva su file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath + slot + ".dat");
        bf.Serialize(file, data);
        file.Close();

        Debug.Log($"Salvato nello slot {slot}!");
    }

    // carica dallo slot scelto
    public void Carica(int slot)
    {
        string path = savePath + slot + ".dat";
        if (!File.Exists(path)) return;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        SaveData data = (SaveData)bf.Deserialize(file);
        file.Close();

        // applica XP e livello
        XPManager.Instance.currentXP = data.currentXP;
        XPManager.Instance.currentLevel = data.currentLevel;

        // applica missioni
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

        // applica posizione player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);

        Debug.Log($"Caricato slot {slot}!");
    }

    // elimina slot
    public void Elimina(int slot)
    {
        string path = savePath + slot + ".dat";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Slot {slot} eliminato!");
        }
    }

    // controlla se slot è occupato
    public bool SlotOccupato(int slot)
    {
        return File.Exists(savePath + slot + ".dat");
    }

    // legge i dati di uno slot senza caricarli
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