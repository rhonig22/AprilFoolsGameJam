using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (DataManager.finishedLevel2)
                ResetPlayerLocation(0);
        }
        else
            ResetPlayerLocation(DataManager.currentCheckPointIndex);
    }

    public void ResetPlayerLocation(int index)
    {
        if (index < checkpoints.Length)
        {
            Vector3 startPosition = checkpoints[index].transform.position;
            startPosition.z = player.transform.position.z;
            player.transform.position = startPosition;
        }
    }

    public void SetCheckPoint(GameObject checkpoint)
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == checkpoint)
                DataManager.Instance.SetCheckPointIndex(i);
        }
    }
}
