using UnityEngine;



public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        DynamicGI.UpdateEnvironment();
        RenderSettings.skybox = RenderSettings.skybox;
    }
}
