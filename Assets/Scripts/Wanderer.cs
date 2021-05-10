using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wanderer : MonoBehaviour
{
    [SerializeField]
    private float wanderRadius = 20f;
    [SerializeField]
    private float wanderTimer = 3.5f;
    [SerializeField]
    private Animator Anim;
    [SerializeField]
    private WolfBehaviour Controller;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        Anim = GetComponent<Animator>();
        Controller = GetComponent<WolfBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.CurrentState == WolfBehaviour.State.WANDERING)
        {
            timer += Time.deltaTime;

            if (agent.velocity.magnitude >= 0.5f)
            {
                Anim.SetBool("walk", true);
            }
            else
            {
                Anim.SetBool("walk", false);
            }

            if (timer >= wanderTimer)
            {
                agent.isStopped = false;
                agent.SetDestination(RandomNavmeshLocation(15f));
                timer = 0;
            }
        }
    }

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, -1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}