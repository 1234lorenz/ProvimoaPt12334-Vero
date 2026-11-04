using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class SaveData
{
    public int currentXP;
    public int currentLevel;
    public int plasticaSmaltite;
    public int organicoSmaltiti;
    public int cartaSmaltita;
    public int seccoSmaltito;
    public bool plasticaCompleta;
    public bool organicoCompleto;
    public bool cartaCompleta;
    public bool seccoCompleto;
    public int vetroSmaltito;
    public int metalloSmaltito;
    public bool vetroCompleto;
    public bool metalloCompleto;
    public int cestiniSmaltiti;
    public float playerX;
    public float playerY;
    public float playerZ;
    public string dataOra;
    public bool slotOccupato;
    public bool tutorialCompleto = false;

    // lista ID rifiuti raccolti
    public List<string> rifiutiRaccolti = new List<string>();
    public List<int> quantitaZaino = new List<int>();

    // nomi delle barriere con minigioco gia' sbloccate
    // [OptionalField] serve perche' i salvataggi vecchi non hanno questo campo
    [OptionalField] public List<string> barriereSbloccate = new List<string>();

    // seed con cui sono stati generati i rifiuti: serve a rigenerare la stessa mappa
    [OptionalField] public int seedRifiuti;
}