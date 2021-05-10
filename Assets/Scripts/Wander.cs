using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{
    [SerializeField]
    private float wanderRadius = 20f;
    [SerializeField]
    private float wanderTimer = 5f;
    [SerializeField]
    private Animator Anim;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (GetComponent<NPCController>().CurrentState != NPCController.State.FOLLOWING && GetComponent<NPCController>().CurrentState != NPCController.State.SLEEPING && GetComponent<NPCController>().CurrentState != NPCController.State.GOING_TO_FOOD && GetComponent<NPCController>().CurrentState != NPCController.State.GOING_TO_WATER && GetComponent<NPCController>().CurrentState != NPCController.State.GOING_TO_NEED && GetComponent<NPCController>().CurrentState != NPCController.State.DEAD)
            {
                timer += Time.deltaTime;
                if (GetComponent<NPCController>().Needs.Age > 5)
                {
                    agent.speed = GetComponent<NPCController>().CurrentState == NPCController.State.RUNNING_AWAY ? 6f : 2f;
                    wanderTimer = GetComponent<NPCController>().CurrentState == NPCController.State.RUNNING_AWAY ? 1f : 5f;
                }
                else
                {
                    agent.speed = GetComponent<NPCController>().CurrentState == NPCController.State.RUNNING_AWAY ? 3f : 1f;
                    wanderTimer = GetComponent<NPCController>().CurrentState == NPCController.State.RUNNING_AWAY ? 0.5f : 2.5f;
                }
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
                    agent.SetDestination(RandomNavmeshLocation(5f));
                    GetComponent<NPCController>().CurrentState = NPCController.State.WANDERING;
                    timer = 0;
                }
            }
            else if (GetComponent<NPCController>().CurrentState == NPCController.State.SLEEPING)
            {
                agent.isStopped = true;
                Anim.SetBool("walk", false);
            }
        }
        catch
        {
            Debug.LogWarning("Ugh DUDE");
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