using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartDataHelper
{
    static Vector3 trackMin = new Vector3(-28, 0, -104.28f),
                   trackMax = new Vector3(82.921f, 0, 96.816f);
    public static Vector2 NormalizePosition(Vector3 position)
    {
        float normalizedX = (position.x - trackMin.x) / (trackMax.x - trackMin.x);
        float normalizedZ = (position.z - trackMin.z) / (trackMax.z - trackMin.z);
        return new Vector2(normalizedX, normalizedZ);
    }
}
