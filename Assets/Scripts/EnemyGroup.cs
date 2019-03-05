using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [Header("Random Number of Enemies in Groups")]
    [Tooltip("If true, select minumum value and maximum value for number of enemies to be spawned in the group.")]
    public bool randomRangeNumberOfEnemies = false;
    public int minNumberOfEnemies;
    public int maxNumberOfEnemies;

    [Header("Number of Enemies in Groups")]
    [Tooltip("Select the number of enemies to be spawned in the group.")]
    public int numberOfEnemies;

    [Header("Customize Group based on Power")]
    public bool powerBalance = false;


    public EnemyGroup() {

    }
}
