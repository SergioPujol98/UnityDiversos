using UnityEngine;
using System.Collections;
public class EnemyRespawner : MonoBehaviour
{
    //Enemigo
    public GameObject spawnEnemy = null;
    // Variable que sirve para decir cuanto tiempo tardara en aparecer otro enemigo.
    float respawnTime = 0.0f;

    // Detectar cuando un enemigo a muerto entonces lanzar el metodo.
    void OnEnable()
    {
        EnemyControllerScript.enemyDied += scheduleRespawn;
    }
    void OnDisable()
    {
        EnemyControllerScript.enemyDied -= scheduleRespawn;
    }
    // Si reaparecen reaparecen en el tiempo que hemos marcado en el respawnTime, en este caso Time.time + 2.5f
    void scheduleRespawn(int enemyScore)
    {
        // Randomly decide if we will respawn or not
        if (Random.Range(0, 10) < 5)
            return;
        respawnTime = Time.time + 2.5f;
    }
    
    void Update()
    {
        if (respawnTime > 0.0f)
        {
            if (respawnTime < Time.time)
            {
                respawnTime = 0.0f;
                GameObject newEnemy =
                Instantiate(spawnEnemy) as GameObject;
                newEnemy.transform.position = transform.position;
            }
        }
    }
}