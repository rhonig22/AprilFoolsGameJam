using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static int currentCheckPointIndex { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set up singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckPointIndex(int index)
    {
        currentCheckPointIndex= index;
    }
}
