using UnityEngine;
using System.Collections;

public class EnemyLogic {
    public float health;
    public float speed;
    public int cashReward;
    public int enemyPenalty = 1;

    public EnemyLogic(int currentWave)
    {
        health = health + currentWave * (currentWave / 4);
        speed = speed + currentWave;
        cashReward = cashReward + currentWave + 2;
    }

	public void takeDamage(float damageInflicted)
	{
		health -= damageInflicted;
	}

}