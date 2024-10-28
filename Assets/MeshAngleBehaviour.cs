using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MeshAngleBehaviour : MonoBehaviour
{
    public MultiStickBehaviour first;
    public MultiStickBehaviour second;
    public MultiStickManagerBehaviour manager;

    public TextMeshPro Billboard;

    private static double threshold = 0.1;

    private static Vector3 ScaleVec(Vector3 vec, double scalar)
    {
        var f = (float)scalar;
        return new Vector3(vec.x * f, vec.y * f, vec.z * f);
    }

    private void UpdateBillboard(Vector3 start, Vector3 middle, Vector3 end)
    {
        var startMiddle = start - middle;
        startMiddle.y = 0;
        var endMiddle = end - middle;
        endMiddle.y = 0;

        Billboard.text = Math.Round(Math.Atan(startMiddle.magnitude / endMiddle.magnitude) * (180 / (Math.PI)), 1).ToString() + " °";
        Billboard.transform.position = middle + ScaleVec(startMiddle, 0.25) + ScaleVec(endMiddle, 0.25);
    }

    // Start is called before the first frame update
    void Start()
    {
        Billboard.text = "";

        //Register to manager
        //manager.CurrentJoints.Add(this);
        var lastIndex = manager.CurrentSticks.Count - 1;
        if (lastIndex > 1) first = manager.CurrentSticks.ElementAt(lastIndex - 1);
        if (lastIndex > 0) second = manager.CurrentSticks.ElementAt(lastIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (first != null && second != null && (!first.LeftFixed || !first.RightFixed || !second.LeftFixed || !second.RightFixed))
        {
            var firstLeft = first.LeftAnchor.transform.position;
            var firstRight = first.RightAnchor.transform.position;
            var secondLeft = second.LeftAnchor.transform.position;
            var secondRight = second.RightAnchor.transform.position;

            var leftleft = (firstLeft - secondLeft).magnitude;
            var leftright = (firstLeft - secondRight).magnitude;
            var rightleft = (firstRight - secondLeft).magnitude;
            var rightright = (firstRight - secondRight).magnitude;

            if (leftleft < threshold)
            {
                var start = firstRight;
                var middle = firstLeft;
                var end = secondRight;
                UpdateBillboard(start, middle, end);

            }
            else if (leftright < threshold)
            {
                var start = firstRight;
                var middle = firstLeft;
                var end = secondLeft;
                UpdateBillboard(start, middle, end);
            }
            else if (rightleft < threshold)
            {
                var start = firstLeft;
                var middle = firstRight;
                var end = secondRight;
                UpdateBillboard(start, middle, end);
            }
            else if (rightright < threshold)
            {
                var start = firstLeft;
                var middle = firstRight;
                var end = secondLeft;
                UpdateBillboard(start, middle, end);
            }
            else
            {
                Billboard.text = "";
            }
        }
        //Billboard behaviour
        Billboard.transform.rotation = Camera.main.transform.rotation;
    }
}
