using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private int level;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
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
        position.x = player.transform.position.x;
        if (level != 1 || (DataManager.finishedLevel2 && position.y > 0) || position.x < -13)
        {
            position.y = player.transform.position.y;
        }

        transform.position = position;
    }
}
