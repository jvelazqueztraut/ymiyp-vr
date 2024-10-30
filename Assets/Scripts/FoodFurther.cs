using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodFurther : MonoBehaviour
{
    private class FoodItem
    {
        public Rigidbody rb;
        public float spiralPhaseRandom { get; set; }
        public Vector3 randomAngularVel { get; set; }
        public bool isClose { get; set; }
    };

    private GameObject character;
    public float secondsToStart = 1.0f;
    private float startTimer;
    private bool started = false;

    private List<FoodItem> foods = new List<FoodItem>();
    private bool foodDropped = false;

    public float farawayDistance = 25;
    public float characterDistance = 0.75F;
    public float minSpeed = 0.1F;
    private float distResponse = 0.1F;

    public float spiralFreq = 0.25F;
    public float spiralSize = 0.05F;

    public float maxAngularVelocity = 1;

    // Start is called before the first frame update
    void Start()
    {
        character = GameObject.FindGameObjectWithTag("MainCamera");
        foodDropped = false;

        //Find all child obj and store to that array
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.gameObject.SetActive(true);
                rb.isKinematic = false;
                rb.useGravity = false;

                Vector3 diff = character.transform.position - rb.position;
                Vector3 norm = diff.normalized;
                rb.transform.position = new Vector3(rb.position.x - farawayDistance * norm.x, character.transform.position.y, rb.position.z - farawayDistance * norm.z);

                FoodItem fi = new FoodItem();
                fi.rb = rb;
                fi.spiralPhaseRandom = Random.Range(-Mathf.PI, Mathf.PI);
                fi.randomAngularVel = new Vector3(
                    Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity)
                );
                fi.isClose = false;

                foods.Add(fi);
                rb.gameObject.SetActive(false);
            }
        }

        startTimer = secondsToStart;
        started = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (startTimer > 0)
        {
            startTimer -= Time.deltaTime;
            return;
        }
        if(!started)
        {
            foreach (FoodItem fi in foods)
                fi.rb.gameObject.SetActive(true);
            started = true;
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

        foreach (FoodItem fi in foods)
        {
            Vector3 diff = character.transform.position - fi.rb.transform.position;
            float dist = diff.magnitude;
            Vector3 norm = diff.normalized;
            if (!fi.isClose)
            {
                if (dist > characterDistance)
                {
                    AttractFood(fi, norm, dist);
                    RotateFood(fi);
                }
                else
                {
                    fi.isClose = true;
                }
            }
        }
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
        foreach (FoodItem fi in foods)
        {
            fi.rb.useGravity = true;
        }
        foodDropped = true;
    }
}
