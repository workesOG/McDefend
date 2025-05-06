using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCollection : MonoBehaviour
{
    public List<Sprite> sprites;

    public static SpriteCollection instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
