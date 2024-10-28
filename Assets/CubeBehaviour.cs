using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviour : MonoBehaviour
{
    public float xAngleOffset;
    public float yAngleOffset;
    public float zAngleOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.Rotate(xAngleOffset, yAngleOffset, zAngleOffset);
    }
}
