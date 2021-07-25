using UnityEngine;

public class Tile : MonoBehaviour
{
    private int value;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public Tile(float x, float y)
    {
        transform.position += new Vector3(x, y);
    }

    private void Start()
    {
        value = -1;
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public int GetValue()
    {
        return value;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        boxCollider.enabled = false;
    }
}
