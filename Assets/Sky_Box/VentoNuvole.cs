using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentoNuvole : MonoBehaviour
{
    public float velocitaVento = 0.02f;
    private float offset = 0f;

    void Update()
    {
        // Sposta solo l'orizzontale della texture, non ruota l'oggetto!
        offset += velocitaVento * Time.deltaTime;
        RenderSettings.skybox.SetFloat("_Rotation", offset * 100f);
    }
}
