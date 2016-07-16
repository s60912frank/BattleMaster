using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class PVPRooms : MonoBehaviour {
	public GameObject socketIOObject;
	private SocketIOComponent socket;
	private GameObject originalBtn;
	private GameObject grid;
	public GameObject confirmPanel;
	public GameObject createRoomPanel;
	private JSONObject userData;
	private string selectedRoomId;
	// Use this for initialization
	void Start () {
        socket = socketIOObject.AddComponent<SocketIOComponent>();
        originalBtn = Resources.Load("RoomListButton") as GameObject;
		grid = GameObject.Find ("Grid");
        //yield return new WaitForSeconds(0.5f);
        //socket.socket.Url = Constant.SOCKET_URL;
		userData = new JSONObject(PlayerPrefs.GetString("userData")); //讀取userData
		socket.On ("roomList", OnGetRoomList);
		socket.On ("roomAdded", OnRoomAdded);
		socket.On ("roomRemoved", OnRoomRemoved);
		socket.On ("battleStart", OnBattleStart);
        string rawSid = PlayerPrefs.GetString("Cookie");
        //附上登入過後server給我們的餅乾再連線
        socket.socket.SetCookie(new WebSocketSharp.Net.Cookie("connect.sid", rawSid.Replace("connect.sid=", "")));
        //連線!!只要有登入過就會連線成功
        socket.Connect();
    }

	private void OnGetRoomList(SocketIOEvent e)
	{
		JSONObject roomList = e.data["data"];
		foreach (JSONObject room in roomList.list) 
		{
			createButton (room);
		}
	}

	private void OnRoomAdded(SocketIOEvent e)
	{
		createButton (e.data);
		Debug.Log (e.data ["name"].ToString () + " added!");
	}

	private void OnRoomRemoved(SocketIOEvent e)
	{
		Debug.Log ("A room removed!");
		JSONObject room = e.data;
		Destroy (GameObject.Find (room ["Id"].ToString ()));
	}

	private void OnBattleStart(SocketIOEvent e)
	{
		DontDestroyOnLoad (socketIOObject);
		SceneManager.LoadScene("BattlePVP");
	}


	private void createButton(JSONObject room)
		//在列表中加入按鈕
	{
		GameObject btn = GameObject.Instantiate (originalBtn);
		btn.GetComponentInChildren<Text> ().text = room ["name"].ToString().Replace("\"", "");
		btn.name = room ["Id"].ToString();
		btn.GetComponent<Button> ().onClick.AddListener (delegate {
			selectedRoomId = btn.name;
			confirmPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		});
		btn.transform.SetParent (grid.transform, false);
	}

	public void JoinRoom() //加入房間
	{
		Dictionary<string,string> data = new Dictionary<string, string>();
		data.Add ("Id", selectedRoomId.ToString ());
		socket.Emit("joinRoom", new JSONObject(data));
	}

	public void ConfirmNo() //小panel們移出可視範圍
	{
		confirmPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.right * 2000;
		createRoomPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.right * 2000;
	}

	public void CreateRoom()
	{
		//socket.Emit ("createRoom", );
		createRoomPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		//createRoomPanel.transform.FindChild("RoomName").GetComponentInChildren<Text>()
	}

	public void SubmitCreateRoom()
	{
		string roomName = createRoomPanel.transform.FindChild ("RoomName").GetComponentInChildren<Text> ().text;
		Dictionary<string,string> data = new Dictionary<string, string> ();
		data.Add ("name", roomName);
		socket.Emit ("createRoom", new JSONObject(data));
		createRoomPanel.transform.FindChild ("Title").GetComponent<Text>().text = "等待對手中";
		createRoomPanel.transform.FindChild ("CreateOK").GetComponent<Button> ().interactable = false;
	}

	public void LeaveRoom()
	{
		socket.Emit ("leaveRoom");
		ConfirmNo ();
		createRoomPanel.transform.FindChild ("CreateOK").GetComponent<Button> ().interactable = true;
	}

	public void LeaveList()
	{
		socket.Close ();
		SceneManager.LoadScene("Status");
	}
}