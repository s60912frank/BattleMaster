using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class CamController : MonoBehaviour {
    private Transform trans;
    private MapView map;
    //private Vector3 nowHit;
    //private float lastDistance;
    private float panDiff;
    private bool rayCasting = true;
    //臭
    public bool MouseRay = true;
    private Vector2 startPos;
	// Use this for initialization
	void Start () {
        trans = gameObject.transform;
        map = GameObject.Find("Map").GetComponent<MapView>(); //存mapView
        StartCoroutine(LessFreqRaycast());
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log("MOUSERAY:" + MouseRay);
        MouseKeyboardControl();

        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                //drag-移動地圖而不跳出敵人訊息
                Vector2 touchDeltaPosition = touch.deltaPosition;
                transform.Translate(touchDeltaPosition * 0.0025f * transform.position.z);
            }
            else if (touch.phase == TouchPhase.Ended && touch.position == startPos && MouseRay)
            {
                //tap-查看敵人訊息
                
                //I wish I have better solution.....
                RaycastHit2D btnHit = Physics2D.Raycast(this.transform.position, Input.mousePosition);
                if(btnHit.collider == null)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
                    Debug.Log("HIT:" + hit.transform.tag);
                    if (hit.transform.tag == "Area" && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        //showMonsterData
                        MouseRay = false;
                        map.ShowEnemyData(hit.transform);
                    }
                    else if (hit.transform.tag == "MapPlane")
                    {
                        map.ShowEnemyData();
                        MouseRay = false;
                    }
                }
                else
                {
                    Debug.Log(btnHit.collider.name);
                }
                startPos = Vector2.zero;
            }
        }

        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * 0.08f;
            //Debug.Log("PANDIFF:" + deltaMagnitudeDiff);
            trans.position = new Vector3(trans.position.x, trans.position.y, trans.position.z - deltaMagnitudeDiff);
        }

        //起始y=-10,-5時可視面積1/4所以zoom+1,-20時可視面積4倍所以zoom-1
        if (trans.position.z > -3f)
        {
            //Debug.Log("太大啦!");
            //RequestNewZoomMap(1);
            trans.position = new Vector3(trans.position.x, trans.position.y, -3);
        }
        if (trans.position.z < -40)
        {
            //Debug.Log("太小啦!");
            //RequestNewZoomMap(-1);     
            trans.position = new Vector3(trans.position.x, trans.position.y, -40);
        }
	}

    private void MouseKeyboardControl()
    {
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
    }

    private IEnumerator LessFreqRaycast()
    {
        while (rayCasting)
        {
            yield return new WaitForSeconds(0.2f); //每200ms就raycast一次
            Debug.Log("HELLO");
            Ray left = Camera.main.ViewportPointToRay(new Vector3(0, 0.5f, 0));
            Ray right = Camera.main.ViewportPointToRay(new Vector3(1, 0.5f, 0));
            Ray top = Camera.main.ViewportPointToRay(new Vector3(0.5f, 1, 0));
            Ray down = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0, 0));
            if (!Physics.Raycast(left))
            {
                int[] tileNum = DetermineCurrentTile();
                if(tileNum != null)
                    yield return map.GetNewTile(tileNum[0] - 1, tileNum[1]);
                //Debug.Log("LEFT!");
            }
            if (!Physics.Raycast(right))
            {
                int[] tileNum = DetermineCurrentTile();
                if (tileNum != null)
                    yield return map.GetNewTile(tileNum[0] + 1, tileNum[1]);
                //Debug.Log("RIGHT!");
            }
            if (!Physics.Raycast(top))
            {
                int[] tileNum = DetermineCurrentTile();
                if (tileNum != null)
                    yield return map.GetNewTile(tileNum[0], tileNum[1] - 1);
                //Debug.Log("TOP!");
            }
            if (!Physics.Raycast(down))
            {
                int[] tileNum = DetermineCurrentTile();
                if (tileNum != null)
                    yield return map.GetNewTile(tileNum[0], tileNum[1] + 1);
                //Debug.Log("Down!");
            }
        }
    }

    private int[] DetermineCurrentTile()
    {
        Ray center = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits = Physics.RaycastAll(center);
        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.tag == "MapPlane")
            {
                try
                {
                    string[] tileNum = hit.transform.name.Split('/');
                    Debug.Log("CAMCONTROL!:" + tileNum[0]  + "/" + tileNum[1]);
                    return new int[] { int.Parse(tileNum[0]), int.Parse(tileNum[1]) };
                }
                catch
                {
                    return null;
                }
            }
        }
        return null;
    }
}
