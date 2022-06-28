using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(ParticleSystem))]
public class Eyeball : BaseEnemy
{
    [Header("Eyeball Noises")]
    [SerializeField]
    private AudioClip eyeballAttackNoise;

    [SerializeField]
    private AudioClip eyeballDeath;

    [SerializeField]
    public CharStats eyeballStatistics;

    private Rigidbody rBody;

    [Header("Particle Effects")]
    [SerializeField]
    private GameObject deathParticle;

    public Eyeball()
    {
        m_EnemyName = "Eyeball";
    }

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void SetToFirstPatrolPosition()
    {
    }

    public override void SetupEnemy()
    {
        // setup with Charstats data for Eyeball
        m_Speed = eyeballStatistics.Speed;
        m_DamageDealt = eyeballStatistics.AttackDamage;
        m_EyesightDistance = eyeballStatistics.EyesightDistance;
        m_Health = eyeballStatistics.MaxHealth;
        maxHealth = eyeballStatistics.MaxHealth;

        m_Anim = GetComponent<Animator>();
        rBody = GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
        
    }

    public override void AddDamage()
    {
        // Adds damage to enemy from Player
        Health -= eyeballStatistics.AttackDamage;

        Debug.Log("Eyeball Health is now: " + Health);

        base.AddDamage(); // checks if dead
        rBody.AddForce(Random.Range(1,3), Random.Range(1, 3), Random.Range(1, 3), ForceMode.Impulse);
    }

    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in Eyeball::AttackPlayer: m_Attacking = {m_Attacking}");
            m_Attacking = true;
            
            //m_Anim.SetBool("Attack", true);
            //m_Anim.SetFloat("Speed", 0f);

            MakeAttackNoise();
        }
    }

    protected override void MakeAttackNoise()
    {
        Debug.Log($"Now in Eyeball::MakeAttackNoise: m_Attacking = {m_Attacking}");

        //StopApproachNoise();

        if (!m_Attacking)
        {
            audioSource.clip = eyeballAttackNoise;
            audioSource.loop = true;
            audioSource.Play();

            // start attack sequence
            StartCoroutine(PlayingAttack());
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
           //     m_Anim.SetBool("Attack", true);
            }

            yield return new WaitForSeconds(2f); // add damage every 2 seconds (for now)

            GameManagement.Instance.ReducePlayerHealth(-10);

            Debug.Log("Eyeball health is now: " + Health);
        }

        if (!m_Attacking)
        {
            StopPlayingAttack();
            //m_Anim.SetBool("Attack", false);
            yield break; // stop coroutine
        }
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
        // override base - eyeballs don't patrol
    }

    // start a timer here to just blow up???
    public override void MoveEnemy()
    {
        // eyeballs do nothing except explode and do damage!
    }

    static public bool bPlayerInRange = false;

    // add paused here too
    private void FixedUpdate()
    {
        if (!GameManagement.Instance.bGameOver)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= m_EyesightDistance)
            {
                // rotate towards player / play sound
                if (!bPlayerInRange)
                {
                    bPlayerInRange = true;
                    audioSource.clip = eyeballAttackNoise;
                    audioSource.Play();
                }
                else
                {
                    bPlayerInRange = false;
                }

                RotateTowardsPlayer();
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        audioSource = GetComponent<AudioSource>();
        SetupEnemy();
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();
    }

    protected override void PlayDeathSound()
    {
        // eyeball is dead
        Debug.Log("Eyeball has Died");

        audioSource.PlayOneShot(eyeballDeath, 1f);
        deathParticle.GetComponent<ParticleSystem>().Play();

        GameManagement.Score += 100;
        deathParticle.GetComponent<ParticleSystem>().Play();
        //StartCoroutine("PlayDeath");
        Destroy(gameObject, 3f);
    }

    protected override IEnumerator PlayDeath()
    {
        // eyeball is dead
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        yield return null;
    }
}
