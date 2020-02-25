using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necesari per utilitzar llistes
public class BossEventController : MonoBehaviour
{
    public delegate void bossEventHandler(int scoreMod);
    public static event bossEventHandler bossDied;
    public delegate void bossAttack();
    public static event bossAttack crushPlayer;
    public GameObject inActiveNode = null;
    public GameObject dropToStartNode = null;
    public GameObject dropFXSpawnPoint = null;
    public List<GameObject> dropNodeList = new List<GameObject>();

    public GameObject bossDeathFX = null;
    public GameObject bossDropFX = null;
    public TakeDamageFromPlayerBullet bulletColliderListener = null;
    public float moveSpeed = 10f;
    public float eventWaitDelay = 3f; // Temps d'espera entre events del Boss
    public int enemiesToStartBattle = 10;
    public enum bossEvents
    {
        inactive = 0,
        fallingToNode,
        waitingToJump,
        waitingToFall,
        jumpingOffPlatform
    }
    // Estat actual del Boss
    public bossEvents currentEvent = bossEvents.inactive;
    // Node cap al qual saltara el Boss.
    private GameObject targetNode = null;
    // Temps a esperar fins el proper salt o caiguda.
    private float timeForNextEvent = 0.0f;
    // Posició de desti quan es salta a la plataforma.
    private Vector3 targetPosition = Vector3.zero;
    // Nivell de vida del Boss
    public int health = 20;
    // Nivell de vida inicial del Boss
    private int startHealth = 20;
    // Indicador de si s'ha matat el Boss
    private bool isDead = false;
    // Enemics que s'han de matar abans que apareixi el Boss
    private int enemiesLeftToKill = 1;
    // Particulas humo
    public ParticleSystem Humo = null;
    // Inicialitzacions. Apuntar-se a escoltar events indicant mètode per fer-ho
    void OnEnable()
    {
        bulletColliderListener.hitByBullet += hitByPlayerBullet;
        EnemyControllerScript.enemyDied += enemyDied;
    }
    void OnDisable()
    {
        bulletColliderListener.hitByBullet -= hitByPlayerBullet;
        EnemyControllerScript.enemyDied -= enemyDied;
    }

    void Start()
    {
        Debug.Log("METODO START");
        transform.position = inActiveNode.transform.position;
        enemiesLeftToKill = enemiesToStartBattle;
    }

    void Update()
    {
        switch (currentEvent)
        {
            case bossEvents.inactive:
                // Not doing anything, so nothing to do.
                break;


            case bossEvents.fallingToNode:
                if (transform.position.y > targetNode.transform.position.y)
                {
                    Debug.Log("FallingToNode");
                    // Velocitat negativa, per desplaçar-se cap abaix
                    transform.Translate(new Vector3(0f, -moveSpeed * Time.deltaTime, 0f));
                    if (transform.position.y < targetNode.transform.position.y)
                    {
                        Debug.Log("Segundo if FallingToNode");
                        Vector3 targetPos = targetNode.transform.position;
                        transform.position = targetPos;
                    }
                }
                else
                {
                    // Crear efecte d'aterratge (Partícules)
                    createDropFX();

                    timeForNextEvent = 0.0f;
                    currentEvent = bossEvents.waitingToJump;
                }
                break;

            case bossEvents.waitingToFall:
                // Boss esperant per caure en un altre node
                if (timeForNextEvent == 0.0f)
                {
                    Debug.Log("1 if waitingToFall");
                    timeForNextEvent = Time.time + eventWaitDelay;
                }
                else if (timeForNextEvent < Time.time)
                {
                    Debug.Log("2 if waitingToFall");
                    // Need to find a new node!
                    targetNode = dropNodeList[Random.Range(0, dropNodeList.Count)];

                    // Posició del Boss SOBRE el node destí
                    transform.position = getSkyPositionOfNode(targetNode);

                    // actualitzar estat
                    currentEvent = bossEvents.fallingToNode;
                    timeForNextEvent = 0.0f;
                }
                break;

            case bossEvents.waitingToJump:
                // Boss espera, situat sobre plataforma, el moment de canviar
                if (timeForNextEvent == 0.0f)
                {
                    timeForNextEvent = Time.time + eventWaitDelay;
                }
                else if (timeForNextEvent < Time.time)
                {
                    // Estableix posició objetiu per elevar-se sobre node actual
                    targetPosition = getSkyPositionOfNode(targetNode);

                    // Actualitzar estat
                    currentEvent = bossEvents.jumpingOffPlatform;
                    timeForNextEvent = 0.0f;

                    targetNode = null;
                }
                break;

            case bossEvents.jumpingOffPlatform:
                if (transform.position.y < targetPosition.y)
                {
                    // Velocitat positiva, moviment ascendent

                    transform.Translate(new Vector3(0f, moveSpeed * Time.deltaTime, 0f));
                    if (transform.position.y > targetPosition.y)
                        transform.position = targetPosition;
                }
                else
                {
                    timeForNextEvent = 0.0f;
                    currentEvent = bossEvents.waitingToFall;
                }
                break;
        }
    }

    public void beginBossBattle()
    {
        // Establir node inicial i fer que el Boss hi caigui
        Debug.Log("beginBossBattle");
        targetNode = dropToStartNode;
        currentEvent = bossEvents.fallingToNode;
        // Inicialitzar variables de control
        timeForNextEvent = 0.0f;
        health = startHealth;
        isDead = false;
    }

    Vector3 getSkyPositionOfNode(GameObject node)
    {
        Vector3 targetPosition = targetNode.transform.position;
        targetPosition.y += 9f;

        return targetPosition;
    }

    void hitByPlayerBullet()
    {
        health -= 1;
        // Si s'ha acabat la vida, el matem
        if (health <= 0)
            killBoss();
    }

    void createDropFX()
    {
        //Implementar sistema de partícules de caiguda sobre plataforma: bossDropFX
        // . . . .
    }

    void killBoss()
    {
        if (isDead)
            return;

        isDead = true;
        //Implementar sistema de partícules de destrucció del Boss: bossDeathFX
        // Crear l'objecte emissor de partícules
        ParticleSystem deathFxParticle =
        (ParticleSystem)Instantiate(Humo);
        // Obtenir la posició de l'enemic
        Vector3 enemyPos = transform.position;
        // Crear un nou vector davant de l'enemic (incrementar component z)
        Vector3 particlePosition =
        new Vector3(enemyPos.x, enemyPos.y, enemyPos.z + 1.0f);
        // Posicionar l'emissor de partícules en aquesta nova posició
        deathFxParticle.transform.position = particlePosition;
        // Generar event enemyDied i donar una puntuacio de 25 punts.
        enemyDied(25);
        // Esperar un moment i destruir l'objecte Enemy
        Destroy(gameObject, 0.1f);

        // Generar l'event “bossDied” amb una puntuació de 1000 punts
        if (bossDied != null)
            bossDied(1000);

        // Tornar a posició inactiva inicial
        transform.position = inActiveNode.transform.position;

        //Reset de camps de control
        currentEvent = BossEventController.bossEvents.inactive;
        timeForNextEvent = 0.0f;

        enemiesLeftToKill = enemiesToStartBattle;
    }
    void enemyDied(int enemyScore)
    {
        if (currentEvent == bossEvents.inactive)
        {
            enemiesLeftToKill -= 1;
            Debug.Log("--- Enemics pendents per apareixer Boss: " +
            enemiesLeftToKill);
            if (enemiesLeftToKill <= 0)
                beginBossBattle();
        }
    }
    public void playerHitByCrusher()
    {
        if (currentEvent == bossEvents.fallingToNode)
        {
            if (crushPlayer != null)
                crushPlayer();
        }
    }
}

