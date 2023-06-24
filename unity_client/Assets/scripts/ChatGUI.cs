using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using LitJson;
using Pomelo.DotNetClient;
using UnityEngine.UI;

public class ChatGUI : MonoBehaviour
{
    // ֱ�Ӵ�login ��ȡuserName,rooms,PomeloClient
    private string userName = LoginGUI.userName;
    private PomeloClient pclient = LoginGUI.pomeloClient;


    public static string inputField = "";   // �������������

    private ArrayList userList = RoomCon.userListInARoom; // ͬһ����������û�

    public bool debug = true;

    public ScrollRect scroll_msg;
    public ScrollRect scroll;
    public InputField inField_msg;
    public Button btn;


    private ArrayList chatRecords = new ArrayList();   // ��¼���ݵ�list

    public Text txtRoomId;
    public Text roomNum;


    void Start()
    {
        Application.runInBackground = true;

        /* ----------------------------------------- */
        //userList = new ArrayList();
        //userList.Add(userName);

        txtRoomId.text = RoomCon.roomid;

        // �÷�����һ���µĵ��û�������
        pclient.on("onCommonUserAddToGame", (data) =>
        {
            string tmp = data.ToString();
            JsonData jdtmp = JsonMapper.ToObject(tmp);

            string newRid = jdtmp["rid"].ToString();
            string uid = jdtmp["uid"].ToString(); // �÷��������

            if (!userList.Contains(uid))
                userList.Add(uid);
        });

        // �����;�˳���
        pclient.on("onDelCommonUserInRoom", (data) =>
        {
            string tmp = data.ToString();
            JsonData jdtmp = JsonMapper.ToObject(tmp);

            string newRid = jdtmp["rid"].ToString();
            string uid = jdtmp["uid"].ToString(); // �÷��������

            if (userList.Contains(uid))
                userList.Remove(uid);
        });

        // ��Ӱ�ť���¼���������
        btn.onClick.AddListener(msgSend);

        pclient.on("onChat", (data) =>
        {
            addMessage(data);
        });

    }
    //Add message to chat window.
    void addMessage(JsonObject messge)
    {
        System.Object msg = null, fromName = null;
        if (messge.TryGetValue("msg", out msg) && messge.TryGetValue("from", out fromName))
        {
            ChatRecord cobj = new ChatRecord(fromName.ToString(), msg.ToString());
            //if(!chatRecords.Contains(cobj))
                chatRecords.Add(cobj);
        }
    }

    void msgSend()
    {
        inputField = inField_msg.text; // ��ȡ������е���Ϣ
        inField_msg.text = "";
        if (inputField == null || inputField == "")
            return;

        JsonObject message = new JsonObject();
        message.Add("content", inputField);
        message.Add("from", userName);

        pclient.request("chat.chatHandler.send", message, (data) =>
        {
            //chatRecords.Add(new ChatRecord(userName, inputField));  
        });
    }

    //When quit, release resource
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey("escape"))
        {
            if (pclient != null)
            {
                pclient.disconnect();
            }
            Application.Quit();
        }

        foreach (Transform child in scroll.transform.Find("Viewport").Find("Content").transform)
        {
            Destroy(child.gameObject);
        }

        if (userList != null && userList.Count >= 1)
            foreach (string roomid in userList)
            {
                GameObject t3 = new GameObject();
                t3.transform.localPosition = new Vector3(0, 0, 0);
                t3.AddComponent<Text>();
                t3.GetComponent<Text>().text = roomid;
                t3.GetComponent<Text>().color = Color.white;
                t3.GetComponent<Text>().fontSize = 20;
                t3.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                t3.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                RectTransform rectTransform2 = t3.GetComponent<RectTransform>();
                rectTransform2.localPosition = new Vector3(0, 0, 0);
                t3.transform.parent = scroll.transform.Find("Viewport").Find("Content");
            }

        // ��������
        int userlistLen = userList.Count;
        roomNum.text = userlistLen.ToString();

        // msglist
        foreach (Transform child in scroll_msg.transform.Find("Viewport").Find("Content").transform)
        {
            Destroy(child.gameObject);
        }

        GameObject msgPrefab = Resources.Load<GameObject>("Prefabs/ChatMsg2");
        foreach (ChatRecord cr in chatRecords)
        {
            GameObject t2 = (GameObject)Instantiate(msgPrefab);
            t2.transform.Find("msg").gameObject.GetComponent<Text>().text = cr.name + ": " + cr.dialog;
            RectTransform rectTransform2 = t2.GetComponent<RectTransform>();
            //rectTransform2.localPosition = new Vector3(0, 0, 0);
            t2.transform.parent = scroll_msg.transform.Find("Viewport").Find("Content");
        }
        
        /*
        foreach (ChatRecord cr in chatRecords)
        {
            string s = cr.name + ": " + cr.dialog;
            GameObject t2 = new GameObject();
            t2.AddComponent<Text>();
            t2.GetComponent<Text>().text = s;
            t2.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            RectTransform rectTransform2 = t2.GetComponent<RectTransform>();
            rectTransform2.localPosition = new Vector3(0, 0, 0);
            t2.transform.parent = scroll_msg.transform.FindChild("Viewport").FindChild("Content");
        }
        */
    }

    //When quit, release resource
    void OnApplicationQuit()
    {
        if (pclient != null)
        {
            pclient.disconnect();
        }
    }

}