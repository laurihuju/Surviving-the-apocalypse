using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float patrolPosDistance;
    [SerializeField] private float patrolTurnSpeed;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float chaseTurnSpeed;

    [Header("Search")]
    [SerializeField] private float delayToSwitchPatrolMode;
    [SerializeField] private float searchTurnSpeed;
    [SerializeField] private float searchTurnAngleChangeDistance;

    [Header("Turn")]
    [SerializeField] private float turnTowardsPlayerSpeed;

    [Header("Seeing")]
    [SerializeField] private float seeAngle;
    [SerializeField] private float seeDistance;
    [SerializeField] private Transform eyePoint;

    [Header("Hearing")]
    [SerializeField] private int hearingPossibilitiesMultiplier;
    [SerializeField] private float maximumHearingDistance;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackDamage;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float speedToSwitchRunAnim;
    [SerializeField] private float speedToSwitchWalkAnim;
    [SerializeField] private float angularSpeedToUseWalkAnim;
    [SerializeField] private float walkAnimSpeedMultiplier;
    [SerializeField] private float runAnimSpeedMultiplier;
    [SerializeField] private float turnAnimSpeedMultiplier;

    private ZombieState state = ZombieState.patrol;

    private float patrolSwitchTime = 0;
    private Quaternion searchCheckAngle;

    private float attackEnd;
    private bool isAttacking;

    private Vector3 previousFacing;
    private float angularVelocity;

    private enum ZombieState
    {
        patrol,
        chase,
        check,
        search,
        turn
    }

    private void Start()
    {
        previousFacing = transform.forward;
    }

    private void Update()
    {
        CheckAngularVelocity();

        if (isAttacking)
        {
            if (Time.time > attackEnd)
            {
                agent.isStopped = false;
                isAttacking = false;
            }
            HandleAnimations();
            return;
        }

        if (state == ZombieState.patrol)
            PatrolMove();
        else if (state == ZombieState.chase)
            ChaseMove();
        else if (state == ZombieState.check)
            CheckMove();
        else if (state == ZombieState.search)
            SearchMove();
        else if (state == ZombieState.turn)
            TurnMove();

        HandleAnimations();
    }

    public void HearSound(float volume, Vector3 worldPosition)
    {
        Debug.Log("!");
        float distanceToPosition = Vector3.Distance(transform.position, worldPosition);
        if (distanceToPosition > maximumHearingDistance)
            return;

        if (Random.Range(0, (int)((distanceToPosition * hearingPossibilitiesMultiplier) / volume) + 1) == 0)
        {
            agent.SetDestination(worldPosition);
            state = ZombieState.turn;
        }
    }

    private void PatrolMove()
    {
        agent.speed = patrolSpeed;
        agent.angularSpeed = patrolTurnSpeed;

        if (Mathf.Abs(transform.position.x - agent.destination.x) < 2 && Mathf.Abs(transform.position.z - agent.destination.z) < 2)
            agent.SetDestination(GeneratePatrolPosition());

        if (IsPlayerInSightArea())
            if (CanSeePlayer())
                state = ZombieState.chase;
    }

    private Vector3 GeneratePatrolPosition()
    {
        return new Vector3(Random.Range(transform.position.x - patrolPosDistance, transform.position.x + patrolPosDistance), transform.position.y, Random.Range(transform.position.z - patrolPosDistance, transform.position.z + patrolPosDistance));
    }

    private void ChaseMove()
    {
        agent.speed = chaseSpeed;
        agent.angularSpeed = chaseTurnSpeed;

        agent.SetDestination(PlayerController.GetInstance().transform.position);

        if (!IsPlayerInSightArea())
        {
            if (CanSeePlayer())
            {
                state = ZombieState.turn;
                return;
            }
        }
        if (!CanSeePlayer())
        {
            state = ZombieState.check;
            return;
        }

        Collider[] targets = Physics.OverlapSphere(attackPoint.position, attackDistance);
        if (targets.Length > 0)
        {
            for(int i = 0; i < targets.Length; i++)
            {
                if (targets[i].CompareTag("Player"))
                {
                    HealthManager health = targets[i].GetComponent<HealthManager>();
                    if (health == null)
                        break;
                    health.ChangeHealth(-attackDamage);

                    attackEnd = Time.time + attackCooldown;
                    agent.isStopped = true;
                    isAttacking = true;
                    animator.SetTrigger("Attack");
                    break;
                }
            }
        }
    }

    private void CheckMove()
    {
        agent.speed = chaseSpeed;
        agent.angularSpeed = chaseTurnSpeed;

        if (IsPlayerInSightArea())
        {
            if (CanSeePlayer())
            {
                state = ZombieState.chase;
                return;
            }
        }

        if (Mathf.Abs(transform.position.x - agent.destination.x) < agent.stoppingDistance + 0.2f && Mathf.Abs(transform.position.z - agent.destination.z) < agent.stoppingDistance + 0.2f)
        {
            state = ZombieState.search;
            patrolSwitchTime = Time.time + delayToSwitchPatrolMode;
            GenerateSearchCheckAngle();
            return;
        }
    }

    private void SearchMove()
    {
        if (Time.time > patrolSwitchTime)
        {
            state = ZombieState.patrol;
            agent.isStopped = false;
            return;
        }

        agent.isStopped = true;

        if (Mathf.Abs(transform.rotation.eulerAngles.y - searchCheckAngle.eulerAngles.y) < searchTurnAngleChangeDistance)
            GenerateSearchCheckAngle();
        transform.rotation = Quaternion.Slerp(transform.rotation, searchCheckAngle, searchTurnSpeed * Time.deltaTime);

        if (IsPlayerInSightArea())
        {
            if (CanSeePlayer())
            {
                state = ZombieState.chase;
                agent.isStopped = false;
                return;
            }
        }
    }

    private void GenerateSearchCheckAngle()
    {
        searchCheckAngle = Quaternion.Euler(0, Random.Range(0, 361), 0);
    }

    private void TurnMove()
    {
        agent.isStopped = true;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetDirectionToPlayer()), turnTowardsPlayerSpeed * Time.deltaTime);

        if (!CanSeePlayer())
        {
            state = ZombieState.check;
            agent.isStopped = false;
            return;
        }
        if (IsPlayerInSightArea())
        {
            state = ZombieState.chase;
            agent.isStopped = false;
            return;
        }
    }

    private bool IsPlayerInSightArea()
    {
        return Vector3.Angle(transform.forward, GetDirectionToPlayer()) <= seeAngle;
    }

    private bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(eyePoint.position, PlayerController.GetInstance().transform.position - eyePoint.position, out hit, seeDistance))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }

    private Vector3 GetDirectionToPlayer()
    {
        Vector3 playerPosition = PlayerController.GetInstance().transform.position;
        playerPosition.y = 0;
        Vector3 ownPosition = transform.position;
        ownPosition.y = 0;
        return playerPosition - ownPosition;
    }

    private void HandleAnimations()
    {
        float agentVelocity = agent.velocity.magnitude;

        if(agentVelocity > speedToSwitchRunAnim)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);

            animator.speed = agent.velocity.magnitude * runAnimSpeedMultiplier;
        } else if (agentVelocity > speedToSwitchWalkAnim)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);

            animator.speed = agent.velocity.magnitude * walkAnimSpeedMultiplier;
        } else if (angularVelocity > angularSpeedToUseWalkAnim)
        {
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);

            animator.speed = angularVelocity * turnAnimSpeedMultiplier;
        } else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            animator.speed = 1;
        }
    }

    private void CheckAngularVelocity()
    {
        Vector3 currentFacing = transform.forward;
        angularVelocity = Vector3.Angle(currentFacing, previousFacing) / Time.deltaTime;
        previousFacing = currentFacing;
    }
}
