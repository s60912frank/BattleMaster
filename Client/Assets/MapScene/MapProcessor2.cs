using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapProcessor2{
    public const float TIMES = 3276.8f;
    private const int ZOOM = 15;
    private float lon;
    private float lat;
    public Vector2 TileCenter;
    public Rect mapBoundary;
    private bool first = true;
    public Dictionary<string, List<Vector2[]>> outputData;

    public MapProcessor2(float lon,float lat)
    {
        this.lon = lon;
        this.lat = lat;
        outputData = new Dictionary<string, List<Vector2[]>>() {
            {"Polygon", new List<Vector2[]>() },
            {"LineString", new List<Vector2[]>() }
        };
        mapBoundary = new Rect(0, 0, 0, 0);
    }

    public void JsonProcessor(string rawData)
    {
        JSONObject data = new JSONObject(rawData);
        JSONObject buildings = data["buildings"]["features"]; //讀取資料中的"building"節點下的"features"節點
        foreach (JSONObject obj in buildings.list) //讀取這個節點的陣列
        {
            List<Vector2> coords = new List<Vector2>();
            string type = obj["geometry"]["type"].str;
            switch (type)
            {
                case "Point":
                    //點不知怎畫，然而好像也不是很重要
                    break;
                case "Polygon":
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list[0].list) //polygon的座標陣列存在陣列[0]的陣列中
                    {
                        coords.Add(new Vector2(co[0].f, co[1].f));
                    }
                    break;
                default:
                    Debug.Log(type);
                    break;
            }
            outputData["Polygon"].Add(CoordTransform(coords));
        }

        JSONObject roads = data["roads"]["features"]; //讀取資料中的"roads"節點下的"features"節點
        foreach (JSONObject obj in roads.list)
        {
            string type = obj["geometry"]["type"].str;
            switch (type)
            {
                case "LineString":
                    List<Vector2> coords = new List<Vector2>();
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list) //lineString的座標陣列存在陣列中
                    {
                        coords.Add(new Vector2(co[0].f, co[1].f));
                    }
                    outputData["LineString"].Add(CoordTransform(coords));
                    break;
                case "MultiLineString":
                    foreach (JSONObject co in obj["geometry"]["coordinates"].list) //multiLineString的座標陣列存在陣列中
                    {
                        List<Vector2> smallLine = new List<Vector2>(); //看名稱就知道啦這個一個就有很多條線，所以一條線就丟去畫
                        foreach (JSONObject co2 in co.list)
                        {
                            smallLine.Add(new Vector2(co2[0].f, co2[1].f));
                        }
                        //存入該mapTile的點List
                        outputData["LineString"].Add(CoordTransform(smallLine));
                    }
                    break;
                default:
                    Debug.Log(type);
                    break;
            }
        }
        Debug.Log("Done drawing map.");
    }

    private Vector2[] CoordTransform(List<Vector2> raw)
    {
        if (first)
        {
            Vector2 temp = raw[0];
            temp.x = (temp.x - lon) * TIMES;
            temp.y = (temp.y - lat) * TIMES;
            mapBoundary.center = temp;
            first = false;
        }
        Vector2[] transformed = new Vector2[raw.Count];
        for (int i = 0; i < raw.Count; i++)
        {
            Vector2 temp = raw[i];
            temp.x = (temp.x - lon) * TIMES;
            temp.y = (temp.y - lat) * TIMES;
            UpdateBound(temp);
            transformed[i] = temp;
        }
        return transformed;
    }

    public void UpdateBound(Vector2 vec) //更新地圖塊範圍用
    {
        if (!mapBoundary.Contains(vec))
        {
            if (vec.x < mapBoundary.xMin)
            {
                mapBoundary.xMin = vec.x;
            }
            else if (vec.x > mapBoundary.xMax)
            {
                mapBoundary.xMax = vec.x;
            }
            if (vec.y > mapBoundary.yMax)
            {
                mapBoundary.yMax = vec.y;
            }
            else if (vec.y < mapBoundary.yMin)
            {
                mapBoundary.yMin = vec.y;
            }
        }
    }
}
