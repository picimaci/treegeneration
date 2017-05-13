using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TreeGenerator))]
[CanEditMultipleObjects]
public class TreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate"))
        {
            foreach (var t in targets)
            {
                ((TreeGenerator)t).Generate();
            }
        }
    }
}