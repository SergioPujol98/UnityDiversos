using UnityEngine;
using System.Collections;
public class PlayerStateController : MonoBehaviour
{
    //internal static object stateDelayTimer;

    //Definicio dels diferents estats del player
    public enum playerStates
    {
        idle = 0,
        left,
        right,
        jump,
        landing,
        falling,
        kill,
        resurrect,
        firingWeapon,
        _stateCount


}
     
    //Definicio del delegate playerStateHandler
    public delegate void playerStateHandler(PlayerStateController.playerStates newState);
    //Definicio d'event onStateChange i assignacio de onStateChange com a EventHandler
    public static event playerStateHandler onStateChange;

    public static float[] stateDelayTimerr = new float[(int)playerStates._stateCount];

    // Aquest metode es crida despres de Update() a cada frame.
    void LateUpdate()
    {
        if (!GameStates.gameActive) return;
        // Recollir l'input actual en el Horizontal axis (eix horitzontal)
        float horizontal = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");

        //Disparar
        float firing = Input.GetAxis("Fire1");
        if (firing > 0.0f)
        {
            if (onStateChange != null)
                onStateChange(PlayerStateController.playerStates.firingWeapon);
        }

        //Tractar segons el valor de l'input recollit
        if (horizontal != 0f)
        {
            //Hi ha algun moviment: canviar l'estat del protagonista a left o right
            if (horizontal < 0f)
            {
                if (onStateChange != null) onStateChange(PlayerStateController.
                 playerStates.left);
            }
            else
            {
                if (onStateChange != null) onStateChange(PlayerStateController.
                 playerStates.right);
            }
        }
        else
        //No hi ha cap moviment: canviar l'estat del protagonista a idle
        {
            if (onStateChange != null) onStateChange(PlayerStateController.
             playerStates.idle);
        }
        {
            if (jump > 0.0f)
            {
                if (onStateChange != null)
                    onStateChange(PlayerStateController.playerStates.jump);
            }

        }

        

    }

}