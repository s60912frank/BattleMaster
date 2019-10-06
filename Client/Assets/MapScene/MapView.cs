using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapView : MonoBehaviour {
    //public GameObject LoadingPanel;
    public GameObject EnemyDataPanel;
    public GameObject Player;
    private MapModel model;
	// Use this for initialization
	IEnumerator Start () {
        //換音樂
        AudioSource bgm = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        bgm.clip = Resources.Load<AudioClip>("music/map");
        bgm.Play();

        //關閉particle system因為不需要
        var particleSys = GameObject.Find("Particle System").GetComponent<ParticleSystem>();
        particleSys.enableEmission = false;
        particleSys.Clear(true);

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
        while(cam.position.x != Player.transform.position.x || cam.position.y != Player.transform.position.y)
        {
            cam.position = new Vector3(Mathf.MoveTowards(cam.position.x, Player.transform.position.x, 0.5f),
                                   Mathf.MoveTowards(cam.position.y, Player.transform.position.y, 0.5f),
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
            Player.transform.position = new Vector3(newPos.x, newPos.y, Player.transform.position.z);
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
        int verticesLength = vertices2D.Length;
        if(verticesLength == 0) return;
        GameObject obj = new GameObject(); //創個新物體
        obj.tag = "MapObj"; //方便清掉
        Vector3[] vertices = new Vector3[verticesLength];
        for (int i = 0; i < vertices.Length; i++) //就只是2D轉3D
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, -0.1f); //一樣設0會有問題
        }        
        switch (type)
        {
            case "Polygon":
                Vector3[] vertices2 = new Vector3[verticesLength * 2];
                float randomHeight = -Random.Range(0.8f,3);
                for (int i = 0; i < verticesLength; i++)        
                    vertices2[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, randomHeight);//高度.高度.高度.高度.高度.高度.
                for (int i = 0; i < verticesLength; i++)
                    vertices2[i+ verticesLength] = new Vector3(vertices2D[i].x, vertices2D[i].y, -0.1f);

                // Use the triangulator to get indices for creating triangles
                Triangulator tr = new Triangulator(vertices2D);
                int[] indices = tr.Triangulate();
                //ebug.Log(indices.Length + (verticesLength-1)*6);
                int[] HeightIndices = new int[indices.Length + (verticesLength-1)*6];

                for (int i = 0; i < indices.Length; i++)
                    HeightIndices[i] = indices[i];
                for (int i = 0; i < verticesLength - 1; i++)
                {
                    HeightIndices[i*6 + indices.Length] = i + 1;
                    HeightIndices[i*6 + indices.Length + 1] = i;
                    HeightIndices[i*6 + indices.Length + 2] = i + verticesLength;
                    HeightIndices[i*6 + indices.Length + 3] = i + verticesLength;
                    HeightIndices[i*6 + indices.Length + 4] = i + verticesLength + 1;
                    HeightIndices[i*6 + indices.Length + 5] = i + 1;
                }

                //Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices2;
                msh.triangles = HeightIndices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                // Set up game object with mesh;
                MeshRenderer msgr = obj.AddComponent<MeshRenderer>();
                msgr.material = new Material(Shader.Find("Diffuse"));
                MeshFilter filter = obj.AddComponent<MeshFilter>() as MeshFilter;
                filter.mesh = msh;

                GameObject[] buildingLine = new GameObject[verticesLength * 2];
                for (int i = 0; i < verticesLength - 1; i++)
                {
                    buildingLine[i] = new GameObject();
                    buildingLine[i].tag = "MapObj";
                    LineRenderer Line2 = buildingLine[i].AddComponent<LineRenderer>();
                    Line2.SetVertexCount(2);
                    Line2.SetPosition(0, vertices2[i]);
                    Line2.SetPosition(1, vertices2[i + 1]);
                    Line2.SetWidth(0.03f, 0.03f);
                    Line2.material = Resources.Load("black") as Material;
                    Line2.useWorldSpace = false;
                    obj.SetActive(true);
                }
                for (int i = 0; i < verticesLength; i++)
                {
                    buildingLine[i + verticesLength] = new GameObject();
                    buildingLine[i + verticesLength].tag = "MapObj";
                    LineRenderer Line2 = buildingLine[i + verticesLength].AddComponent<LineRenderer>();
                    Line2.SetVertexCount(2);
                    Line2.SetPosition(0, vertices2[i]);
                    Line2.SetPosition(1, vertices2[i + verticesLength]);
                    Line2.SetWidth(0.03f, 0.03f);
                    Line2.material = Resources.Load("black") as Material;
                    Line2.useWorldSpace = false;
                    obj.SetActive(true);
                }
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
