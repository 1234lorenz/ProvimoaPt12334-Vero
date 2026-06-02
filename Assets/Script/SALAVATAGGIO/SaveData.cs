using System;

[Serializable]
public class SaveData
{
    // XP e livello
    public int currentXP;
    public int currentLevel;

    // Missioni livello 1
    public int plasticaSmaltite;
    public int organicoSmaltiti;
    public int cartaSmaltita;
    public bool plasticaCompleta;
    public bool organicoCompleto;
    public bool cartaCompleta;

    // Missioni livello 2
    public int vetroSmaltito;
    public int metalloSmaltito;
    public bool vetroCompleto;
    public bool metalloCompleto;

    // Missioni livello 3
    public int cestiniSmaltiti;

    // Posizione player
    public float playerX;
    public float playerY;
    public float playerZ;

    // Info slot
    public string dataOra;
    public bool slotOccupato;
}