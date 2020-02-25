using UnityEngine;
using System.Collections;
public class EnemyControllerScript : MonoBehaviour
{
    public TakeDamageFromPlayerBullet bulletColliderListener = null;
    public float walkingSpeed = 0.45f;
    private bool walkingLeft = true;
    public ParticleSystem EnemyDeathFX = null;
    // Delegat i event que permetran als objectes del joc saber quan mor un Enemic
    public delegate void enemyEventHandler(int scoreMod);
    public static event enemyEventHandler enemyDied;

    void Start()
    {
        // Inicialitzar aleatòriament la direcció de desplaçament
        walkingLeft = (Random.Range(0, 2) == 1);
        updateVisualWalkOrientation();

        // Obtenir nombre de ronda actual
        GameObject roundWatcherObject =
        GameObject.FindGameObjectWithTag("RoundWatcher");
        if (roundWatcherObject != null)
        {
            RoundWatcher roundWatcherComponent =
            roundWatcherObject.GetComponent<RoundWatcher>();
            // Assignar velocitat segons ronda en curs
            walkingSpeed = walkingSpeed * roundWatcherComponent.currRound;
        }
    }
    void Update()
    {
        // Moure l'enemic segons la direcció actual de moviment
        // Es modifica la component x.
        if (walkingLeft)
        {
            transform.Translate(new Vector3(walkingSpeed *
            Time.deltaTime, 0.0f, 0.0f));
        }
        else
        {
            transform.Translate(new Vector3((walkingSpeed * -1.0f)
            * Time.deltaTime, 0.0f, 0.0f));
        }
    }
    public void switchDirections()
    {
        // Canviar la direcció de desplaçament a la contrària de l'actual
        walkingLeft = !walkingLeft;
        // Modificar l'orientació del gràfic associat a Enemy segons

        // l'orientació actual (valor de walkingLeft)
        updateVisualWalkOrientation();
    }
    void updateVisualWalkOrientation()
    {
        Vector3 localScale = transform.localScale;
        if (walkingLeft)
        {
            if (localScale.x > 0.0f)
            {
                localScale.x = localScale.x * -1.0f;
                transform.localScale = localScale;
            }
        }
        else
        {
            if (localScale.x < 0.0f)
            {
                localScale.x = localScale.x * -1.0f;
                transform.localScale = localScale;
            }
        }
    }

    void OnEnable()
    {
        // Suscripció a l'event hitByBullet.
        bulletColliderListener.hitByBullet += hitByPlayerBullet;
    }

    void OnDisable()
    {
        // cancel.lar la suscripció a l'event hitByBullet.
        bulletColliderListener.hitByBullet -= hitByPlayerBullet;
    }

    public void hitByPlayerBullet()
    {
        // Crear l'objecte emissor de partícules
        ParticleSystem deathFxParticle =
        (ParticleSystem)Instantiate(EnemyDeathFX);
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
    }

    public void hitByCrusher()
    {
        // Crear l'objecte emissor de partícules
        ParticleSystem deathFxParticle =
        (ParticleSystem)Instantiate(EnemyDeathFX);
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
    }
}

