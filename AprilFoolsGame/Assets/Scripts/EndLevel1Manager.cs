using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel1Manager : MonoBehaviour
{
    [SerializeField] GameObject moveRightWall;
    [SerializeField] GameObject moveLeftWall;
    [SerializeField] GameObject moveCeiling;
    [SerializeField] GameObject moveFloor;

    private BoxCollider2D boxColliderTop;
    private BoxCollider2D boxColliderBottom;
    private BoxCollider2D boxColliderRight;
    private BoxCollider2D boxColliderLeft;
    private GameObject player;
    private Vector3 playerPosition;
    private bool isActive = false;
    private const float threshhold = 1.2f;

    public void Activate()
    {
        moveLeftWall.SetActive(true);
        player = GameObject.Find("Player");
        playerPosition = player.transform.position;
        isActive = true;
        boxColliderTop = moveCeiling.GetComponent<BoxCollider2D>();
        boxColliderBottom = moveFloor.GetComponent<BoxCollider2D>();
        boxColliderRight = moveRightWall.GetComponent<BoxCollider2D>();
        boxColliderLeft = moveLeftWall.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Move the walls to close in on the player
        if (isActive)
        {
            Vector3 diff = playerPosition - player.transform.position;
            diff.y = diff.x / 2;
            playerPosition = player.transform.position;
            if (diff.x > 0) {
                Vector3 leftDiff = diff;
                leftDiff.y = 0;
                moveLeftWall.transform.position += leftDiff;
                Vector3 downDiff = diff;
                downDiff.x = 0;
                moveFloor.transform.position += downDiff;
            }
            else
            {
                Vector3 rightDiff = diff;
                rightDiff.y = 0;
                moveRightWall.transform.position += rightDiff;
                Vector3 upDiff = diff;
                upDiff.x = 0;
                moveCeiling.transform.position += upDiff;
            }

            // When they have closed in enough to surround the player, end the level and move to level 2
            if ((moveRightWall.transform.position.x  + boxColliderRight.offset.x - boxColliderRight.size.x / 2) - (moveLeftWall.transform.position.x + boxColliderLeft.offset.x + boxColliderLeft.size.x / 2) < threshhold ||
                (moveCeiling.transform.position.y + boxColliderTop.offset.y - boxColliderTop.size.y/2) - (moveFloor.transform.position.y + boxColliderBottom.offset.y + boxColliderBottom.size.y/2) < threshhold)
            {
                // TODO: End Animation
                SceneManager.LoadScene(2);
            }
        }
    }
}
