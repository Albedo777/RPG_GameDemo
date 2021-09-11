using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(IconCreator))]
public class IconCreatorEditor : Editor {

	public override void OnInspectorGUI() {

        Repaint();

        IconCreator iconCreator = ((IconCreator)target);

        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Model preview index", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("<", GUILayout.Width(100))) {
            iconCreator.currentIndex--;
        }
        GUILayout.FlexibleSpace();
        GUILayout.Label((iconCreator.currentIndex + 1).ToString());
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(">", GUILayout.Width(100))) {
            iconCreator.currentIndex++;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUI.color = Color.green;
        if(GUILayout.Button("Create icons")) {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType ("UnityEditor.GameView");
            EditorWindow gameView = EditorWindow.GetWindow (type);
            gameView.Focus ();
            iconCreator.StartCoroutine(iconCreator.CreateIcons());
        }
    }
}
