using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
[CustomEditor(typeof(CTerrainRampGenerator))]
public class Edit_TerrainSlope : Editor
{
    CTerrainRampGenerator select = null;

    private void OnEnable()
    {
        select = target as CTerrainRampGenerator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("MAKE SLOPE")) 
        { select.GenerateRamp(); }
    }
}
