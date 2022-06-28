using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // Start is called before the first frame update
    public string mainImage;
    public Vector2 res;

    public GameObject tilePre;
    public GameObject blankPre;
    [HideInInspector]
    public float width;
    public float height;
    public Vector2 pixelRes;

    public float timetosolve;

    public Transform genTransform;

    public List<GameObject> tiles;
    public List<GameObject> blankTiles;

    public static TileManager instance;

    //public Sprite sprite;

    public Texture2D texture;

    public float maxWidth;
    public float maxHeight;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        //Texture2D t = Resources.Load<Texture2D>("sprites/images/" + mainImage + "/" + mainImage);
        
        //StartTiles(t);
    }
    public void StartTiles(Texture2D t)
    {
        texture = t;
        //width = t.width / (res.x*100);
        //height = t.height / (res.y*100);
        if(maxWidth/t.width <= maxHeight / t.height)
        {
            width = maxWidth / res.x;
            //height = t.height * width / t.width;
            height = ((t.height * maxWidth) / t.width) / res.y;
        } else
        {
            height = maxHeight / res.y;
            width = ((t.width * maxHeight) / t.height) / res.x;
        }

        Debug.Log("Height: " + height + " Width: " + width);
        
        //string path = "sprites/images/" + mainImage + "/" + mainImage;
        pixelRes.x = t.width / res.x;
        pixelRes.y = t.height / res.y;
        //Debug.Log(pixelWidth);
        //Debug.Log(pixelWidth);
        GenerateTiles();
        GenerateAndPlaceBlanks();
        PlaceInitialTile();
        GridManager.instance.DrawGrid(res.y , res.x, width, height);
        //StartCoroutine(Solve());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceInitialTile()
    {
        float pos = 0;
        if(res.y%2 == 0)
        {
            //no middle row number of pieces
            pos = (res.x * res.y) / 2 + (res.y / 2);
        } else
        {
            //middle row number of pieces
            pos = (res.x*res.y) / 2;
        }
        tiles[(int)pos ].gameObject.SetActive(true);
        placeTileInCorrectPosition((int)pos);
        ChangeFront();

    }

    public void GenerateTiles()
    {
        for(int i = 0; i < (res.x * res.y); i++)
        {
            tiles.Add(Instantiate(tilePre, genTransform.position,Quaternion.identity));
            tiles[i].GetComponent<Tile>().SetImage( i, texture, pixelRes);
            tiles[i].name = "Tile" + i;
            tiles[i].GetComponent<Tile>().number = i;
            tiles[i].GetComponent<Tile>().dragable = true;
            tiles[i].GetComponent<Tile>().placed = false;
            tiles[i].gameObject.SetActive(false);
            //tiles[i].gameObject.transform.localScale = new Vector3(tiles[i].transform.localScale.x * ((width) / 2.4f), tiles[i].transform.localScale.y * ((height) / 2.4f), 1f);
        }



        //Shuffle(tiles);
    }

    public void GenerateAndPlaceBlanks()
    {
        for(int i = 0; i < (res.x * res.y); i++)
        {
            float xOffset = width * (i % res.x);
            float yOffset = -height * (int)(i / res.x);
            //Debug.Log(width);
            blankTiles.Add(Instantiate(blankPre, new Vector3(transform.position.x + xOffset + width/2, transform.position.y + yOffset - height/2, -.4f), Quaternion.identity));
            blankTiles[i].transform.localScale = new Vector3(blankTiles[i].transform.localScale.x *width, blankTiles[i].transform.localScale.y * height, 1f);

        }
    }

    public void CheckPlace(int i)
    {

        if (blankTiles[i].GetComponent<BlankTile>().isOver)
        {
            placeTileInCorrectPosition(i);
        } else
        {
            tiles[i].transform.position = genTransform.position;
            tiles[i].GetComponent<Tile>().dragging = false;
            SetFront(i);
        }

    }

    public void ChangeFront()
    {
        //set new top
        List<GameObject> frontTiles = new List<GameObject>();
        for (int u = 0; u < (res.x*res.y); u++)
        {
            if (tiles[u].activeSelf && tiles[u].GetComponent<Tile>().dragable && !tiles[u].GetComponent<Tile>().dragging)
            {
                frontTiles.Add(tiles[u]);
            }
        }
        if (frontTiles.Count > 0)
        {
            int rand = Random.Range(0, frontTiles.Count);
            frontTiles[rand].transform.position = new Vector3(frontTiles[rand].transform.position.x, frontTiles[rand].transform.position.y, frontTiles[rand].transform.position.z - .1f);
            Debug.Log("Changed Front");
        }
    }

    public void SetFront(int i)
    {
        for(int u = 0; u < (res.x * res.y); u++)
        {
            tiles[u].transform.position = new Vector3(tiles[u].transform.position.x, tiles[u].transform.position.y, 0);
        }

        tiles[i].transform.Translate(0, 0, -.1f);
    }

    /*public void Solve()
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            placeTileInCorrectPosition(i);
        }
    }*/

    public void placeTileInCorrectPosition(int i)
    {
        //Debug.Log(i);
        GameObject t = tiles[i];
        float xOffset = width * ((t.GetComponent<Tile>().number)%res.x);
        float yOffset = -height * (int)((t.GetComponent<Tile>().number) / res.x);
        t.transform.position = new Vector3(transform.position.x + xOffset + width/2, transform.position.y + yOffset - height/2, 0);
        t.GetComponent<Tile>().dragable = false;
        t.GetComponent<Tile>().placed = true;
        EnableSurroundingTiles(i);

        //check if all the tiles are placed
        if (CheckAllPlaced())
        {
            //erase grid
            GridManager.instance.EraseLines();
            //clear and delete all tiles
            EraseTiles();
            //tell sector manager
            SectorManager.instance.SectorSolved();
            
        }

    }

    public void Solve2()
    {
        StartCoroutine(Solve());
    }
    public IEnumerator Solve()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            placeTileInCorrectPosition(i);
            yield return new WaitForSeconds(timetosolve / (res.x * res.y));
        }
        
    }

    void Shuffle(List<GameObject> a)
    {
        // Loop array
        for (int i = a.Count - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = UnityEngine.Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overwrite when we swap the values
            GameObject temp = a[i];

            // Swap the new and old values
            a[i] = a[rnd];
            a[rnd] = temp;
        }
    }

    public void EnableSurroundingTiles(int i)
    {
        //get up
        //Debug.Log(i);
        if(i-res.x >= 0)
        {
           // Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!tiles[i - (int)res.x].activeSelf)
            {
                tiles[i - (int)res.x].SetActive(true);
                //placeTileInCorrectPosition(i - (int)Mathf.Sqrt(numPieces));
            }
        }
        //get down
        if(i + res.x < (res.x * res.y))
        {
            //Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!tiles[i + (int)res.x].activeSelf)
            {
                tiles[i + (int)res.x].SetActive(true);
                //placeTileInCorrectPosition(i + (int)Mathf.Sqrt(numPieces));
            }
        }
        //get left
        if(i % res.x != 0)
        {
            //Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!tiles[i - 1].activeSelf)
            {
                tiles[i - 1].SetActive(true);
                //placeTileInCorrectPosition(i - 1);
            }
        }
        //get right
        if(i % res.x != res.x - 1)
        {
            //Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!tiles[i + 1].activeSelf)
            {
                tiles[i + 1].SetActive(true);
                //placeTileInCorrectPosition(i + 1);
            }
        }
            
    }

    public void DestoyTiles()
    {
        tiles.Clear();
        blankTiles.Clear();
    }

    public bool CheckAllPlaced()
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            if(tiles[i].GetComponent<Tile>().placed == false)
            {
                return false;
            }
        }
        return true;
    }

    public void EraseTiles()
    {
        for(int i = tiles.Count - 1; i >= 0; i--)
        {
            Destroy(tiles[i]);
            Destroy(blankTiles[i]);
        }
        tiles.Clear();
        blankTiles.Clear();
    }


}
