using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DD_JAM.LevelGeneration
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelGenerationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update"))
            {
                LevelGenerator gen = target as LevelGenerator;
                gen.Generate();
            }
        }
    }
}