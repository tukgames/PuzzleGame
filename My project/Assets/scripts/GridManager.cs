using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float lineWidth;

    TileManager tm;

    public Color c;

    public Material m;

    public float alpha;
    // Start is called before the first frame update

    public static GridManager instance;

    public List<GameObject> lines;

    public void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }
    }
    void Start()
    {
        Debug.Log("tm set");
        tm = TileManager.instance;
        //DrawGrid();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawGrid(float rows, float columns, float width, float height)
    {
        //tm = TileManager.instance;
        //Debug.Log(tm.numPieces);
        //Debug.L
        //float rows = tm.res.y;
        //float columns = tm.res.x;
        
        for (int i = 0; i < rows+1; i++)
        {
            lines.Add(DrawLine(new Vector3(transform.position.x, transform.position.y - i*height, transform.position.z), new Vector3(transform.position.x + width*columns, transform.position.y - i * height, transform.position.z), c, m));
        }

        for (int i = 0; i < columns + 1; i++)
        {
            lines.Add(DrawLine(new Vector3(transform.position.x + i * width, transform.position.y, transform.position.z), new Vector3(transform.position.x + i * width, transform.position.y - height * rows, transform.position.z), c, m));
        }
    }


    GameObject DrawLine(Vector3 start, Vector3 end, Color color, Material material)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = material;

        /*material.color = color;
        lr.startColor = color;
        lr.endColor = color;
        lr.SetColors(color, color);*/

        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lr.colorGradient = gradient;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        return myLine;
        //GameObject.Destroy(myLine, duration);
    }

    public void EraseLines()
    {
        for(int i = lines.Count-1; i >= 0; i--)
        {
            Destroy(lines[i]);
        }
        lines.Clear();
    }
}
