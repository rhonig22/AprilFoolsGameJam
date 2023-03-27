using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private int jumpForce = 10;
    private int additionalJumpForce = 8;
    private int speed = 8;
    private int jumps = 0;
    private float teleportDistance = 3.5f;
    private float teleportTime = .2f;
    private float maxVelocity = 12f;
    private bool facingRight = true;
    private bool canTeleport = true;
    private bool grounded = false;
    private bool isTeleporting = false;
    [SerializeField] Sprite spriteRight;
    [SerializeField] Sprite spriteLeft;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material translucent;

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
        if (isTeleporting)
        {
            transform.Translate(Vector3.right * (facingRight ? 1 : -1) * teleportDistance * Time.deltaTime / teleportTime);
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        // Control the sprite for the character
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

        // Control movement and teleporting
        if (Input.GetKeyDown(KeyCode.DownArrow) && canTeleport) {
            isTeleporting= true;
            spriteRenderer.material= translucent;
            StartCoroutine(EndTeleport());
        }
        else
        {
            transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        }

        // Cap the player's max velocity
        Vector2 currentVelocity = playerRB.velocity;
        if (currentVelocity.y > maxVelocity)
        {
            currentVelocity.y = maxVelocity;
            playerRB.velocity = currentVelocity;
        }
        else if (currentVelocity.y < -maxVelocity)
        {
            currentVelocity.y = -maxVelocity;
            playerRB.velocity = currentVelocity;
        }

        // Control jumping
        if (Input.GetKeyDown(KeyCode.UpArrow) && jumps > 0)
        {
            currentVelocity.y = 0;
            playerRB.velocity = currentVelocity;
            playerRB.AddForce(Vector3.up * (grounded ? jumpForce : additionalJumpForce), ForceMode2D.Impulse);
            jumps--;
            grounded = false;
        }

        // Control surface collision and grounding logic
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        Vector2 maxMoveDistance = new Vector2(0, 0);
        foreach (Collider2D hit in hits)
        {
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
            if (colliderDistance.isOverlapped && !hit.gameObject.GetComponent<Rigidbody2D>().IsUnityNull())
            {
                Vector2 moveDistance = colliderDistance.pointA - colliderDistance.pointB;
                if (moveDistance.magnitude == 0)
                    continue;

                if (Mathf.Abs(moveDistance.x) > Mathf.Abs(maxMoveDistance.x))
                    maxMoveDistance.x = moveDistance.x;

                if (Mathf.Abs(moveDistance.y) > Mathf.Abs(maxMoveDistance.y))
                    maxMoveDistance.y = moveDistance.y;
            }

            if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && playerRB.velocity.y < 0)
            {
                grounded = true;
                jumps = 1;
            }
        }

        if (maxMoveDistance.magnitude != 0)
            transform.Translate(maxMoveDistance);
    }

    private IEnumerator EndTeleport()
    {
        yield return new WaitForSeconds(teleportTime);
        isTeleporting = false;
        spriteRenderer.material = normalMaterial;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike") && !isTeleporting)
        {
            PlayerDeath();
        }

        if (collision.gameObject.CompareTag("Jump"))
        {
            Destroy(collision.gameObject);
            jumps++;
        }

        if (collision.gameObject.CompareTag("EndTrigger"))
        {
            EndLevelLogic();
        }
    }

    public void PlayerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndLevelLogic()
    {
        GameObject.Find("EndLevelManager").GetComponent<EndLevel1Manager>().Activate();
        canTeleport = false;
    }
}
