using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer sr;

    public int number;

    public bool dragging;
    public bool dragable;
    public bool isOver;
    private float distance;

    //public Texture2D t;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        isOver = false;
        dragging = false;
    }

    void OnMouseDown()
    {
        if (isOver)
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
            transform.Translate(new Vector3(0, 0, -.2f));
            TileManager.instance.ChangeFront();
        }
    }
    void OnMouseEnter()
    {
        isOver = true;
    }

    void OnMouseExit()
    {
        isOver = false;
    }

    void OnMouseUp()
    {
        dragging = false;

        TileManager.instance.CheckPlace(number);
    }

    void Update()
    {
        if (dragging && dragable)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x,rayPoint.y,transform.position.z);
        }
    }
    /*
     // assume "sprite" is your Sprite object
 var croppedTexture = new Texture2D( (int)sprite.rect.width, (int)sprite.rect.height );
 var pixels = sprite.texture.GetPixels(  (int)sprite.textureRect.x, 
                                         (int)sprite.textureRect.y, 
                                         (int)sprite.textureRect.width, 
                                         (int)sprite.textureRect.height );
 croppedTexture.SetPixels( pixels );
 croppedTexture.Apply();
     */
    public void SetImage( int num, Texture2D texture, Vector2 res)
    {
        //string path = "sprites/images/" + mainS;//specName;
        //Texture2D texture = Resources.Load<Texture2D>(path);

        //float width = //TileManager.instance.width;
        //Debug.Log("xres: " + res.x);
        //Debug.Log("yres: " + res.y);
        float width2 = res.x - 0.001f;
        float height2 = res.y - 0.000f;
        float mult = (100f * 1199f) / texture.width;// /2;
        mult = 100f / 2f;
        //Debug.Log(transform.localScale.x);
        //mult = (1199 * 100) / texture.width;
        mult = texture.width / (TileManager.instance.width * TileManager.instance.res.x);
        //mult = 100f;
        //Debug.Log(mult);
        Rect rect = new Rect(res.x * (num % TileManager.instance.res.x),texture.height-( res.y * (int)(num / TileManager.instance.res.x)), width2, -height2);
        //Debug.Log(width * (num % Mathf.Sqrt(TileManager.instance.numPieces)) + width);
        // Debug.Log(texture.height - (width * (int)(num / Mathf.Sqrt(TileManager.instance.numPieces))) - width);
        // Create the sprite
        //float mult = (100f * 1199f * 5f * 2.4f) / (texture.width * TileManager.instance.res.x * TileManager.instance.width);

        //Debug.Log(mult + " muilt");
        Sprite s = Sprite.Create(texture, rect, Vector2.one * 0.5f, mult);//,100);// as Sprite;

        //Debug.Log(s.pixelsPerUnit);
        //float mult = s.bounds.size.x / GetComponent<BoxCollider2D>().size.x;
        
        /*Sprite[] sprites = Resources.LoadAll<Sprite>("sprites/images/" + mainS + "/" + mainS);
        string specName = mainS + "_" + num;
        //string path = "";// = "sprites/images/" + mainS + "/" + mainS;//specName;
        //string[] paths = UnityEditor.AssetDatabase.FindAssets("t:Sprite");
        Sprite s = null;
        for (int i = 0 ; i < sprites.Length; i++)
        {
            //Debug.Log(sprites[i].name);
            if (sprites[i].name.Contains(specName))
            {
                s = sprites[i];
                break;
            }
        }*/
        // Debug.Log(sprites.Length);
        //Debug.Log(rect.xMin + " y, " + rect.yMin);
        GetComponent<SpriteRenderer>().sprite = s;
        Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = S;
        //gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);

    }
}
