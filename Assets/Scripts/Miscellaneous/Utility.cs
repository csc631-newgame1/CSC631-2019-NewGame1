using System.Collections.Generic;
using UnityEngine;

public class Utility {

    // Get a number with a Normal distribution
    public static int NextGaussian(float mean, float standard_deviation, float min, float max) {
        float x;
        do {
            x = NextGaussian(mean, standard_deviation);
        } while (x < min || x > max);
        return Mathf.RoundToInt(x);
    }

    private static float NextGaussian(float mean, float standard_deviation) {
        return mean + NextGaussian() * standard_deviation;
    }

    private static float NextGaussian() {
        float v1, v2, s;
        do {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
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
}
