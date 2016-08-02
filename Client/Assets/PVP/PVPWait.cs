﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using SocketIO;

public class PVPWait : MonoBehaviour {
    public Text RoomNameText;
    public Text OwnerText;
    public Text RivalText;
    public Text OwnerReadyText;
    public Text RivalReadyText;
    public GameObject LoadingPanel;
    private GameObject socketIOObj;
    private SocketIOComponent socket;
    private JSONObject roomInfo;
    private bool amIready = false;
    private LoadingScript panelScript;
	// Use this for initialization
	void Start () {
        socketIOObj = GameObject.Find("SocketIO");
        socket = socketIOObj.GetComponent<SocketIOComponent>();
        socket.On("ready", OnReady);
        socket.On("unReady", OnUnReady);
        socket.On("roomChanged", OnRoomChanged);
        socket.On("battleStart", OnBattleStart);
        panelScript = LoadingPanel.GetComponent<LoadingScript>();

        roomInfo = new JSONObject(PlayerPrefs.GetString("RoomInfo"));
        if (roomInfo.HasField("enemyReady") && roomInfo["enemyReady"].b)
        {
            OwnerReadyText.text = "準備";
        }
        SetRoomInfo();
    }

    private void SetRoomInfo()
    {
        Debug.Log("有對手?:" + roomInfo.HasField("rival"));
        RoomNameText.text = roomInfo["name"].str;
        OwnerText.text = roomInfo.HasField("owner") ? string.Format("({0}){1}", roomInfo["owner"]["id"].str, roomInfo["owner"]["name"].str) : "";
        RivalText.text = roomInfo.HasField("rival") ? string.Format("({0}){1}", roomInfo["rival"]["id"].str, roomInfo["rival"]["name"].str) : "";
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnReady(SocketIOEvent e)
    {
        if(e.data["id"].str == roomInfo["owner"]["id"].str)
        {
            OwnerReadyText.text = "準備";
        }
        else if (e.data["id"].str == roomInfo["rival"]["id"].str)
        {
            RivalReadyText.text = "準備";
        }
    }

    private void OnUnReady(SocketIOEvent e)
    {
        if (e.data["id"].str == roomInfo["owner"]["id"].str)
        {
            OwnerReadyText.text = "未準備";
        }
        else if (e.data["id"].str == roomInfo["rival"]["id"].str)
        {
            RivalReadyText.text = "未準備";
        }
    }

    private void OnBattleStart(SocketIOEvent e)
    {
        DontDestroyOnLoad(socketIOObj);
        //轉去戰鬥畫面
        StartCoroutine(GoBattle());
    }

    private IEnumerator GoBattle()
    {
        panelScript.StartLoading();
        float timeStart = Time.time;
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("BattlePVP");
        loadScene.allowSceneActivation = false;
        while (loadScene.progress < 0.9f)
        {
            //Debug.Log("PROGRESS:" + loadScene.progress);
            yield return null;
        }
        while (Time.time - timeStart < 0.8f)
        {
            //Debug.Log(Time.time - timeStart);
            yield return null;
        }
        panelScript.OnHidedCallback(() =>
        {
            //在動畫撥放完成後轉到新場景
            loadScene.allowSceneActivation = true;
        });
        panelScript.EndLoading();
    }

    private void OnRoomChanged(SocketIOEvent e)
    {
        Debug.Log("WHEEEEEEEEEEEEEEEE:" + e.data);
        roomInfo = e.data;
        SetRoomInfo();
    }

    public void ReadyButtonClicked()
    {
        if (amIready)
        {
            socket.Emit("unReady");
            amIready = false;
        }
        else
        {
            socket.Emit("ready");
            amIready = true;
        }
    }

    public void LeaveButtonClicked()
    {
        socket.Emit("leaveRoom");
        socket.Close();
        Destroy(socketIOObj);
        SceneManager.LoadScene("PVPRooms");
    }
}
