using UnityEngine;
using System.Collections;

public class GameLogic  {
    public int wave = 1;
    public int maxWave = 10;
    public int cash = 150;
    public int lives = 3;
    public int waveCounter = 10;
    public float spawnTimer = 0f;
    
    public void updateCash(int cashAmount)
    {
        cash += cashAmount;
    }

    public void updateLives(int livesAmount)
    {
        lives -= livesAmount;
    }

    public bool haveCashForTower(int costOfTower)
    {
        if (cash >= costOfTower)
            return true;
            return false;
    }

    public int spawnAmount(int currentWave)
    {
        return currentWave + 8;
    }

    public float spawnInterval(int currentWave)
    {
        return 8f / currentWave;
    }

    public bool spawnWaveEnemy(int numberOfEnemies, float betweenSpawns)
    {
        if (spawnTimer >= betweenSpawns && waveCounter <= numberOfEnemies)
        {
            spawnTimer = 0;
            waveCounter++;
            return true;
        }

        else
        {
            if (waveCounter >= numberOfEnemies)
            {
                waveCounter = 0;
                wave++;
            }

            spawnTimer += Time.deltaTime;
            return false;
        }
    }
}
