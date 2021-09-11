using UnityEngine;
using System.Collections;
using UnityEditor;

namespace MalbersAnimations.Utilities
{

    [CustomEditor(typeof(CommentB))]
    public class CommentBEditor : Editor
    {
        private CommentB script { get { return target as CommentB; } }
        GUIStyle style;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            style = EditorStyles.helpBox;
            string text = EditorGUILayout.TextArea(script.text, style);
            if (text != script.text)
            {
                Undo.RecordObject(script, "Edit Comments");
                script.text = text;
            }

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
