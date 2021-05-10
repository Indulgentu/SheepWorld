using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfBehaviour : MonoBehaviour
{
    [SerializeField]
    private List<string> Pray = new List<string>();
    [SerializeField]
    private List<string> ScaredOf = new List<string>();
    [SerializeField]
    private Transform CurrentTarget;
    [SerializeField]
    private Transform CurrentAfraid;
    [SerializeField]
    private NavMeshAgent Agent;
    [SerializeField]
    public State CurrentState = State.WANDERING;
    [SerializeField]
    private Animator Anim;

    public enum State
    {
        WANDERING, CHASING, AFRAID
    }

    void OnTriggerStay(Collider c)
    {
        Debug.Log(c.tag);
        if (c.tag == "Doggo"){
            CurrentAfraid = c.transform;
            Debug.Log("Wtf dude");
            Debug.Log(Vector3.Distance(transform.position, CurrentAfraid.position));
            if(Vector3.Distance(transform.position, CurrentAfraid.position) < 10f)
            {
                CurrentState = State.AFRAID;
                CurrentTarget = null;
                Anim.SetBool("eat", false);
                Anim.SetBool("walk", true);
                Agent.isStopped = false;
                Agent.SetDestination(transform.parent.position);
                return;
            }
        }
        if (Pray.Contains(c.tag.ToLower()) && CurrentState != State.AFRAID)
        {
            switch (CurrentState)
            {
                case State.WANDERING:
                    if (c.GetComponent<NPCController>().CurrentState != NPCController.State.DEAD)
                    {
                        CurrentTarget = c.transform;
                        CurrentState = State.CHASING;
                    }
                    break;
                case State.CHASING:
                    try { 
                        if (c.gameObject == CurrentTarget.gameObject)
                        {
                            if(CurrentTarget.GetComponent<NPCController>().CurrentState == NPCController.State.DEAD)
                            {
                                Agent.isStopped = false;
                                CurrentTarget = null;
                                Anim.SetBool("eat", false);
                                CurrentState = State.WANDERING;
                                break;
                            }
                            if (Vector3.Distance(transform.position, CurrentTarget.position) > 2f)
                            {
                                Agent.isStopped = false;
                                Agent.SetDestination(CurrentTarget.position);
                                Anim.SetBool("walk", true);
                                Anim.SetBool("eat", false);
                            }
                            else if (Vector3.Distance(transform.position, CurrentTarget.position) < 2f)
                            {
                                Agent.isStopped = true;
                                Anim.SetBool("walk", false);
                                Anim.SetBool("eat", true);
                                CurrentTarget.GetComponent<NPCController>().Needs.Health -= 0.2f;
                                if (!EnvController.KnownPredators.Contains(gameObject.tag))
                                {
                                    EnvController.KnownPredators.Add(gameObject.tag);
                                }
                            }
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        CurrentTarget = null;
                        CurrentState = State.WANDERING;
                    }
                    break;

                default: break;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if(Pray.Contains(c.tag.ToLower()))
        {
            switch (CurrentState)
            {
                case State.CHASING:
                    if(c.gameObject == CurrentTarget.gameObject)
                    {
                        if(CurrentTarget.GetComponent<NPCController>().CurrentState == NPCController.State.DEAD)
                        {
                            CurrentTarget = null;
                            CurrentState = State.WANDERING;
                        }
                    }
                    break;
                default: break;
            }
        }
    }

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(CurrentState == State.AFRAID && Vector3.Distance(transform.position, transform.parent.position) < 2f)
        {
            CurrentState = State.WANDERING;
        }
    }

}
