using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider;
    [SerializeField] private int jumpForce = 5;
    [SerializeField] private int speed = 8;
    [SerializeField] private int teleport = 3;
    private int jumps = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            transform.Translate(Vector3.right * horizontalInput * teleport);
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
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
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
}
