using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions {

    public static Vector3 GetWorldPositionOnPlane(this Vector3 original, Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

}
