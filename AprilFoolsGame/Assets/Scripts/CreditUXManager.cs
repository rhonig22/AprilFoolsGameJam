using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditUXManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI deathCount;

    // Start is called before the first frame update
    void Start()
    {
        player.GetComponent<PlayerController>().startEndCredits.AddListener(StartCreditScroll);
    }

    void StartCreditScroll()
    {
        deathCount.text = "Death Count: " + DataManager.deathCount;
        gameObject.GetComponentInChildren<Animator>().SetTrigger("StartCredits");
    }

    public void OnReturnToTitlePress()
    {
        SceneManager.LoadScene(0);
    }
}
