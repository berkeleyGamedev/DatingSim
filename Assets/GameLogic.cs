using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using Yarn.Unity;

public class GameLogic : MonoBehaviour
{
    int numPlayers;
    Dictionary<string, int> votes;
    int numVotes;
    [SerializeField]
    private GameObject OptionList;
    private bool voting_active = false;

    public class Message
    {
        public string type { get; set; }
        public string data { get; set; }
    }


    // Start is called before the first frame update
    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;

    }

    void Start()
    {
        numPlayers = 0;
        numVotes = 0;
        votes = new Dictionary<string, int>();
    }

    /// <summary>
    /// Handles device connect
    /// </summary>
    void OnConnect(int deviceID)
    {
        numPlayers++;
        Debug.Log("connect");
    }

    /// <summary>
    /// Handles device disconnect
    /// </summary>
    void OnDisconnect(int deviceID)
    {
        numPlayers--;
        Debug.Log("disconnect");
    }

    /// <summary>
    /// Ends voting seesion and selects winning option
    /// </summary>
    void endVoting()
    {
        voting_active = false;
        Debug.Log("round over");
        string winner = "";
        int maxVal = 0;
        foreach (string optionText in votes.Keys)
        {
            if (votes[optionText] >= maxVal)
            {
                maxVal = votes[optionText];
                winner = optionText;
            }
        }
        foreach (Transform child in OptionList.GetComponentsInChildren<Transform>())
        {
            GameObject obj = child.gameObject;
            Debug.Log(obj.name);
            if (obj.name.Contains("Option View") && obj.GetComponentInChildren<TextMeshProUGUI>().text.Equals(winner))
            {
                Debug.Log("winner is " + winner);
                Debug.Log(obj.name);
                obj.GetComponent<OptionView>().InvokeOptionSelected();
            }
        }
        numVotes = 0;
        votes.Clear();
    }

    /// <summary>
    /// Creates a new voting option json to send to devices and activates voting session
    /// </summary>
    public void broadcastVotingOption(string optionText)
    {
        voting_active = true;
        Message msg = new Message
        {
            type = "vote",
            data = optionText
        };
        votes[optionText] = 0;
        SendBroadcast(msg);
    }

    /// <summary>
    /// Sends voting option stringified json to each device
    /// </summary>
    void SendBroadcast(Message msg)
    {
        foreach (int deviceID in AirConsole.instance.GetControllerDeviceIds())
        {
            string messageStr = JsonConvert.SerializeObject(msg);
            AirConsole.instance.Message(deviceID, messageStr);
        }
    }

    /// <summary>
    /// Handles message received from device
    /// </summary>
    void OnMessage(int deviceID, JToken data)
    {
        Debug.Log("message from" + deviceID + ", data: " + data);
        if (data["action"] != null && data["action"].ToString().Equals("interact"))
        {
            Camera.main.backgroundColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        if (data["vote"] != null)
        {
            Debug.Log("voting");
            string optionText = data["vote"].ToString();
            votes[optionText] += 1;
            numVotes += 1;
            if (numVotes == numPlayers)
            {
                endVoting();
            }
        }
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
