using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SpiderQueen : BaseEnemy
{
    [Header("Spider Queen Noises")]
    [SerializeField]
    private AudioClip approachNoise;
    [SerializeField]
    private AudioClip spiderQueenAttackNoise;
    [SerializeField]
    private AudioClip spiderQueenHitNoise;
    [SerializeField]
    private AudioClip spiderQueenDeath;

    [SerializeField]
    public CharStats spiderQueenStatistics; // enemy stats

    public SpiderQueen()
    {
        m_EnemyName = "Spider Queen";
        m_PatrolPoints = new GameObject[3]; // could change to set from enemy stats (if abstract base class allows)
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
        
        // setup with Charstats data for SpiderQueen
        m_Speed = spiderQueenStatistics.Speed;
        m_DamageDealt = spiderQueenStatistics.AttackDamage;
        m_EyesightDistance = spiderQueenStatistics.EyesightDistance;
        maxHealth = spiderQueenStatistics.MaxHealth;
        m_Health = spiderQueenStatistics.MaxHealth;

        navAgent.speed = m_Speed; // set speed of character
        m_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // ensure on mesh
        //navAgent.Warp(transform.position);
    }

    public override void AddDamage()
    {
        if (!GameManagement.Instance.bGameOver)
        {
            // Adds damage to enemy from Player   ** need to do one for attacking player too!!! ****
            Health -= spiderQueenStatistics.AttackDamage;
            audioSource.PlayOneShot(spiderQueenHitNoise);

            base.AddDamage(); // checks if dead

            Debug.Log("Spider Queen Health is now: " + Health);
            m_Anim.SetTrigger("Jump");
        }
        else
        {
            audioSource.Stop();
        }
    }

    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in SpiderQueen::AttackPlayer: m_Attacking = {m_Attacking}");
            m_Attacking = true;
            m_Anim.SetBool("Attack", true);
            m_Anim.SetFloat("Speed", 0f);

            MakeAttackNoise();
        }

        RotateTowardsPlayer();
    }

    protected override void MakeAttackNoise()
    {
        Debug.Log($"Now in SpiderQueen::MakeAttackNoise: m_Attacking = {m_Attacking}");

        //StopApproachNoise();

        if (!GameManagement.Instance.bGameOver)
        {
            audioSource.clip = spiderQueenAttackNoise;
            audioSource.loop = true;
            audioSource.Play();

            // start attack sequence
            StartCoroutine(PlayingAttack());
        }
        else
        {
            audioSource.Stop();
        }
    }

    protected override IEnumerator PlayingAttack()
    {
        while (m_Attacking)
        {
            if (!bPlayingAttack)
            {
                // play attack animation
                bPlayingAttack = true;
                m_Anim.SetBool("Attack", true);
            }

            yield return new WaitForSeconds(2.5f); // add damage every 2.5 seconds (for now)

            GameManagement.Instance.ReducePlayerHealth(spiderQueenStatistics.AttackDamage);

            Debug.Log("Reducing player health by: " + spiderQueenStatistics.AttackDamage);
        }

        if (!m_Attacking || GameManagement.Instance.bGameOver)
        {
            StopPlayingAttack();
            m_Anim.SetBool("Attack", false);
            audioSource.loop = false;
            audioSource.Stop();
            yield break; // stop coroutine
        }

        audioSource.loop = false;
        audioSource.Stop();
        yield break;
    }

    protected override void StopPlayingAttack()
    {
        base.StopPlayingAttack();

        if (GameManagement.Instance.bGameOver)
        {
            StopCoroutine("PlayingAttack");
        }

        audioSource.loop = false;
        audioSource.Stop();
    }

    public override void Patrol()
    {
        base.Patrol(); // added - but may not be necessary in this example project
    }

    public override void MoveEnemy()
    {
        // enemies should ALWAYS be moving (or doing something if not patrolling)
        if (!m_Attacking)
        {
            m_Anim.SetFloat("Speed", 1f);
        }

        if (PlayerSeenOrInRange())
        {
            m_Anim.SetFloat("Speed", 1.6f);
            MoveTowardsPlayer();
        }
        else
        {
            m_Attacking = false;
            Patrol();
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (!GameManagement.Instance.bGameOver)
        {
            // move the enemy (maybe patrolling or idling around
            MoveEnemy();
        }
        else
        {
            StopPlayingAttack();
            StopApproachNoise();
            audioSource.loop = false;
            audioSource.Stop();
        }
    }
    
    protected void RotateTowardsPlayer()
    {
        // rotate enemy to face player
        transform.LookAt(Player.transform);
    }

    protected override void Start()
    {
        base.Start();

        m_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        SetupEnemy();
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();
    }

    protected override void MakeApproachNoise()
    {
        if (!GameManagement.Instance.bGameOver)
        {
            if (!bMakeApproachNoise && Vector3.Distance(playerTransform.position, transform.position) <= 5f)
            {
                bMakeApproachNoise = true;

                if (!bPlayingApproach)
                {
                    bPlayingApproach = true;

                    // approach noise
                    audioSource.clip = approachNoise;
                    audioSource.volume = 1f;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
        else
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    protected override void StopApproachNoise()
    {
        audioSource.loop = false;
        audioSource.Stop();
        bPlayingApproach = false;
    }

    protected override void PlayDeathSound()
    {
        // eyeball is dead
        Debug.Log("Spider Queen has Died");

        audioSource.PlayOneShot(spiderQueenDeath, 1f);
        GameManagement.Score += 100;

        StartCoroutine("PlayDeath");
    }

    protected override IEnumerator PlayDeath()
    {
        // spider queen is dead
        yield return new WaitForSeconds(2f);
        GameManagement.Instance.RemoveAnEnemy();
        Destroy(gameObject);
        yield return null;
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum, obj);
    }
}
