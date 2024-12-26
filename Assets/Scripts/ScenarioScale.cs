using UnityEngine;

public class ScenarioScale : MonoBehaviour
{
    public static float staticScale;

    void Awake()
    {
        staticScale = transform.localScale.x;
    }
}