using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(RegionsSystem))]
public class RegionEditor2 : Editor
{
    private RegionsSystem regionsSystem;

    public UnityEngine.Color selectedRegionSerdColor;
    public UnityEngine.Color unselectedRegionSerdColor;
    public UnityEngine.Color undermousePointColor;

    private List<RegionSerd> regionSerds;
    private RegionSerd selectedRegionSerd;

    private bool repaintQueried;

    private int oldRegionSerdsCount;

    private Vector3 mousePosition;

    private ClosestPointInfo draggedPoint;
    private Vector3 startOfDraggingPosition;

    private void OnEnable()
    {
        selectedRegionSerdColor = new UnityEngine.Color(1.0f, 1.0f, 1.0f);
        unselectedRegionSerdColor = new UnityEngine.Color(0.0f, 0.0f, 0.0f);
        undermousePointColor = new UnityEngine.Color(0.6f, 0.6f, 0.6f);

        startOfDraggingPosition = new Vector3();

        regionsSystem = target as RegionsSystem;
        regionSerds = regionsSystem.regionSerds;

        Undo.undoRedoPerformed += OnUndoOrRedo;
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoOrRedo;
        Tools.hidden = false;
    }

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        CheckRegionAndRegionSerdsMatching();
        UpdateSelectedRegionSerdIfItIsIncorrect();
        DeleteRegionSerdsThatHaveNoPoints();

        //UpdateDraggedPointPosition();

        if (guiEvent.type == EventType.Repaint)
            RepaintGraphics();
        else if (guiEvent.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // Can't be unselected
        else
        {
            HandleDevicesInput(guiEvent);
            if (repaintQueried)
            {
                HandleUtility.Repaint();
                repaintQueried = false;
            }
        }
    }

    public void OnUndoOrRedo()
    {
        UpdateRegionAndRegionSerdsMatching();
        QueryGraphicsRepaint();
    }

    private void DrawPointsAndLines()
    {
        List<ClosestPointInfo> pointsInfo = GetPointsUnderMouse();
        List<ClosestLineInfo> lineInfo = GetClosestLinesInfoClosserThan(regionsSystem.handleRadius, GetMousePosition());

        for (int i = 0; i < regionSerds.Count; i++)
        {
            RegionSerd currentRegionSerd = regionSerds[i];
            for (int j = 0; j < regionSerds[i].points.Count; j++)
            {
                int nextPointId = (j + 1) % currentRegionSerd.points.Count;
                Vector3 currentPoint = currentRegionSerd.points[j];
                Vector3 nextPoint = currentRegionSerd.points[nextPointId];

                if (regionSerds[i] == selectedRegionSerd)
                    Handles.color = selectedRegionSerdColor;
                else
                    Handles.color = unselectedRegionSerdColor;


                UnityEngine.Color lastColor = Handles.color;

                for (int k = 0; k < lineInfo.Count; k++)
                {
                    if (lineInfo[k].regionSerdId == i && lineInfo[k].p0Id == j)
                    {
                        Handles.color = new UnityEngine.Color(0, 0, 1.0f);
                    }
                }
                Handles.DrawDottedLine(currentPoint, nextPoint, 4);

                Handles.color = lastColor;

                for (int k = 0; k < pointsInfo.Count; k++)
                    if (pointsInfo[k].regionSerdId == i && pointsInfo[k].pointId == j)
                    {
                        Handles.color = undermousePointColor;
                        break;
                    }

                Handles.DrawSolidDisc(currentPoint, Vector3.back, regionsSystem.handleRadius);
            }
        }
    }

    private void RepaintGraphics()
    {
        DrawPointsAndLines();
        for (int i = 0; i < regionSerds.Count; i++)
            regionSerds[i].UpdateMesh();
    }

    public void QueryGraphicsRepaint()
    {
        repaintQueried = true;
    }

    private void HandleDevicesInput(Event guiEvent)
    {
        UpdateMousePosition(guiEvent);

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
            CreatePointOnMousePositionInSelectedRegionSerd();

        if (onlyShiftLeftMouseDown)
        {
            List<ClosestPointInfo> pointsUnderMouse = GetPointsUnderMouse();
            if (pointsUnderMouse.Count > 0)
            {
                for (int i = 0; i < pointsUnderMouse.Count; i++)
                    pointsUnderMouse[i].regionSerd.points.RemoveAt(pointsUnderMouse[i].pointId);
            }
            else 
            {
                RegionSerd regionSerd = CreateRegionSerd();
                SelectRegionSerd(regionSerd);
                CreatePointOnMousePositionInSelectedRegionSerd();
            }
        }

        if (leftMouseDrag)
        {
            UpdateDraggedPointPosition();
            QueryGraphicsRepaint();
        }

        if (leftMouseUp)
        {
            if (draggedPoint != null)
            {
                Vector3 tempPos = draggedPoint.regionSerd.points[draggedPoint.pointId];

                draggedPoint.regionSerd.points[draggedPoint.pointId] = startOfDraggingPosition;
                Undo.RecordObject(regionsSystem, "Move a point");
                draggedPoint.regionSerd.points[draggedPoint.pointId] = tempPos;
                draggedPoint = null;
            }
        }
    }

    public RegionSerd CreateRegionSerd()
    {
        Region region = InstantiateNewRegion();
        region.SetColor(regionsSystem.nextRegionColor);
        RegionSerd regionSerd = new RegionSerd(region);

        Undo.RecordObject(regionsSystem, "Add new regionSerd");

        regionSerds.Add(regionSerd);
        return regionSerd;
    }

    public void SelectRegionSerd(RegionSerd regionSerd)
    {
        selectedRegionSerd = regionSerd;
    }

    public void CreatePointInSelectedRegionSerd(Vector3 position)
    {
        Undo.RecordObject(regionsSystem, "Add new point to selected region");
        CreatePoint(position, selectedRegionSerd);
    }

    // The name doesn't match its content
    public void CreatePointOnMousePositionInSelectedRegionSerd()
    {
        if (regionsSystem.regionSerds.Count <= 0)
        {
            RegionSerd regionSerd = CreateRegionSerd();
            SelectRegionSerd(regionSerd);
        }

        List<ClosestPointInfo> pointsUnderMouse = GetPointsUnderMouse();
        List<ClosestLineInfo> linesInfoUnderMouse = GetLinesInfoUnderMouse();
        if (pointsUnderMouse.Count <= 0)
        {
            if (linesInfoUnderMouse.Count <= 0)
                CreatePointOnMousePosition(selectedRegionSerd);
            else
            {
                SelectRegionSerd(linesInfoUnderMouse[0].regionSerd);
                CreatePoint(GetMousePosition(), linesInfoUnderMouse[0].regionSerd, linesInfoUnderMouse[0].p1Id);
                if (draggedPoint == null)
                {
                    startOfDraggingPosition = GetMousePosition();
                    draggedPoint = new ClosestPointInfo(GetMousePosition(),
                        linesInfoUnderMouse[0].regionSerd, 0, GetMousePosition(),
                        linesInfoUnderMouse[0].regionSerdId, linesInfoUnderMouse[0].p1Id);
                }
            }
        }
        else
        {
            if (draggedPoint == null)
            {
                startOfDraggingPosition = pointsUnderMouse[0].point;
                SelectRegionSerd(pointsUnderMouse[0].regionSerd);
                draggedPoint = pointsUnderMouse[0];
            }
        }
        QueryGraphicsRepaint();
    }

    public void CreatePointOnMousePosition(RegionSerd regionSerd)
    {
        CreatePoint(GetMousePosition(), regionSerd);
    }

    public void CreatePoint(Vector3 position, RegionSerd regionSerd, int lineId = -1)
    {
        Undo.RecordObject(regionsSystem, "Add point");
        if (lineId == -1)
            regionSerd.points.Add(position);
        else
            regionSerd.points.Insert(lineId, position);
    }

    public void UpdateRegionAndRegionSerdsMatching()
    {
        foreach (Transform regionTransform in regionsSystem.gameObject.transform)
        {
            bool foundRegion = false;
            for (int i = 0; i < regionSerds.Count; i++)
            {
                if (regionTransform.GetComponent<Region>() == regionSerds[i].region)
                {
                    foundRegion = true;
                    break;
                }
            }

            if (!foundRegion)
                DestroyImmediate(regionTransform.gameObject);
        }

        for (int i = 0; i < regionSerds.Count; i++)
        {
            if (regionSerds[i].region == null)
                regionSerds[i].region = InstantiateNewRegion();
        }

        oldRegionSerdsCount = regionSerds.Count;
    }

    public void UpdateMousePosition(Event guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneZ = 0;
        float dstToDrawPlane = (drawPlaneZ - mouseRay.origin.z) / mouseRay.direction.z;
        mousePosition = mouseRay.origin + mouseRay.direction * dstToDrawPlane;
    }

    public Vector3 GetMousePosition()
    {
        return mousePosition;
    }

    private void CheckRegionAndRegionSerdsMatching()
    {
        if (oldRegionSerdsCount != regionSerds.Count)
            UpdateRegionAndRegionSerdsMatching();
    }

    private Region InstantiateNewRegion()
    {
        GameObject regionGameObject = Instantiate(regionsSystem.regionPrefab, regionsSystem.transform);
        Region region = regionGameObject.GetComponent<Region>();
        return region;
    }

    private void UpdateSelectedRegionSerdIfItIsIncorrect()
    {
        bool foundMatch = false;
        for (int i = 0; i < regionsSystem.regionSerds.Count; i++)
        {
            if (selectedRegionSerd == regionsSystem.regionSerds[i])
            {
                foundMatch = true;
                break;
            }
        }

        if (!foundMatch)
        {
            if (regionsSystem.regionSerds.Count > 0)
                selectedRegionSerd = regionsSystem.regionSerds[regionsSystem.regionSerds.Count - 1];
            else
                selectedRegionSerd = null;
        }
    }

    private List<ClosestPointInfo> GetPointsUnderMouse()
    {
        return GetClosestPointsCloserThenDistance(regionsSystem.handleRadius, GetMousePosition());
    }

    private ClosestPointInfo GetClosestPoint(Vector3 position)
    {
        return GetClosestPoints(1, position)[0];
    }

    private List<ClosestPointInfo> GetClosestPoints(int deep, Vector3 position)
    {
        int pointsCountFromAllRegionSerds = GetPointsCountFromAllRegionSerds();
        if (deep > pointsCountFromAllRegionSerds) deep = pointsCountFromAllRegionSerds;

        List<ClosestPointInfo> closestPoints = new List<ClosestPointInfo>();

        for (int z = 0; z < deep; z++)
        {
            float minDistance = 3.4e+38f;
            ClosestPointInfo pointInfo = new ClosestPointInfo();
            for (int x = 0; x < regionSerds.Count; x++)
                for (int y = 0; y < regionSerds[x].points.Count; y++)
                {
                    float distance = Vector3.Distance(regionSerds[x].points[y], position);
                    if (distance < minDistance && (z == 0 ? true : distance > closestPoints[z - 1].distanceToTarget))
                    {
                        minDistance = distance;
                        pointInfo.point = regionSerds[x].points[y];
                        pointInfo.regionSerd = regionSerds[x];
                        pointInfo.regionSerdId = x;
                        pointInfo.pointId = y;
                    }
                }
            closestPoints.Add(new ClosestPointInfo(pointInfo.point, pointInfo.regionSerd, minDistance, position, pointInfo.regionSerdId, pointInfo.pointId));
        }
        return closestPoints;
    }

    private List<ClosestPointInfo> GetClosestPointsCloserThenDistance(float distance, Vector3 position)
    {
        int pointsCountFromAllRegionSerds = GetPointsCountFromAllRegionSerds();
        List<ClosestPointInfo> closestPoints = new List<ClosestPointInfo>();

        for (int z = 0; z < pointsCountFromAllRegionSerds; z++)
        {
            float minDistance = 3.4e+38f;
            ClosestPointInfo pointInfo = new ClosestPointInfo();
            for (int x = 0; x < regionSerds.Count; x++)
                for (int y = 0; y < regionSerds[x].points.Count; y++)
                {
                    float foundDistance = Vector3.Distance(regionSerds[x].points[y], position);
                    //if (foundDistance < minDistance && (z == 0 ? true : foundDistance > closestPoints[z - 1].distanceToTarget))
                    if (foundDistance < minDistance && (z == 0 ? true : foundDistance > closestPoints[closestPoints.Count - 1].distanceToTarget))
                    {
                        minDistance = foundDistance;
                        pointInfo.point = regionSerds[x].points[y];
                        pointInfo.regionSerd = regionSerds[x];
                        pointInfo.regionSerdId = x;
                        pointInfo.pointId = y;
                    }
                }

            if (minDistance > distance)
                break;

            closestPoints.Add(new ClosestPointInfo(pointInfo.point, pointInfo.regionSerd, minDistance, position, pointInfo.regionSerdId, pointInfo.pointId));
        }
        return closestPoints;
    }

    private Vector3 GetFirstPointFromAllRegionSerds()
    {
        for (int i = 0; i < regionSerds.Count; i++)
        {
            if (regionSerds[i].points.Count > 0)
                return regionSerds[i].points[0];
        }
        return new Vector3(0, 0, 0);
    }

    private int GetPointsCountFromAllRegionSerds()
    {
        int count = 0;
        for (int i = 0; i < regionSerds.Count; i++)
            count += regionSerds[i].points.Count;
        return count;
    }

    private void DeleteRegionSerdsThatHaveNoPoints()
    {
        for (int i = 0; i < regionSerds.Count; i++)
        {
            if (regionSerds[i].points.Count == 0)
            {
                regionSerds.RemoveAt(i);
                i--;
            }
        }
    }

    private void UpdateDraggedPointPosition()
    {
        if (draggedPoint != null)
        {
            // Here we're trying to figure out wether there's any point or line near our mouse or not

            List<ClosestPointInfo> pointsInfoUnderMouse = GetPointsUnderMouse();
            List<ClosestLineInfo> linesInfoUnderMouse = GetLinesInfoUnderMouse();

            int closestPointFromAnotherRegionId = -1;
            for (int i = 0; i < pointsInfoUnderMouse.Count; i++)
                if (pointsInfoUnderMouse[i].regionSerd != selectedRegionSerd)
                    closestPointFromAnotherRegionId = i;

            int closestLineFromAnotherRegionId = -1;
            if (closestPointFromAnotherRegionId < 0)
            {
                for (int i = 0; i < linesInfoUnderMouse.Count; i++)
                    if (linesInfoUnderMouse[i].regionSerd != selectedRegionSerd)
                        closestLineFromAnotherRegionId = i;
            }

            if (closestPointFromAnotherRegionId >= 0)
                draggedPoint.regionSerd.points[draggedPoint.pointId] = pointsInfoUnderMouse[closestPointFromAnotherRegionId].point;
            else if (closestLineFromAnotherRegionId >= 0)
            {
                draggedPoint.regionSerd.points[draggedPoint.pointId] = 
                    linesInfoUnderMouse[closestLineFromAnotherRegionId].closestPositionOnLine;
            }
            else
                draggedPoint.regionSerd.points[draggedPoint.pointId] = GetMousePosition();
        }
    }

    private List<ClosestLineInfo> GetLinesInfoUnderMouse()
    {
        List<ClosestLineInfo> closestLines = GetClosestLinesInfoClosserThan(regionsSystem.handleRadius, GetMousePosition());
        return closestLines;
    }

    private List<ClosestLineInfo> GetClosestLinesInfoClosserThan(float distance, Vector3 position)
    {
        int linesCountFromAllRegionSerds = GetLinesCountFromAllRegionSerds();
        List<ClosestLineInfo> closestLinesInfo = new List<ClosestLineInfo>();

        ClosestLineInfo closestLineInfo = new ClosestLineInfo();
        Vector3 closestPosition;
        float minDistance = 3.4e+38f;
        for (int k = 0; k < linesCountFromAllRegionSerds; k++)
        {
            minDistance = 3.4e+38f;
            bool found = false;
            for (int i = 0; i < regionSerds.Count; i++)
            {
                if (regionSerds[i].points.Count > 1)
                {
                    int pointsCountToLoopOver;
                    if (regionSerds[i].points.Count == 2)
                        pointsCountToLoopOver = 1;
                    else
                        pointsCountToLoopOver = regionSerds[i].points.Count;

                    for (int j = 0; j < pointsCountToLoopOver; j++)
                    {
                        int indexForB = (j + 1) % regionSerds[i].points.Count;
                        Vector3 a = regionSerds[i].points[j];
                        Vector3 b = regionSerds[i].points[indexForB];
                        Vector3 point = position;

                        Vector3 closestPositionOnLine = GetClosestPositionOnLine(point, a, b);
                        float foundDistance = Vector3.Distance(closestPositionOnLine, point);
                        
                        if (foundDistance < minDistance && foundDistance < distance && (k == 0 ? true : foundDistance > closestLinesInfo[closestLinesInfo.Count - 1].minDistanceFromTargetToLine))
                        {
                            closestPosition = closestPositionOnLine;
                            minDistance = foundDistance;

                            closestLineInfo.p0 = a;
                            closestLineInfo.p1 = b;
                            closestLineInfo.minDistanceFromTargetToLine = foundDistance;
                            closestLineInfo.closestPositionOnLine = closestPositionOnLine;
                            closestLineInfo.target = point;
                            closestLineInfo.regionSerd = regionSerds[i];
                            closestLineInfo.regionSerdId = i;
                            closestLineInfo.p0Id = j;
                            closestLineInfo.p1Id = indexForB;
                            found = true;
                        }
                    }
                }
            }
            if (found)
                closestLinesInfo.Add(new ClosestLineInfo(closestLineInfo));
        }
        return closestLinesInfo;
    }

    private ClosestLineInfo GetClosestLineInfo(Vector3 position)
    {
        List<ClosestLineInfo> closestLineInfo = GetClosestLinesInfo(1, position);
        return closestLineInfo[0];
    }

    // Doesn't work correctly if two lines have the same closest distance
    private List<ClosestLineInfo> GetClosestLinesInfo(int deep, Vector3 position)
    {
        int linesCountFromAllRegionSerds = GetLinesCountFromAllRegionSerds();
        if (deep > linesCountFromAllRegionSerds)
            deep = linesCountFromAllRegionSerds;

        List<ClosestLineInfo> closestLinesInfo = new List<ClosestLineInfo>();


        ClosestLineInfo closestLineInfo = new ClosestLineInfo();
        Vector3 closestPosition;
        float minDistance = 3.4e+38f;
        for (int k = 0; k < deep; k++)
        {
            minDistance = 3.4e+38f;
            for (int i = 0; i < regionSerds.Count; i++)
            {
                if (regionSerds[i].points.Count > 1)
                {
                    int pointsCountToLoopOver;
                    if (regionSerds[i].points.Count == 2)
                        pointsCountToLoopOver = 1;
                    else
                        pointsCountToLoopOver = regionSerds[i].points.Count;

                    for (int j = 0; j < pointsCountToLoopOver; j++)
                    {
                        int indexForB = (j + 1) % regionSerds[i].points.Count;
                        Vector3 a = regionSerds[i].points[j];
                        Vector3 b = regionSerds[i].points[indexForB];
                        Vector3 point = position;

                        Vector3 closestPositionOnLine = GetClosestPositionOnLine(point, a, b);
                        float distance = Vector3.Distance(closestPositionOnLine, point);
                        if (distance < minDistance && (k == 0 ? true : distance > closestLinesInfo[k - 1].minDistanceFromTargetToLine))
                        {
                            closestPosition = closestPositionOnLine;
                            minDistance = distance;

                            closestLineInfo.p0 = a;
                            closestLineInfo.p1 = b;
                            closestLineInfo.minDistanceFromTargetToLine = distance;
                            closestLineInfo.closestPositionOnLine = closestPositionOnLine;
                            closestLineInfo.target = point;
                            closestLineInfo.regionSerd = regionSerds[i];
                            closestLineInfo.regionSerdId = i;
                            closestLineInfo.p0Id = j;
                            closestLineInfo.p1Id = indexForB;
                        }
                    }
                }
            }
            closestLinesInfo.Add(new ClosestLineInfo(closestLineInfo));
        }
        return closestLinesInfo;
    }


    private float GetMinDistanceToLine(Vector3 point, Vector3 lineOrigin, Vector3 lineEnd)
    {
        Vector3 lineVec = lineEnd - lineOrigin;
        Vector3 originToPointVec = point - lineOrigin;

        Vector3 lineDir = Vector3.Normalize(lineVec);
        Vector3 originToPointDir = Vector3.Normalize(originToPointVec);

        float distanceFromPointToLineOrigin = originToPointVec.magnitude;

        float cos = Vector3.Dot(lineDir, originToPointDir);
        float sin = 1 - (cos * cos);

        float minDistance = sin * distanceFromPointToLineOrigin;

        return minDistance;
    }

    private Vector3 GetClosestPositionOnLine(Vector3 point, Vector3 lineOrigin, Vector3 lineEnd)
    {
        Vector3 lineVec = lineEnd - lineOrigin;
        Vector3 originToPointVec = point - lineOrigin;

        Vector3 lineDir = Vector3.Normalize(lineVec);
        Vector3 originToPointDir = Vector3.Normalize(originToPointVec);

        float distanceFromPointToLineOrigin = originToPointVec.magnitude;

        float cos = Vector3.Dot(lineDir, originToPointDir);

        float t = distanceFromPointToLineOrigin * cos;

        Vector3 closestPointOnLine;
        if (t <= 0)
            closestPointOnLine = lineOrigin;
        else if (t >= Vector3.Distance(lineOrigin, lineEnd))
            closestPointOnLine = lineEnd;
        else
            closestPointOnLine = lineOrigin + lineDir * t;

        return closestPointOnLine;
    }

    private int GetLinesCountFromAllRegionSerds()
    {
        int count = 0;
        for (int i = 0; i < regionSerds.Count; i++)
        {
            if (regionSerds[i].points.Count == 2)
                count++;
            else if (regionSerds[i].points.Count > 2)
                count += regionSerds[i].points.Count;
        }
        return count;
    }

    public class ClosestLineInfo
    {
        public Vector3 p0;
        public Vector3 p1;

        public float minDistanceFromTargetToLine;
        public Vector3 closestPositionOnLine;

        public Vector3 target;

        public RegionSerd regionSerd;
        public int regionSerdId = 0;
        public int p0Id = 0;
        public int p1Id = 0;

        public ClosestLineInfo()
        {

        }

        public ClosestLineInfo(ClosestLineInfo info)
        {
            p0 = info.p0;
            p1 = info.p1;
            minDistanceFromTargetToLine = info.minDistanceFromTargetToLine;
            closestPositionOnLine = info.closestPositionOnLine;
            target = info.target;
            regionSerd = info.regionSerd;
            regionSerdId = info.regionSerdId;
            p0Id = info.p0Id;
            p1Id = info.p1Id;
        }

    }

    public class ClosestPointInfo
    {
        public Vector3 point;
        public RegionSerd regionSerd;
        public float distanceToTarget;
        public Vector3 target;

        public int regionSerdId = 0;
        public int pointId = 0;

        public ClosestPointInfo()
        {

        }

        public ClosestPointInfo(Vector3 point, RegionSerd regionSerd, float distanceToTarget, Vector3 target, int regionSerdId, int pointId)
        {
            this.point = point;
            this.regionSerd = regionSerd;
            this.distanceToTarget = distanceToTarget;
            this.target = target;
            this.regionSerdId = regionSerdId;
            this.pointId = pointId;
        }
    }
}
#endif
