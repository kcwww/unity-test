using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class moveMent : MonoBehaviour
{
    public float speed = 5.0f;
    public Vector2 inputVec;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] controllers;

    Animator anim;
    Rigidbody2D rigid;

    SpriteRenderer spriter;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    void OnEnable() {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = controllers[GameManager.instance.playerId];
    }

    // Update is called once per frame
    void OnMove(InputValue value)
    {
        if (!GameManager.instance.isLive) return;
        inputVec = value.Get<Vector2>();
    }

    void FixedUpdate() {
        if (!GameManager.instance.isLive) return;
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate() {
        if (!GameManager.instance.isLive) return;

        anim.SetFloat("Speed", inputVec.magnitude);
        
        if (inputVec.x != 0) {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        if (!GameManager.instance.isLive) return;

        GameManager.instance.health -= Time.deltaTime * 10;

        if (GameManager.instance.health <= 0) {
            for (int index=2; index < transform.childCount; index++) {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }
}
