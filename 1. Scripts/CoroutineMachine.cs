using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomCoroutine
{
    // todo: ...Check if this is more effective to run coroutines.
    public class CoroutineMachine : MonoBehaviour
    {
        public static CoroutineMachine Instance => instance;
        private static CoroutineMachine instance;

        public new CoroutineInfo StartCoroutine(IEnumerator routine)
        {
            CoroutineInfo coroutineInfo = new CoroutineInfo(base.StartCoroutine(routine));

            return coroutineInfo;
        }

        public bool StopCoroutine(CoroutineInfo coroutine)
        {
            if (coroutine.IsRunning)
            {
                StopCoroutine(coroutine.Coroutine);
                coroutine.OnCouroutineEnded();

                return true;
            }
            
            return false;
        }

        
    }

    public struct CoroutineInfo
    {
        public Coroutine Coroutine { get; private set; }
        public bool IsRunning => Coroutine != null;

        public CoroutineInfo(Coroutine coroutine)
        {
            Coroutine = coroutine;
        }

        // todo: Stop user from getting a chance to call this function.
        public void OnCouroutineEnded()
        {
            Coroutine = null;
        }
    }
}
