using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(TestSystem))]
public class TestEditor : Editor
{
    private TestSystem testSystem;

    private TestObject selectedTestObject;
    private void OnEnable()
    {
        testSystem = target as TestSystem;
    }

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
            RepaintGraphics();
        else if (guiEvent.type == EventType.Layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // Can't be unselected
        else
        {
            bool onlyLeftMouseDown = guiEvent.type == EventType.MouseDown && 
                guiEvent.button == 0 && 
                guiEvent.modifiers == EventModifiers.None;

            if (onlyLeftMouseDown)
            {
                if (testSystem.testObjects.Count <= 0)
                {
                    TestObject newTestObject = new TestObject();
                    testSystem.testObjects.Add(newTestObject);

                    selectedTestObject = newTestObject;
                }

                Undo.RecordObject(testSystem, "Add testObject");

                int pointsCount = selectedTestObject.points.Count;
                selectedTestObject.points.Add(new Vector3(pointsCount + 1, pointsCount + 2, pointsCount + 3));

                Debug.Log(selectedTestObject.points.Count);
            }
        }
    }

    public void RepaintGraphics()
    {

    }
}
#endif