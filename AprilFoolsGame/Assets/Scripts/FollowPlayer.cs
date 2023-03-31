using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    [SerializeField] private int level;
    private int endGameYPos = -8;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        if (DataManager.finishedLevel2)
        {
            Vector3 position = transform.position;
            position.y = player.transform.position.y;
            transform.position = position;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 position = transform.position;
        if (playerController.isEndgame)
        {
            if (position.y > endGameYPos) {
                position.y += endGameYPos*Time.deltaTime;
            }
        }
        else
        {
            position.x = player.transform.position.x;
            if (level != 1 || (position.y > 0))
            {
                position.y = player.transform.position.y;
            }
        }

        transform.position = position;
    }
}
