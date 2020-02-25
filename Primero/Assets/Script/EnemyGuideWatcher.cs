using System.Collections;
using UnityEngine;
using System.Collections;
public class EnemyGuideWatcher : MonoBehaviour
{
    public EnemyControllerScript enemyObject = null;
    void OnTriggerExit2D(Collider2D otherObj)
    {
        // Aquest event es produeix quan Enemy està a punt
        //de sortir de la Platform.
        //Canviem la direcció del desplaçament
        if (otherObj.tag == "Platform")
            enemyObject.switchDirections();
    }
}
