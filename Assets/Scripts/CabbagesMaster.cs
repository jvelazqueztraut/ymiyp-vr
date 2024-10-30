using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CabbagesMaster : MonoBehaviour
{
    private class FoodItem
    {
        public Rigidbody rb;
        public bool soundPlayed;
        public Vector3 randomAngularVel { get; set; }
        public bool isClose { get; set; }
    };

    private GameObject character;
    public StaticMaster staticMaster;
    public FoodMaster foodMaster;
    private bool hasDroppedFood = false;
    public LightsMaster lightsMaster;
    private bool hasTurnedOffLights = false;
    public SoundMaster soundMaster;
    private bool hasTurnedOffMusic = false;
    private bool hasAccelerated = false;
    private bool hasDeactivatedOthers = false;
    public float dissolveDelay = 10.0F;
    public float dissolveFadeTime = 5.0F;
    private float dissolveDelayTimer, dissolveFadeTimer;
    private bool hasDissolved = false;

    private List<FoodItem> fullCabbages = new List<FoodItem>();
    private List<FoodItem> halfCabbages = new List<FoodItem>();
    private List<FoodItem> greenCabbages = new List<FoodItem>();

    public float greenCabbageDelay = 5.0f;
    private float greenCabbageTimer;
    public Material greenCabbageMaterial0, greenCabbageMaterial1;

    public float farawayDistance = 100;
    public float farawayHeight = 25;
    public float characterDistance = 5F;
    public float halfCabbageThresholdDistance = 2;
    public float halfCabbageTargetDistance = 2;
    public float greenCabbageThresholdDistance = 2;
    public float greenCabbageTargetDistance = 1;
    public float minSpeed = 0.05F;
    private float distResponse = 0.3F;

    public float halfCabbageInitSize = 1.0f;
    public float halfCabbageTargetSize = 1.5f;

    public float maxAngularVelocity = 0.1F;
    public float maxRadiansSpeed = 10;

    public int secondsToStart = 50;
    private float startTimer;
    public int secondsToEnd = 5;
    private float endTimer;
    private bool started = false;
    private bool ended = false;

    public ResetGame resetGame;

    // Start is called before the first frame update
    void Start()
    {
        resetGame = GameObject.FindGameObjectWithTag("XR").GetComponent<ResetGame>();
        character = GameObject.FindGameObjectWithTag("MainCamera");
        staticMaster = GameObject.FindGameObjectWithTag("Static Master").GetComponent<StaticMaster>();
        foodMaster = GameObject.FindGameObjectWithTag("Food Master").GetComponent<FoodMaster>();
        lightsMaster = GameObject.FindGameObjectWithTag("Lights Master").GetComponent<LightsMaster>();
        soundMaster = GameObject.FindGameObjectWithTag("Sound Master").GetComponent<SoundMaster>();
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
                rb.transform.position = new Vector3(rb.position.x - farawayDistance * norm.x, character.transform.position.y + farawayHeight, rb.position.z - farawayDistance * norm.z);

                rb.transform.localScale = halfCabbageInitSize * Vector3.one;

                FoodItem fi = new FoodItem();
                fi.rb = rb;
                fi.soundPlayed = false;
                fi.randomAngularVel = new Vector3(
                    0,//maxAngularVelocity * 0.1F,//Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    0,//maxAngularVelocity * 0.1F,//Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity)
                );
                fi.isClose = false;

                switch( child.tag )
                {
                    case "Green Cabbage":
                        greenCabbages.Add(fi);
                        break;
                    case "Half Cabbage":
                        halfCabbages.Add(fi);
                        break;
                    default:
                        fullCabbages.Add(fi);
                        break;
                }
                rb.gameObject.SetActive(false);
            }
        }

        //greenCabbageMaterial0.SetFloat("_Clipping_value", 0.0f);
        //greenCabbageMaterial1.SetFloat("_Clipping_value", 0.0f);

        started = false;
        startTimer = secondsToStart;
        endTimer = secondsToEnd;
        ended = false;
        hasDroppedFood = false;
        hasTurnedOffLights = false;
        hasTurnedOffMusic = false;
        hasAccelerated = false;
        hasDeactivatedOthers = false;
        greenCabbageTimer = greenCabbageDelay;
        dissolveDelayTimer = dissolveDelay;
        dissolveFadeTimer = dissolveFadeTime;
        hasDissolved = false;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if (startTimer > 0)
        {
            startTimer -= Time.deltaTime;
            return;
        }
        if(!started)
        {
            foreach (FoodItem fi in fullCabbages.Concat(halfCabbages))
            {
                fi.rb.gameObject.SetActive(true);
            }
            started = true;
        }
        // Full cabbages
        bool allFCClose = true;
        foreach (FoodItem fc in fullCabbages)
        {
            if(!fc.isClose)
            {
                allFCClose = false;

                Vector3 diff = character.transform.position - fc.rb.transform.position;
                float dist = diff.magnitude;

                Vector3 norm = diff.normalized;
                if (dist > characterDistance)
                {
                    AttractFood(fc, norm, dist, minSpeed);
                    RotateFood(fc);
                }
                else
                {
                    fc.isClose = true;
                }
            }
        }
        //if (!hasDroppedFood && allFCClose)
        //{
        //    foodMaster.DropFood(gameObject);
        //    hasDroppedFood = true;
        //}

        // Half cabbages
        bool allHCClose = true;
        foreach (FoodItem hc in halfCabbages)
        {
            Vector3 diff = character.transform.position - hc.rb.transform.position;
            Vector3 norm = diff.normalized;
            float dist = diff.magnitude;

            if (!hc.isClose)
            {
                allHCClose = false;
                if (dist > characterDistance)
                {
                    AttractFood(hc, norm, dist, minSpeed);
                    RotateFood(hc);
                }
                else if (dist > halfCabbageTargetDistance)
                {
                    if (!hc.soundPlayed)
                    {
                        AudioSource sound = hc.rb.gameObject.GetComponent<AudioSource>();
                        if (sound)
                            sound.Play();
                        hc.soundPlayed = true;
                    }
                    AttractFood(hc, norm, 0, minSpeed);
                    RotateFoodTowards(hc, norm);
                    SetFoodScale(hc,Mathf.Lerp(halfCabbageTargetSize, halfCabbageInitSize, (dist - halfCabbageTargetDistance)/(characterDistance - halfCabbageTargetDistance)));
                }
                else
                {
                    hc.isClose = true;
                }
            }
        }
        if(!hasAccelerated && allHCClose)
        {
            minSpeed *= 2;
            hasAccelerated = true;
        }


        // Green cabbages
        if (greenCabbageTimer > 0)
        {
            greenCabbageTimer -= Time.deltaTime;
            if (greenCabbageTimer < 1.0f && !hasDroppedFood) // Drop food 1 sec before starting -- OPTIMISATION
            {
                foodMaster.DropFood(gameObject);
                hasDroppedFood = true;
            }
            if(greenCabbageTimer < 0)
            {
                foreach (FoodItem fi in greenCabbages)
                {
                    fi.rb.gameObject.SetActive(true);
                }
            }
            return;
        }

        bool allGCClose = true;
        bool allGCPastThreshold = true;
        foreach (FoodItem gc in greenCabbages)
        {
            Vector3 diff = character.transform.position - gc.rb.transform.position;
            Vector3 norm = diff.normalized;
            float dist = diff.magnitude;

            if (!gc.isClose)
            {
                allGCClose = false;
                if (dist > greenCabbageThresholdDistance)
                    allGCPastThreshold = false;

                if (dist > characterDistance)
                {
                    AttractFood(gc, norm, dist, minSpeed);
                    RotateFood(gc);
                }
                else if (dist > greenCabbageTargetDistance)
                {
                    AttractFood(gc, norm, 0, minSpeed);
                    RotateFoodTowards(gc, norm);
                }
                else
                {
                    gc.isClose = true;
                }
            }
        }

        if (!hasTurnedOffMusic && allGCPastThreshold)
        {
            soundMaster.TurnOnFilter(gameObject);
            hasTurnedOffMusic = true;
        }

        if (!hasTurnedOffLights && allGCPastThreshold)
        {
            lightsMaster.TurnOffLights(gameObject);
            dissolveDelayTimer = dissolveDelay;
            dissolveFadeTimer = dissolveFadeTime;
            hasTurnedOffLights = true;
        }

        if(!hasDeactivatedOthers && allGCClose)
        {
            foreach (FoodItem fi in fullCabbages.Concat(halfCabbages))
            {
                fi.rb.gameObject.SetActive(false);
            }
            Vector3 avgPos = Vector3.zero;
            foreach (FoodItem gc in greenCabbages)
            {
                avgPos += gc.rb.transform.position;
            }
            if(greenCabbages.Count() > 0)
                avgPos /= greenCabbages.Count();
            staticMaster.SetRoomOrigin(new Vector3(avgPos.x, 0, avgPos.z));
            staticMaster.SwitchEnvironment(gameObject);
            hasDeactivatedOthers = true;
        }

        if (!hasDissolved && hasTurnedOffLights)
        {
            if(dissolveDelayTimer > 0)
            {
                dissolveDelayTimer -= Time.deltaTime;
                if (dissolveDelayTimer < 0) { 
                    lightsMaster.TurnOnLights(gameObject);
                    soundMaster.TurnOffFilter(gameObject);
                }
            }
            else
            {
                if (dissolveFadeTimer > 0)
                {
                    float ratio = dissolveFadeTimer / dissolveFadeTime;
                    //greenCabbageMaterial0.SetFloat("_Clipping_value", 1 - ratio);
                    //greenCabbageMaterial1.SetFloat("_Clipping_value", 1 - ratio);

                    foreach (FoodItem gc in greenCabbages)
                    {
                        Vector3 pos = gc.rb.transform.position;
                        
                        AttractFood(gc, Vector3.down, 0, minSpeed*2.5f);
                        RotateFoodTowards(gc, Vector3.up);

                        gc.rb.gameObject.GetComponentInChildren<AudioSource>().volume = ratio;
                    }
                    dissolveFadeTimer -= Time.deltaTime;
                }
                else
                {
                    hasDissolved = true;
                }
            }
        }

        if (!ended && hasDissolved)
        {
            if (endTimer > 0)
                endTimer -= Time.deltaTime;
            else
            {
                soundMaster.TurnOffMusic(gameObject);
                resetGame.Ending(gameObject);
                ended = true;
            }
        }

    }

    void AttractFood(FoodItem fi, Vector3 direction, float dist, float speed)
    {
        fi.rb.velocity = direction * (distResponse * dist + speed);
    }

    void RotateFood(FoodItem fi)
    {
        fi.rb.angularVelocity = fi.randomAngularVel;
    }

    void RotateFoodTowards(FoodItem fi, Vector3 direction)
    {
        //create the rotation we need to be in to look at the target
        Quaternion lookRotation = Quaternion.LookRotation(-direction);

        //rotate us over time according to speed until we are in the required rotation
        fi.rb.MoveRotation(Quaternion.RotateTowards(fi.rb.transform.rotation, lookRotation, Time.deltaTime * maxRadiansSpeed));
    }

    void SetFoodScale(FoodItem fi, float scale)
    {
        fi.rb.transform.localScale = scale * Vector3.one;
    }
}
