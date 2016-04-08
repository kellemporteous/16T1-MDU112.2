using UnityEngine;
using System.Collections;

public class TowerLogic {
    public float fireRate;
    public float damage;
    public float fireTime;
    public int muzzleIndex;
    public int Tower;

    public TowerLogic(int towerType)
    {
        Tower = towerType;
        if (Tower == 0)
        {
            fireRate = 1.2f;
            damage = 3f;
        }

        else if(Tower == 1)
        {
            fireRate = 0.6f;
            damage = 6f;
        }
    }

	public int closestEnemy(Vector3 TowerPosition, GameObject[] enemies)
	{
		foreach (GameObject enemy in enemies)
		{
			//float distance = 
		}

	}

    public int fireProjectile(GameObject[] turrentMuzzle, float TowerFireRate)
    {

    }
}
