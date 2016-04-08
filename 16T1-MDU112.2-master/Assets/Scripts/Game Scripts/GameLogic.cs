using UnityEngine;
using System.Collections;

public class GameLogic  {
    // required variables for GameLogic
    public int wave = 1;
    public int maxWave = 10;
    public int cash = 150;
    public int lives = 3;
    public int waveCounter = 10;
    public float spawnTimer = 0f;
    
    // The updateCash function adjusts the amount of cash shown depending the the amount the player has accumulated
    public void updateCash(int cashAmount)
    {
        cash += cashAmount;
    }

    // the updateLives function changes the amount of lives the player has depending on many lives have been lost
    public void updateLives(int livesAmount)
    {
        lives -= livesAmount;
    }

    //the haveCashForTower function is to see if the player has enough cash to buy a turret
    public bool haveCashForTower(int costOfTower)
    {   // to see if the amount of cash is higher than the cost of a turret
        if (cash >= costOfTower)
            return true;
            // if cash is not higher than the cost of a turret
            return false;
    }

    // the function spawnAmount dictates how many will spawn in the current round
    public int spawnAmount(int currentWave)
    {   // the amount of enemies that will spawn per round
        return currentWave + 8;
    }

    // the spawnInterval function dictates the time between enemies spawning
    public float spawnInterval(int currentWave)
    {   // the rate at which the enemies will spawn per round
        return 8f / currentWave;
    }

    //the spawnWaveEnemy function dictates how many enemies are left to spawn per round and the time between waves
    public bool spawnWaveEnemy(int numberOfEnemies, float betweenSpawns)
    {   // checks if its time for an enemy to spawn
        // and checks how many enemies are left to spawn in the current round
        if (spawnTimer >= betweenSpawns && waveCounter <= numberOfEnemies)
        {
            //this resets the spawn timer, increase the wave counter and then spawns an enemy
            spawnTimer = 0;
            waveCounter++;
            return true;
        }

        else
        {
            //when the last enemy spawns the wave counter will reset and the next round will start
            if (waveCounter >= numberOfEnemies)
            {
                waveCounter = 0;
                wave++;
            }

            // the return will be false if the spawn timer is not correct
            spawnTimer += Time.deltaTime;
            return false;
        }
    }
}
