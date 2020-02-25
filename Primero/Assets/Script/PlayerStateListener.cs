using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{
    public float playerWalkSpeed = 3f, playerJumpStrongY = 100f, 
    playerJumpStrongX = 100f;
    public GameObject playerRespawnPoint = null;
    public float playerJumpForceVertical = 500f; 
    public float playerJumpForceHorizontal = 250f;


    //Bullet
    public Transform bulletSpawnTransform; 
    public GameObject bulletPrefab = null;

    private PlayerStateController.playerStates previousState;
    private PlayerStateController.playerStates currentState;
    private Animator playerAnimator = null;
    private bool right = false;
    private bool playerHasLanded = true;

    public AudioClip Jump_clip;

    private Rigidbody2D rb2D;
   // private PlayerStateController.playerStates previousState = PlayerStateController.playerStates.idle;
    //private PlayerStateController.playerStates currentState = PlayerStateController.playerStates.idle;

    void OnEnable()
    {
        PlayerStateController.onStateChange += onStateChange;
    }

    void OnDisable()
    {
        PlayerStateController.onStateChange -= onStateChange;
    }
    void Start()
    {
        
        playerAnimator = GetComponent<Animator>();
        PlayerStateController.stateDelayTimerr[(int)PlayerStateController.playerStates.firingWeapon] = 0.0f;

    }
   
    void LateUpdate()
    {
        onStateCycle();
    }
    public void hitDeathTrigger()
    {
        onStateChange(PlayerStateController.playerStates.kill);
    }

    public void hitByCrusher()
    {
        onStateChange(PlayerStateController.playerStates.kill);
    }

    // Processar l'estat en cada cicle
    void onStateCycle()
    {

        Vector3 localScale = transform.localScale;
        transform.localEulerAngles = Vector3.zero;
        switch (currentState)
        {
            case PlayerStateController.playerStates.idle:
                break;
            case PlayerStateController.playerStates.left:

                transform.Translate(new Vector3((playerWalkSpeed * -1.0f) * Time.deltaTime, 0.0f, 0.0f));
                right = false;

                if (localScale.x > 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;

                }
                /*Debug.Log(left);
                Debug.Log(right);*/

                break;
            case PlayerStateController.playerStates.right:
                transform.Translate(new Vector3(playerWalkSpeed * Time.deltaTime,
                0.0f, 0.0f));
                if (localScale.x < 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;


                }
                right = true;

                /*   Debug.Log(left);
                   Debug.Log(right);*/
                break;
            case PlayerStateController.playerStates.jump:
                if (playerHasLanded)
                {
                    // jumpDirection determina si el salt es a la dreta, esquerra o vertical 
                    float jumpDirection = 0.0f;
                    if (currentState == PlayerStateController.playerStates.left)
                        jumpDirection = -1.0f;
                    else if (currentState == PlayerStateController.playerStates.right)
                        jumpDirection = 1.0f;
                    else jumpDirection = 0.0f;
                    // aplicar la força per fer el salt
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));
                    //indicar que el Player esta saltant en l'aire
                    playerHasLanded = false;
                }
                break;
            case PlayerStateController.playerStates.landing:
                break;
            case PlayerStateController.playerStates.falling:
                break;
            case PlayerStateController.playerStates.kill:
                onStateChange(PlayerStateController.playerStates.resurrect);

                break;
            case PlayerStateController.playerStates.resurrect:
                /* transform.position = playerRespawnPoint.transform.position;
                 transform.rotation = Quaternion.identity;
                 rb2D.velocity = Vector2.zero;*/
                onStateChange(PlayerStateController.playerStates.idle);

                break;
            case PlayerStateController.playerStates.firingWeapon:
                break;
        }

    

    }
    public void onStateChange(PlayerStateController.playerStates newState)
    {
        if (newState == currentState)
            return;
        // Comprovar que no hi hagi condicions per abortar l'estat
        if (checkIfAbortOnStateCondition(newState))
            return;

        if (!checkForValidStatePair(newState))
            return;
        Debug.Log(newState + ", current: " + currentState);
        rb2D = GetComponent<Rigidbody2D>();

        switch (newState)
        {

            case PlayerStateController.playerStates.idle:
                playerAnimator.SetBool("Walking", false);
                break;
            case PlayerStateController.playerStates.left:
                playerAnimator.SetBool("Walking", true);
                break;
            case PlayerStateController.playerStates.right:
                playerAnimator.SetBool("Walking", true);
                break;
            case PlayerStateController.playerStates.jump:
                if (playerHasLanded)
                {
                    // jumpDirection determina si el salt es a la dreta, esquerra o vertical 
                    float jumpDirection = 0.0f;
                    if (currentState == PlayerStateController.playerStates.left)
                        jumpDirection = -1.0f;
                    else if (currentState == PlayerStateController.playerStates.right)
                        jumpDirection = 1.0f;
                    else jumpDirection = 0.0f;
                    // aplicar la força per fer el salt
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));
                    //indicar que el Player esta saltant en l'aire
                    playerHasLanded = false;
                }
                break;
            case PlayerStateController.playerStates.landing:
                playerHasLanded = true;
                break;
            case PlayerStateController.playerStates.falling:
                break;
            case PlayerStateController.playerStates.kill:
                break;
            case PlayerStateController.playerStates.resurrect:

                transform.position = playerRespawnPoint.transform.position;
                transform.rotation = Quaternion.identity;
                rb2D.velocity = Vector2.zero;

                break;
            case PlayerStateController.playerStates.firingWeapon:
                // Construir l'objecte bala a partir del Prefab
                GameObject newBullet = (GameObject)Instantiate(bulletPrefab);


                PlayerStateController.stateDelayTimerr[(int)PlayerStateController.playerStates.firingWeapon] = Time.time + 0.25f;
                // Establir la posicio inicial de la bala creada
                //(la posicio de BulletSpawnTransform)
                newBullet.transform.position = bulletSpawnTransform.position;
                PlayerBulletController bullCon = newBullet.GetComponent<PlayerBulletController>();
                bullCon.playerObject = gameObject;

                bullCon.launchBullet();

                onStateChange(currentState); 
                break;

        }
        previousState = currentState;
        currentState = newState;
    }
    // Comprovar si es pot passar al nou estat des de l'actual.
    // Es tracten diversos estats que encara no estan implementats, perquè el
    // codi sigui més ilustratiu
    bool checkForValidStatePair(PlayerStateController.playerStates newState)
    {
        rb2D = GetComponent<Rigidbody2D>();
        bool returnVal = false;
        // Comparar estat actual amb el candidat a nou estat.
        switch (currentState)
        {
            case PlayerStateController.playerStates.idle:
                // Des de idle es pot passar a qualsevol altre estat
                returnVal = true;
                break;
            case PlayerStateController.playerStates.left:
                // Des de moving left es pot passar a qualsevol altre estat
                returnVal = true;
                break;
            case PlayerStateController.playerStates.right:
                // Des de moving right es pot passar a qualsevol altre estat
                returnVal = true;
                break;
            case PlayerStateController.playerStates.jump:
                // Des de Jump només es pot passar a landing o a kill.
                if (newState == PlayerStateController.playerStates.landing || newState == PlayerStateController.playerStates.kill
                    || newState == PlayerStateController.playerStates.firingWeapon
)
                {
                    returnVal = true;
                }
                else
                    returnVal = false;
                break;

               
            case PlayerStateController.playerStates.landing:
                // Des de landing només es pot passar a idle, left o right.
                if (
                newState == PlayerStateController.playerStates.left
                || newState == PlayerStateController.playerStates.right
                || newState == PlayerStateController.playerStates.idle
                || newState == PlayerStateController.playerStates.firingWeapon
                )
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.falling:
                // Des de falling només es pot passar a landing o a kill.
                if (
                newState == PlayerStateController.playerStates.landing
                || newState == PlayerStateController.playerStates.kill
                || newState == PlayerStateController.playerStates.firingWeapon
                )
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.kill:
                // Des de kill només es pot passar resurrect
                if (newState == PlayerStateController.playerStates.resurrect)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.resurrect:
                // Des de resurrect només es pot passar Idle
                if (newState == PlayerStateController.playerStates.idle)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.firingWeapon:
                returnVal = true;
                break;

        }
        return returnVal;
    }
    // Aquesta funció comprova si hi ha algun motiu que impedeixi passar al nou estat.
    // De moment no hi ha cap motiu per cancel·lar cap estat.
    bool checkIfAbortOnStateCondition(PlayerStateController.playerStates newState)
    {
        bool returnVal = false;
        switch (newState)
        {
            case PlayerStateController.playerStates.idle:
                break;
            case PlayerStateController.playerStates.left:
                break;
            case PlayerStateController.playerStates.right:
                break;
            case PlayerStateController.playerStates.jump:
                break;
            case PlayerStateController.playerStates.landing:
                break;
            case PlayerStateController.playerStates.falling:
                break;
            case PlayerStateController.playerStates.kill:
                break;
            case PlayerStateController.playerStates.resurrect:
                break;
            case PlayerStateController.playerStates.firingWeapon:
                if (PlayerStateController.stateDelayTimerr[(int)PlayerStateController.playerStates.firingWeapon] > Time.time)
                { returnVal = true; }
                break;
        }
        // Retornar True vol dir 'Abort'. Retornar False vol dir 'Continue'.
        return returnVal;
    }
}