using UnityEngine;
using UnityEngine.AI;

public abstract class ZombieController : MonoBehaviour
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

    [Header("Group Joining")]
    [SerializeField] private float groupJoiningDistance;
    [SerializeField] private GameObject groupPrefab;
    [SerializeField] private float groupJoinCheckDelay;
    private protected float nextGroupJoinCheckTime;

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

    private protected float patrolSwitchTime = 0;
    private protected Quaternion searchCheckAngle;

    private protected float attackEnd;
    private protected bool isAttacking;

    private protected Vector3 previousFacing;
    private protected float angularVelocity;

    public float PatrolSpeed { get => patrolSpeed;}
    public float PatrolPosDistance { get => patrolPosDistance;}
    public float PatrolTurnSpeed { get => patrolTurnSpeed;}
    public float ChaseSpeed { get => chaseSpeed;}
    public float ChaseTurnSpeed { get => chaseTurnSpeed;}
    public float DelayToSwitchPatrolMode { get => delayToSwitchPatrolMode;}
    public float SearchTurnSpeed { get => searchTurnSpeed;}
    public float SearchTurnAngleChangeDistance { get => searchTurnAngleChangeDistance;}
    public float TurnTowardsPlayerSpeed { get => turnTowardsPlayerSpeed;}
    public float SeeAngle { get => seeAngle;}
    public float SeeDistance { get => seeDistance;}
    public Transform EyePoint { get => eyePoint;}
    public int HearingPossibilitiesMultiplier { get => hearingPossibilitiesMultiplier;}
    public float MaximumHearingDistance { get => maximumHearingDistance;}
    public Transform AttackPoint { get => attackPoint;}
    public float AttackDistance { get => attackDistance;}
    public float AttackCooldown { get => attackCooldown;}
    public float AttackDamage { get => attackDamage;}
    public Animator Animator { get => animator;}
    public float SpeedToSwitchRunAnim { get => speedToSwitchRunAnim;}
    public float SpeedToSwitchWalkAnim { get => speedToSwitchWalkAnim;}
    public float AngularSpeedToUseWalkAnim { get => angularSpeedToUseWalkAnim;}
    public float WalkAnimSpeedMultiplier { get => walkAnimSpeedMultiplier;}
    public float RunAnimSpeedMultiplier { get => runAnimSpeedMultiplier;}
    public float TurnAnimSpeedMultiplier { get => turnAnimSpeedMultiplier;}
    public NavMeshAgent Agent { get => agent;}
    public ZombieState State { get => state; set => state = value; }
    public float GroupJoiningDistance { get => groupJoiningDistance;}
    public GameObject GroupPrefab { get => groupPrefab;}
    public float GroupJoinCheckDelay { get => groupJoinCheckDelay;}

    public enum ZombieState
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

    public virtual void HearSound(float volume, Vector3 worldPosition)
    {
        float distanceToPosition = Vector3.Distance(transform.position, worldPosition);
        if (distanceToPosition > MaximumHearingDistance)
            return;

        if (Random.Range(0, (int)((distanceToPosition * HearingPossibilitiesMultiplier) / volume) + 1) == 0)
            HearedSound(worldPosition);
    }

    private protected abstract void HearedSound(Vector3 position);

    private protected virtual void PatrolMove()
    {
        Agent.speed = PatrolSpeed;
        Agent.angularSpeed = PatrolTurnSpeed;

        if (PatrolHasReachedDestination())
            PatrolReachedDestination();

        if (IsPlayerInSightArea())
            if (CanSeePlayer())
                PatrolSeePlayer();
    }

    private protected abstract void PatrolSeePlayer();

    private protected abstract bool PatrolHasReachedDestination();

    private protected abstract void PatrolReachedDestination();

    private protected virtual void ChaseMove()
    {
        Agent.speed = ChaseSpeed;
        Agent.angularSpeed = ChaseTurnSpeed;

        ChaseSetDestination();

        if (!IsPlayerInSightArea())
        {
            if (CanSeePlayer())
            {
                ChaseCanSeePlayer();
                return;
            }
        }
        if (!CanSeePlayer())
        {
            ChaseCannotSeePlayer();
            return;
        }

        ChaseSeePlayer();

        Attack();
    }

    private protected virtual void Attack()
    {
        Collider[] targets = Physics.OverlapSphere(AttackPoint.position, AttackDistance);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].CompareTag("Player"))
                {
                    HealthManager health = targets[i].GetComponent<HealthManager>();
                    if (health == null)
                        break;
                    health.ChangeHealth(-AttackDamage);

                    attackEnd = Time.time + AttackCooldown;
                    Agent.isStopped = true;
                    isAttacking = true;
                    Animator.SetTrigger("Attack");
                    break;
                }
            }
        }
    }

    private protected abstract void ChaseSetDestination();

    private protected abstract void ChaseCanSeePlayer();

    private protected abstract void ChaseCannotSeePlayer();

    private protected abstract void ChaseSeePlayer();

    private protected virtual void CheckMove()
    {
        Agent.speed = ChaseSpeed;
        Agent.angularSpeed = ChaseTurnSpeed;

        if (IsPlayerInSightArea())
        {
            if (CanSeePlayer())
            {
                CheckSeePlayer();
                return;
            }
        }

        if (CheckHasReachedDestination())
        {
            CheckReachedDestination();
            return;
        }
    }

    private protected abstract void CheckSeePlayer();

    private protected abstract bool CheckHasReachedDestination();

    private protected abstract void CheckReachedDestination();

    private protected virtual void SearchMove()
    {
        if (Time.time > patrolSwitchTime)
        {
            SearchTimeEnded();
            return;
        }

        Agent.isStopped = true;

        if (Mathf.Abs(transform.rotation.eulerAngles.y - searchCheckAngle.eulerAngles.y) < SearchTurnAngleChangeDistance)
            GenerateSearchCheckAngle();
        transform.rotation = Quaternion.Slerp(transform.rotation, searchCheckAngle, SearchTurnSpeed * Time.deltaTime);

        if (IsPlayerInSightArea())
        {
            if (CanSeePlayer())
            {
                SearchSeePlayer();
                return;
            }
        }
    }

    private protected abstract void SearchTimeEnded();

    private protected abstract void SearchSeePlayer();

    private protected virtual void GenerateSearchCheckAngle()
    {
        searchCheckAngle = Quaternion.Euler(0, Random.Range(0, 361), 0);
    }

    private protected virtual void TurnMove()
    {
        Agent.isStopped = true;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetDirectionToPlayer()), TurnTowardsPlayerSpeed * Time.deltaTime);

        if (!CanSeePlayer())
        {
            TurnCannotSeePlayer();
            return;
        }
        if (IsPlayerInSightArea())
        {
            TurnSeePlayer();
            return;
        }
    }

    private protected abstract void TurnCannotSeePlayer();

    private protected abstract void TurnSeePlayer();

    private protected virtual bool IsPlayerInSightArea()
    {
        return Vector3.Angle(transform.forward, GetDirectionToPlayer()) <= SeeAngle;
    }

    private protected virtual bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(EyePoint.position, PlayerController.GetInstance().transform.position - EyePoint.position, out hit, SeeDistance))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }

    private protected virtual Vector3 GetDirectionToPlayer()
    {
        Vector3 playerPosition = PlayerController.GetInstance().transform.position;
        playerPosition.y = 0;
        Vector3 ownPosition = transform.position;
        ownPosition.y = 0;
        return playerPosition - ownPosition;
    }

    private protected virtual void HandleAnimations()
    {
        float agentVelocity = Agent.velocity.magnitude;

        if(agentVelocity > SpeedToSwitchRunAnim)
        {
            Animator.SetBool("Walk", false);
            Animator.SetBool("Run", true);

            Animator.speed = Agent.velocity.magnitude * RunAnimSpeedMultiplier;
        } else if (agentVelocity > SpeedToSwitchWalkAnim)
        {
            Animator.SetBool("Walk", true);
            Animator.SetBool("Run", false);

            Animator.speed = Agent.velocity.magnitude * WalkAnimSpeedMultiplier;
        } else if (angularVelocity > AngularSpeedToUseWalkAnim)
        {
            Animator.SetBool("Walk", true);
            Animator.SetBool("Run", false);

            Animator.speed = angularVelocity * TurnAnimSpeedMultiplier;
        } else
        {
            Animator.SetBool("Walk", false);
            Animator.SetBool("Run", false);

            Animator.speed = 1;
        }
    }

    private protected virtual void CheckAngularVelocity()
    {
        Vector3 currentFacing = transform.forward;
        angularVelocity = Vector3.Angle(currentFacing, previousFacing) / Time.deltaTime;
        previousFacing = currentFacing;
    }

    public abstract void CheckIfCanJoinGroup();
}
