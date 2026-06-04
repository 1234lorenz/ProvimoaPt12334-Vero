using UnityEngine;
using System.Collections.Generic;

public class RifiutiManager : MonoBehaviour
{
    public static RifiutiManager Instance;

    private List<string> rifiutiRaccolti = new List<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegistraRaccolta(string id)
    {
        if (!rifiutiRaccolti.Contains(id))
            rifiutiRaccolti.Add(id);
    }

    public List<string> GetRifiutiRaccolti()
    {
        return rifiutiRaccolti;
    }

    public void ApplicaRifiutiRaccolti(List<string> ids)
    {
        rifiutiRaccolti = ids;

        RIFIUTI[] tuttiRifiuti = FindObjectsOfType<RIFIUTI>();
        foreach (var rifiuto in tuttiRifiuti)
        {
            if (rifiutiRaccolti.Contains(rifiuto.rifiutoID))
                rifiuto.gameObject.SetActive(false);
        }
    }
}