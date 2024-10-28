using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ToolSelection;

public class LaserTool : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject plane;

    public ReadHands Hands;

    private List<GameObject> clones = new List<GameObject>();
    public List<LasterTest> Scripts = new List<LasterTest>();

    private bool enablePlanes = false;
    private bool spawned = false;
    private bool toggled = false;

    public ToolSelection ToolSelection;

    private bool isActive;

    private void FixedUpdate()
    {
        isActive = ToolSelection.getCurrentTool() == Tools.LASER;
        plane.transform.gameObject.SetActive(isActive);
    }

    private void Update()
    {
        if (!isActive) return;

        plane.transform.position = Hands.getFingerPosition(ReadHands.HandPoints.R_INDEX);

        if (spawned)
        {
            plane.transform.gameObject.SetActive(false);
            var angle = Hands.getRotation(ReadHands.HandPoints.L_PALM).eulerAngles.y;

            Scripts.Last().RotYDEG = ((float)angle);
        }
        else
        {
            plane.transform.gameObject.SetActive(true);
        }

        if (Hands.getClick(ReadHands.HandPoints.R_THUMB, ReadHands.HandPoints.R_INDEX))
        {
            if(!spawned)
            {
                clones.Add(Instantiate(plane, Hands.getFingerPosition(ReadHands.HandPoints.R_INDEX), Quaternion.identity));
                clones.Last().gameObject.SetActive(true);
                spawned = true; 
            }
        }
        else
        {
            spawned = false;
        }

        if (Hands.getClick(ReadHands.HandPoints.R_THUMB, ReadHands.HandPoints.R_MIDDLE))
        {
            if (!toggled)
            {
                enablePlanes = !enablePlanes;
                foreach (var script in Scripts.Skip(1))
                {
                    script.EnabledPlanes = enablePlanes;
                }
            }
            toggled = true;
        }
        else
        {
            toggled = false;
        }

        if (Hands.getClick(ReadHands.HandPoints.L_THUMB, ReadHands.HandPoints.R_THUMB) && Hands.getClick(ReadHands.HandPoints.L_INDEX, ReadHands.HandPoints.R_INDEX))
        {
            foreach (GameObject obj in clones)
            {
                Destroy(obj);
            }
            clones.Clear();
            Scripts.Clear();
        }
    }
}
