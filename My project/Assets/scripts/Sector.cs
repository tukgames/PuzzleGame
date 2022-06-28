using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{

    public int num;

    public bool isOver;

    public bool isSelectable;
    // Start is called before the first frame update
    void Start()
    {
        isOver = false;
        isSelectable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        isOver = true;
    }

    void OnMouseExit()
    {
        isOver = false;
    }

    private void OnMouseUp()
    {
        if (isOver && isSelectable)
        {
            SectorManager.instance.beginPuzzle(num);
        }
    }

    public void SetImage( Texture2D texture, Vector2 res)
    {
        float width2 = res.x - 0.001f;
        float height2 = res.y - 0.000f;
        float mult = (100f * 1199f) / texture.width;// /2;
        mult = texture.width / (SectorManager.instance.width * SectorManager.instance.res.x);

        Rect rect = new Rect(res.x * (num % SectorManager.instance.res.x), texture.height - (res.y * (int)(num / SectorManager.instance.res.x)), width2, -height2);

        Sprite s = Sprite.Create(texture, rect, Vector2.one * 0.5f, mult);//,100);// as Sprite;

        GetComponent<SpriteRenderer>().sprite = s;
        GetComponent<SpriteRenderer>().enabled = false;
        Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = S;
    }

    public void EnableSprite()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }


}
