using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

public class ReadHands : MonoBehaviour
{
    public GameObject ColIndex;
    public GameObject ColMiddle;
    public GameObject ColThumb;
    public GameObject ColRing;
    public GameObject ColPinky;
    public GameObject ColPalm;

    public GameObject L_ColIndex;
    public GameObject L_ColMiddle;
    public GameObject L_ColThumb;
    public GameObject L_ColRing;
    public GameObject L_ColPinky;
    public GameObject L_ColPalm;

    public enum HandPoints
    {
        R_INDEX,
        R_MIDDLE,
        R_THUMB,
        R_PALM,
        R_RING,
        R_PINKY,
        L_INDEX,
        L_MIDDLE,
        L_THUMB,
        L_PALM,
        L_RING,
        L_PINKY
    }

    float getPalmDistance(GameObject finger)
    {
        return Vector3.Distance(finger.transform.position, ColPalm.transform.position);
    }

    public Boolean getClick(params HandPoints[] fingers)
    {
        float distance = 0;
        int distance_counter = 0;
        if (fingers.Length < 2) {
            return false;
        }

        for (int i = 0; i < fingers.Length; i++)
        {
            for (int j = 0; j < fingers.Length; j++)
            {
                if(i == j)
                {
                    continue;
                }

                distance += Vector3.Distance(getFingerPosition(fingers[i]), getFingerPosition(fingers[j]));
                distance_counter++;
            }
        }

        Debug.Log(distance/distance_counter);
        return distance / distance_counter < 0.02;
    }

    public Vector3 getFingerPosition(HandPoints handpoint)
    {
        switch(handpoint) {
            case HandPoints.R_INDEX:
                return ColIndex.transform.position;
            case HandPoints.R_MIDDLE:
                return ColMiddle.transform.position;
            case HandPoints.R_THUMB:
                return ColThumb.transform.position;
            case HandPoints.R_PALM:
                return ColPalm.transform.position;
            case HandPoints.R_RING:
                return ColRing.transform.position;
            case HandPoints.R_PINKY:
                return ColPinky.transform.position;

            case HandPoints.L_INDEX:
                return L_ColIndex.transform.position;
            case HandPoints.L_MIDDLE:
                return L_ColMiddle.transform.position;
            case HandPoints.L_THUMB:
                return L_ColThumb.transform.position;
            case HandPoints.L_PALM:
                return L_ColPalm.transform.position;
            case HandPoints.L_RING:
                return L_ColRing.transform.position;
            case HandPoints.L_PINKY:
                return L_ColPinky.transform.position;
            default:
                return new Vector3(0,0,0);
        }
    }

    public Vector3 getOrientationVector(HandPoints handpoint)
    {
        switch(handpoint) {
            case HandPoints.R_INDEX:
                return ColIndex.transform.rotation.eulerAngles;
            case HandPoints.R_MIDDLE:
                return ColMiddle.transform.rotation.eulerAngles;
            case HandPoints.R_THUMB:
                return ColThumb.transform.rotation.eulerAngles;
            case HandPoints.R_PALM:
                return ColPalm.transform.rotation.eulerAngles;
            case HandPoints.R_RING:
                return ColRing.transform.rotation.eulerAngles;
            case HandPoints.R_PINKY:
                return ColPinky.transform.rotation.eulerAngles;

            case HandPoints.L_INDEX:
                return L_ColIndex.transform.rotation.eulerAngles;
            case HandPoints.L_MIDDLE:
                return L_ColMiddle.transform.rotation.eulerAngles;
            case HandPoints.L_THUMB:
                return L_ColThumb.transform.rotation.eulerAngles;
            case HandPoints.L_PALM:
                return L_ColPalm.transform.rotation.eulerAngles;
            case HandPoints.L_RING:
                return L_ColRing.transform.rotation.eulerAngles;
            case HandPoints.L_PINKY:
                return L_ColPinky.transform.rotation.eulerAngles;
            default:
                return new Vector3(0,0,0);
        }
    }

    public Quaternion getRotation(HandPoints handpoint)
    {
        switch (handpoint)
        {
            case HandPoints.R_INDEX:
                return ColIndex.transform.rotation;
            case HandPoints.R_MIDDLE:
                return ColMiddle.transform.rotation;
            case HandPoints.R_THUMB:
                return ColThumb.transform.rotation;
            case HandPoints.R_PALM:
                return ColPalm.transform.rotation;
            case HandPoints.R_RING:
                return ColRing.transform.rotation;
            case HandPoints.R_PINKY:
                return ColPinky.transform.rotation;

            case HandPoints.L_INDEX:
                return L_ColIndex.transform.rotation;
            case HandPoints.L_MIDDLE:
                return L_ColMiddle.transform.rotation;
            case HandPoints.L_THUMB:
                return L_ColThumb.transform.rotation;
            case HandPoints.L_PALM:
                return L_ColPalm.transform.rotation;
            case HandPoints.L_RING:
                return L_ColRing.transform.rotation;
            case HandPoints.L_PINKY:
                return L_ColPinky.transform.rotation;
            default:
                return Quaternion.identity;
        }
    }
}
