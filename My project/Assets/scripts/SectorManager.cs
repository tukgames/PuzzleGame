using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static SectorManager instance;

    public List<Texture2D> images;

    public List<GameObject> sectors;

    public Vector2 res;

    public float maxHeight;
    public float maxWidth;

    public Vector2 pixelRes;

    [HideInInspector]
    public float width;
    [HideInInspector]
    public float height;

    public GameObject sectorPre;

    Texture2D texture;

    int activatedSector;

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
        StartSectors(images[0]);
    }

    public void StartSectors(Texture2D t)
    {
        texture = t;
        if (maxWidth / t.width <= maxHeight / t.height)
        {
            width = maxWidth / res.x;
            //height = t.height * width / t.width;
            height = ((t.height * maxWidth) / t.width) / res.y;
        }
        else
        {
            height = maxHeight / res.y;
            width = ((t.width * maxHeight) / t.height) / res.x;
        }

        pixelRes.x = t.width / res.x;
        pixelRes.y = t.height / res.y;

        GridManager.instance.DrawGrid(res.y, res.x, width, height);

        GenerateAndPlaceSectors();
        CreateAndAssignSprites();
        //GenerateAllTilesWithSprite();
        Sprite sprite = sectors[0].GetComponent<SpriteRenderer>().sprite;
        //activatedSector = ;
        //Debug.Log("width: " + sprite.rect.width + ", height: " + sprite.rect.height);
        var croppedTexture = new Texture2D((int)Mathf.Abs(sprite.rect.width), (int)Mathf.Abs(sprite.rect.height));
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                 (int)(sprite.textureRect.y) - (int)pixelRes.y,
                                                (int)Mathf.Abs(sprite.textureRect.width),
                                                (int)Mathf.Abs(sprite.textureRect.height));
        Debug.Log(sprite.textureRect.y);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        TileManager.instance.GenerateAllInitialTiles((int)(res.x * res.y), croppedTexture);
        GenerateAllTilesWithSprite();
    }

    public void GenerateAllTilesWithSprite()
    {
        for(int i = 0; i < res.y*res.x; i++)
        {
            Sprite sprite = sectors[i].GetComponent<SpriteRenderer>().sprite;
            //activatedSector = i;
            //Debug.Log("width: " + sprite.rect.width + ", height: " + sprite.rect.height);
            var croppedTexture = new Texture2D((int)Mathf.Abs(sprite.rect.width), (int)Mathf.Abs(sprite.rect.height));
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                     (int)(sprite.textureRect.y) - (int)pixelRes.y,
                                                    (int)Mathf.Abs(sprite.textureRect.width),
                                                    (int)Mathf.Abs(sprite.textureRect.height));
            Debug.Log(sprite.textureRect.y);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            
            TileManager.instance.textures.Add(croppedTexture);
            //TileManager.instance.AssignSprites(i);

        }
    }

    public void GenerateAndPlaceSectors()
    {
        for (int i = 0; i < (res.x * res.y); i++)
        {
            float xOffset = width * (i % res.x);
            float yOffset = -height * (int)(i / res.x);
            //Debug.Log(width);
            sectors.Add(Instantiate(sectorPre, new Vector3(transform.position.x + xOffset + width / 2, transform.position.y + yOffset - height / 2, -.4f), Quaternion.identity));
            sectors[i].GetComponent<Sector>().num = i;

            
        }
    }

    public void CreateAndAssignSprites()
    {
        for(int i = 0; i < sectors.Count; i++)
        {
            sectors[i].GetComponent<Sector>().SetImage(texture, pixelRes);
        }
    }

    public void beginPuzzle(int i)
    {
        Sprite sprite = sectors[i].GetComponent<SpriteRenderer>().sprite;
        activatedSector = i;
        //Debug.Log("width: " + sprite.rect.width + ", height: " + sprite.rect.height);
        var croppedTexture = new Texture2D((int)Mathf.Abs(sprite.rect.width),(int)Mathf.Abs(sprite.rect.height));
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                 (int)(sprite.textureRect.y) - (int)pixelRes.y,
                                                (int)Mathf.Abs(sprite.textureRect.width),
                                                (int)Mathf.Abs(sprite.textureRect.height));
        Debug.Log(sprite.textureRect.y);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        GridManager.instance.EraseLines();
        TileManager.instance.StartTiles( i);
        DeactivateSectors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SectorSolved()
    {
        /*sector was solved and now the sprite should be shown and
        the sectors should be redrawn with the grid manager.*/
        ActivateSectors();
        GridManager.instance.DrawGrid(res.y, res.x, width, height);
        sectors[activatedSector].GetComponent<SpriteRenderer>().enabled = true;
        sectors[activatedSector].GetComponent<Sector>().isSelectable = false;

    }

    public void DeactivateSectors()
    {
        for(int i = 0; i < sectors.Count; i++)
        {
            sectors[i].SetActive(false);
        }
    }
    public void ActivateSectors()
    {
        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].SetActive(true);
            
        }
    }
}
