using UnityEngine;

public class SingleZombie : ZombieController
{
    [SerializeField] private GroupedZombie groupedZombie;

    [Header("Group Joining")]
    [SerializeField] private float groupJoiningDistance;
    [SerializeField] private GameObject groupPrefab;
    [SerializeField] private float groupJoinCheckDelay;
    private protected float nextGroupJoinCheckTime;

    [Header("Light Detection")]
    [SerializeField] private float maxAttackBrightness;
    [SerializeField] private float stopDistanceFromBright;

    public GroupedZombie GroupedZombie { get => groupedZombie;}
    public float GroupJoinCheckDelay { get => groupJoinCheckDelay;}
    public float GroupJoiningDistance { get => groupJoiningDistance;}
    public GameObject GroupPrefab { get => groupPrefab;}

    private void Update()
    {
        CheckAngularVelocity();

        if (isAttacking)
        {
            if (Time.time > attackEnd)
            {
                Agent.isStopped = false;
                isAttacking = false;
            }
            HandleAnimations();
            return;
        }

        if (State == ZombieState.patrol)
            PatrolMove();
        else if (State == ZombieState.chase)
            ChaseMove();
        else if (State == ZombieState.check)
            CheckMove();
        else if (State == ZombieState.search)
            SearchMove();
        else if (State == ZombieState.turn)
            TurnMove();

        HandleAnimations();

        CheckIfCanJoinGroup();
    }

    private protected override void HearedSound(Vector3 position)
    {
        Agent.SetDestination(position);
        State = ZombieState.turn;
    }

    private protected override void PatrolSeePlayer()
    {
        State = ZombieState.chase;
    }

    private protected override bool PatrolHasReachedDestination()
    {
        return Mathf.Abs(transform.position.x - Agent.destination.x) < 2 && Mathf.Abs(transform.position.z - Agent.destination.z) < 2;
    }

    private protected override void PatrolReachedDestination()
    {
        Agent.SetDestination(GeneratePatrolPosition());
    }

    private Vector3 GeneratePatrolPosition()
    {
        return new Vector3(Random.Range(transform.position.x - PatrolPosDistance, transform.position.x + PatrolPosDistance), transform.position.y, Random.Range(transform.position.z - PatrolPosDistance, transform.position.z + PatrolPosDistance));
    }

    private protected override void ChaseSetDestination()
    {
        if(ZombieManager.GetInstance().PlayerLocationLightLevel > maxAttackBrightness)
        {
            Agent.SetDestination(PlayerController.GetInstance().transform.position + Vector3.Normalize(transform.position - PlayerController.GetInstance().transform.position) * stopDistanceFromBright);
            return;
        }
        Agent.SetDestination(PlayerController.GetInstance().transform.position);
    }

    private protected override void ChaseCanSeePlayer()
    {
        if (ZombieManager.GetInstance().PlayerLocationLightLevel > maxAttackBrightness)
            if (Agent.remainingDistance > Agent.stoppingDistance + 0.05f)
            {
                Agent.isStopped = false;
                if (Vector3.Distance(Agent.destination, PlayerController.GetInstance().transform.position) > Vector3.Distance(transform.position, PlayerController.GetInstance().transform.position))
                    return;
            }
        State = ZombieState.turn;
    }

    private protected override void ChaseCannotSeePlayer()
    {
        Agent.isStopped = false;
        Agent.SetDestination(PlayerController.GetInstance().transform.position);
        State = ZombieState.check;
    }

    private protected override void ChaseSeePlayer()
    {
        if (ZombieManager.GetInstance().PlayerLocationLightLevel > maxAttackBrightness)
            if (Agent.remainingDistance <= Agent.stoppingDistance + 0.05f)
            {
                Agent.isStopped = true;
                return;
            }
        Agent.isStopped = false;
    }

    private protected override bool CanAttack()
    {
        return ZombieManager.GetInstance().PlayerLocationLightLevel <= maxAttackBrightness && Vector3.Distance(AttackPoint.position, PlayerController.GetInstance().PlayerCollider.ClosestPoint(AttackPoint.position)) <= AttackDistance;
    }

    private protected override void CheckSeePlayer()
    {
        State = ZombieState.chase;
    }

    private protected override bool CheckHasReachedDestination()
    {
        return Mathf.Abs(transform.position.x - Agent.destination.x) < Agent.stoppingDistance + 0.2f && Mathf.Abs(transform.position.z - Agent.destination.z) < Agent.stoppingDistance + 0.2f;
    }

    private protected override void CheckReachedDestination()
    {
        State = ZombieState.search;
        patrolSwitchTime = Time.time + DelayToSwitchPatrolMode;
        GenerateSearchCheckAngle();
    }

    private protected override void SearchTimeEnded()
    {
        State = ZombieState.patrol;
        Agent.isStopped = false;
    }

    private protected override void SearchSeePlayer()
    {
        State = ZombieState.chase;
        Agent.isStopped = false;
    }

    private protected override void TurnCannotSeePlayer()
    {
        State = ZombieState.check;
        Agent.isStopped = false;
    }

    private protected override void TurnSeePlayer()
    {
        State = ZombieState.chase;
        Agent.isStopped = false;
    }

    public void CheckIfCanJoinGroup()
    {
        if (Time.time < nextGroupJoinCheckTime)
            return;
        nextGroupJoinCheckTime = Time.time + GroupJoinCheckDelay;

        SingleZombie[] zombies = ZombieManager.GetInstance().GetZombiesNearPosition(transform.position, GroupJoiningDistance, this);
        if (zombies == null)
            return;
        for (int i = 0; i < zombies.Length; i++)
        {
            GroupedZombie groupedZombie = zombies[i].GetComponent<GroupedZombie>();
            if (!groupedZombie.enabled)
                continue;
            groupedZombie.GetGroup().AddZombie(this);
            return;
        }
        ZombieGroup newGroup = Instantiate(GroupPrefab).GetComponent<ZombieGroup>();
        newGroup.AddZombie(this);
        for (int i = 0; i < zombies.Length; i++)
        {
            newGroup.AddZombie(zombies[i]);
        }
    }
}
