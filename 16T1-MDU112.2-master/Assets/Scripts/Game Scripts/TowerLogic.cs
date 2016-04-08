using UnityEngine;
using System.Collections;

public class TowerLogic {
    //The required variables for TowerLogic
    public float fireRate;
    public float damage;
    public float fireTime;
    public int muzzleIndex;
    public int Tower;

    //This is the constuctor for the TowerLogic
    //This constructor is here to change the stats for the towers
    public TowerLogic(int towerType)
    {
        Tower = towerType;
        if (Tower == 0)
        {
            //stats for the missile tower
            fireRate = 1.2f;
            damage = 3f;
        }

        else if(Tower == 1)
        {
            //stats for the laser tower
            fireRate = 0.6f;
            damage = 6f;
        }
    }
    // the closestEnemy function is for determining which enemy is close to the turrent
	public GameObject closestEnemy(Vector3 TowerPosition, GameObject[] enemies)
	{
        //this is set what the turret can see
        GameObject closestEnemy = null;
        float closestDistance = 1000.0f;

        //the foreach is so that the turrent can determine the distance between the turrent and the enemy array
        foreach (GameObject enemy in enemies)
        {
            //finds the distance from turrent to the enemy
            float distance = Vector3.Distance(TowerPosition, enemy.transform.position);
            // checks if the closestDistance is greater then the distance
            if (closestDistance > distance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    //the function fireProjectile determines when the turrent should fire
    public int fireProjectile(GameObject[] turrentMuzzle, float TowerFireRate)
    {
        if(Time.time >= fireTime)
        {   // checks that the muzzle index hasnt incease a lot
            if (muzzleIndex < turrentMuzzle.Length - 1)
            {
                muzzleIndex++;
            }

            else muzzleIndex = 0;
            fireTime += TowerFireRate;
        }

        //the index resets if this happens
        return muzzleIndex;
    }
}
