using System.Collections;
using UnityEngine;

// Since IEnumerator are non-static, we have to create an object and run the coroutine on it
// Once the coroutine is finished, we destroy the object
public class StaticCoroutineManager {
    public void StartNewCoroutine(IEnumerator coroutine) {
        string gameObjectName = "StaticCoroutine: " + Time.time;
        StaticCoroutine mInstance = new GameObject(gameObjectName).AddComponent<StaticCoroutine>();

        StaticCoroutine.DoCoroutine(mInstance, coroutine);
    }
}

public class StaticCoroutine : MonoBehaviour {

    IEnumerator Perform(StaticCoroutine instance, IEnumerator coroutine) {
        yield return StartCoroutine(coroutine);
        Die(instance);
    }

    public static void DoCoroutine(StaticCoroutine instance, IEnumerator coroutine) {
        instance.StartCoroutine(instance.Perform(instance, coroutine)); //this will launch the coroutine on our instance
    }

    void Die(StaticCoroutine instance) {
        instance = null;
        Destroy(gameObject);
    }
}
