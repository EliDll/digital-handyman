using UnityEngine;
using System.Collections.Generic;
using static ReadHands;

public class DrawingLogic : MonoBehaviour 
{ 
    public ReadHands Hands;
    public ToolSelection ToolSelection;
    public GameObject tip_left, tip_right;
    private LineRenderer currentDrawing_left, currentDrawing_right;
    private int currentColorIndex_left, currentColorIndex_right;
    public Color[] penColors;
    public float penWidth_left = 0.02f, penWidth_right = 0.02f;
    private int index_left, index_right;
    public Material drawingMaterial;
    public Material tipMaterial_left, tipMaterial_right;
    private bool leftDraw, leftChange, rightDraw, rightChange, lock_change_left, lock_change_right, 
                    left_make_smaller, right_make_smaller, left_make_bigger, right_make_bigger, active,
                    lock_smaller_left, lock_smaller_right, lock_bigger_left, lock_bigger_right;
    private List<LineRenderer> drawing_list = new List<LineRenderer>();

    void Start()
    {
        lock_change_left = true;
        lock_change_right = true;
        lock_smaller_left = true;
        lock_smaller_right = true;
        lock_bigger_left = true;
        lock_bigger_right = true;
        active = false;
    }

    private void FixedUpdate()
    {
        active = ToolSelection.getCurrentTool() == ToolSelection.Tools.PENCIL;
        tip_right.SetActive(active);
        tip_left.SetActive(active);
    }

    private void Update() {
        if (!active) return;

        // reset all lines 
        if(Hands.getClick(HandPoints.R_INDEX, HandPoints.L_INDEX) && Hands.getClick(HandPoints.R_THUMB, HandPoints.L_THUMB))
        {
            deleteAll();
            return;
        }
        
        // skip if the tips are not active
        // Check if the primary button is pressed
        rightDraw = Hands.getClick(HandPoints.R_INDEX, HandPoints.R_THUMB);
        rightChange = Hands.getClick(HandPoints.R_THUMB, HandPoints.R_MIDDLE);
        leftDraw = Hands.getClick(HandPoints.L_INDEX, HandPoints.L_THUMB);
        leftChange = Hands.getClick(HandPoints.L_MIDDLE, HandPoints.L_THUMB);
        //
        left_make_smaller = Hands.getClick(HandPoints.L_THUMB, HandPoints.L_RING);
        right_make_smaller = Hands.getClick(HandPoints.R_THUMB, HandPoints.R_RING);
        left_make_bigger = Hands.getClick(HandPoints.L_THUMB, HandPoints.L_PINKY);
        right_make_bigger = Hands.getClick(HandPoints.R_THUMB, HandPoints.R_PINKY);

        // lock colour changing
        if (!lock_change_left && !leftChange) lock_change_left = true;
        if (!lock_change_right && !rightChange) lock_change_right = true;
        //
        if (!lock_smaller_left && !left_make_smaller) lock_smaller_left = true;
        if (!lock_smaller_right && !right_make_smaller) lock_smaller_right = true;
        if (!lock_bigger_left && !left_make_bigger) lock_bigger_left = true;
        if (!lock_bigger_right && !right_make_bigger) lock_bigger_right = true;
        
        // the logic of the pen
        if (leftDraw || rightDraw){
            Draw();
        }
        else if (currentDrawing_left != null || currentDrawing_right != null) {
            currentDrawing_left = null;
            currentDrawing_right = null;
        }
        else if (leftChange || rightChange) {
            if (lock_change_left && leftChange) {
                lock_change_left = false;
                SwitchColor();
            }
            if (lock_change_right && rightChange) {
                lock_change_right = false;
                SwitchColor();
            }
        }
        else if (left_make_smaller || right_make_smaller){
            if (lock_smaller_left && left_make_smaller){
                lock_smaller_left = false;
                make_pen_smaller();
            }
            if (lock_smaller_right && right_make_smaller){
                lock_smaller_right = false;
                make_pen_smaller();
            }
        }
        else if (left_make_bigger || right_make_bigger){
            if (lock_bigger_left && left_make_bigger){
                lock_bigger_left = false;
                make_pen_bigger();
            }
            if (lock_bigger_right && right_make_bigger){
                lock_bigger_right = false;
                make_pen_bigger();
            }
        }
        // no action case
        if (!leftDraw && !rightDraw){
            tip_left.SetActive(true);
            tip_right.SetActive(true);
            if (currentColorIndex_left==1)
            {
                tip_left.transform.localScale = new Vector3(penWidth_left, penWidth_left, penWidth_left);
            }
            if (currentColorIndex_right==1)
            {
                tip_right.transform.localScale = new Vector3(penWidth_right, penWidth_right, penWidth_right);
            }
        }
    }

    private void make_pen_bigger(){
        if (penWidth_left < 0.1f && left_make_bigger){
            penWidth_left += 0.01f;
            tip_left.transform.localScale = new Vector3(penWidth_left, penWidth_left, penWidth_left);
        }
        if (penWidth_right < 0.1f && right_make_bigger){
            penWidth_right += 0.01f;
            tip_right.transform.localScale = new Vector3(penWidth_right, penWidth_right, penWidth_right);
        }
    }
    private void make_pen_smaller(){
        if (penWidth_left > 0.02f && left_make_smaller){
            penWidth_left -= 0.01f;
            tip_left.transform.localScale = new Vector3(penWidth_left, penWidth_left, penWidth_left);
        }
        if (penWidth_right > 0.02f && right_make_smaller){
            penWidth_right -= 0.01f;
            tip_right.transform.localScale = new Vector3(penWidth_right, penWidth_right, penWidth_right);
        }
    }
    
    private void SwitchColor(){
        // change the colour of the tip 
        if (leftChange) 
        {
            if (currentColorIndex_left == penColors.Length - 1) currentColorIndex_left = 0;
            else currentColorIndex_left++;
            tipMaterial_left.color = penColors[currentColorIndex_left];
        }
        if (rightChange)
            {
            if (currentColorIndex_right == penColors.Length - 1) currentColorIndex_right = 0;
            else currentColorIndex_right++;
            tipMaterial_right.color = penColors[currentColorIndex_right];
        }
    }
    
    void deleteAll()
    {
        drawing_list.ForEach(Destroy);
        drawing_list.Clear();
    }

    void Draw()
    {
        Vector3 devicePos;
        if(leftDraw)
        {
            devicePos = tip_left.transform.position;

            // Draw the actual line
            if (currentDrawing_left == null) {
                index_left = 0;
                currentDrawing_left = new GameObject().AddComponent<LineRenderer>();
                currentDrawing_left.material = drawingMaterial;
                currentDrawing_left.startColor = currentDrawing_left.endColor = penColors[currentColorIndex_left];
                currentDrawing_left.startWidth = currentDrawing_left.endWidth = penWidth_left * 0.5f;
                currentDrawing_left.positionCount = 1;   
                currentDrawing_left.SetPosition(0, devicePos);
            }
            else {
                var currentPosition = currentDrawing_left.GetPosition(index_left);
                if (Vector3.Distance(currentPosition, devicePos) > 0.01f) {
                    index_left++;
                    currentDrawing_left.positionCount = index_left + 1;
                    currentDrawing_left.SetPosition(index_left, devicePos);
                }
            }
            drawing_list.Add(currentDrawing_left);
        }
        if (rightDraw)
        {
            devicePos = tip_right.transform.position;

            // Draw the actual line
            if (currentDrawing_right == null) {
                index_right = 0;
                currentDrawing_right = new GameObject().AddComponent<LineRenderer>();
                currentDrawing_right.material = drawingMaterial;
                currentDrawing_right.startColor = currentDrawing_right.endColor = penColors[currentColorIndex_right];
                currentDrawing_right.startWidth = currentDrawing_right.endWidth = penWidth_right * 0.5f;
                currentDrawing_right.positionCount = 1;   
                currentDrawing_right.SetPosition(0, devicePos);
            }
            else {
                var currentPosition = currentDrawing_right.GetPosition(index_right);
                if (Vector3.Distance(currentPosition, devicePos) > 0.01f) {
                    index_right++;
                    currentDrawing_right.positionCount = index_right + 1;
                    currentDrawing_right.SetPosition(index_right, devicePos);
                }
            }
            drawing_list.Add(currentDrawing_right);
        }
    }
}