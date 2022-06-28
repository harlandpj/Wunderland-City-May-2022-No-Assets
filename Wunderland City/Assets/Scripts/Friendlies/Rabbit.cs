using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : BaseEnemy
{
    [Header("Rabbit Noises")]
    [SerializeField]
    private AudioClip rabbitDeath;
    [SerializeField]
    private AudioClip playerSeenNoise;

    public CharStats rabbitStats;

    private Rigidbody rBody;


    public Rabbit()
    {
        m_EnemyName = "Rabbit"; // but not an enemy!
    }

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void SetToFirstPatrolPosition()
    {
        base.SetToFirstPatrolPosition();
    }

    public override void SetupEnemy()
    {
        base.SetupEnemy();

        // setup with Charstats data for Eyeball
        m_Speed = rabbitStats.Speed;
        m_DamageDealt = 0;
        m_EyesightDistance = rabbitStats.EyesightDistance;
        m_Health = rabbitStats.MaxHealth;
        maxHealth = rabbitStats.MaxHealth;
        
        navAgent.speed = m_Speed; // set speed of character
        m_Anim = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void AddDamage()
    {
        // can't kill these, only collect for points!
        rBody.AddForce(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3), ForceMode.Impulse);
    }

    public override void AttackPlayer()
    {
    }

    protected override void MakeAttackNoise()
    {
    }

    //protected override IEnumerator PlayingAttack()
    //{
    //    while (m_Attacking)
    //    {
    //        if (!bPlayingAttack)
    //        {
    //            // play attack animation
    //            bPlayingAttack = true;
    //            //     m_Anim.SetBool("Attack", true);
    //        }

    //        yield return new WaitForSeconds(2f); // add damage every 2 seconds (for now)

    //        GameManagement.Instance.ReducePlayerHealth(-10);

    //        Debug.Log("Eyeball health is now: " + Health);
    //    }

    //    if (!m_Attacking)
    //    {
    //        StopPlayingAttack();
    //        //m_Anim.SetBool("Attack", false);
    //        yield break; // stop coroutine
    //    }
    //}

    protected override void StopPlayingAttack()
    {
        //base.StopPlayingAttack();

        //if (GameManagement.Instance.bGameOver)
        //{
        //    StopCoroutine("PlayingAttack");
        //}
    }

    private bool collectedIt = false;

    private void OnTriggerEnter(Collider other)
    {
        // check who entered
        if (other.gameObject.tag == "Player" && !collectedIt)
        {
            collectedIt = true;

            // player has picked me up
            GameManagement.Instance.RemoveARabbit();
            audioSource.PlayOneShot(rabbitDeath);
            Destroy(gameObject, 1f);
        }
    }

    public override void Patrol()
    {

        if (m_PatrolPoints != null)
        {
            // just patrol from our current position to next patrol point
            // reversing at end to go back to original position

            if (Vector3.Distance(transform.position,
                                 m_DestinationPoint.transform.position) <= 1.5f)
            {
                // check if we were reversing and reached beginning
                if (m_Reversing) 
                { 
                    if (CurrentPatrolPoint == 0)
                    {
                        // we are at start of patrol
                        m_Reversing = false;
                        CurrentPatrolPoint++;
                    }
                    else
                    {
                        // go to next point
                        CurrentPatrolPoint--;
                    }
                }
                else
                {
                    // not currently reversing, check have reached end of patrol
                    if (CurrentPatrolPoint >= m_PatrolPoints.Length - 1)
                    {
                        // go in reverse
                        m_Reversing = true;
                        CurrentPatrolPoint--;
                    }
                    else
                    {
                        // continue on to next patrol point
                        CurrentPatrolPoint++;
                    }
                }

                // set next destination
                m_DestinationPoint = m_PatrolPoints[CurrentPatrolPoint];

                if (navAgent != null)
                {
                    navAgent.destination = m_DestinationPoint.transform.position;
                }
            }
            else
            {
                // set destination
                if (navAgent != null)
                {
                    navAgent.destination = m_DestinationPoint.transform.position;
                }
            }
        }
    }

    // start a timer here to just blow up???
    public override void MoveEnemy()
    {
        m_Attacking = false;
        m_Patrolling = true;
        m_Anim.SetFloat("Speed", 7f);
        Patrol();
    }

    static public bool bPlayerInRange = false;

    // add paused here too
    private void FixedUpdate()
    {
        if (!GameManagement.Instance.bGameOver)
        {
            bPlayerInRange = false;
            m_Patrolling = true;
            Patrol();
        }
    }

    protected override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();
        SetupEnemy();
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();
        m_Anim.SetFloat("Speed", 7);
       }

    protected override void PlayDeathSound()
    {
        // rabbit is dead
        Debug.Log("Rabbit has Died");

        //audioSource.PlayOneShot(rabbitDeath, 1f);

        GameManagement.Score += 100;
        StartCoroutine("PlayDeath");
    }

    protected override IEnumerator PlayDeath()
    {
        // rabbit is dead
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        yield return null;
    }
}
