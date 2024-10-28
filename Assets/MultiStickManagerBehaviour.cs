using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using HandPoints = ReadHands.HandPoints;
using Tools = ToolSelection.Tools;

public class MultiStickManagerBehaviour : MonoBehaviour
{
    //Curernt Sticks is populated by beavhiours registering themselves at startup
    public List<MultiStickBehaviour> CurrentSticks;
    public GameObject StickObj;
    private List<GameObject> stickClones = new List<GameObject>();
    public TextMeshPro TotalDistanceText;
    public TextMeshPro ConnectingAngleText;
    public float yOffset;
    public GameObject TapeSource;

    private List<TextMeshPro> distances = new List<TextMeshPro>();
    private List<TextMeshPro> angles = new List<TextMeshPro>();

    private double currentTotalDistance = 0.0d;

    public ReadHands Hands;
    public ToolSelection ToolSelection;

    private static double threshold = 0.01;

    private bool isEnabled = false;

    private static double GetAngleDEG(Vector3 start, Vector3 middle, Vector3 end)
    {
        //var middleToStart = start - middle;
        //var middleToEnd = end - middle;

        //middleToStart.y = 0;
        //middleToEnd.y = 0;

        //var cos = Vector3.Dot(middleToStart, middleToEnd) / (middleToStart.magnitude * middleToEnd.magnitude);
        //return Math.Acos(cos) * (180 / Math.PI);
        return Vector3.Angle(start - middle, end - middle);
    }

    void UpdateLastAngle(double? angle)
    {
        var currentAngle = angles.Last();
        currentAngle.text = angle != null ? "Δ " + (Math.Round(-angle.Value, 1)).ToString() + " °" : "";
    }

    void UpdateLastDistance(double distance)
    {
        var currentDistance = distances.Last();
        currentDistance.text = "∑ " + (Math.Round(distance * 100, 1)).ToString() + " cm";
        currentTotalDistance = distance;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        var toolActive = ToolSelection.getCurrentTool() == Tools.MEASURING_TAPE;
        var currentlyActive = stickClones.Count > 0 ? stickClones.Last() : StickObj;
        currentlyActive.transform.gameObject.SetActive(toolActive);
        TapeSource.transform.gameObject.SetActive(toolActive);
        isEnabled = toolActive;
    }


    // Update is called once per frame
    void Update()
    {
        if (isEnabled)
        {
            //left.TryGetFeatureValue(CommonUsages.primaryButton, out var leftPrimary);
            //right.TryGetFeatureValue(CommonUsages.primaryButton, out var rightPrimary);
            //left.TryGetFeatureValue(CommonUsages.secondaryButton, out var leftSecondary);
            //right.TryGetFeatureValue(CommonUsages.secondaryButton, out var rightSecondary);

            var leftPrimary = Hands.getClick(HandPoints.L_THUMB, HandPoints.L_MIDDLE);
            var leftSecondary = Hands.getClick(HandPoints.L_THUMB, HandPoints.L_INDEX);

            var rightPrimary = Hands.getClick(HandPoints.R_THUMB, HandPoints.R_MIDDLE);
            var rightSecondary = Hands.getClick(HandPoints.R_THUMB, HandPoints.R_INDEX);

            var currentStick = CurrentSticks.Last();

            var spawned = false;

            if (Hands.getClick(HandPoints.R_INDEX, HandPoints.L_INDEX) && Hands.getClick(HandPoints.R_THUMB, HandPoints.L_THUMB))
            {
                foreach (var clone in stickClones)
                {
                    Destroy(clone);
                }
                stickClones = new();

                foreach (var distance in distances)
                {
                    Destroy(distance);
                }

                distances = new();
                foreach (var angle in angles)
                {
                    Destroy(angle);
                }
                angles = new();

                var firstStick = CurrentSticks.First();
                firstStick.RightFixed = false;
                firstStick.LeftFixed = false;
                //Reset current sticks to first only
                CurrentSticks = new() { firstStick };
            }
            else
            {
                //Determine potential new joint pos before fixing both sides
                var newJointPos = currentStick.RightFixed ? currentStick.LeftAnchor.transform.position : currentStick.RightAnchor.transform.position;

                //Only process one button input at a time
                if (leftPrimary) currentStick.LeftFixed = true;
                else if (rightPrimary) currentStick.RightFixed = true;
                else if (leftSecondary) currentStick.LeftFixed = false;
                else if (rightSecondary) currentStick.RightFixed = false;

                if (currentStick.LeftFixed && currentStick.RightFixed)
                {
                    //Spawn new stick as current
                    stickClones.Add(Instantiate(StickObj, currentStick.transform.position, Quaternion.identity));

                    //Add new connecting labels
                    var newDistance = Instantiate(TotalDistanceText, newJointPos + new Vector3(0, yOffset, 0), Quaternion.identity);
                    newDistance.transform.gameObject.SetActive(true);
                    distances.Add(newDistance);

                    var newAngle = Instantiate(ConnectingAngleText, newJointPos - new Vector3(0, yOffset * 3, 0), Quaternion.identity);
                    newAngle.transform.gameObject.SetActive(true);
                    angles.Add(newAngle);

                    UpdateLastDistance(currentTotalDistance + currentStick.CurrentDistance);
                    spawned = true;
                }
            }

            TapeSource.transform.position = currentStick.RightFixed ? currentStick.RightAnchor.transform.position : currentStick.LeftAnchor.transform.position;

            if (CurrentSticks.Count > 1)
            {
                var previousStick = CurrentSticks.ElementAt(CurrentSticks.Count - 2);

                var previousLeft = previousStick.LeftAnchor.transform.position;
                var previousRight = previousStick.RightAnchor.transform.position;
                var currentLeft = currentStick.LeftAnchor.transform.position;
                var currentRight = currentStick.RightAnchor.transform.position;

                //Check if last sticks are connected on left anchor
                if (currentStick.LeftFixed && (currentLeft - previousLeft).magnitude < threshold)
                {
                    var angle = GetAngleDEG(
                        start: previousRight,
                        middle: currentLeft,
                        end: currentRight
                        );
                    UpdateLastAngle(angle);
                }
                //Check if last sticks are connected on right anchor
                else if (currentStick.RightFixed && (currentRight - previousRight).magnitude < threshold)
                {
                    var angle = GetAngleDEG(
                        start: previousLeft,
                        middle: currentRight,
                        end: currentLeft
                        );
                    UpdateLastAngle(angle);
                }
                else
                {
                    //Dont show angle and reset accumulated distance since not connected to previous shape
                    UpdateLastAngle(null);
                    //If we already spawned new (connected) line, we do not reset distance in this frame again
                    if (!spawned) currentTotalDistance = 0;
                }
            }
        }

        //Billboard behaviour
        foreach(var distance in distances)
        {
            distance.transform.rotation = Camera.main.transform.rotation;
        }
        foreach(var angle in angles)
        {
            angle.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
