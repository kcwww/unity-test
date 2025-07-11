using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer sprite;

    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.36f, -0.27f, 0);
    Vector3 rightPosReverse = new Vector3(0.05f, -0.27f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);

    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;
        if (isLeft)
        {
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            sprite.flipY = isReverse;
            sprite.sortingOrder = isReverse ? 4 : 6;
        }   else {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            sprite.flipX = isReverse;
            sprite.sortingOrder = isReverse ? 6 : 4;
        }
    }
}
