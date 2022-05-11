using UnityEngine;

public class GroupedZombie : ZombieController
{
    private ZombieGroup group = null;
    private int indexInGroup = -1;

    private bool canSeePlayer = false;
    private bool isCheckingSound = false;
    private bool searchTimeEnded = false;

    public bool CanSeePlayer1 { get => canSeePlayer;}
    public bool SearchTimeEnded1 { get => searchTimeEnded; set => searchTimeEnded = value; }
    public int IndexInGroup { get => indexInGroup; set => indexInGroup = value; }

    private void Update()
    {
        CheckAngularVelocity();

        if (group == null)
            return;

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

        if (isCheckingSound)
            CheckSoundMove();
        else if (group.State == ZombieGroup.ZombieGroupState.patrol)
            PatrolMove();
        else if (group.State == ZombieGroup.ZombieGroupState.chase && State == ZombieState.turn)
            TurnMove();
        else if (group.State == ZombieGroup.ZombieGroupState.chase && State == ZombieState.chase)
            ChaseMove();
        else if (group.State == ZombieGroup.ZombieGroupState.check && State == ZombieState.search)
            SearchMove();
        else if (group.State == ZombieGroup.ZombieGroupState.check && State == ZombieState.check)
            CheckMove();

        HandleAnimations();
    }

    private protected override void HearedSound(Vector3 position)
    {
        if (group.State == ZombieGroup.ZombieGroupState.chase)
            return;
        isCheckingSound = true;
    }

    private void CheckSoundMove()
    {
        Agent.isStopped = true;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetDirectionToPlayer()), TurnTowardsPlayerSpeed * Time.deltaTime);

        if (!CanSeePlayer())
        {
            isCheckingSound = false;
            return;
        }
        if (IsPlayerInSightArea())
        {
            isCheckingSound = false;
            group.State = ZombieGroup.ZombieGroupState.chase;
            group.SetZombieStates(ZombieState.chase);
            return;
        }
    }

    private protected override void PatrolSeePlayer()
    {
        group.State = ZombieGroup.ZombieGroupState.chase;
        group.SetZombieStates(ZombieState.chase);
    }

    private protected override void PatrolReachedDestination()
    {
        group.GeneratePatrolPositions();
    }

    private protected override void ChaseSetDestination()
    {
        Agent.SetDestination(group.GenerateChasePosition(IndexInGroup));
    }

    private protected override void ChaseCanSeePlayer()
    {
        State = ZombieState.turn;
    }

    private protected override void ChaseCannotSeePlayer()
    {
        if (!canSeePlayer)
            return;
        canSeePlayer = false;
        group.CheckIfNoOneCanSeePlayer();
    }

    private protected override void ChaseSeePlayer()
    {
        canSeePlayer = true;
    }

    private protected override void TurnCannotSeePlayer()
    {
        State = ZombieState.chase;
        Agent.isStopped = false;
    }

    private protected override void TurnSeePlayer()
    {
        State = ZombieState.chase;
        Agent.isStopped = false;
    }

    private protected override void CheckSeePlayer()
    {
        group.State = ZombieGroup.ZombieGroupState.chase;
        group.SetZombieStates(ZombieState.chase);
    }

    private protected override void CheckReachedDestination()
    {
        State = ZombieState.search;
        patrolSwitchTime = Time.time + DelayToSwitchPatrolMode;
        GenerateSearchCheckAngle();
    }

    private protected override void SearchTimeEnded()
    {
        if (SearchTimeEnded1)
            return;
        SearchTimeEnded1 = true;
        group.CheckIfEveryoneHasEndedSearch();
    }

    private protected override void SearchSeePlayer()
    {
        group.State = ZombieGroup.ZombieGroupState.chase;
        group.SetZombieStates(ZombieState.chase);
    }

    public void SetGroup(ZombieGroup group, int indexInGroup)
    {
        this.group = group;
        this.IndexInGroup = indexInGroup;
    }

    public ZombieGroup GetGroup()
    {
        return group;
    }

    private protected override bool PatrolHasReachedDestination()
    {
        return Mathf.Abs(transform.position.x - Agent.destination.x) < 2 && Mathf.Abs(transform.position.z - Agent.destination.z) < 2;
    }

    private protected override bool CheckHasReachedDestination()
    {
        return Mathf.Abs(transform.position.x - Agent.destination.x) < Agent.stoppingDistance + 0.2f && Mathf.Abs(transform.position.z - Agent.destination.z) < Agent.stoppingDistance + 0.2f;
    }

    public override void CheckIfCanJoinGroup()
    {
        
    }
}
