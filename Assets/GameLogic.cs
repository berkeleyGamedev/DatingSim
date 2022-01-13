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
    List<int> votes;
    [SerializeField]
    private GameObject OptionList;

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
    }

    void Start()
    {
        numPlayers = 0;
    }

    void OnConnect(int deviceID)
    {
        numPlayers++;

    }

    // void Update()
    // {
    //     Debug.Log(OptionList.GetComponentsInChildren<Transform>().Length);
    // }

    public void broadcastVotingOption(string optionText)
    {
        Debug.Log(optionText);
        Message msg = new Message
        {
            type = "vote",
            data = optionText
        };
        SendBroadcast(msg);
    }

    public void BroadcastVotingOptions()
    {
        Transform[] options = OptionList.GetComponentsInChildren<Transform>();
        if (options.Length == 0)
        {
            return;
        }
        votes = new List<int>(new int[options.Length]);
        Debug.Log(options.Length);
        List<string> optionStrings = new List<string>();
        foreach (Transform option in options)
        {
            Debug.Log(option.gameObject.name);
            // optionStrings.Add(option.gameObject.GetComponentInChildren<TextMeshProUGUI>().text);
        }
        Debug.Log("broadcasting voting options");
    }

    void SendBroadcast(Message msg)
    {
        foreach (int deviceID in AirConsole.instance.GetControllerDeviceIds())
        {
            string messageStr = JsonConvert.SerializeObject(msg);
            AirConsole.instance.Message(deviceID, messageStr);
        }
    }

    void OnMessage(int deviceID, JToken data)
    {
        Debug.Log("message from" + deviceID + ", data: " + data);
        if (data["action"] != null && data["action"].ToString().Equals("interact"))
        {
            Camera.main.backgroundColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        if (data["vote"] != null)
        {
            int index = 0;
            foreach (Transform child in OptionList.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text.Equals(data["vote"].ToString()))
                {
                    votes[index]++;
                    return;
                }
                index++;
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
