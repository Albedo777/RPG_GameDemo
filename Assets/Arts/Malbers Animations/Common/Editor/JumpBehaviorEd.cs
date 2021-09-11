using UnityEngine;
using UnityEditor;

namespace MalbersAnimations
{
    [CustomEditor(typeof(JumpBehavior))]
    public class JumpBehaviorEd : Editor
    {
        private JumpBehavior MJB;
        bool activeCustomEditor;

        private void OnEnable()
        {
            MJB = ((JumpBehavior)target);

        }

        public override void OnInspectorGUI()
        {
        
            activeCustomEditor = EditorGUILayout.Toggle(activeCustomEditor);
            
            if (activeCustomEditor)
            {
                DrawDefaultInspector();
                return;
            } 
            serializedObject.Update(); 

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("This Script manage all the Jump Behavior, Deactivate the PosY Constraints(RigidBody) when is in Air", MessageType.None);
            EditorGUILayout.EndVertical();
            //  


            GUIStyle b = new GUIStyle();
            b.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            EditorGUILayout.LabelField(new GUIContent("Jumping Momentum", "Range in the animation when the character is in the Air"), b);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            {
                //   MJB.startJump = EditorGUILayout.FloatField(MJB.startJump, GUILayout.Width(50));
                EditorGUILayout.LabelField(string.Format("{0:0.###}", MJB.startJump), GUILayout.Width(40));

                float startjump = MJB.startJump;
                float finishjump = MJB.finishJump;

                EditorGUILayout.MinMaxSlider(ref startjump, ref finishjump, -0.01f, 1.01f);

                if (startjump != MJB.startJump)
                {
                    Undo.RecordObject(MJB, "Changed Jump Range");
                    MJB.startJump = startjump;
                }
                if (finishjump != MJB.finishJump)
                {
                    Undo.RecordObject(MJB, "Changed Jump Range");
                    MJB.finishJump = finishjump;
                }


                //   MJB.finishJump = EditorGUILayout.FloatField(MJB.finishJump, GUILayout.Width(50));
                EditorGUILayout.LabelField(string.Format("{0:0.###}", MJB.finishJump), GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Fall Ray", GUILayout.Width(50));
                MJB.FallRay = EditorGUILayout.FloatField(MJB.FallRay);
                EditorGUILayout.LabelField(new GUIContent("      Treshold", "Max value when is finding a lower jumping down point (greater than that will activate Fall transition  )"), GUILayout.Width(80));
                MJB.treshold = EditorGUILayout.FloatField(MJB.treshold);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUILayout.LabelField(new GUIContent("Cliff Momentum (IDInt = 110)", "Range in the animation when the character Can Jump over a Cliff"), b);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("{0:0.###}", MJB.startEdge), GUILayout.Width(40));

                float startEdge = MJB.startEdge;
                float finishEdge = MJB.finishEdge;

                EditorGUILayout.MinMaxSlider(ref startEdge, ref finishEdge, 0, 1);


                if (startEdge != MJB.startEdge)
                {
                    Undo.RecordObject(MJB, "Changed Edge Range");
                    MJB.startEdge = startEdge;
                }
                if (finishEdge != MJB.finishEdge)
                {
                    Undo.RecordObject(MJB, "Changed Edge Range");
                    MJB.finishEdge = finishEdge;
                }


                EditorGUILayout.LabelField(string.Format("{0:0.###}", MJB.finishEdge), GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();
                MJB.GroundRay = EditorGUILayout.FloatField("Ground Ray", MJB.GroundRay);
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();

        }


    }
}