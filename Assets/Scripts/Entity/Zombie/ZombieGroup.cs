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

    private GroupedZombie[] zombies;

    private ZombieGroupState state;

    public ZombieGroupState State { get => state; set => state = value; }

    public enum ZombieGroupState
    {
        patrol,
        chase,
        check
    }

    public void CheckIfNoOneCanSeePlayer()
    {
        for(int i = 0; i < zombies.Length; i++)
            if (!zombies[i].CanSeePlayer1)
                return;
        state = ZombieGroupState.check;
        GenerateCheckPositions();
    }

    public void CheckIfEveryoneHasEndedSearch()
    {
        for (int i = 0; i < zombies.Length; i++)
            if (!zombies[i].SearchTimeEnded1)
                return;
        state = ZombieGroupState.patrol;
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Agent.isStopped = false;
        }

        GeneratePatrolPositions();
    }

    public void GeneratePatrolPositions()
    {
        Vector3 groupTarget = new Vector3(Random.Range(zombies[0].transform.position.x - patrolPosDistance, zombies[0].transform.position.x + patrolPosDistance), zombies[0].transform.position.y, Random.Range(zombies[0].transform.position.z - patrolPosDistance, zombies[0].transform.position.z + patrolPosDistance));
        for (int i = 0; i < zombies.Length; i++)
        {
            Vector3 zombieTarget = groupTarget + Vector3.right * i * patrolPosSpread;
            zombies[i].Agent.SetDestination(zombieTarget);
        }
    }

    public void GenerateCheckPositions()
    {
        Vector3 playerPosition = PlayerController.GetInstance().transform.position;
        for(int i = 0; i < zombies.Length; i++)
        {
            zombies[i].Agent.SetDestination(playerPosition + new Vector3(Random.Range(-checkMaxDistanceFromPlayer, checkMaxDistanceFromPlayer), 0, Random.Range(-checkMaxDistanceFromPlayer, checkMaxDistanceFromPlayer)));
            zombies[i].State = ZombieController.ZombieState.check;
            zombies[i].SearchTimeEnded1 = false;
        }
    }

    public void SetZombieStates(ZombieController.ZombieState state)
    {
        for (int i = 0; i < zombies.Length; i++)
            zombies[i].State = state;
    }

    public Vector3 GenerateChasePosition(int zombieIndex)
    {
        float angle = (360f / zombies.Length) * zombieIndex;
        Vector3 position = PlayerController.GetInstance().transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * chaseDistanceFromTarget;
        return position;
    }

    public void AddZombie(SingleZombie zombie)
    {
        zombie.enabled = false;

        GroupedZombie groupedZombie = zombie.GetComponent<GroupedZombie>();
        if (zombies == null)
            zombies = new GroupedZombie[0];
        groupedZombie.SetGroup(this, zombies.Length);
        groupedZombie.enabled = true;

        GroupedZombie[] newZombies = new GroupedZombie[zombies.Length + 1];
        for(int i = 0; i < zombies.Length; i++)
        {
            newZombies[i] = zombies[i];
        }
        newZombies[zombies.Length] = groupedZombie;
        zombies = newZombies;
    }

    public void RemoveZombie(GroupedZombie zombie)
    {
        if(zombies.Length == 2)
        {
            int remainingZombie;
            if (zombie.IndexInGroup == 0)
                remainingZombie = 1;
            else
                remainingZombie = 0;

            zombies[remainingZombie].enabled = false;
            zombies[remainingZombie].GetComponent<SingleZombie>().enabled = true;
        }

        if(zombies.Length <= 2)
        {
            Destroy(gameObject);
            return;
        }

        GroupedZombie[] newZombies = new GroupedZombie[zombies.Length - 1];
        for(int i = 0; i < zombies.Length - 1; i++)
        {
            if(i < zombie.IndexInGroup)
            {
                newZombies[i] = zombies[i];
                continue;
            }
            zombies[i + 1].IndexInGroup = i;
            newZombies[i] = zombies[i + 1];
        }
    }
}
