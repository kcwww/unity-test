using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public RuntimeAnimatorController[] animCon;
    public float speed;
    public float health;
    public float maxHealth;

    

    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;
    Animator anim;
    WaitForFixedUpdate wait;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate() 
    {
        if (!GameManager.instance.isLive) return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        Vector2 dirVec = target.position - rb.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
            
    }

    void LateUpdate() {
        if (!GameManager.instance.isLive) return;
        
        sr.flipX = target.position.x < rb.position.x;
    }

    void OnEnable() {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        
        col.enabled = true;
        rb.simulated = true;
        sr.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = maxHealth;
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Bullet") || !isLive) return;


        
        Bullet bullet = other.GetComponent<Bullet>();
        health -= bullet.damage;

        StartCoroutine(KnockBack());

        if (health > 0) {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        } else {
            isLive = false;
            col.enabled = false;
            rb.simulated = false;
            sr.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }

    }

    IEnumerator KnockBack()
    {
        yield return wait; // wait for the next fixed update
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rb.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
    }

    void Dead() {

        gameObject.SetActive(false);
    }
}
