using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSheke : MonoBehaviour
{
    [SerializeField] private float Durata_shake;
    [SerializeField] private float intensita_shake;



    public IEnumerator Shake()
    {
        Vector3 position = transform.localPosition;
        float Tempo_partenza = Time.unscaledTime;

        while (Time.unscaledTime - Tempo_partenza < Durata_shake)
        {
            transform.localPosition = position + new Vector3(
                Random.Range(-intensita_shake, intensita_shake),
                Random.Range(-intensita_shake, intensita_shake),
                0);
            yield return null;
        }

        transform.localPosition = position;
    }

}



