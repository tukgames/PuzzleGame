using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorPreviewManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    public List<List<Sprite>> images = new List<List<Sprite>>();
    public GameObject PreviewTilePrefab;
    public List<List<GameObject>> previewTiles = new List<List<GameObject>>();

    public static SectorPreviewManager instance;

    public float width;
    public float height;

    public Vector2 pixelRes;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }
    }
    void Start()
    {
       
    }

    public void Begin()
    {
        AssignImages();
        GenerateTiles();
    }
    public void AssignImages()
    {
        CalculateWidthAndHeight();
        for(int i = 0; i < TileManager.instance.puzzles.Count; i++)
        {
            List<Sprite> l = new List<Sprite>();
            for(int u = 0; u < TileManager.instance.puzzles[0].Count; u++)
            {
                l.Add(TileManager.instance.puzzles[i][u].GetComponent<SpriteRenderer>().sprite);
            }
           // Debug.Log(l.Count);
            images.Add(l);
        }

        Debug.Log(CalculatePixelsPerUnit());
        //Debug.Log(images[0][0].name);
    }

    public void GenerateTiles()
    {
        float ppl = CalculatePixelsPerUnit();
        for(int i = 0; i < TileManager.instance.puzzles.Count; i++)
        {
            List<GameObject> l = new List<GameObject>();
            for(int u = 0; u < TileManager.instance.puzzles[0].Count; u++)
            {
                Vector3 pos = CalcPosition(i, u);
                GameObject g = Instantiate(PreviewTilePrefab, pos, Quaternion.identity);
                g.GetComponent<SpriteRenderer>().sprite = images[i][u];
                g.SetActive(false);
                l.Add(g);
                //CHANGE PPL 8/3/22
                

               
                
            }
            previewTiles.Add(l);
        }

        EnableTiles();
        //Debug.Log(SectorManager.instance.sectors[0].transform.position);
    } 

    public Vector3 CalcPosition(int sec, int num)
    {
        Vector3 origin = SectorManager.instance.sectors[sec].transform.position;
        
        float xOffset = width * ((num) % TileManager.instance.res.x);
        float yOffset = -height * (int)((num) / TileManager.instance.res.x);
        return new Vector3(origin.x + xOffset + width / 2 - (width * TileManager.instance.res.x) / 2, origin.y + yOffset - height / 2 + (height*TileManager.instance.res.y)/2, 0);
        //return Vector3.zero;
    }

    void CalculateWidthAndHeight()
    {
        width = SectorManager.instance.width / TileManager.instance.res.x;
        height = SectorManager.instance.height / TileManager.instance.res.x;
        pixelRes.x = SectorManager.instance.images[0].width / TileManager.instance.res.x;
        pixelRes.y = SectorManager.instance.images[0].height / TileManager.instance.res.y;
    }

    public float CalculatePixelsPerUnit()
    {
        return SectorManager.instance.images[0].width / (width * TileManager.instance.res.x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableTiles()
    {
       for(int i = 0; i < TileManager.instance.puzzles.Count; i++)
        {
            for(int u = 0; u < TileManager.instance.puzzles[i].Count; u++)
            {
                Debug.Log(i + " + " + u  + " us " + TileManager.instance.puzzles[i].Count + " prev tiles " + previewTiles[0].Count);
                if (TileManager.instance.puzzles[i][u].GetComponent<Tile>().placed)
                {
                    previewTiles[i][u].SetActive(true);
                }
            }
        }
        //Debug.Log(TileManager.instance.puzzles[1][1].name);
    }

    public void DisableTiles()
    {
        for (int i = 0; i < TileManager.instance.puzzles.Count; i++)
        {
            for (int u = 0; u < TileManager.instance.puzzles[0].Count; u++)
            {
                
                    previewTiles[i][u].SetActive(false);
                
            }
        }
    }
}
