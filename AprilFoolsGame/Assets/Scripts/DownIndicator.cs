using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownIndicator : MonoBehaviour
{
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    private SpriteRenderer spriteRenderer;
    private bool flip = true;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FlipArrow());
    }

    private IEnumerator FlipArrow()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            flip = !flip;
            spriteRenderer.sprite = flip ? sprite1 : sprite2;
        }
    }
}
