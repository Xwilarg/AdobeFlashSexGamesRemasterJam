using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFade : MonoBehaviour
{
    private void Update()
    {
        var newZ = 0.15f;

        this.transform.Rotate(0, 0, newZ, Space.Self);
    }
}
