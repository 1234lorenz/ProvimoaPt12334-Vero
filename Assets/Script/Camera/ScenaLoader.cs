using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenaLoader : MonoBehaviour
{
    void Start()
    {
        // forza il refresh della skybox quando la scena viene caricata
        DynamicGI.UpdateEnvironment();
        RenderSettings.skybox = RenderSettings.skybox;
    }
}
