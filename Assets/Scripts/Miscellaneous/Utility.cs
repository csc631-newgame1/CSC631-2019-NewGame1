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

    // To use this static method, call it like this:
    // StaticCoroutine.DoCoroutine(MyScript.MyStaticCoroutine());
    public class StaticCoroutine : MonoBehaviour {
        private static StaticCoroutine mInstance = null;

        private static StaticCoroutine instance {
            get {
                if (mInstance == null) {
                    mInstance = GameObject.FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine;

                    if (mInstance == null) {
                        mInstance = new GameObject("StaticCoroutine").AddComponent<StaticCoroutine>();
                    }
                }
                return mInstance;
            }
        }

        void Awake() {
            if (mInstance == null) {
                mInstance = this as StaticCoroutine;
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
