using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMaster : MonoBehaviour
{
    public GameObject room;
    public GameObject floor;

    // Start is called before the first frame update
    void Start()
    {
        room.SetActive(false);
        floor.SetActive(true);
    }

    public void SetRoomOrigin(Vector3 origin)
    {
        room.transform.position = origin;
    }

    public void SwitchEnvironment(GameObject obj)
    {
        room.SetActive(true);
        floor.SetActive(false);
    }
}
