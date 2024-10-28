using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using HandPoints = ReadHands.HandPoints;

public class MultiStickBehaviour : MonoBehaviour
{
    public GameObject LeftAnchor;
    public GameObject RightAnchor;
    public GameObject CenterPoint;
    public LineRenderer Line;
    public TextMeshPro DistanceText;
    public TextMeshPro AngleText;
    public float yOffset;
    public Transform XRigTransform;
    public MultiStickManagerBehaviour manager;
    public float CurrentDistance;

    public ReadHands Hands;

    public bool LeftFixed;
    public bool RightFixed;

    private static Vector3 ScaleVec(Vector3 vec, double scalar)
    {
        var f = (float)scalar;
        return new Vector3(vec.x * f, vec.y * f, vec.z * f);
    }

    private Vector3 GetPos(HandPoints finger, GameObject anchor, bool isFixed)
    {
        if (isFixed)
        {
            return anchor.transform.position;
        }
        else
        {
            var pos = Hands.getFingerPosition(finger);
            //Update anchor position to device
            anchor.transform.position = pos;
            return pos;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LeftFixed = false;
        RightFixed = false;

        //Register to manager
        manager.CurrentSticks.Add(this);

        Update();
    }

    // Update is called once per frame
    void Update()
    {
        if(!LeftFixed || !RightFixed)
        {

            var leftPos = GetPos(HandPoints.L_INDEX, LeftAnchor, LeftFixed);
            var rightPos = GetPos(HandPoints.R_INDEX, RightAnchor, RightFixed);

            //If right fixed, protract tape from right anchor (start line render at right pos)
            if (RightFixed)
            {
                Line.SetPosition(0, rightPos);
                Line.SetPosition(1, leftPos);
            }
            else
            {
                Line.SetPosition(0, leftPos);
                Line.SetPosition(1, rightPos);
            }

            var vec = leftPos - rightPos;
            var center = leftPos - ScaleVec(vec, 0.5);
            var offset = new Vector3(0, yOffset, 0);
            var distance = vec.magnitude;

            //Update public member variable
            CurrentDistance = distance;

            DistanceText.text = Math.Round(distance * 100, 1).ToString() + " cm";
            DistanceText.transform.position = center + offset;
            CenterPoint.transform.position = center;

            //Calculate rotation
            var horizontal = new Vector3(vec.x, 0, vec.z).magnitude;
            var vertical = vec.y;
            var angle = Math.Atan(vertical / horizontal) * (180 / (Math.PI));

            AngleText.text = (Math.Round(-angle, 1)).ToString() + " °";
            AngleText.transform.position = center - ScaleVec(offset, 3);
        }

        //Billboard behaviour
        DistanceText.transform.rotation = Camera.main.transform.rotation;
        AngleText.transform.rotation = Camera.main.transform.rotation;
    }
}
