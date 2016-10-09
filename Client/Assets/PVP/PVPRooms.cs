using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using SocketIO;

public class PVPRooms : MonoBehaviour {
    #region public vars
    public GameObject socketIOObject;
    public GameObject confirmPanel;
	public GameObject createRoomPanel;
	public GameObject LoadingPanel;
    public GameObject NotifyPanel;
    #endregion

    #region private vars
    private SocketIOComponent socket;
	private GameObject originalBtn;
	private GameObject grid;
	private JSONObject userData;
	private string selectedRoomId;
    private LoadingScript panelScript;
    private NotifyPanel notifyScript;
    #endregion
    // Use this for initialization
    void Start () {
        //換音樂
        AudioSource bgm = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        bgm.clip = Resources.Load<AudioClip>("music/Room system");
        bgm.Play();

        panelScript = LoadingPanel.GetComponent<LoadingScript>();
        notifyScript = NotifyPanel.GetComponent<NotifyPanel>();

        socket = socketIOObject.AddComponent<SocketIOComponent>();
        originalBtn = Resources.Load("RoomListButton") as GameObject;
		grid = GameObject.Find ("Grid");
		userData = new JSONObject(PlayerPrefs.GetString("userData")); //讀取userData
        SetSocketEventHandler();
        string rawSid = PlayerPrefs.GetString("Cookie");
        //附上登入過後server給我們的餅乾再連線
        socket.socket.SetCookie(new WebSocketSharp.Net.Cookie("connect.sid", rawSid.Replace("connect.sid=", "")));
        //連線!!只要有登入過就會連線成功
        socket.Connect();
        panelScript.SetText("取得房間列表中");
        panelScript.StartLoading();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //使用者按了返回鍵
            LeaveList();
        }
    }

    private void SetSocketEventHandler()
    {
        socket.On("roomList", OnGetRoomList);
        socket.On("roomAdded", OnRoomAdded);
        socket.On("roomRemoved", OnRoomRemoved);
        socket.On("roomAvaliable", OnRoomAvaliable);
        socket.On("roomFull", OnRoomFull);
        socket.On("joinResult", OnJoinResult);
    }


    #region Socket Event Handlers
    private void OnGetRoomList(SocketIOEvent e)
	{
		foreach (JSONObject room in e.data["data"].list) 
		{
			createButton (room);
		}
        panelScript.EndLoading();
	}

	private void OnRoomAdded(SocketIOEvent e)
	{
		createButton (e.data);
		Debug.Log (e.data ["name"].ToString () + " added!");
	}

    private void OnJoinResult(SocketIOEvent e)
    {
        if (e.data["success"].b)
        {
            //進入房間內場景
            PlayerPrefs.SetString("RoomInfo", e.data["room"].ToString());
            DontDestroyOnLoad(socketIOObject);
            SceneManager.LoadScene("PVPWait");
        }
        else
        {
            //顯示錯誤訊息
            notifyScript.SetText(e.data["message"].str);
            notifyScript.Show();
        }
    }

    private void OnRoomRemoved(SocketIOEvent e)
	{
		Debug.Log ("A room removed!");
		Destroy (GameObject.Find (e.data["id"].str));
	}

    private void OnRoomAvaliable(SocketIOEvent e)
    {
        GameObject.Find(e.data["id"].str).GetComponent<Button>().interactable = true;
    }

    private void OnRoomFull(SocketIOEvent e)
    {
        GameObject.Find(e.data["id"].str).GetComponent<Button>().interactable = false;
    }
    #endregion

    private void createButton(JSONObject room)
		//在列表中加入按鈕
	{
		GameObject btn = GameObject.Instantiate (originalBtn);
        SetRoomButton setScript = btn.GetComponent<SetRoomButton>();
        setScript.SetRoomName(room["name"].str);
        setScript.SetRoomOwner(room["owner"]["id"].str, room["owner"]["name"].str);
		btn.name = room["id"].str;
		btn.GetComponent<Button> ().onClick.AddListener (delegate {
			selectedRoomId = btn.name;
			confirmPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		});
        btn.GetComponent<Button>().interactable = !room["isFull"].b;
        btn.transform.SetParent (grid.transform, false);
	}

	public void JoinRoom() //加入房間
	{
		Dictionary<string,string> data = new Dictionary<string, string>();
		data.Add ("id", selectedRoomId.ToString ());
		socket.Emit("joinRoom", new JSONObject(data));
	}

	public void ConfirmNo() //小panel們移出可視範圍
	{
		confirmPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.right * 2000;
		createRoomPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.right * 2000;
	}

	public void CreateRoom()
	{
		createRoomPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
	}

	public void SubmitCreateRoom()
	{
		string roomName = createRoomPanel.transform.FindChild ("RoomName").GetComponentInChildren<Text> ().text;
		Dictionary<string,string> data = new Dictionary<string, string> ();
		data.Add ("name", roomName);
		socket.Emit("createRoom", new JSONObject(data));

        //載入中....
        //server會以join result回應
	}

	/*public void LeaveRoom()
	{
		socket.Emit ("leaveRoom");
		ConfirmNo ();
		createRoomPanel.transform.FindChild ("CreateOK").GetComponent<Button> ().interactable = true;
	}*/

	public void LeaveList()
	{
		socket.Close();
		SceneManager.LoadScene("Status");
	}
}