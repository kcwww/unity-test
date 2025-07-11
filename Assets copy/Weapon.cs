using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public int id;
    public int prefabId;
    public int count;
    public float speed;


    float timer;
    moveMent moveMent;

    void Awake() {
        moveMent = GameManager.instance.player;
    }

    

    void Update() {
        if (!GameManager.instance.isLive) return;
        
        switch (id) {
            case 0 :
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default:
                timer += Time.deltaTime;

                if (timer > speed) {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        
    }

    public void LevelUp(float damage, int count) {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0) Batch();

        moveMent.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data) {

        // Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = moveMent.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount * Character.Count;

        for (int idx = 0; idx < GameManager.instance.poolManager.prefabs.Length; idx++) {
            if (data.projectile == GameManager.instance.poolManager.prefabs[idx]) {
                prefabId = idx;
                break;
            }
        }

        switch (id) {
            case 0 :
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;

            default:
                speed = 0.5f * Character.WeaponRate;
                break;
        }

        Hand hand = moveMent.hands[(int)data.itemType];
        hand.sprite.sprite = data.hand;
        hand.gameObject.SetActive(true);
        moveMent.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Batch()
    {
        for (int idx=0; idx < count; idx++) {
            Transform bullet;
            if (idx < transform.childCount) {
                bullet = transform.GetChild(idx);
            } else {
                bullet = GameManager.instance.poolManager.Get(prefabId).transform;
                bullet.parent = transform;
            }
            
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * idx / count;

            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity per.
        }
    }

    void Fire() {
        if (!moveMent.scanner.nearestTarget) return;

        Vector3 targetPos = moveMent.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;

        Transform bullet = GameManager.instance.poolManager.Get(prefabId).transform;
        bullet.parent = transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
