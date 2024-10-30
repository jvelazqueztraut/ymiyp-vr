using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMaster : MonoBehaviour
{
    public float delay = 5.0f;
    private float delayTimer;

    public FoodEmittersMaster foodEmitters;
    public float foodEmittersDelay = 1.0F;
    private float foodEmittersTimer;
    private bool enabledFoodEmitters = false;

    private class FoodItem{
        public Rigidbody rb;
        public float delay;
        public float targetDistance;
        public float targetOffset;
        public float spiralPhaseRandom { get; set; }
        public Vector3 randomAngularVel { get; set; }
        public bool isClose { get; set; }
    };

    private GameObject character;

    /*
    private GameObject leftHand, rightHand, leftController, rightController;
    private Vector3 leftHandPos, rightHandPos;
    private Vector3 handsVelocity;
    public float handsVelocityMultiplier = 100;
    */

    private List<FoodItem> foods = new List<FoodItem>();
    private bool foodDropped = false;

    public float farawayDistance = 25;
    public float characterDistanceMax = 0.75F;
    public float characterDistanceMin = 0.5F;
    public float characterOffset = 0.5f;
    public float randomDelay = 2.0f;
    public float minSpeed = 0.1F;
    public float distResponse = 0.25F;

    public float spiralFreq = 0.25F;
    public float spiralSize = 0.05F;

    public float maxAngularVelocity = 1;

    public AudioSource effect;

    // Start is called before the first frame update
    void Start()
    {
        character = GameObject.FindGameObjectWithTag("MainCamera");

        foodEmitters = GameObject.FindGameObjectWithTag("Food Emitters Master").GetComponent<FoodEmittersMaster>();
        foodEmittersTimer = foodEmittersDelay;
        enabledFoodEmitters = false;
        foodDropped = false;

        /*
        leftHand = GameObject.FindGameObjectWithTag("XR Left Hand");
        rightHand = GameObject.FindGameObjectWithTag("XR Right Hand");
        leftController = GameObject.FindGameObjectWithTag("XR Left Controller");
        rightController = GameObject.FindGameObjectWithTag("XR Right Controller");

        if(leftHand && rightHand)
        {
            leftHandPos = leftHand.transform.position;
            rightHandPos = rightHand.transform.position;
        }
        else if(leftController && rightController)
        {
            leftHandPos = leftController.transform.position;
            rightHandPos = rightController.transform.position;
        }
        else
        {
            leftHandPos = new Vector3();
            rightHandPos = new Vector3();
        }
        handsVelocity = new Vector3();
        */

        delayTimer = delay;

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = false;

                Vector3 diff = character.transform.position - rb.position;
                Vector3 norm = diff.normalized;
                rb.transform.position = new Vector3(rb.position.x - farawayDistance * norm.x, character.transform.position.y, rb.position.z - farawayDistance * norm.z);

                FoodItem fi = new FoodItem();
                fi.rb = rb;
                fi.delay = Random.Range(0, randomDelay);
                fi.targetDistance = Random.Range(characterDistanceMin, characterDistanceMax);
                fi.targetOffset = Random.Range(-characterOffset, characterOffset);
                fi.spiralPhaseRandom = Random.Range(-Mathf.PI, Mathf.PI);
                fi.randomAngularVel = new Vector3(
                    Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity)
                );
                fi.isClose = false;

                foods.Add(fi);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 leftHandDiff, rightHandDiff;
        if (leftHand && rightHand)
        {
            leftHandDiff = leftHand.transform.position - leftHandPos;
            rightHandDiff = rightHand.transform.position - rightHandPos;
        }
        else if (leftController && rightController)
        {
            leftHandDiff = leftController.transform.position - leftHandPos;
            rightHandDiff = rightController.transform.position - rightHandPos;
        }
        else
        {
            leftHandDiff = new Vector3();
            rightHandDiff = new Vector3();
        }

        handsVelocity =  leftHandDiff + rightHandDiff;

        leftHandPos += leftHandDiff;
        rightHandPos += rightHandDiff;
        */
    }

    void FixedUpdate()
    {
        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            return;
        }

        if (foodDropped)
        {
            foreach (Transform child in transform)
            {
                if (child.transform.position.y < -1)
                {
                    child.gameObject.SetActive(false);
                }
            }
            return;
        }

        bool allClose = true;
        foreach (FoodItem fi in foods)
        {
            if(fi.delay > 0)
            {
                fi.delay -= Time.deltaTime;
                continue;
            }
            Vector3 target = new Vector3(character.transform.position.x, character.transform.position.y + fi.targetOffset, character.transform.position.z);
            Vector3 diff = target - fi.rb.transform.position;
            float dist = diff.magnitude;
            Vector3 norm = diff.normalized;

            if (!fi.isClose)
            {
                allClose = false;
                if (dist > fi.targetDistance)
                {
                    AttractFood(fi, norm, dist);
                    RotateFood(fi);
                }
                else
                {
                    fi.isClose = true;
                }
            }
            else
            {
                //Add force with velocity squared so the strenght is greater on big hand movements
                //fi.rb.AddForce(handsVelocityMultiplier * handsVelocity.sqrMagnitude * handsVelocity);
            }
        }

        if (!enabledFoodEmitters && allClose)
        {
            if (foodEmittersTimer > 0)
                foodEmittersTimer -= Time.deltaTime;
            else
            {
                foodEmitters.EnableAllEmitters(gameObject);
                enabledFoodEmitters = true;
            }
        }
        /*
        if (enabledFoodEmitters)
        {
            if (checkClosestTimer > 0)
                checkClosestTimer -= Time.deltaTime;
            else
            {
                float closeDist = float.MaxValue;
                FoodItem closest = foods[0];
                foreach (FoodItem fi in foods)
                {
                    Vector3 diff = character.transform.position - fi.rb.transform.position;
                    float dist = diff.magnitude;
                    if (dist < closeDist)
                    {
                        closest = fi;
                        closeDist = dist;
                    }
                }
            }
        }
        */

    }

    void AttractFood(FoodItem fi, Vector3 direction, float dist)
    {
        fi.rb.velocity = direction * (distResponse * dist + minSpeed);
        fi.rb.velocity += new Vector3(spiralSize * Mathf.Cos(Time.time * spiralFreq + fi.spiralPhaseRandom), spiralSize * Mathf.Sin(Time.time * spiralFreq + fi.spiralPhaseRandom), 0);
    }

    void RotateFood(FoodItem fi)
    {
        fi.rb.angularVelocity = fi.randomAngularVel;
    }

    public void DropFood(GameObject obj)
    {
        effect.Play();
        foreach (FoodItem fi in foods)
        {
            fi.rb.useGravity = true;
        }
        foodEmitters.DisableAllEmitters(gameObject);
        foodDropped = true;
    }
}
