using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEmittersMaster : MonoBehaviour
{
    //private List<ParticleSystem> emitters = new List<ParticleSystem>();
    public ParticleSystem ps;
    public AudioSource initial, loop, end;
    // Start is called before the first frame update
    void Start()
    {
        ps.gameObject.SetActive(false);
        //Find all child obj and store to that array
        /*foreach (Transform child in transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (ps)
            {
                ps.gameObject.SetActive(false);
                emitters.Add(ps);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableAllEmitters(GameObject gameObject)
    {
        ps.gameObject.SetActive(true);
        end.Stop();
        initial.Play();
        loop.PlayDelayed(initial.clip.length);
        /*foreach (ParticleSystem ps in emitters)
        {
            ps.gameObject.SetActive(true);
        }*/
    }

    public void DisableAllEmitters(GameObject gameObject)
    {
        initial.Stop();
        loop.Stop();
        end.Play();
        ps.gameObject.SetActive(false);
        /*foreach (ParticleSystem ps in emitters)
        {
            ps.gameObject.SetActive(false);
        }*/
    }
}
