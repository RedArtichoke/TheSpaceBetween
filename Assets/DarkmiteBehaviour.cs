using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class DarkmiteBehaviour : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 6f;
    public float moveSpeed = 3f;
    public float jumpForce = 10f;
    public float jumpCooldown = 2f;

    private Rigidbody rb;
    private NavMeshAgent agent;
    private bool canJump = true;
    private bool isAttacking = false;

    public Animator animator;

    public GameObject splatDecal;

    [Header("SFX")]
    public AudioSource SplatAudio;

    public AudioClip soundEffect1;
    public AudioClip soundEffect2;
    public AudioClip squishClip;

    public AudioSource RandomSFX;

    public AudioSource squishAudio;

    [Header("Pitch")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.2f;
    public float minInterval = 8f;
    public float maxInterval = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        
        StartCoroutine(PlayRandomSound());
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && !isAttacking)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        if (distance <= attackRange && canJump && !isAttacking)
        {
            StartCoroutine(JumpAttack());
        }

        UpdateAnimations();
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    void Wander()
    {
        if (agent.remainingDistance <= 0.5f)
        {
            Vector3 randomPos = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            agent.SetDestination(randomPos);
        }
    }
    IEnumerator JumpAttack()
    {
        isAttacking = true;
        canJump = false;

        agent.isStopped = true;
        agent.enabled = false; 
        rb.velocity = Vector3.zero; 
        rb.isKinematic = false; 
        rb.useGravity = true; 

        animator.SetTrigger("Jump");

        yield return new WaitForSeconds(1f); 

        Vector3 targetPosition = player.position;
        Vector3 startPosition = transform.position;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float heightOffset = 2.5f; 
        float timeToPeak = Mathf.Sqrt(2 * heightOffset / gravity);
        float timeToTarget = timeToPeak * 2; 

        Vector3 horizontalDirection = (targetPosition - startPosition);
        horizontalDirection.y = 0;
        Vector3 horizontalVelocity = horizontalDirection / timeToTarget;

        float verticalVelocity = Mathf.Sqrt(2 * gravity * heightOffset);

        rb.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        yield return new WaitForSeconds(timeToTarget); 

        agent.enabled = true;
        agent.isStopped = false;
    
        isAttacking = false;

        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<SplatEffect>().ShowSplat();

            Destroy(gameObject);
            Debug.Log("Darkmite hit player");
        }
    }

    private IEnumerator PlayRandomSound()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            AudioClip chosenClip = (Random.Range(0, 2) == 0) ? soundEffect1 : soundEffect2;

            RandomSFX.pitch = Random.Range(minPitch, maxPitch);

            RandomSFX.PlayOneShot(chosenClip);
        }
    }

    void UpdateAnimations()
    {
        if (isAttacking)
        {
            return; // Don't update animations while attacking
        }

        if (!agent.enabled || agent.velocity.magnitude < 0.1f)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
    }

    public void Splat()
    {
        Quaternion rotationOffset = Quaternion.Euler(90f, 0f, 0f);
    
        Quaternion newRotation = transform.rotation * rotationOffset;
    
        GameObject decal = Instantiate(splatDecal, transform.position, newRotation);

        SplatAudio.Play();
    }

    public void Squish()
    {
        squishAudio.clip = squishClip;
        squishAudio.Play();
    }

    public void Gargle()
    {
        squishAudio.clip = soundEffect2;
        squishAudio.Play();
    }


}
