using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    public class Tweener : MonoBehaviour
    {

        public Transform target;
        public float time;
        public AnimationCurve curve;

        // Use this for initialization

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(MoveToPosition(target.position, time));
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(MoveToPositionEaseIn(target.position, time));
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(MoveToPositionCurve(target.position, time));
            }
        }


        IEnumerator MoveToPosition(Vector3 newPosition, float time)
        {
            float elapsedTime = 0;
            Vector3 startingPos = transform.position;

            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }



        IEnumerator MoveToPositionEaseIn(Vector3 endPos, float time)
        {
            float elapsedtime = 0;
            Vector3 startPos = transform.position;


            while (elapsedtime < time)
            {
                transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, elapsedtime));

                elapsedtime += Time.deltaTime;
                yield return null;
            }
        }


        IEnumerator MoveToPositionCurve(Vector3 endPos, float time)
        {
            float t = 0;
            Vector3 startPos = transform.position;

            while (t < time)
            {
                t += Time.deltaTime;
                float s = t / time;
                transform.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(s));
                yield return null;
            }
        }
    }
}