using System;
using UnityEngine;

public class ShineLogic : MonoBehaviour
{
    Material material1;
    Material material2;
    
    Color color;

    float t = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnEnable()
    {
        material1 = GetComponent<Renderer>().materials[0];
        material2 = GetComponent<Renderer>().materials[1];
        color = material1.color;
        material1.SetColor("_EmissionColor", color);
        material2.SetColor("_EmissionColor", color);
        t = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;

        material1.SetColor("_EmissionColor", color * t);
        material2.SetColor("_EmissionColor", color * t);
        
        if (t <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }
}
