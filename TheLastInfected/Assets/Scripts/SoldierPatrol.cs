using UnityEngine;
using UnityEngine.AI;

public class SoldierPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float stopDistanceToPlayer = 10f;
    public Transform player;

    private Vector3 targetPoint;
    private NavMeshAgent agent;
    private SoldierShooting shootingScript;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootingScript = GetComponent<SoldierShooting>();
        animator = GetComponentInChildren<Animator>();
        targetPoint = pointA.position;
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopDistanceToPlayer)
        {
            if (!agent.isStopped)
                agent.isStopped = true;

            FacePlayer();
            shootingScript.StartShooting();
        }
        else
        {
            if (agent.isStopped)
                agent.isStopped = false;

            shootingScript.StopShooting();

            if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
            {
                targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;
            }

            agent.SetDestination(targetPoint);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
