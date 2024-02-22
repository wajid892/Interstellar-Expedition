using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AstroData))]
public class AstroDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AstroData ScriptHandler = (AstroData)target;

        if (GUILayout.Button("Update This Buffer Zones"))
        {
            ScriptHandler.EnforceMyBufferZone();
        }
    }
}
