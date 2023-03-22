using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    private const float bound = .3f;
    private float delta = 0;
    private bool up = true;

    // Update is called once per frame
    void Update()
    {
        float diff = Time.deltaTime * (up ? 1 : -1);
        delta += diff;
        if (delta >= bound || delta <= -bound)
            up = !up;

        transform.Translate(0, diff, 0);
    }
}
