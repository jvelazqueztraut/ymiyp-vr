using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodMovement : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject character;
    public float farawayDistance = 25;
    public float characterDistance = 0.75F;
    public float minSpeed = 0.1F;
    private float distResponse = 0.1F;
    public float spiralFreq = 0.25F;
    public float spiralSize = 0.05F;
    private float spiralPhaseRandom;
    public float maxAngularVelocity = 1;
    private Vector3 randomAngularVel;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();

        Vector3 diff = character.transform.position - transform.position;
        Vector3 norm = diff.normalized;
        //rb.MovePosition(new Vector3(transform.position.x - farawayDistance * norm.x, transform.position.y , transform.position.z - farawayDistance * norm.z));

        spiralPhaseRandom = Random.Range(-Mathf.PI,Mathf.PI);

        randomAngularVel = new Vector3(
            Random.Range(-maxAngularVelocity, maxAngularVelocity),
            Random.Range(-maxAngularVelocity, maxAngularVelocity),
            Random.Range(-maxAngularVelocity, maxAngularVelocity)
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 diff = character.transform.position - transform.position;
        float dist = diff.magnitude;
        Vector3 norm = diff.normalized;
        //Debug.Log("Dist:" + dist);
        if ( dist > characterDistance)
        {
            //AttractFood(norm, dist);
        }

    }

    void AttractFood(Vector3 direction, float dist)
    {
        rb.velocity = direction * (distResponse * dist + minSpeed);
        rb.velocity += new Vector3(spiralSize * Mathf.Cos(Time.time * spiralFreq + spiralPhaseRandom), spiralSize * Mathf.Sin(Time.time * spiralFreq + spiralPhaseRandom), 0);
        rb.angularVelocity = randomAngularVel;
    }
}
