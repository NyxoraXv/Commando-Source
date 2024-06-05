using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;


[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LevelGenerator levelGenerator = (LevelGenerator)target;
        if (GUILayout.Button("Generate Level"))
        {
            levelGenerator.generateLevel();
        };
        
    }
}