using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private SpawnManager spawnManager;
    private AudioSource audioSource;
    private int jumpForce = 10;
    private int additionalJumpForce = 8;
    private int speed = 8;
    private int jumps = 0;
    private const int originalGravity = 2;
    private float teleportDistance = 3.5f;
    private float teleportTime = .2f;
    private float maxVelocity = 12f;
    private float deathWaitTime = 1f;
    private float teleportCooldownTime = 1f;
    private float endGameWaitTime = 1.2f;
    private bool facingRight = true;
    private bool canTeleport = true;
    private bool grounded = false;
    private bool isTeleporting = false;
    private bool isDead = false;
    [SerializeField] Sprite spriteRight;
    [SerializeField] Sprite spriteLeft;
    [SerializeField] Material normalMaterial;
    [SerializeField] Material translucent;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip deathSound;
    public bool isEndgame = false;
    public UnityEvent startEndCredits = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        if (isTeleporting)
        {
            transform.Translate(Vector3.right * (facingRight ? 1 : -1) * teleportDistance * Time.deltaTime / teleportTime);
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 currentVelocity = playerRB.velocity;
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
        if (Input.GetKeyDown(KeyCode.DownArrow) && canTeleport)
        {
            currentVelocity.y = 0;
            playerRB.velocity = currentVelocity;
            isTeleporting = true;
            spriteRenderer.material= translucent;
            playerRB.gravityScale = 0;
            audioSource.clip = dashSound;
            audioSource.Play();
            StartCoroutine(EndTeleport());
        }
        else if (horizontalInput != 0)
        {
            Vector2 boxSize = boxCollider.size;
            boxSize.y = 0;
            Vector2 position = playerRB.position + (horizontalInput > 0 ? boxSize / 2 : -boxSize / 2);
            RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector3.right * horizontalInput, speed * Time.deltaTime);
            int index = 0;
            bool found = false;
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                if (hit.collider != boxCollider && hit.collider != null && hit.rigidbody != null)
                {
                    found = true;
                    index = i;
                    break;
                }
            }

            if (found)
                transform.Translate(Vector3.right * horizontalInput * hits[index].distance);
            else
                transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
        }

        // Cap the player's max velocity
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
        if (Input.GetKeyDown(KeyCode.UpArrow) && (grounded || jumps > 0))
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
            currentVelocity.y = 0;
            playerRB.velocity = currentVelocity;
            playerRB.AddForce(Vector3.up * (grounded ? jumpForce : additionalJumpForce), ForceMode2D.Impulse);
            if (!grounded)
                jumps--;
            grounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGrounding(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGrounding(collision);
    }

    private void CheckGrounding(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            grounded |= Vector2.Angle(normal, Vector2.up) < 90;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    private IEnumerator EndTeleport()
    {
        yield return new WaitForSeconds(teleportTime);
        isTeleporting = false;
        canTeleport = false;
        spriteRenderer.material = normalMaterial;
        playerRB.gravityScale = originalGravity;
        StartCoroutine(RefreshTeleport());
    }

    private IEnumerator RefreshTeleport()
    {
        yield return new WaitForSeconds(teleportCooldownTime);
        canTeleport = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
            return;

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

        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            spawnManager.SetCheckPoint(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("EndGame"))
        {
            EndGameLogic();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Spike") && !isTeleporting)
        {
            PlayerDeath();
        }
    }

    public void PlayerDeath()
    {
        isDead= true;
        DataManager.Instance.IncreaseDeath();
        Vector2 currentVelocity = playerRB.velocity;
        currentVelocity.y = 0;
        playerRB.velocity = currentVelocity;
        float xForce = UnityEngine.Random.Range(-2f, 2f);
        playerRB.AddForce(Vector3.up * jumpForce + Vector3.right * xForce, ForceMode2D.Impulse);
        spriteRenderer.flipY = true;
        audioSource.clip = deathSound;
        audioSource.Play();
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(deathWaitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndLevelLogic()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                GameObject.Find("EndLevelManager").GetComponent<EndLevel1Manager>().Activate();
                canTeleport = false;
                break;
            case 2:
                DataManager.Instance.EndLevelTwo();
                SceneManager.LoadScene(1);
                break;
        }
    }

    public void EndGameLogic()
    {
        isDead = true;
        isEndgame = true;
        StartCoroutine(StartEyeAnimation());
    }

    private IEnumerator StartEyeAnimation()
    {
        yield return new WaitForSeconds(endGameWaitTime);
        GameObject eyeBlock = GameObject.Find("EyeBlock");
        eyeBlock.GetComponent<Animator>().SetTrigger("EndGame");
        startEndCredits.Invoke();
    }
}
