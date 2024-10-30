using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsMaster : MonoBehaviour
{
    public LightHandle leftLightHandle;
    public LightHandle rightLightHandle;
    public Light directionalLight;

    public float lightOnIntensity = 1.0f;
    public float lightOffIntensity = 0.0f;
    public Color scOn, ecOn, gcOn;
    public Color scOff, ecOff, gcOff;
    public float lightFadeTime = 1.0F;
    private float lightTimer;
    private bool lightsOn = true;

    // Start is called before the first frame update
    void Start()
    {
        lightsOn = true;
        lightTimer = 0;
        directionalLight.intensity = lightOnIntensity;
        RenderSettings.ambientSkyColor = scOn;
        RenderSettings.ambientEquatorColor = ecOn;
        RenderSettings.ambientGroundColor = gcOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (lightTimer > 0)
        {
            lightTimer -= Time.deltaTime;
            float ratio = lightTimer / lightFadeTime;
            if (lightsOn)
            {
                directionalLight.intensity = Mathf.Lerp(lightOnIntensity, lightOffIntensity, ratio);
                RenderSettings.ambientSkyColor = Color.Lerp(scOn, scOff, ratio);
                RenderSettings.ambientEquatorColor = Color.Lerp(ecOn, ecOff, ratio);
                RenderSettings.ambientGroundColor = Color.Lerp(gcOn, gcOff, ratio);
            }
            else
            {
                directionalLight.intensity = Mathf.Lerp(lightOffIntensity, lightOnIntensity, ratio);
                RenderSettings.ambientSkyColor = Color.Lerp(scOff, scOn, ratio);
                RenderSettings.ambientEquatorColor = Color.Lerp(ecOff, ecOn, ratio);
                RenderSettings.ambientGroundColor = Color.Lerp(gcOff, gcOn, ratio);
            }
        }
    }

    public void TurnOnLights(GameObject obj)
    {
        leftLightHandle.TurnOffLight(gameObject);
        rightLightHandle.TurnOffLight(gameObject);
        lightsOn = true;
        lightTimer = lightFadeTime;
    }

    public void TurnOffLights(GameObject obj)
    {
        leftLightHandle.TurnOnLight(gameObject);
        rightLightHandle.TurnOnLight(gameObject);
        lightsOn = false;
        lightTimer = lightFadeTime;
    }
}
