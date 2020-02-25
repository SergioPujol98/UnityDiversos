using UnityEngine;
using System.Collections;
[RequireComponent(typeof(TextMesh))]
public class RoundWatcher : MonoBehaviour
{
    public int currRound = 1;
    private TextMesh roundDisplayMesh = null;
    void Start()
    {
        roundDisplayMesh = gameObject.GetComponent<TextMesh>();
        roundDisplayMesh.text = "Round: " + currRound.ToString();
    }
    void OnEnable()
    {
        BossEventController.bossDied += increaseRound;
    }
    void OnDisable()
    {
        BossEventController.bossDied -= increaseRound;
    }
    void increaseRound(int ignore)
    {
        currRound += 1;
        roundDisplayMesh.text = "Round: " + currRound.ToString();
    }
}
