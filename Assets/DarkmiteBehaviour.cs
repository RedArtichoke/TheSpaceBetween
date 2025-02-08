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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
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

        // Stop movement and prepare for the jump
        agent.isStopped = true;
        agent.enabled = false; // Disable NavMeshAgent to allow free Y movement
        rb.velocity = Vector3.zero; // Stop Rigidbody movement
        rb.isKinematic = false; // Ensure physics is applied
        rb.useGravity = true; // Enable gravity

        yield return new WaitForSeconds(1f); // 1-second warning time

        Vector3 targetPosition = player.position;
        Vector3 startPosition = transform.position;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float heightOffset = 2.5f; // Increase this for a higher jump
        float timeToPeak = Mathf.Sqrt(2 * heightOffset / gravity);
        float timeToTarget = timeToPeak * 2; // Time to go up + time to land

        // Calculate horizontal velocity (move towards player)
        Vector3 horizontalDirection = (targetPosition - startPosition);
        horizontalDirection.y = 0;
        Vector3 horizontalVelocity = horizontalDirection / timeToTarget;

        // Calculate proper vertical velocity to form an arc
        float verticalVelocity = Mathf.Sqrt(2 * gravity * heightOffset);

        // Apply jump force
        rb.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        yield return new WaitForSeconds(timeToTarget); // Wait until landing

        // Re-enable NavMeshAgent after landing
        agent.enabled = true;
        agent.isStopped = false;
    
        isAttacking = false;

        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }


    // Detect when the Darkmite lands
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("Darkmite hit player");
        }
    }

}
