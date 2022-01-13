using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionViewAirConsole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Kenneth is a big nerd.");
        GameObject.Find("GameLogic").GetComponent<GameLogic>().broadcastVotingOption(GetComponentInChildren<TextMeshProUGUI>().text);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
