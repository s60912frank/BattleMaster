using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CamController : MonoBehaviour {
    private Transform trans;
    private MapView map;
    private Vector3 nowHit;
    private float panDiff;
    private bool rayCasting = true;
	// Use this for initialization
	void Start () {
        trans = gameObject.transform;
        map = GameObject.Find("Map").GetComponent<MapView>(); //存mapView
        StartCoroutine(LessFreqRaycast());
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Debug.Log("HITTTT:" + hit.transform.name);
                //StartCoroutine(GetEnemyData(hit.transform.name));
            }
        }

        //方向建移動cam
        if (Input.GetKey(KeyCode.UpArrow))
        {
            trans.Translate(Vector3.up * 0.5f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            trans.Translate(Vector3.down * 0.5f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            trans.Translate(Vector3.left * 0.5f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            trans.Translate(Vector3.right * 0.5f);
        }

        //滑鼠滾輪縮放
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            trans.position += Vector3.forward * 0.4f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            trans.position -= Vector3.forward * 0.4f;
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition * 0.025f);
        }

        //雙指縮放,未測試
        if (Input.touchCount == 2)
        {
			//Vector2 one = Input.GetTouch (0).deltaPosition;
			//Vector2 two = Input.GetTouch (1).deltaPosition;
			//Debug.Log ("ONE:" + (one - two).ToString());
            //float touchDelta = Mathf.Sqrt((Input.GetTouch(0).deltaPosition - Input.GetTouch(1).deltaPosition).sqrMagnitude);
            //trans.position -= Vector3.forward * touchDelta * 0.05f;

            if(Input.touches[0].phase == TouchPhase.Began && Input.touches[1].phase == TouchPhase.Began)
            {
                panDiff = Mathf.Sqrt((Input.touches[0].position - Input.touches[1].position).sqrMagnitude);
            }
            else if (Input.touches[0].phase == TouchPhase.Moved && Input.touches[1].phase == TouchPhase.Moved)
            {
                float diff = Mathf.Sqrt((Input.touches[0].position - Input.touches[1].position).sqrMagnitude);
                float dir = panDiff - diff;
                if (dir < 0)
                {
                    trans.position -= Vector3.forward * dir * 0.0015f;
                }
                else
                {
                    trans.position -= Vector3.forward * dir * 0.0015f;
                }
                Debug.Log(dir);
            }
        }

        //起始y=-10,-5時可視面積1/4所以zoom+1,-20時可視面積4倍所以zoom-1
        if (trans.position.z > -3f)
        {
            Debug.Log("太大啦!");
            //RequestNewZoomMap(1);
            trans.position = new Vector3(trans.position.x, trans.position.y, -3);
        }
        if (trans.position.z < -40)
        {
            Debug.Log("太小啦!");
            //RequestNewZoomMap(-1);     
            trans.position = new Vector3(trans.position.x, trans.position.y, -40);
        }
	}

    private void RequestNewZoomMap(int diff)
    {
        //清空地圖
        GameObject[] gos = GameObject.FindGameObjectsWithTag("MapObj");
        foreach (GameObject go in gos)
        {
            Destroy(go);
        }
        //cam z變回-10
        trans.position = new Vector3(trans.position.x, trans.position.y, -10);
        //request
        //map.BroadcastMessage("GetNewZoomTile", new object[] { new Vector2(trans.position.x, trans.position.y), diff });
    }

    private IEnumerator LessFreqRaycast()
    {
        while (rayCasting)
        {
            yield return new WaitForSeconds(0.1f); //每300ms就raycast一次
            //Debug.Log("HELLO");
            Ray left = Camera.main.ViewportPointToRay(new Vector3(0, 0.5f, 0));
            Ray right = Camera.main.ViewportPointToRay(new Vector3(1, 0.5f, 0));
            Ray top = Camera.main.ViewportPointToRay(new Vector3(0.5f, 1, 0));
            Ray down = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0, 0));
            if (!Physics.Raycast(left))
            {
                int[] tileNum = DetermineCurrentTile();
                if(tileNum != null)
                    yield return map.GetNewTile(tileNum[0] - 1, tileNum[1]);
                Debug.Log("LEFT!");
            }
            if (!Physics.Raycast(right))
            {
                int[] tileNum = DetermineCurrentTile();
                if (tileNum != null)
                    yield return map.GetNewTile(tileNum[0] + 1, tileNum[1]);
                Debug.Log("RIGHT!");
            }
            if (!Physics.Raycast(top))
            {
                int[] tileNum = DetermineCurrentTile();
                if (tileNum != null)
                    yield return map.GetNewTile(tileNum[0], tileNum[1] - 1);
                Debug.Log("TOP!");
            }
            if (!Physics.Raycast(down))
            {
                int[] tileNum = DetermineCurrentTile();
                if (tileNum != null)
                    yield return map.GetNewTile(tileNum[0], tileNum[1] + 1);
                Debug.Log("Down!");
            }
        }
    }

    private int[] DetermineCurrentTile()
    {
        Ray center = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(center, out hit))
        {
            try
            {
                string[] tileNum = hit.transform.name.Split('/');
                return new int[] { int.Parse(tileNum[0]), int.Parse(tileNum[1]) };
            }
            catch
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private IEnumerator GetEnemyData(string enemyName)
    {
        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        JSONObject data = new JSONObject(PlayerPrefs.GetString("userData"));
        headers.Add("Cookie", data["cookie"].str); //加入認證過的cookie就不用重新登入了
        //將來這項資料從地圖取得
        form.AddField("enemyName", enemyName);
        WWW w = new WWW(Constant.SERVER_URL + "/battle", form.data, headers);
        yield return w;
        //就只是看有沒有錯誤而已
        if (!string.IsNullOrEmpty(w.error))
        {
            Debug.Log(w.error);
        }
        else
        {
            Debug.Log(w.text);
            PlayerPrefs.SetString("enemyAI", w.text);
            //LoadingPanel.GetComponent<LoadingScript>().EndLoading();
            SceneManager.LoadScene("Battle2");
        }
    }
}
