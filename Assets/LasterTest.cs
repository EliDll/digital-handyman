using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LasterTest : MonoBehaviour
{
    public float RotYDEG;
    public GameObject[] Planes;
    public bool EnabledPlanes;
    public LaserTool Manager;

    // Start is called before the first frame update
    void Start()
    {
        Manager.Scripts.Add(this);
        EnabledPlanes = false;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Quaternion.Euler(new Vector3(0, RotYDEG, 0));

        foreach(var plane in Planes)
        {
            plane.transform.gameObject.SetActive(EnabledPlanes);
        }
    }
}
