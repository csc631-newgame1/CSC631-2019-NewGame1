using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

    // Get a number with a Normal distribution
    public static int NextGaussian(float mean, float standard_deviation, float min, float max, System.Random rng) {
        float x;
        do {
            x = NextGaussian(mean, standard_deviation, rng);
        } while (x < min || x > max);
        return Mathf.RoundToInt(x);
    }

    private static float NextGaussian(float mean, float standard_deviation, System.Random rng) {
        return mean + NextGaussian(rng) * standard_deviation;
    }

    private static float NextGaussian(System.Random rng) {
        float v1, v2, s;
        do {
            v1 = 2.0f * rng.Next(0, 100)/100f - 1.0f;
            v2 = 2.0f * rng.Next(0, 100)/100f - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s;
    }

    public static int GetRandomIntWithExclusion(int start, int end, System.Random rng, List<int> exclude) {
        int random = start + rng.Next(0, (end - start + 1 - exclude.Count));
        foreach (int ex in exclude) {
            if (random < ex) {
                break;
            }
            random++;
        }
        return random;
    }

    // To use this static class, call it like this:
    // StaticActionCoroutine.DoCoroutine(MyScript.MyStaticCoroutine());
    public class StaticCoroutine : MonoBehaviour {

        IEnumerator Perform(StaticCoroutine instance, IEnumerator coroutine) {
            yield return StartCoroutine(coroutine);
            Die(instance);
        }

        /// <summary>
        /// Place your lovely static IEnumerator in here and witness magic!
        /// </summary>
        /// <param name="coroutine">Static IEnumerator</param>
        public static void DoCoroutine(StaticCoroutine instance, IEnumerator coroutine) {
            instance.StartCoroutine(instance.Perform(instance, coroutine)); //this will launch the coroutine on our instance
        }

        void Die(StaticCoroutine instance) {
            instance = null;
            Destroy(gameObject);
        }
    }

    public class StaticCoroutineManager {
        public List<StaticCoroutine> coroutines;

        public void init() {
            coroutines = new List<StaticCoroutine>();
        }

        public void StartNewCoroutine(IEnumerator coroutine) {
            string gameObjectName = "StaticCoroutine: " + Time.time;
            StaticCoroutine mInstance = new GameObject(gameObjectName).AddComponent<StaticCoroutine>();
            Debug.Log("New StaticCoroutine: " + gameObjectName);

            StaticCoroutine.DoCoroutine(mInstance, coroutine);
            //coroutines.Add
        }

        void Die(StaticCoroutine instance) {
            //mInstance = null;
            
            //Destroy(instance);
        }
    }

    // To use this static class, call it like this:
    // StaticAnimationCoroutine.DoCoroutine(MyScript.MyStaticCoroutine());
    public class StaticAnimationCoroutine : MonoBehaviour {
        private static StaticAnimationCoroutine mInstance = null;

        private static StaticAnimationCoroutine instance {
            get {
                if (mInstance == null) {
                    mInstance = GameObject.FindObjectOfType(typeof(StaticAnimationCoroutine)) as StaticAnimationCoroutine;

                    if (mInstance == null) {
                        string gameObjectName = "StaticAnimationCoroutine: " + Time.time;
                        mInstance = new GameObject(gameObjectName).AddComponent<StaticAnimationCoroutine>();
                    }
                }
                return mInstance;
            }
        }

        void Awake() {
            if (mInstance == null) {
                mInstance = this as StaticAnimationCoroutine;
            }
        }

        IEnumerator Perform(IEnumerator coroutine) {
            yield return StartCoroutine(coroutine);
            Die();
        }

        /// <summary>
        /// Place your lovely static IEnumerator in here and witness magic!
        /// </summary>
        /// <param name="coroutine">Static IEnumerator</param>
        public static void DoCoroutine(IEnumerator coroutine) {
            instance.StartCoroutine(instance.Perform(coroutine)); //this will launch the coroutine on our instance
        }

        void Die() {
            mInstance = null;
            Destroy(gameObject);
        }

        void OnApplicationQuit() {
            mInstance = null;
        }
    }
}
