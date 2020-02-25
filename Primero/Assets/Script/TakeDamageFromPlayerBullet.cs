using UnityEngine;
using System.Collections;
public class TakeDamageFromPlayerBullet : MonoBehaviour
{
    //Definicio de delegat i event per tractar impacte de bala
    public delegate void hitByPlayerBullet();
    public event hitByPlayerBullet hitByBullet;
    //Tractament de col.lisio amb DefenseCollider: si
    //es amb una bala, generar event.
    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        if (collidedObject.tag == "PlayerBullet")
        {
            Debug.Log("TakeDamageFromPlayer.DefenseCollider col.lisiona amb:" + collidedObject.tag);
            if (hitByBullet != null)
            {
                Debug.Log("TakeDamageFromPlayer. Enemic Tocat!!!!");
                hitByBullet();
            }
        }
    }


}
