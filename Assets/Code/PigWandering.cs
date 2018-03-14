using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PigWandering : MonoBehaviour
{

    public float pigWanderRadius;
    public float pigWanderTimer;

    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    private float timeOfWander;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timeOfWander = pigWanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timeOfWander += Time.deltaTime;

        if (timeOfWander >= pigWanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, pigWanderRadius, -1);
            agent.SetDestination(newPos);
            timeOfWander = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        UnityEngine.AI.NavMeshHit navHit;

        UnityEngine.AI.NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}

