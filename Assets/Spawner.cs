
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public SpawnData[] spawndatas;

    int level;
    float timer; 

    void Awake() {
        spawnPoints = GetComponentsInChildren<Transform>();
    }
    void Update()
    {
        if (!GameManager.instance.isLive) return;
        
        timer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 10f);
        level = Mathf.Min(level, spawndatas.Length - 1);
        if (timer > spawndatas[level].spawnTime)
        {
            Spawn();
            timer = 0;
        }

        void Spawn() {
            int randomMonster = Random.Range(0, level + 1);
            GameObject enemy = GameManager.instance.poolManager.Get(0);
            enemy.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
            enemy.GetComponent<Enemy>().Init(spawndatas[randomMonster]);
        }
    }
}


[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;

    
}
