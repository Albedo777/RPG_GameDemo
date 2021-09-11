using UnityEngine;
using System.Collections;
using UnityEditor;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(ActiveMesh))]
    public class ActiveMeshEditor : Editor
    {
         ActiveMesh MActiveMesh;
        private SerializedProperty meshes;
        

        private void OnEnable()
        {
            MActiveMesh = ((ActiveMesh)target);
            meshes = serializedObject.FindProperty("meshes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Very basic script to swap between meshes", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(meshes, new GUIContent("Meshes"), true);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("ChangeMesh"))
            {
                MActiveMesh.ToogleMesh();
            }

            if (EditorGUI.EndChangeCheck())
            {
              
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}