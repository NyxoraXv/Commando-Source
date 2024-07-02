using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(ProceduralLevelGenerator))]
public class LevelGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ProceduralLevelGenerator levelGenerator = (ProceduralLevelGenerator)target;
        if (GUILayout.Button("Generate Level"))
        {
            levelGenerator.generate();
        };
        if (GUILayout.Button("Clear"))
        {
            levelGenerator.ClearTiles();
        };

    }
}