using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets i;

    private void Awake()
    {
        i = this;
    }
    public Sprite snakeHeadSprite;
    public Sprite foodSprite;
    public Sprite snakeBodySpriteVertical;
    public Sprite snakeBodySpriteHorizontal;
    public Sprite snakeCornerUpperLeft;
    public Sprite snakeCornerUpperRight;
    public Sprite snakeCornerLowerLeft;
    public Sprite snakeCornerLowerRight;
    public int Width, Height;
}
