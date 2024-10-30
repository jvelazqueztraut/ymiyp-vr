using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands;

public class LightHandle : MonoBehaviour
{
    public Light spotLight;
    public GameObject controllerAnchor;

    public Handedness handedness;
    XRHandSubsystem handSubsystem;

    private bool suscribed = false;
    private float seekerPeriod = 1.0f;
    private float seekerTimer;

    public float lightOnIntensity = 1.0f;
    public float lightOffIntensity = 0.0f;
    public float lightFadeTime = 1.0F;
    private float lightTimer;
    private bool lightOn = false;

    // Start is called before the first frame update
    void Start()
    {
        suscribed = searchForHands();
        seekerTimer = suscribed ? 0 : seekerPeriod;

        lightOn = false;
        lightTimer = 0;
        spotLight.intensity = lightOffIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!suscribed && seekerTimer > 0)
        {
            seekerTimer -= Time.deltaTime;
            if (seekerTimer < 0)
            {
                suscribed = searchForHands();
                seekerTimer = suscribed ? 0 : seekerPeriod;
            }

        }

        if (lightTimer > 0)
        {
            lightTimer -= Time.deltaTime;
            float ratio = lightTimer / lightFadeTime;
            if(lightOn)
                spotLight.intensity = Mathf.Lerp(lightOnIntensity, lightOffIntensity, ratio);
            else
                spotLight.intensity = Mathf.Lerp(lightOffIntensity, lightOnIntensity, ratio);
        }

        if (lightOn || lightTimer > 0)
        {
            bool useHand = false;
            if (handSubsystem != null)
            {
                XRHand hand = handedness == Handedness.Left ? handSubsystem.leftHand : handSubsystem.rightHand;
                if (hand.isTracked)
                {
                    useHand = true;
                    Pose palm;
                    bool hasPalm = hand.GetJoint(XRHandJointID.Palm).TryGetPose(out palm);
                    if (hasPalm)
                    {
                        transform.position = palm.position;
                        transform.rotation = palm.rotation * Quaternion.Euler(90*Vector3.right);
                    }
                    else
                    {
                        transform.position = hand.rootPose.position + 0.1f*hand.rootPose.forward;
                        transform.rotation = hand.rootPose.rotation * Quaternion.Euler(90 * Vector3.right);
                    }
                }
            }
            if (!useHand && controllerAnchor && controllerAnchor.activeInHierarchy)
            {
                transform.position = controllerAnchor.transform.position;
                transform.rotation = controllerAnchor.transform.rotation;
            }
        }
    }

    bool searchForHands()
    {
        Debug.Log("Looking for hands (" + gameObject.name + ")");
        handSubsystem =
            XRGeneralSettings.Instance?
                .Manager?
                .activeLoader?
                .GetLoadedSubsystem<XRHandSubsystem>();

        if (handSubsystem != null)
        {
            return true;
        }
        return false;
    }

    public void TurnOnLight(GameObject obj)
    {
        handSubsystem =
            XRGeneralSettings.Instance?
                .Manager?
                .activeLoader?
                .GetLoadedSubsystem<XRHandSubsystem>();

        lightOn = true;
        lightTimer = lightFadeTime;
    }

    public void TurnOffLight(GameObject obj)
    {
        lightOn = false;
        lightTimer = lightFadeTime;
    }
}
