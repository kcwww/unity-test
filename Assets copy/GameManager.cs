using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive = false;
    public float gameTime;
    public float maxGameTime = 2 * 10f;
    
    
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;    
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = {3, 5, 10, 100, 150, 210, 280, 360, 450, 600};

    [Header("# Game Object")]
    public PoolManager poolManager;
    public moveMent player;
    public LevelUp uiLevelUp;

    public Result uiResult;
    public Transform uiJoy;
    public GameObject enemyCleaner;

    
    void Awake() {
        instance = this;
        instance.isLive = false;
        
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;
        player.gameObject.SetActive(true);

        // 임시
        uiLevelUp.Select(playerId % 2);
        isLive = true;
        instance.isLive = true;
        Resume();

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        instance.isLive = false;
        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);

    }

    public void GameVictory()
    {
        
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        instance.isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry ()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GameQuit ()
    {
        Application.Quit();
    }

    void Update()
    {
        if (!isLive) return;

        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }

        
    }

    public void GetExp()
    {
        if (!isLive) return;
        exp++;
        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            exp = 0;
            level++;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one;
    }
}
