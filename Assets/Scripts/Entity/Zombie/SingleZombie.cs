using UnityEngine;

public class SingleZombie : ZombieController
{
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
        Agent.SetDestination(PlayerController.GetInstance().transform.position);
    }

    private protected override void ChaseCanSeePlayer()
    {
        State = ZombieState.turn;
    }

    private protected override void ChaseCannotSeePlayer()
    {
        State = ZombieState.check;
    }

    private protected override void ChaseSeePlayer()
    {

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

    public override void CheckIfCanJoinGroup()
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


        /*Collider[] zombies = Physics.OverlapSphere(transform.position, GroupJoiningDistance, ZombieLayers);
        if (zombies.Length <= 1)
            return;
        for (int i = 0; i < zombies.Length; i++)
        {
            if (zombies[i].gameObject.Equals(gameObject))
                continue;
            GroupedZombie groupedZombie = zombies[i].GetComponent<GroupedZombie>();
            if (!groupedZombie.enabled)
                continue;
            groupedZombie.GetGroup().AddZombie(this);
            return;
        }
        ZombieGroup newGroup = Instantiate(GroupPrefab).GetComponent<ZombieGroup>();
        for (int i = 0; i < zombies.Length; i++)
        {
            newGroup.AddZombie(zombies[i].GetComponent<SingleZombie>());
        }*/
    }
}
