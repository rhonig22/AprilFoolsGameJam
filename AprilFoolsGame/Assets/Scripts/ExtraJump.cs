using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    private bool up = true;

    private void Start()
    {
        StartCoroutine(FlipDiff());
    }

    // Update is called once per frame
    void Update()
    {
        float diff = Time.deltaTime * (up ? 1 : -1);
        transform.Translate(0, diff, 0);
    }

    private IEnumerator FlipDiff()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            up = !up;
        }
    }
}
