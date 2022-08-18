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

    
    public List<List<GameObject>> puzzles;
    public List<GameObject> blankTiles;

    public static TileManager instance;

    //public Sprite sprite;

    public List<Texture2D> textures;

    public float maxWidth;
    public float maxHeight;
    //keeping track of the current sector;
    public int cS;

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
    public void StartTiles( int curSector)
    {
        
        cS = curSector;
        //width = t.width / (res.x*100);
        //height = t.height / (res.y*100);
        if(maxWidth/textures[cS].width <= maxHeight / textures[cS].height)
        {
            width = maxWidth / res.x;
            //height = t.height * width / t.width;
            height = ((textures[cS].height * maxWidth) / textures[cS].width) / res.y;
        } else
        {
            height = maxHeight / res.y;
            width = ((textures[cS].width * maxHeight) / textures[cS].height) / res.x;
        }

        Debug.Log("Height: " + height + " Width: " + width);
        
        //string path = "sprites/images/" + mainImage + "/" + mainImage;
        pixelRes.x = textures[cS].width / res.x;
        pixelRes.y = textures[cS].height / res.y;
        //Debug.Log(pixelWidth);
        //Debug.Log(pixelWidth);
        //GenerateTiles();
        GenerateAndPlaceBlanks();
        //AssignSprites(cS);
        //PlaceInitialTile();
        GridManager.instance.DrawGrid(res.y , res.x, width, height);
        EnableTiles();
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
        puzzles[cS][(int)pos ].gameObject.SetActive(true);
        puzzles[cS][(int)pos].GetComponent<Tile>().active = true;
        placeTileInCorrectPosition((int)pos);
        ChangeFront();

    }

    public List<GameObject> GenerateTiles()
    {
        List<GameObject> initialGen = new List<GameObject>();
        for(int i = 0; i < (res.x * res.y); i++)
        {
            initialGen.Add(Instantiate(tilePre, genTransform.position,Quaternion.identity));
            //initialGen[i].GetComponent<Tile>().SetImage( i, textures[cS], pixelRes);
            initialGen[i].name = "Tile" + i;
            initialGen[i].GetComponent<Tile>().number = i;
            initialGen[i].GetComponent<Tile>().dragable = true;
            initialGen[i].GetComponent<Tile>().placed = false;
            
            initialGen[i].gameObject.SetActive(false);
            //puzzles[cS][i].gameObject.transform.localScale = new Vector3(puzzles[cS][i].transform.localScale.x * ((width) / 2.4f), puzzles[cS][i].transform.localScale.y * ((height) / 2.4f), 1f);
        }

        
        return initialGen;
        //Shuffle(puzzles[cS]);
    }

    public void GenerateAllInitialTiles(int numSectors, Texture2D stext)//, Texture2D t)
    {

        if (maxWidth / stext.width <= maxHeight / stext.height)
        {
            width = maxWidth / res.x;
            //height = t.height * width / t.width;
            height = ((stext.height * maxWidth) / stext.width) / res.y;
        }
        else
        {
            height = maxHeight / res.y;
            width = ((stext.width * maxHeight) / stext.height) / res.x;
        }


        puzzles = new List<List<GameObject>>();
        //textures[cS] = t;
        cS = 0;
        for(int i = 0; i < numSectors; i++)
        {
            cS = i;
            //Debug.Log(puzzles.Count);
            puzzles.Add(GenerateTiles());
            PlaceInitialTile();
            DisableTiles();
        }

    }
    public void AssignAllSprites()
    {
        pixelRes.x = textures[cS].width / res.x;
        pixelRes.y = textures[cS].height / res.y;
        for (int i = 0; i< puzzles.Count; i++)
        {
            AssignSprites(i);
        }

        //SectorPreviewManager.instance.AssignImages();
    }

    public void AssignSprites(int cS)
    {
        //pixelRes.x = textures[cS].width / res.x;
        //pixelRes.y = textures[cS].height / res.y;
        for (int i = 0; i < res.x*res.y; i++)
        {
            puzzles[cS][i].GetComponent<Tile>().SetImage(i, textures[cS], pixelRes);
        }
        //puzzles[cS]
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
            puzzles[cS][i].transform.position = genTransform.position;
            puzzles[cS][i].GetComponent<Tile>().dragging = false;
            SetFront(i);
        }

    }

    public void ChangeFront()
    {
        //set new top
        List<GameObject> frontTiles = new List<GameObject>();
        for (int u = 0; u < (res.x*res.y); u++)
        {
            if (puzzles[cS][u].activeSelf && puzzles[cS][u].GetComponent<Tile>().dragable && !puzzles[cS][u].GetComponent<Tile>().dragging)
            {
                frontTiles.Add(puzzles[cS][u]);
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
            puzzles[cS][u].transform.position = new Vector3(puzzles[cS][u].transform.position.x, puzzles[cS][u].transform.position.y, 0);
        }

        puzzles[cS][i].transform.Translate(0, 0, -.1f);
    }

    /*public void Solve()
    {
        for(int i = 0; i < puzzles[cS].Count; i++)
        {
            placeTileInCorrectPosition(i);
        }
    }*/

    public void placeTileInCorrectPosition(int i)
    {
        //Debug.Log(i);
        GameObject t = puzzles[cS][i];
        float xOffset = width * ((t.GetComponent<Tile>().number)%res.x);
        float yOffset = -height * (int)((t.GetComponent<Tile>().number) / res.x);
        t.transform.position = new Vector3(transform.position.x + xOffset + width/2, transform.position.y + yOffset - height/2, 0);
        t.GetComponent<Tile>().dragable = false;
        t.GetComponent<Tile>().placed = true;
        EnableSurroundingTiles(i);

        //check if all the puzzles[cS] are placed
        if (CheckAllPlaced())
        {
            //erase grid
            GridManager.instance.EraseLines();
            //clear and delete all puzzles[cS]
            DisableTiles();
            EraseBlanks();
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
        for (int i = 0; i < puzzles[cS].Count; i++)
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
            if (!puzzles[cS][i - (int)res.x].activeSelf)
            {
                puzzles[cS][i - (int)res.x].SetActive(true);
                puzzles[cS][i - (int)res.x].GetComponent<Tile>().active = true;
                //placeTileInCorrectPosition(i - (int)Mathf.Sqrt(numPieces));
            }
        }
        //get down
        if(i + res.x < (res.x * res.y))
        {
            //Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!puzzles[cS][i + (int)res.x].activeSelf)
            {
                puzzles[cS][i + (int)res.x].SetActive(true);
                puzzles[cS][i + (int)res.x].GetComponent<Tile>().active = true;
                //placeTileInCorrectPosition(i + (int)Mathf.Sqrt(numPieces));
            }
        }
        //get left
        if(i % res.x != 0)
        {
            //Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!puzzles[cS][i - 1].activeSelf)
            {
                puzzles[cS][i - 1].SetActive(true);
                puzzles[cS][i - 1].GetComponent<Tile>().active = true;
                //placeTileInCorrectPosition(i - 1);
            }
        }
        //get right
        if(i % res.x != res.x - 1)
        {
            //Debug.Log(i - (int)Mathf.Sqrt(numPieces));
            if (!puzzles[cS][i + 1].activeSelf)
            {
                puzzles[cS][i + 1].SetActive(true);
                puzzles[cS][i + 1].GetComponent<Tile>().active = true;
                //placeTileInCorrectPosition(i + 1);
            }
        }
            
    }

    public void DestoyTiles()
    {
        puzzles[cS].Clear();
        blankTiles.Clear();
    }

    public bool CheckAllPlaced()
    {
        for(int i = 0; i < puzzles[cS].Count; i++)
        {
            if(puzzles[cS][i].GetComponent<Tile>().placed == false)
            {
                return false;
            }
        }
        return true;
    }

    public void DisableTiles()
    {
        for(int i = puzzles[cS].Count - 1; i >= 0; i--)
        {
            puzzles[cS][i].SetActive(false);
            
        }
        //puzzles[cS].Clear();
    }

    public void EnableTiles()
    {
        for (int i = puzzles[cS].Count - 1; i >= 0; i--)
        {
            if (puzzles[cS][i].GetComponent<Tile>().active == true)
            {
                puzzles[cS][i].SetActive(true);
            }

        }
    }
    public void EraseBlanks()
    {
        for (int i = blankTiles.Count - 1; i >= 0; i--)
        {
            
            Destroy(blankTiles[i]);
        }
        
        blankTiles.Clear();
    }


}
