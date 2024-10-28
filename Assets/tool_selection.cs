using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToolSelection : MonoBehaviour
{
    public ReadHands Hands;
    public GameObject TapeMeasure;
    public GameObject Pencil;
    public GameObject Laser;
    public GameObject Hammer;

    private bool isOpen;

    public enum Tools
    {
        MEASURING_TAPE,
        PENCIL,
        LASER,
        NONE,
        HAMMER
    }

    private Tools current_tool = Tools.NONE;

    private static void Activate(GameObject obj)
    {
        obj.transform.gameObject.SetActive(true);
    }

    private static void Deactivate(GameObject obj)
    {
        obj.transform.gameObject.SetActive(false);
    }

    private static void Rotate(GameObject obj)
    {
        obj.transform.RotateAround(obj.transform.position, Vector3.up, 1);
    }

    private void Open()
    {
        isOpen = true;
        Activate(TapeMeasure);
        Activate(Pencil);
        Activate(Laser);
    }

    private void Close()
    {
        isOpen = false;
        Deactivate(TapeMeasure);
        Deactivate(Pencil);
        Deactivate(Laser);
    }

    private void FixedUpdate()
    {
        if (isOpen)
        {
            //Rotate props
            Rotate(TapeMeasure);
            Rotate(Pencil);
            Rotate(Laser);
        }
        if(current_tool == Tools.HAMMER)
        {
            Activate(Hammer);
        }
        else
        {
            Deactivate(Hammer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Hands.getOrientationVector(ReadHands.HandPoints.L_PALM));
        var palmUp = (Hands.getRotation(ReadHands.HandPoints.L_PALM) * Vector3.up).y > 0.5;
        if (palmUp && !isOpen)
        {
            Open();
        }

        if (isOpen)
        {
            //Update props to finger positions
            var offset = new Vector3(0, 0.05f, 0);
            TapeMeasure.transform.position = Hands.getFingerPosition(ReadHands.HandPoints.L_INDEX) + offset;
            Pencil.transform.position = Hands.getFingerPosition(ReadHands.HandPoints.L_MIDDLE) + offset;
            Laser.transform.position = Hands.getFingerPosition(ReadHands.HandPoints.L_RING) + offset;
        }

        //Only register first click
        if (isOpen)
        {
            if (Hands.getClick(ReadHands.HandPoints.L_INDEX, ReadHands.HandPoints.R_INDEX))
            {
                current_tool = Tools.MEASURING_TAPE;
            }
            else if (Hands.getClick(ReadHands.HandPoints.L_MIDDLE, ReadHands.HandPoints.R_INDEX))
            {
                current_tool = Tools.PENCIL;
            }
            else if (Hands.getClick(ReadHands.HandPoints.L_RING, ReadHands.HandPoints.R_INDEX))
            {
                current_tool = Tools.LASER;
            }
            else if (Hands.getClick(ReadHands.HandPoints.L_PALM, ReadHands.HandPoints.R_INDEX) ||
                Hands.getClick(ReadHands.HandPoints.L_PINKY, ReadHands.HandPoints.R_INDEX) ||
                Hands.getClick(ReadHands.HandPoints.L_THUMB, ReadHands.HandPoints.R_INDEX)
                )
            {
                current_tool = Tools.NONE;
            }else if (Hands.getClick(ReadHands.HandPoints.L_PINKY, ReadHands.HandPoints.R_PINKY))
            {
                current_tool = Tools.HAMMER;
            }
        }

        if (!palmUp && isOpen)
        {
            Close();
        }
    }

    public Tools getCurrentTool()
    {
        return current_tool;
    }
}
