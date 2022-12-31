using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RegionEditor;

[CustomEditor(typeof(RegionCreator))]
public class RegionEditor : Editor
{
    private RegionCreator regionCreator;
    private SelectionInfo selectionInfo;
    private bool regionChangedSinceLastRepaint;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        int shapeDeleteIndex = -1;
        regionCreator.showRegionsList = EditorGUILayout.Foldout(regionCreator.showRegionsList, "Show Regions List");
        if (regionCreator.showRegionsList)
        {
            for (int i = 0; i < regionCreator.shapes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Region " + i);

                GUI.enabled = i != selectionInfo.selectedShapeIndex;
                if (GUILayout.Button("Select"))
                    selectionInfo.selectedShapeIndex = i;
                GUI.enabled = true;

                if (GUILayout.Button("Delete"))
                    shapeDeleteIndex = i;
                GUILayout.EndHorizontal();
            }
        }

        if (shapeDeleteIndex != -1)
        {
            //Undo.RecordObject(regionCreator, "Delete shape");
            DestroyImmediate(regionCreator.shapes[shapeDeleteIndex]);
            regionCreator.shapes.RemoveAt(shapeDeleteIndex);
            selectionInfo.selectedShapeIndex = Mathf.Clamp(selectionInfo.selectedShapeIndex, 0, regionCreator.shapes.Count - 1);
        }

        if (GUI.changed)
        {
            regionChangedSinceLastRepaint = true;
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if(guiEvent.type == EventType.Repaint)
            Draw();
        else if (guiEvent.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // Can't be unselected
        else
        {
            HandleInput(guiEvent);
            if (regionChangedSinceLastRepaint)
                HandleUtility.Repaint();
        }
    }

    private void CreateNewShape()
    {
        //Undo.RecordObject(regionCreator, "Create shape");
        regionCreator.shapes.Add(Instantiate(regionCreator.regionPrefab, regionCreator.transform));
        selectionInfo.selectedShapeIndex = regionCreator.shapes.Count - 1;
    }

    private void CreateNewPoint(Vector3 position)
    {
        bool mouseIsOverSelectedShape = selectionInfo.mouseOverShapeIndex == selectionInfo.selectedShapeIndex;
        int newPointIndex = (selectionInfo.mouseIsOverLine && mouseIsOverSelectedShape)
            ? selectionInfo.lineIndex + 1 : SelectedShape.points.Count;
        Undo.RecordObject(regionCreator, "Add point");
        SelectedShape.points.Insert(newPointIndex, position);
        selectionInfo.pointIndex = newPointIndex;
        selectionInfo.mouseOverShapeIndex = selectionInfo.selectedShapeIndex;
        regionChangedSinceLastRepaint = true;

        SelectPointUnderMouse();
    }

    void DeletePointUnderMouse()
    {
        //Undo.RecordObject(regionCreator, "Delete point");
        SelectedShape.points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.pointIsSelected = false;
        selectionInfo.mouseIsOverPoint = false;
        regionChangedSinceLastRepaint = true;
    }

    void SelectPointUnderMouse()
    {
        selectionInfo.pointIsSelected = true;
        selectionInfo.mouseIsOverPoint = true;
        selectionInfo.mouseIsOverLine = false;
        selectionInfo.lineIndex = -1;

        selectionInfo.positionAtStartOfDrag = SelectedShape.points[selectionInfo.pointIndex];
        regionChangedSinceLastRepaint = true;
    }

    void SelectShapeUnderMouse()
    {
        if (selectionInfo.mouseOverShapeIndex != -1)
        {
            selectionInfo.selectedShapeIndex = selectionInfo.mouseOverShapeIndex;
            regionChangedSinceLastRepaint = true;
        }
    }

    private void HandleInput(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneZ = 0;
        float dstToDrawPlane = (drawPlaneZ - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePosition = mouseRay.origin + mouseRay.direction * dstToDrawPlane;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
            HandleShiftLeftMouseDown(mousePosition);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            HandleLeftMouseDown(mousePosition);

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
            HandleLeftMouseUp(mousePosition);

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            HandleLeftMouseDrag(mousePosition);

        if (!selectionInfo.pointIsSelected)
            UpdateMouseOverInfo(mousePosition);
    }

    private void HandleShiftLeftMouseDown(Vector3 mousePosition)
    {
        if(selectionInfo.mouseIsOverPoint)
        {
            SelectShapeUnderMouse();
            DeletePointUnderMouse();
        }
        else
        {
            CreateNewShape();
            CreateNewPoint(mousePosition);
        }
    }

    private void HandleLeftMouseDown(Vector3 mousePosition)
    {
        if (regionCreator.shapes.Count == 0)
            CreateNewShape();

        SelectShapeUnderMouse();

        if (selectionInfo.mouseIsOverPoint)
            SelectPointUnderMouse();
        else
            CreateNewPoint(mousePosition);
    }

    private void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            //Undo.RecordObject(regionCreator, "Move point");
            SelectedShape.points[selectionInfo.pointIndex] = mousePosition;

            selectionInfo.pointIsSelected = false; 
            selectionInfo.pointIndex = -1;
            regionChangedSinceLastRepaint = true;
        }
    }

    private void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape.points[selectionInfo.pointIndex] = mousePosition;
            regionChangedSinceLastRepaint = true;
        }
    }

    private void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        int mouseOverPointIndex = -1;
        int mouseOverShapeIndex = -1;
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            Region currentShape = regionCreator.shapes[shapeIndex].GetComponent<Region>();
            for (int i = 0; i < currentShape.points.Count; i++)
            {
                if (Vector3.Distance(mousePosition, currentShape.points[i]) < regionCreator.handleRadius)
                {
                    mouseOverPointIndex = i;
                    mouseOverShapeIndex = shapeIndex;
                    break;
                }
            }
        }

        if (mouseOverPointIndex != selectionInfo.pointIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
        {
            selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;
            regionChangedSinceLastRepaint = true;
        }

        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }    
        else
        {
            int mouseOverLineIndex = -1;
            float closestLineDst = regionCreator.handleRadius;
            for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
            {
                Region currentShape = regionCreator.shapes[shapeIndex].GetComponent<Region>();
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPointInRegion = currentShape.points[(i + 1) % currentShape.points.Count];

                    float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition.ToXY(),
                        currentShape.points[i].ToXY(), nextPointInRegion.ToXY());

                    if (dstFromMouseToLine <= closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;
                        mouseOverShapeIndex = shapeIndex;
                    }
                }
            }

            if (selectionInfo.lineIndex != mouseOverLineIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
            {
                selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                regionChangedSinceLastRepaint = true; 
            }
        }
    }

    private void Draw()
    {
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            Region shapeToDraw = regionCreator.shapes[shapeIndex].GetComponent<Region>();
            bool shapeIsSelected = shapeIndex == selectionInfo.selectedShapeIndex;
            bool mouseIsOverShape = shapeIndex == selectionInfo.mouseOverShapeIndex;
            Color deselectedShapeColor = Color.gray;

            for (int i = 0; i < shapeToDraw.points.Count; i++)
            {
                Vector3 nextPoint = shapeToDraw.points[(i + 1) % shapeToDraw.points.Count];

                if (i == selectionInfo.lineIndex && mouseIsOverShape)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(shapeToDraw.points[i], nextPoint);
                }
                else
                {
                    Handles.color = (shapeIsSelected) ? Color.black : deselectedShapeColor;
                    Handles.DrawDottedLine(shapeToDraw.points[i], nextPoint, 4);
                }

                if (i == selectionInfo.pointIndex && mouseIsOverShape)
                    Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
                else
                    Handles.color = (shapeIsSelected) ? Color.white : deselectedShapeColor;
                Handles.DrawSolidDisc(shapeToDraw.points[i], Vector3.back, regionCreator.handleRadius);
            }
        }

        if (regionChangedSinceLastRepaint)
            regionCreator.UpdateMeshDisplay();
        regionChangedSinceLastRepaint = false;
    }

    private void OnEnable()
    {
        regionChangedSinceLastRepaint = true;
        regionCreator = target as RegionCreator;
        selectionInfo = new SelectionInfo();
        //Undo.undoRedoPerformed += OnUndoOrRedo;
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        //Undo.undoRedoPerformed -= OnUndoOrRedo;
        Tools.hidden = false;
    }

    private void OnUndoOrRedo()
    {
        if (selectionInfo.selectedShapeIndex >= regionCreator.shapes.Count || selectionInfo.selectedShapeIndex == -1)
            selectionInfo.selectedShapeIndex = regionCreator.shapes.Count - 1;
    }

    Region SelectedShape
    {
        get
        {
            return regionCreator.shapes[selectionInfo.selectedShapeIndex].GetComponent<Region>();
        }
    }
    public class SelectionInfo
    {
        public int selectedShapeIndex;
        public int mouseOverShapeIndex;

        public int pointIndex = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;

        public int lineIndex = -1;
        public bool mouseIsOverLine;
    }
}
