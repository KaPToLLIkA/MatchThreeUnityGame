using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Game
{
    public static class Animations
    {
        public static IEnumerator ScaleAnimator(
            GameObject gameObject, 
            Vector3 target, 
            AnimationCurve curve,
            float speed = 1.0f)
        {
            var start = gameObject.transform.localScale;
            var distance = target - start;
            float maxTime = 1f, elapsedTime = 0f;

            while (elapsedTime < maxTime)
            {
                elapsedTime += Time.deltaTime * speed;
                gameObject.transform.localScale = start + distance * curve.Evaluate(elapsedTime);
                yield return null;
            }

            gameObject.transform.localScale = target;
        }
        
        public static IEnumerator MoveAnimator(
            GameObject gameObject, 
            Vector3 target, 
            AnimationCurve curve,
            float speed = 1.0f)
        {
            var start = gameObject.transform.localPosition;
            var distance = target - start;
            float maxTime = 1f, elapsedTime = 0f;

            while (elapsedTime < maxTime)
            {
                elapsedTime += Time.deltaTime * speed;
                gameObject.transform.localPosition = start + distance * curve.Evaluate(elapsedTime);
                yield return null;
            }

            gameObject.transform.localPosition = target;
        }
    }
}