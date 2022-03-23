using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionViewAirConsole : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            GameObject.Find("GameLogic").GetComponent<GameLogic>().broadcastVotingOption(GetComponentInChildren<TextMeshProUGUI>().text);
            transform.hasChanged = false;
        }
    }
}
