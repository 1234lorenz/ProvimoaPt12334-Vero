using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personaggio : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float defense;


    public float HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp < 0) hp = 0;
        }
    }

    public float Defense
    {
        get { return defense; }
        set
        {
            defense = value;
            if (defense < 0) defense = 0;
        }
    }
}
