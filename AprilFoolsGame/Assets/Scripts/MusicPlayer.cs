using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip endingMusic;
    [SerializeField] public GameObject player;
    private AudioSource audioSource;
    private const float fadeTimeLength = 1f;
    private bool endStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set up singleton
        if (Instance != null)
        {
            Instance.ResetAudio();
            if (player != null)
            {
                Instance.player = player;
            }

            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!endStarted && player!= null && SceneManager.GetActiveScene().buildIndex == 1 && player.transform.position.x < -8)
        {
            StartCoroutine(StartFade());
            player.GetComponent<PlayerController>().startEndCredits.AddListener(StartEndMusic);
            endStarted = true;
        }
        
    }

    public void ResetAudio()
    {
        if (endStarted)
        {
            audioSource.clip = mainMusic;
            audioSource.Play();
            endStarted= false;
        }
    }

    private IEnumerator StartFade()
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < fadeTimeLength)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / fadeTimeLength);
            yield return null;
        }

        audioSource.Stop();
        yield break;
    }

    private IEnumerator StartFadeIn()
    {
        float currentTime = 0;
        while (currentTime < fadeTimeLength)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, currentTime / fadeTimeLength);
            yield return null;
        }
        yield break;
    }

    void StartEndMusic()
    {
        audioSource.clip = endingMusic;
        audioSource.Play();
        StartCoroutine(StartFadeIn());
    }
}
