using System.Collections;
using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation;

public class Example : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        Debug.Log("started");
        yield return CoroutineScheduler.Run(ExampleCoroutine());
        Debug.Log("finished");
    }

    private IEnumerator ExampleCoroutine()
    {
        Debug.Log("Coroutine started");
        yield return CoroutineScheduler.Run(ExampleCoroutine2());
        Debug.Log("Coroutine finished");
    }

    private IEnumerator ExampleCoroutine2()
    {
        Debug.Log("Coroutine2 started");
        yield return null;
        Debug.Log("Coroutine2 finished");
    }
}