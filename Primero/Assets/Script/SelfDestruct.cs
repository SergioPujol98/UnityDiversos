using UnityEngine;
using System.Collections;
public class SelfDestruct : MonoBehaviour
{
    public float fuseLength = 0.1f; // llargada de la metxa
    private float destructTime = 0.0f;
    void Start()
    {
        //Quan es crea l'objecte, establir moment de la destrucció
        destructTime = Time.time + fuseLength;
    }
    void Update()
    {
        //A cada frame, mirar si ha arribat el moment de la destrucció.
        if (destructTime < Time.time)
            Destroy(gameObject);
    }
}
