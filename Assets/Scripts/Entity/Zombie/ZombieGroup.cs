using UnityEngine;

public class ZombieGroup : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float patrolPosDistance = 30;
    [SerializeField] private float patrolPosSpread = 1;

    [Header("Chase")]
    [SerializeField] private float chaseDistanceFromTarget = 1;

    [Header("Check")]
    [SerializeField] private float checkMaxDistanceFromPlayer = 5;

    [Header("Group Joining")]
    [SerializeField] private float groupJoiningDistance;
    [SerializeField] private float groupJoinCheckDelay;
    private protected float nextGroupJoinCheckTime;

    private GroupedZombie[] zombies;

    private ZombieGroupState state;

    public ZombieGroupState State { get => state; set => state = value; }
    public float GroupJoinCheckDelay { get => groupJoinCheckDelay;}
    public float GroupJoiningDistance { get => groupJoiningDistance;}
    public GroupedZombie[] Zombies { get => zombies;}

    public enum ZombieGroupState
    {
        patrol,
        chase,
        check
    }

    private void Awake()
    {
        ZombieManager.GetInstance().RegisterGroup(this);
    }

    private void Update()
    {
        CheckIfCanJoinGroup();
    }

    public void CheckIfNoOneCanSeePlayer()
    {
        for(int i = 0; i < Zombies.Length; i++)
            if (!Zombies[i].CanSeePlayer1)
                return;
        state = ZombieGroupState.check;
        GenerateCheckPositions();
    }

    public void CheckIfEveryoneHasEndedSearch()
    {
        for (int i = 0; i < Zombies.Length; i++)
            if (!Zombies[i].SearchTimeEnded1)
                return;
        state = ZombieGroupState.patrol;
        for (int i = 0; i < Zombies.Length; i++)
        {
            Zombies[i].Agent.isStopped = false;
        }

        GeneratePatrolPositions();
    }

    public void GeneratePatrolPositions()
    {
        Vector3 groupTarget = new Vector3(Random.Range(Zombies[0].transform.position.x - patrolPosDistance, Zombies[0].transform.position.x + patrolPosDistance), Zombies[0].transform.position.y, Random.Range(Zombies[0].transform.position.z - patrolPosDistance, Zombies[0].transform.position.z + patrolPosDistance));
        for (int i = 0; i < Zombies.Length; i++)
        {
            Vector3 zombieTarget = groupTarget + Vector3.right * i * patrolPosSpread;
            Zombies[i].Agent.SetDestination(zombieTarget);
        }
    }

    public void GenerateCheckPositions()
    {
        Vector3 playerPosition = PlayerController.GetInstance().transform.position;
        for(int i = 0; i < Zombies.Length; i++)
        {
            Zombies[i].Agent.SetDestination(playerPosition + new Vector3(Random.Range(-checkMaxDistanceFromPlayer, checkMaxDistanceFromPlayer), 0, Random.Range(-checkMaxDistanceFromPlayer, checkMaxDistanceFromPlayer)));
            Zombies[i].State = ZombieController.ZombieState.check;
            Zombies[i].SearchTimeEnded1 = false;
        }
    }

    public void SetZombieStates(ZombieController.ZombieState state)
    {
        for (int i = 0; i < Zombies.Length; i++)
            Zombies[i].State = state;
    }

    public Vector3 GenerateChasePosition(int zombieIndex)
    {
        float angle = (360f / Zombies.Length) * zombieIndex;
        Vector3 position = PlayerController.GetInstance().transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * chaseDistanceFromTarget;
        return position;
    }

    public void AddZombie(SingleZombie zombie)
    {
        zombie.enabled = false;

        GroupedZombie groupedZombie = zombie.GroupedZombie;
        if (zombies == null)
            zombies = new GroupedZombie[0];
        groupedZombie.SetGroup(this, Zombies.Length);
        groupedZombie.enabled = true;

        GroupedZombie[] newZombies = new GroupedZombie[Zombies.Length + 1];
        for(int i = 0; i < Zombies.Length; i++)
        {
            newZombies[i] = Zombies[i];
        }
        newZombies[Zombies.Length] = groupedZombie;
        zombies = newZombies;

        groupedZombie.Agent.isStopped = false;
        if (state == ZombieGroupState.chase)
        {
            groupedZombie.State = ZombieController.ZombieState.chase;
        } else if (state == ZombieGroupState.check)
        {
            groupedZombie.State = ZombieController.ZombieState.check;
        }
    }

    public void RemoveZombie(GroupedZombie zombie)
    {
        if(Zombies.Length == 2)
        {
            int remainingZombie;
            if (zombie.IndexInGroup == 0)
                remainingZombie = 1;
            else
                remainingZombie = 0;

            Zombies[remainingZombie].enabled = false;
            Zombies[remainingZombie].GetComponent<SingleZombie>().enabled = true;
        }

        if(Zombies.Length <= 2)
        {
            ZombieManager.GetInstance().UnRegisterGroup(this);
            Destroy(gameObject);
            return;
        }

        GroupedZombie[] newZombies = new GroupedZombie[Zombies.Length - 1];
        for(int i = 0; i < Zombies.Length - 1; i++)
        {
            if(i < zombie.IndexInGroup)
            {
                newZombies[i] = Zombies[i];
                continue;
            }
            Zombies[i + 1].IndexInGroup = i;
            newZombies[i] = Zombies[i + 1];
        }
        zombies = newZombies;
    }

    public void CheckIfCanJoinGroup()
    {
        if (Zombies == null)
            return;
        if (Zombies.Length == 0)
            return;
        if (Time.time < nextGroupJoinCheckTime)
            return;
        nextGroupJoinCheckTime = Time.time + GroupJoinCheckDelay;

        ZombieGroup[] otherNearGroups = ZombieManager.GetInstance().GetOtherGroupsNearPosition(Zombies[0].transform.position, groupJoiningDistance, this);
        if (otherNearGroups == null)
            return;

        for(int i = 0; i < otherNearGroups.Length; i++)
        {
            for(int j = 0; j < otherNearGroups[i].Zombies.Length; j++)
            {
                AddZombie(otherNearGroups[i].Zombies[j].SingleZombie);
            }
            ZombieManager.GetInstance().UnRegisterGroup(otherNearGroups[i]);
            Destroy(otherNearGroups[i].gameObject);
        }
    }
}
