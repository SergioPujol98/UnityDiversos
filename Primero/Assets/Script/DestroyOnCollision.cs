using UnityEngine;
using System.Collections;
public class DestroyOnCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D hitObj)
    {
        Debug.Log("DestroyOnCollision. Bala colisiona amb ojecte: " + hitObj.tag);
        DestroyObject(gameObject);
    }
}
