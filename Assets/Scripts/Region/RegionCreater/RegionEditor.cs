using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(RegionCreator))]
public class RegionEditor : Editor
{
    private RegionCreator regionCreator;
    private bool regionChangedSinceLastRepaint;

    private MouseInfo mouseInfo;
    private RegionInfo selectedRegionInfo;
    private LineInfo selectedLineInfo = new LineInfo();
    private PointInfo selectedPointInfo = new PointInfo();

    private void OnEnable()
    {
        regionChangedSinceLastRepaint = true;
        regionCreator = target as RegionCreator;
        selectedRegionInfo = new RegionInfo(regionCreator);
        mouseInfo = new MouseInfo(regionCreator);
        Undo.undoRedoPerformed += OnUndoOrRedo;
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoOrRedo;
        Tools.hidden = false;
    }

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
                GUI.enabled = i != selectedRegionInfo.id;
                if (GUILayout.Button("Select"))
                    selectedRegionInfo.id = i;
                GUI.enabled = true;

                if (GUILayout.Button("Delete"))
                    shapeDeleteIndex = i;
                GUILayout.EndHorizontal();
            }
        }

        if (shapeDeleteIndex != -1)
        {
            regionCreator.regions.Remove(regionCreator.shapes[shapeDeleteIndex].region);
            Undo.RecordObject(regionCreator, "Delete shape");

            DestroyImmediate(regionCreator.shapes[shapeDeleteIndex].region);
            regionCreator.shapes.RemoveAt(shapeDeleteIndex);
            selectedRegionInfo.id = 
                Mathf.Clamp(selectedRegionInfo.id, 0, regionCreator.shapes.Count - 1);
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

    private Region AddNewRegion()
    {
        GameObject regionGameObject = Instantiate(regionCreator.regionPrefab, regionCreator.transform);
        regionGameObject.GetComponent<Region>().SetColor(regionCreator.nextRegionColor);
        //regionGameObject.GetComponent<Region>().regionColor = regionCreator.nextRegionColor;
        regionCreator.regions.Add(regionGameObject.GetComponent<Region>());

        return regionGameObject.GetComponent<Region>();
    }

    private void CreateNewShape()
    {
        Region newRegion = AddNewRegion();

        Shape shape = new Shape(newRegion, regionCreator.regions.Count - 1);
        regionCreator.shapes.Add(shape);

        Undo.RecordObject(regionCreator, "Create shape");

        shape.needDestroyRegion = false;
        selectedRegionInfo.id = regionCreator.shapes.Count - 1;
    }

    private void CreateNewPoint(Vector3 position, bool needSelectPoint = true)
    {
        int newPointIndex = selectedLineInfo.id + 1;
        Undo.RecordObject(regionCreator, "Add point");
        regionCreator.shapes[selectedRegionInfo.id].points.Insert(newPointIndex, position);

        selectedPointInfo.id = newPointIndex;

        regionChangedSinceLastRepaint = true;

        if (needSelectPoint)
        {
            SelectPointUnderMouse();
        }
        else
        {
            selectedPointInfo.isSelected = false;
            mouseInfo.isOverPoint = false;
            selectedPointInfo.id = -1;
        }
    }

    private void DeletePoint(int regionId, int pointId)
    {
        regionCreator.shapes[regionId].points.RemoveAt(pointId);
    }

    void DeletePointUnderMouse()
    {
        Undo.RecordObject(regionCreator, "Delete point");
        if (mouseInfo.isOverPoint)
            DeletePoint(mouseInfo.underMouseRegionInfo.id, mouseInfo.underMousePointInfo.id);
    }

    void SelectPointUnderMouse()
    {
        if (mouseInfo.underMousePointInfo.id != -1)
        {
            selectedPointInfo.isSelected = true;
            selectedPointInfo.regionInfo = mouseInfo.underMousePointInfo.regionInfo;
            selectedPointInfo.id = mouseInfo.underMousePointInfo.id;

            mouseInfo.positionAtStartOfDrag = selectedPointInfo.point;
            regionChangedSinceLastRepaint = true;
        }

    }

    private void SelectOrCreatePoint()
    {
        bool regionUnderMouseIsSelected = mouseInfo.underMouseRegionInfo.id == selectedRegionInfo.id;
        bool noRegionsUnderMouse = mouseInfo.underMouseRegionInfo.id == -1;
        bool noRegions = regionCreator.shapes.Count == 0;

        if (noRegions)
            CreateNewShape();

        if (regionUnderMouseIsSelected || noRegionsUnderMouse)
        {
            if (mouseInfo.isOverPoint)
                SelectPointUnderMouse();
            else
            {
                if (!CreatePointOnClosestToMouseLine())
                    CreatePointUnderMouse();
            }
        }
    }

    private bool CreatePointUnderMouse()
    {
        CreateNewPoint(mouseInfo.position);
        return true;
    }

    private bool CreatePointOnClosestToMouseLine()
    {
        if (mouseInfo.isOverLine)
        {
            CreateNewPoint(selectedLineInfo.closestToMousePoint, false);
            return true;
        }
        return false;
    }

    private void HandleInput(Event guiEvent)
    {
        mouseInfo.UpdateMousePosition(guiEvent);

        bool vKey = guiEvent.keyCode == KeyCode.V;

        bool controlLeftMouseDown =
            guiEvent.type == EventType.MouseDown &&
            guiEvent.button == 0 &&
            guiEvent.modifiers == EventModifiers.Control;

        bool leftMouseUp =
            guiEvent.type == EventType.MouseUp &&
            guiEvent.button == 0;

        bool leftMouseDrag =
            guiEvent.type == EventType.MouseDrag &&
            guiEvent.button == 0 &&
            guiEvent.modifiers == EventModifiers.None;

        bool onlyLeftMouseDown =
             guiEvent.type == EventType.MouseDown &&
             guiEvent.button == 0 &&
             guiEvent.modifiers == EventModifiers.None;

        bool onlyShiftLeftMouseDown =
            guiEvent.type == EventType.MouseDown &&
            guiEvent.button == 0 &&
            guiEvent.modifiers == EventModifiers.Shift;


        if (onlyLeftMouseDown)
            SelectOrCreatePoint();

        if (onlyShiftLeftMouseDown)
            DeletePointOrCreateNewPointInNewShape();

        if (vKey)
            CreatePointOnClosestToMouseLine();

        if (controlLeftMouseDown)
            SelectShapeUnderMouse();

        if (leftMouseDrag)
            DragPoint();

        if (leftMouseUp)
            EndMouseDragging();

        if (!selectedPointInfo.isSelected)
            UpdateMouseOverInfo();
    }

    private void DeletePointOrCreateNewPointInNewShape()
    {
        if(mouseInfo.isOverPoint)
        {
            SelectShapeUnderMouse();
            DeletePointUnderMouse();
        }
        else
        {
            CreateNewShape();
            CreateNewPoint(mouseInfo.position);
            SelectPointUnderMouse();
        }
    }

    private void SelectShape(int shapeIndex)
    {
        selectedRegionInfo.id = shapeIndex;
        regionChangedSinceLastRepaint = true;
    }

    private void SelectShapeUnderMouse()
    {
        if (mouseInfo.isOverLine || mouseInfo.isOverPoint)
            SelectShape (mouseInfo.underMouseRegionInfo.id);
    }

    private void EndMouseDragging()
    {
        if (selectedPointInfo.isSelected)
        {
            selectedPointInfo.point = mouseInfo.positionAtStartOfDrag;
            Undo.RecordObject(regionCreator, "Move point");
            selectedPointInfo.point = FindClosestToMousePointOnLineOrPointExeptRegion(selectedRegionInfo.id);
            selectedPointInfo.Unselect();
            regionChangedSinceLastRepaint = true;
        }
    }

    private Vector3 FindClosestToMousePointOnLineAmongAllRegions()
    {
        int mouseOverLineIndex = -1;
        float closestLineDst = regionCreator.handleRadius;
        Vector3 closestPositionOnLine = mouseInfo.position;
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            Shape currentShape = regionCreator.shapes[shapeIndex];
            for (int i = 0; i < currentShape.points.Count; i++)
            {
                Vector3 nextPointInRegion = currentShape.points[(i + 1) % currentShape.points.Count];

                float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mouseInfo.position.ToXY(),
                    currentShape.points[i].ToXY(), nextPointInRegion.ToXY());

                if (dstFromMouseToLine <= closestLineDst)
                {
                    closestLineDst = dstFromMouseToLine;
                    mouseOverLineIndex = i;


                    Vector3 line = nextPointInRegion - currentShape.points[i];
                    Vector3 linePerpendicularDir = Vector3.Normalize(Vector3.Cross(line, new Vector3(0, 0, 1)));
                    closestPositionOnLine = mouseInfo.position + linePerpendicularDir * dstFromMouseToLine;
                }
            }
        }
        return closestPositionOnLine;
    }


    private Vector3 FindClosestToMousePointOnLineExeptRegion(int regionId)
    {
        int mouseOverLineIndex = -1;
        float closestLineDst = regionCreator.handleRadius;
        Vector3 closestPositionOnLine = mouseInfo.position;
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            if (shapeIndex != regionId)
            {
                Shape currentShape = regionCreator.shapes[shapeIndex];
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPointInRegion = currentShape.points[(i + 1) % currentShape.points.Count];

                    float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mouseInfo.position.ToXY(),
                        currentShape.points[i].ToXY(), nextPointInRegion.ToXY());

                    if (dstFromMouseToLine <= closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;

                        Vector3 line = nextPointInRegion - currentShape.points[i];
                        Vector3 linePerpendicularDir = Vector3.Normalize(Vector3.Cross(line, new Vector3(0, 0, 1)));
                        closestPositionOnLine = mouseInfo.position + linePerpendicularDir * dstFromMouseToLine;
                    }
                }
            }     
        }
        return closestPositionOnLine;
    }

    private Vector3 FindClosestToMousePointOnLineOrPointExeptRegion(int regionId)
    {

        Vector3 closestPoint = mouseInfo.position;
        float closestPointDst = regionCreator.handleRadius;
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            if (shapeIndex != regionId)
            {
                Shape currentShape = regionCreator.shapes[shapeIndex];
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    float dst = Vector3.Distance(mouseInfo.position, currentShape.points[i]);
                    if (dst < closestPointDst)
                    {
                        closestPointDst = dst;
                        closestPoint = currentShape.points[i];
                    }
                }
            }
        }

        int mouseOverLineIndex = -1;
        float closestLineDst = regionCreator.handleRadius;
        Vector3 closestPositionOnLine = mouseInfo.position;
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            if (shapeIndex != regionId)
            {
                Shape currentShape = regionCreator.shapes[shapeIndex];
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPointInRegion = currentShape.points[(i + 1) % currentShape.points.Count];

                    float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mouseInfo.position.ToXY(),
                        currentShape.points[i].ToXY(), nextPointInRegion.ToXY());

                    if (dstFromMouseToLine <= closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;

                        Vector3 line = nextPointInRegion - currentShape.points[i];
                        Vector3 linePerpendicularDir = Vector3.Normalize(Vector3.Cross(line, new Vector3(0, 0, 1)));
                        closestPositionOnLine = mouseInfo.position + linePerpendicularDir * dstFromMouseToLine;
                    }
                }
            }
        }

        if (closestPointDst < regionCreator.handleRadius)
            return closestPoint;
        else
            return closestPositionOnLine;
    }

    private void DragPoint()
    {
        if (selectedPointInfo.isSelected)
        {
            //bool pointWasFound = false;
            //float closestToTheMouseDst = regionCreator.handleRadius;
            //Vector3 closestPoint = FindClosestToMousePointOnLine(0);
            //for (int i = 1; i < regionCreator.shapes.Count; i++)
            //{
            //    Vector3 point = FindClosestToMousePointOnLine(i);
            //    float dst = Vector3.Distance(mouseInfo.position, point);

            //    if (dst < closestToTheMouseDst)
            //    {
            //        closestToTheMouseDst = dst;
            //        closestPoint = point;
            //    }
            //}

            //SelectedShape.points[selectedPointInfo.id] = selectedLineInfo.closestToMousePoint;

            

            SelectedShape.points[selectedPointInfo.id] = FindClosestToMousePointOnLineOrPointExeptRegion(selectedRegionInfo.id);
            regionChangedSinceLastRepaint = true;
            //selectedPointInfo.point = mouseInfo.position;
            //regionChangedSinceLastRepaint = true;
        }
    }

    private void UpdateMouseOverInfo()
    {
        int mouseOverPointIndex = -1;
        int mouseOverShapeIndex = -1;
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            Shape currentShape = regionCreator.shapes[shapeIndex];
            for (int i = 0; i < currentShape.points.Count; i++)
            {
                if (Vector3.Distance(mouseInfo.position, currentShape.points[i]) < regionCreator.handleRadius)
                {
                    mouseOverPointIndex = i;
                    mouseOverShapeIndex = shapeIndex;
                    break;
                }
            }
        }

        if (mouseOverPointIndex != selectedPointInfo.id || mouseOverShapeIndex != mouseInfo.underMouseRegionInfo.id)
        {
            mouseInfo.underMouseRegionInfo.id = mouseOverShapeIndex;
            mouseInfo.underMousePointInfo.regionInfo = mouseInfo.underMouseRegionInfo;
            mouseInfo.underMousePointInfo.id = mouseOverPointIndex;
            mouseInfo.isOverPoint = mouseOverPointIndex != -1;
            regionChangedSinceLastRepaint = true;
        }

        if (mouseInfo.isOverPoint)
        {
            mouseInfo.isOverLine = false;
            selectedLineInfo.id = -1;
        }
        else
        {
            int mouseOverLineIndex = -1;
            float closestLineDst = regionCreator.handleRadius;
            Vector3 closestPositionOnLine = mouseInfo.position;
            for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
            {
                Shape currentShape = regionCreator.shapes[shapeIndex];
                for (int i = 0; i < currentShape.points.Count; i++)
                {
                    Vector3 nextPointInRegion = currentShape.points[(i + 1) % currentShape.points.Count];

                    float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mouseInfo.position.ToXY(),
                        currentShape.points[i].ToXY(), nextPointInRegion.ToXY());

                    if (dstFromMouseToLine <= closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;
                        mouseOverShapeIndex = shapeIndex;

                        Vector3 line = nextPointInRegion - currentShape.points[i];
                        Vector3 linePerpendicularDir = Vector3.Normalize(Vector3.Cross(line, new Vector3(0, 0, 1)));
                        closestPositionOnLine = mouseInfo.position + linePerpendicularDir * dstFromMouseToLine;
                    }
                }
            }

            if (selectedLineInfo.id != mouseOverLineIndex || mouseOverShapeIndex != mouseInfo.underMouseRegionInfo.id)
            {
                mouseInfo.underMouseRegionInfo.id = mouseOverShapeIndex;
                selectedLineInfo.id = mouseOverLineIndex;
                mouseInfo.isOverLine = mouseOverLineIndex != -1;
                selectedLineInfo.closestToMousePoint = closestPositionOnLine;
                regionChangedSinceLastRepaint = true;
            }
        }
    }

    private void Draw()
    {
        for (int shapeIndex = 0; shapeIndex < regionCreator.shapes.Count; shapeIndex++)
        {
            Shape shapeToDraw = regionCreator.shapes[shapeIndex];
            bool shapeIsSelected = shapeIndex == selectedRegionInfo.id;
            bool mouseIsOverShape = shapeIndex == mouseInfo.underMouseRegionInfo.id;
            UnityEngine.Color deselectedShapeColor = UnityEngine.Color.gray;

            for (int i = 0; i < shapeToDraw.points.Count; i++)
            {
                Vector3 nextPoint = shapeToDraw.points[(i + 1) % shapeToDraw.points.Count];

                if (i == selectedLineInfo.id && mouseIsOverShape)
                {
                    Handles.color = UnityEngine.Color.red;
                    Handles.DrawLine(shapeToDraw.points[i], nextPoint);
                }
                else
                {
                    Handles.color = (shapeIsSelected) ? UnityEngine.Color.black : deselectedShapeColor;
                    Handles.DrawDottedLine(shapeToDraw.points[i], nextPoint, 4);
                }

                if (i == mouseInfo.underMousePointInfo.id && mouseIsOverShape)
                    Handles.color = selectedPointInfo.isSelected ? UnityEngine.Color.black : UnityEngine.Color.red;
                else
                    Handles.color = (shapeIsSelected) ? UnityEngine.Color.white : deselectedShapeColor;
                Handles.DrawSolidDisc(shapeToDraw.points[i], Vector3.back, regionCreator.handleRadius);
            }
        }

        if (regionChangedSinceLastRepaint)
            regionCreator.UpdateMeshDisplay();
        regionChangedSinceLastRepaint = false;
    }

    private void OnUndoOrRedo()
    {
        regionChangedSinceLastRepaint = true;

        for (int i = 0; i < regionCreator.shapes.Count; i++)
        {
            if (regionCreator.shapes[i].region == null)
            {
                Region newRegion = AddNewRegion();
                regionCreator.shapes[i].region = newRegion;
            }
            if (regionCreator.shapes[i].needDestroyRegion == true)
            {
                regionCreator.regions.Remove(regionCreator.shapes[i].region);
                DestroyImmediate(regionCreator.shapes[i].region);
                regionCreator.shapes.RemoveAt(i);
            }
        }

        if (selectedRegionInfo.id >= regionCreator.shapes.Count || selectedRegionInfo.id == -1)
            selectedRegionInfo.id = regionCreator.shapes.Count - 1;
    }

    Shape SelectedShape
    {
        get
        {
            return regionCreator.shapes[selectedRegionInfo.id];
        }
    }

    public class LineInfo
    {
        public RegionInfo regionInfo;
        public bool isSelected;
        public int id = -1;
        public PointInfo startPoint;
        public PointInfo endPoint;
        public Vector3 closestToMousePoint;
    }

    public class PointInfo
    {
        public bool isSelected;
        public RegionInfo regionInfo;
        public int id = -1;

        public void Unselect()
        {
            isSelected = false;
            id = -1;
        }
        public Vector3 point
        {
            get
            {
                return regionInfo.region.points[id];
            }
            set 
            {
                regionInfo.region.points[id] = value;
            }
        }
    }
    public class RegionInfo
    {
        private RegionCreator regionCreator;
        public bool isSelected;
        public int id = -1;
        public Shape region 
        {
            set
            {
                regionCreator.shapes[id] = value;
            }
            get
            {
                return regionCreator.shapes[id];
            }
        }

        public RegionInfo(RegionCreator regionCreator)
        {
            this.regionCreator = regionCreator;
        }
    }

    public class MouseInfo
    {
        private RegionCreator regionCreator;

        public Vector3 position;
        public Vector3 positionAtStartOfDrag;

        public bool isOverRegion;
        public bool isOverLine;
        public bool isOverPoint;

        public RegionInfo underMouseRegionInfo;
        public LineInfo underMouseLineInfo = new LineInfo();
        public PointInfo underMousePointInfo = new PointInfo();

        public void UpdateMousePosition(Event guiEvent)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float drawPlaneZ = 0;
            float dstToDrawPlane = (drawPlaneZ - mouseRay.origin.z) / mouseRay.direction.z;
            position = mouseRay.origin + mouseRay.direction * dstToDrawPlane;
        }

        public MouseInfo(RegionCreator regionCreator)
        {
            underMouseRegionInfo = new RegionInfo(regionCreator);
        }
    }
}
#endif