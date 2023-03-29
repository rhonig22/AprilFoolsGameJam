using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        if (DataManager.currentCheckPointIndex < checkpoints.Length)
        {
            Vector3 startPosition = checkpoints[DataManager.currentCheckPointIndex].transform.position;
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
