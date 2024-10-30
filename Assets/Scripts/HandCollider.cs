using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Hands;

[RequireComponent(typeof(Collider))]
public class HandCollider : MonoBehaviour
{
    public GameObject controllerAnchor;

    XRHandSubsystem handSubsystem;
    public Handedness handedness;

    private bool suscribed = false;
    private float seekerPeriod = 1.0f;
    private float seekerTimer;

    // Start is called before the first frame update
    void Start()
    {
        suscribed = searchForHands();
        seekerTimer = suscribed ? 0 : seekerPeriod;
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
                    transform.rotation = palm.rotation;
                }
                else
                {
                    transform.position = hand.rootPose.position + 0.1f * hand.rootPose.forward;
                    transform.rotation = hand.rootPose.rotation;
                }
            }
        }

        if (!useHand && controllerAnchor && controllerAnchor.activeInHierarchy)
        {
            transform.position = controllerAnchor.transform.position;
            transform.rotation = controllerAnchor.transform.rotation;
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

        if (handSubsystem != null) {
            //handSubsystem.updatedHands += OnHandUpdate;
            return true;
        }
        return false;
    }
}
