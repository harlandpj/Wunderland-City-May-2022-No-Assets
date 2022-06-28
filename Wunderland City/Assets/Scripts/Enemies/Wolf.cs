using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : BaseEnemy
{
     [Header("Wolf Noises")]
    [SerializeField]
    private AudioClip approachNoise;
    [SerializeField]
    private AudioClip WolfAttackNoise;
    [SerializeField]
    private AudioClip WolfHitNoise;
    [SerializeField]
    private AudioClip WolfDeath;

    [SerializeField]
    public CharStats WolfStatistics; // enemy stats

    public Wolf()
    {
        m_EnemyName = "Wolf";
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
        
        // setup with Charstats data for Wolf
        m_Speed = WolfStatistics.Speed;
        m_DamageDealt = WolfStatistics.AttackDamage;
        m_EyesightDistance = WolfStatistics.EyesightDistance;
        maxHealth = WolfStatistics.MaxHealth;
        m_Health = WolfStatistics.MaxHealth;

        navAgent.speed = m_Speed; // set speed of character
        m_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // ensure on mesh
        //navAgent.Warp(transform.position);
    }

    public override void AddDamage()
    {
        // Adds damage to enemy from Player   ** need to do one for attacking player too!!! ****
        Health -= WolfStatistics.AttackDamage;
        audioSource.PlayOneShot(WolfHitNoise);

        base.AddDamage(); // checks if dead
        
        Debug.Log("Wolf Health is now: " + Health);
        m_Anim.SetTrigger("Damage");
    }

    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in Wolf::AttackPlayer: m_Attacking = {m_Attacking}");
            m_Attacking = true;
            m_Anim.SetBool("Attack", true);
            m_Anim.SetFloat("Speed", 0f);

            MakeAttackNoise();
        }

        RotateTowardsPlayer();
    }

    protected override void MakeAttackNoise()
    {
        Debug.Log($"Now in Wolf::MakeAttackNoise: m_Attacking = {m_Attacking}");

        //StopApproachNoise();

        audioSource.clip = WolfAttackNoise;
        audioSource.loop = true;
        audioSource.Play();

        // start attack sequence
        StartCoroutine(PlayingAttack());
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

            GameManagement.Instance.ReducePlayerHealth(WolfStatistics.AttackDamage);

            Debug.Log("Reducing player health by: " + WolfStatistics.AttackDamage);
        }

        if (!m_Attacking)
        {
            StopPlayingAttack();
            m_Anim.SetBool("Attack", false);
            audioSource.Stop();
            yield break; // stop coroutine
        }

        yield break;
    }

    protected override void StopPlayingAttack()
    {
        base.StopPlayingAttack();

        if (GameManagement.Instance.bGameOver)
        {
            StopCoroutine("PlayingAttack");
        }
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
            m_Anim.SetFloat("Speed", 0.5f);
            m_Anim.SetBool("Attack", false);
        }

        if (PlayerSeenOrInRange())
        {
            m_Anim.SetFloat("Speed", 1.6f);
            MoveTowardsPlayer();
        }
        else
        {
            m_Attacking = false;
            m_Anim.SetBool("Attack", false);
            Patrol();
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
        Debug.Log("Wolf has Died");

        audioSource.PlayOneShot(WolfDeath, 1f);
        GameManagement.Score += 100;

        StartCoroutine("PlayDeath");
    }

    protected override IEnumerator PlayDeath()
    {
        // Wolf is dead
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        yield return null;
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum, obj);
    }
}
