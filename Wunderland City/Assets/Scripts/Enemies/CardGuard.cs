using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGuard : BaseEnemy
{
    [Header("Card Guard Noises")]
    [SerializeField]
    private AudioClip approachNoise;

    [SerializeField]
    private AudioClip CardGuardAttackNoise;

    [SerializeField]
    private AudioClip CardGuardDeath;
    
    [SerializeField]
    private AudioClip CardGuardHitNoise;

    [SerializeField]
    public CharStats CardGuardStatistics;

    public CardGuard()
    {
        m_EnemyName = "Card Guard";
        m_PatrolPoints = new GameObject[3];
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
        base.SetupEnemy(); // call this first

        // setup with Charstats data for CardGuard
        m_Speed = CardGuardStatistics.Speed;
        m_DamageDealt = CardGuardStatistics.AttackDamage;
        m_EyesightDistance = CardGuardStatistics.EyesightDistance;
        maxHealth = CardGuardStatistics.MaxHealth;
        m_Health = CardGuardStatistics.MaxHealth;

        navAgent.speed = m_Speed; // set speed of character
        m_Anim = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        m_Attacking = false;
    }

    public override void AddDamage()
    {
        // Adds damage to enemy from Player
        Health -= CardGuardStatistics.AttackDamage;

        base.AddDamage(); // checks if dead
        Debug.Log("Card Guard Health is now: " + Health);
        m_Anim.SetTrigger("HitByPlayer");
        audioSource.PlayOneShot(CardGuardHitNoise);
    }

    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in CardGuard::AttackPlayer: m_Attacking = {m_Attacking}");
            m_Attacking = true;
            m_Anim.SetBool("Attack", true);
            m_Anim.SetFloat("Speed", 0f);

            MakeAttackNoise();
        }

        RotateTowardsPlayer();
    }

    protected override void MakeAttackNoise()
    {
        Debug.Log($"Now in CardGuard::MakeAttackNoise: m_Attacking = {m_Attacking}");

        //StopApproachNoise();

        audioSource.clip = CardGuardAttackNoise;
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

            yield return new WaitForSeconds(2.5f); // add damage every 2.5 seconds

            GameManagement.Instance.ReducePlayerHealth(CardGuardStatistics.AttackDamage);

            Debug.Log("Reducing player health by " + CardGuardStatistics.AttackDamage);
        }

        if (!m_Attacking)
        {
            StopPlayingAttack();
            m_Anim.SetBool("Attack", false);
            audioSource.loop = false;
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
        m_Speed = CardGuardStatistics.Speed;
        StopApproachNoise();
    }

    public override void MoveEnemy()
    {
        // enemies should ALWAYS be moving (or doing something if not patrolling)
        if (!m_Attacking)
        {
            m_Anim.SetFloat("Speed", 0.5f);
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
                    audioSource.volume = 0.5f;
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
        // card guard is dead
        Debug.Log("Card Guard has Died");

        audioSource.PlayOneShot(CardGuardDeath, 1f);
        GameManagement.Score += 100;
        GameManagement.Instance.RemoveAnEnemy();
        Destroy(gameObject, 1f);
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum, obj);
    }

}
