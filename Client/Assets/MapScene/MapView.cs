using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapView : MonoBehaviour {
    //public GameObject LoadingPanel;
    public GameObject EnemyDataPanel;
    public Text GPSText;
    public GameObject Player;
    private MapModel model;
	// Use this for initialization
	IEnumerator Start () {
        //LoadingPanel.GetComponent<LoadingScript>().Start();
        //LoadingPanel.GetComponent<LoadingScript>().StartLoading();
        model = new MapModel();
        yield return model.MapInit();
        MapProcessor2 result = model.GetMapResult();
        foreach(KeyValuePair<string,List<Vector2[]>> mapObj in result.outputData)
        {
            foreach(Vector2[] vecs in mapObj.Value)
            {
                DrawMapObj(mapObj.Key, vecs);
            }
        }
        SetPlane(result);
        //LoadingPanel.GetComponent<LoadingScript>().EndLoading();
        yield return UpdateGPS();
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void CurrentLoactionClicked()
    {
        StartCoroutine(MoveCamToPlayer());
    }

    private IEnumerator MoveCamToPlayer()
    {
        Transform cam = Camera.main.transform;
        while(cam.position.x != Player.transform.position.x && cam.position.y != Player.transform.position.y)
        {
            cam.position = new Vector3(Mathf.MoveTowards(cam.position.x, Player.transform.position.x, 0.1f),
                                   Mathf.MoveTowards(cam.position.y, Player.transform.position.y, 0.1f),
                                   cam.position.z);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator UpdateGPS()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Vector2 newPos = model.PlayerLocation;
            //Player.transform.position.Set(newPos.x, newPos.y, Player.transform.position.z);
            Player.transform.position = new Vector3(newPos.x, newPos.y, Player.transform.position.z);
            GPSText.text = model.GPSStatus;
        }
    }

    public void ShowEnemyData()
    {
        JSONObject enemy = model.GetEnemyData("Trash").Copy();
        enemy.AddField("inRange", true);
        EnemyDataPanel.GetComponent<EnemyPanel>().SetEnemyData(enemy);
    }

    public void ShowEnemyData(Transform trans)
    {
        JSONObject enemy = model.GetEnemyData(trans.name).Copy();
        Vector2 disVec = new Vector2(Player.transform.position.x - trans.position.x, Player.transform.position.y - trans.position.y);
        float distance = Mathf.Sqrt(disVec.sqrMagnitude);
        if(distance < trans.localScale.x / 2.0f)
        {
            enemy.AddField("inRange", true);
        }
        else
        {
            enemy.AddField("inRange", false);
        }
        EnemyDataPanel.GetComponent<EnemyPanel>().SetEnemyData(enemy);
    }

    public IEnumerator GetNewTile(int xTile, int yTile)
    {
        //LoadingPanel.GetComponent<LoadingScript>().StartLoading();
        yield return model.RequestMap(xTile, yTile);
        MapProcessor2 result = model.GetMapResult();
        foreach (KeyValuePair<string, List<Vector2[]>> mapObj in result.outputData)
        {
            foreach (Vector2[] vecs in mapObj.Value)
            {
                DrawMapObj(mapObj.Key, vecs);
            }
        }
        SetPlane(result);
        //LoadingPanel.GetComponent<LoadingScript>().EndLoading();
    }

    private void DrawMapObj(string type, Vector2[] vertices2D) //畫地圖物件
    {
        //yield return false;
        //Debug.Log(vertices2D[0]);
        GameObject obj = new GameObject(); //創個新物體
        obj.tag = "MapObj"; //方便清掉
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++) //就只是2D轉3D
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, -0.1f); //一樣設0會有問題
        }
        switch (type)
        {
            case "Polygon":
                // Use the triangulator to get indices for creating triangles
                Triangulator tr = new Triangulator(vertices2D);
                int[] indices = tr.Triangulate();
                //Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                // Set up game object with mesh;
                MeshRenderer msgr = obj.AddComponent<MeshRenderer>();
                msgr.material = Resources.Load("building") as Material;
                MeshFilter filter = obj.AddComponent<MeshFilter>() as MeshFilter;
                filter.mesh = msh;
                break;
            case "Point":
                //還是不知怎畫
                break;
            case "LineString":
                LineRenderer line = obj.AddComponent<LineRenderer>();
                //set the number of points to the line
                line.SetVertexCount(vertices.Length);
                for (int i = 0; i < vertices.Length; i++)
                {
                    line.SetPosition(i, vertices[i]);
                }
                //set the width
                line.SetWidth(0.1f, 0.1f);
                line.material = Resources.Load("road") as Material;
                //line.transform.parent = this.gameObject.transform;
                line.useWorldSpace = false;
                obj.SetActive(true);
                break;
        }
        //yield return false;
    }

    //這裡有點臭
    public void SetPlane(MapProcessor2 data)
    {
        Debug.Log("SET PLANE!");
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.Rotate(Vector3.right, -90);
        plane.tag = "MapPlane";
        plane.name = model.TileNum.x + "/" + model.TileNum.y;
        plane.transform.localScale = new Vector3(data.mapBoundary.width / 10.0f, 1, data.mapBoundary.height / 10.0f);
        plane.transform.position = new Vector3(data.mapBoundary.center.x, data.mapBoundary.center.y);
        plane.GetComponent<Renderer>().material = Resources.Load<Material>("plane");
        float rad = (data.mapBoundary.width + data.mapBoundary.height) / 8.0f;
        Vector2[] poss = new Vector2[] {
            new Vector2(plane.transform.position.x - rad, plane.transform.position.y + rad),
            new Vector2(plane.transform.position.x + rad, plane.transform.position.y + rad),
            new Vector2(plane.transform.position.x - rad, plane.transform.position.y - rad),
            new Vector2(plane.transform.position.x + rad, plane.transform.position.y - rad)
        };
        string[] enemyKinds = model.EnemyNames;
        for (int i = 0; i < 4; i++)
        {
            GameObject area = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(area.GetComponent<CapsuleCollider>());
            area.AddComponent<MeshCollider>();
            area.tag = "Area";
            area.name = enemyKinds[Random.Range(0, enemyKinds.Length)];
            area.transform.Rotate(new Vector3(90, 0, 0));
            area.transform.localScale = new Vector3(rad * 2.0f, 0.1f, rad * 2.0f);
            area.transform.position = new Vector3(poss[i].x, poss[i].y, -0.1f);
            area.GetComponent<Renderer>().material = Resources.Load<Material>("area");
        }
    }

    public void StopGps()
    {
        model.StopGps();
    }
}
