using UnityEngine;
using System.Collections;

public class EnemyLogic {
    // the required variables for the EnemyLogic
    public float health;
    public float speed;
    public int cashReward;
    public int enemyPenalty = 1;

    //this is the constructor for the EnemyLogic
    //this is so the enemy stats will change depending on the current wave
    public EnemyLogic(int currentWave)
    {
        health = health + currentWave * (currentWave / 4);
        speed = speed + currentWave;
        cashReward = cashReward + currentWave + 2;
    }

    //the takeDamage functions calculated the amount of damage inflicted on an enemy
	public void takeDamage(float damageInflicted)
	{
		health -= damageInflicted;
	}

}