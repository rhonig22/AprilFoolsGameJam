using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private int jumpForce = 10;
    private int speed = 8;
    private float teleport = 3.5f;
    private int jumps = 1;
    private bool facingRight = true;
    [SerializeField] Sprite spriteRight;
    [SerializeField] Sprite spriteLeft;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0 )
        {
            facingRight= true;
            spriteRenderer.sprite = spriteRight;
        }
        else if (horizontalInput < 0 )
        {
            facingRight= false;
            spriteRenderer.sprite = spriteLeft;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            transform.Translate(Vector3.right * (facingRight ? 1 : -1) * teleport);
        }
        else
        {
            transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && jumps > 0)
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            jumps--;
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        foreach (Collider2D hit in hits)
        {
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            if (colliderDistance.isOverlapped)
            {
                Vector2 moveDistance = colliderDistance.pointA - colliderDistance.pointB;
                if (moveDistance.y < .1)
                {
                    moveDistance.y = Mathf.Round(moveDistance.y);
                }

                if (moveDistance.magnitude < 0)
                    continue;

                transform.Translate(moveDistance);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            jumps = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            PlayerDeath();
        }

        if (collision.gameObject.CompareTag("Jump"))
        {
            Destroy(collision.gameObject);
            jumps++;
        }
    }

    public void PlayerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
