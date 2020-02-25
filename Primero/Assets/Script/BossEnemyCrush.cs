using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyCrush : MonoBehaviour
{
    public delegate void hitByPlayerBullet();
    public event hitByPlayerBullet hitByBullet;
    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        if (collidedObject.tag == "Boss")
        {
            Vector3 enemyPos = transform.position;
            Vector3 particlePosition =
            new Vector3(enemyPos.x, enemyPos.y, enemyPos.z + 1.0f);
            Destroy(gameObject, 0.1f);

        }
    }

}
