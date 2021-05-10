using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem Zs;
    [SerializeField]
    private AudioClip[] Baahs;
    [SerializeField]
    private AudioSource SheepAudioSource;
    [SerializeField]
    private GameObject Poop;
    [SerializeField]
    private GameObject Rip;
    [SerializeField]
    private GameObject CurrentPartner;
    [SerializeField]
    private SpriteRenderer NeedSprite;
    [SerializeField]
    private Sprite FoodSprite;
    [SerializeField]
    private Sprite ThirstSprite;
    [SerializeField]
    private Sprite ToiletSprite;
    [SerializeField]
    private Sprite HappySprite;
    [SerializeField]
    private Sprite ScaredSprite;
    [SerializeField]
    private Sprite LibidoSprite;
    [SerializeField]
    private int Gender = 0;
    [SerializeField]
    private int MinAge = 5;
    [SerializeField]
    private int YearOfBirth = 0;
    public List<Transform> Children = new List<Transform>(); // Kids
    public List<Transform> Parents = new List<Transform>(); // Mother, father
    [SerializeField]
    private Transform Following;
    private int Ticks = 0;
    public Needs Needs;
    public Needs.Need CurrentNeed = Needs.Need.NONE;
    public static NPCController Instance;
    
    public enum State
    {
        DEFAULT, WANDERING, FOLLOWING, SLEEPING, GOING_TO_FOOD, GOING_TO_WATER, DEAD, SEEKING_NEED, GOING_TO_NEED, RUNNING_AWAY
    }

    public State CurrentState = State.DEFAULT;
    private Animator Anim;
    private NavMeshAgent Agent;
    private static readonly System.Random random = new System.Random();
    private static readonly object syncLock = new object();

    public bool isKidFollowing()
    {
        if (Children.Count < 0)
        {
            return false;
        }
        if(Children.Count > 0 && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.SLEEP && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.NONE && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.LIBIDO && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.TOILET && Children[0].GetComponent<NPCController>().Following == transform)
        {
            return true;
        } 
        return false;
    }

    void Start()
    {
        Instance = this;
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("Baah", r(8, 20), r(8, 20));
        if (Parents.Count == 0)
        {
            YearOfBirth = EnvController.CurrentYear - 5;
        }
        else
        {
            YearOfBirth = EnvController.CurrentYear;
        }
        Gender = r(0, 2);
        Needs = new Needs();
        InvokeRepeating("ChangeNeeds", 1f, (15f/EnvController.ScaleOfTime));
        EnvController.SheepsAlive++;
        EnvController.SafePlaces.Add(transform.position);
    }

    void OnDestroy()
    {
        EnvController.SheepsAlive--;
        if (Parents.Count > 0)
        {
            foreach (var obj in Parents)
            {
                obj.GetComponent<NPCController>().Children.Remove(transform);
            }
        }
        if(Children.Count > 0)
        {
            foreach (var obj in Children)
            {
                obj.GetComponent<NPCController>().Parents.Remove(transform);
            }
        }
    }

    public void ChangeNeeds()
    {
        if (CurrentState != State.DEAD)
        {
            if (CurrentState == State.RUNNING_AWAY)
            {
                NeedSprite.sprite = ScaredSprite;
                return;
            }
            CurrentNeed = Needs.Saturation();
            switch (CurrentNeed)
            {
                case Needs.Need.HUNGER:
                    NeedSprite.sprite = FoodSprite;
                    break;
                case Needs.Need.THIRST:
                    NeedSprite.sprite = ThirstSprite;
                    break;
                case Needs.Need.TOILET:
                    NeedSprite.sprite = ToiletSprite;
                    break;
                case Needs.Need.LIBIDO:
                    NeedSprite.sprite = LibidoSprite;
                    break;
                default: NeedSprite.sprite = null; break;
            }
        }

    }

    private void OnTriggerStay(Collider c)
    {

        if (EnvController.KnownPredators.Contains(c.tag) && Vector3.Distance(transform.position, c.transform.position) < 8f && CurrentState != State.DEAD)
        {
            CurrentState = State.RUNNING_AWAY;
            Needs.SetGains(Needs.Need.SLEEP, 0f, 0f);
            Anim.SetBool("sleeping", false);
            Anim.SetBool("woohoo", false);
            Anim.SetBool("walk", true);
            Zs.gameObject.SetActive(false);
            return;
        }
        if (CurrentState == State.WANDERING || CurrentState == State.DEFAULT || CurrentState == State.SEEKING_NEED)
        {
            switch (c.tag)
            {
                case "Item":
                    if (c.GetComponent<Item>() != null && c.GetComponent<Item>().Attributes.Contains(Item.ItemType.EDIBLE) && c.GetComponent<Item>().ItemMods.ContainsKey(CurrentNeed))
                    {
                        WalkAndAnim(c.transform.position, State.GOING_TO_FOOD);
                    }
                    else if (c.GetComponent<Item>() != null && c.GetComponent<Item>().Attributes.Contains(Item.ItemType.USABLE) && c.GetComponent<Item>().ItemMods.ContainsKey(CurrentNeed))
                    {
                        WalkAndAnim(c.transform.position, State.GOING_TO_WATER);
                    }
                    break;
                case "Sheep":
                    try
                    {
                        if (Needs.Age < MinAge && Parents.Contains(c.transform))
                        {
                            WalkAndAnim(c.transform.position, State.FOLLOWING);
                            Following = c.transform;
                        }
                        if (c.GetComponent<NPCController>().Gender != Gender && c.GetComponent<NPCController>().Needs.Age >= MinAge && Needs.Age >= MinAge && CurrentNeed == Needs.Need.LIBIDO && c.GetComponent<NPCController>().CurrentNeed == Needs.Need.LIBIDO)
                        {
                            if (CurrentPartner == null && c.GetComponent<NPCController>().CurrentPartner == null && !Parents.Contains(c.transform) && !Children.Contains(c.transform))
                            {
                                CurrentPartner = c.gameObject;
                                c.GetComponent<NPCController>().CurrentPartner = gameObject;
                            }
                            else if (CurrentPartner == c.gameObject && c.GetComponent<NPCController>().CurrentPartner == gameObject)
                            {
                                if (Vector3.Distance(transform.position, c.transform.position) <= 2f)
                                {
                                    Agent.isStopped = true;
                                    Anim.SetBool("walk", false);
                                    Anim.SetBool("woohoo", true);
                                    if (!IsInvoking("Reproduce"))
                                    {
                                        Invoke("Reproduce", 5f);
                                    }
                                }
                                else
                                {
                                    WalkAndAnim(CurrentPartner.transform.position, State.GOING_TO_NEED);
                                }
                            }
                        }
                    }
                    catch
                    {
                        Debug.LogWarning("Idk dude something oopsie happened.");
                    }
                    break;
                default: break;
            }
        }
    }

    public void Reset()
    {
        CurrentPartner = null;
        Gender = 0;
        MinAge = 5;
        YearOfBirth = 0;
        Children.Clear();
        Parents.Clear();
        Ticks = 0;
        Needs = new Needs();
        CurrentNeed = Needs.Need.NONE;
        Instance = this;

        CurrentState = State.DEFAULT;
    }

    private void Reproduce()
    {
        if (Gender == 0)
        {
            GameObject kid = Instantiate(gameObject, transform.parent);
            kid.SetActive(false);
            kid.GetComponent<NPCController>().Reset();
            kid.GetComponent<NPCController>().YearOfBirth = EnvController.CurrentYear;
            kid.GetComponent<NPCController>().Gender = r(0, 2);
            kid.GetComponent<NPCController>().Parents.Add(transform);
            kid.GetComponent<NPCController>().Parents.Add(CurrentPartner.transform);
            kid.SetActive(true);
            CurrentPartner.GetComponent<NPCController>().Children.Add(kid.transform);
            Children.Add(kid.transform);
        }
        Anim.SetBool("woohoo", false);
        Needs.SetGains(Needs.Need.LIBIDO, 100f, 100f);
        CancelInvoke("Reproduce");
        ChangeNeeds();
        CurrentState = State.DEFAULT;
    }

    private void OnTriggerExit(Collider c)
    {
        if(Following == c.transform)
        {
            Following = null;
        }
        if (CurrentState == State.GOING_TO_WATER || CurrentState == State.GOING_TO_FOOD || CurrentState == State.GOING_TO_NEED || CurrentState == State.SEEKING_NEED || CurrentState == State.FOLLOWING)
        {
            CurrentState = State.DEFAULT;
        }
    }

    void Update()
    {
        if(Children.Count > 0) 
        {
            if (Children.Count > 1)
            {
                foreach (var obj in Children)
                {
                    if (obj.GetComponent<NPCController>().CurrentNeed != Needs.Need.SLEEP && obj.GetComponent<NPCController>().CurrentNeed != Needs.Need.NONE && obj.GetComponent<NPCController>().CurrentNeed != Needs.Need.LIBIDO && obj.GetComponent<NPCController>().CurrentNeed != Needs.Need.TOILET && obj.GetComponent<NPCController>().Following == transform)
                    {
                        if (EnvController.LastKnownPlaces.ContainsKey(obj.GetComponent<NPCController>().CurrentNeed))
                        {
                            if (EnvController.LastKnownPlaces[obj.GetComponent<NPCController>().CurrentNeed].gameObject.activeSelf)
                            {
                                WalkAndAnim(EnvController.LastKnownPlaces[obj.GetComponent<NPCController>().CurrentNeed].position, State.GOING_TO_NEED);
                                return;
                            }
                        }
                    }
                }
            }
            else if(Children.Count == 1 && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.SLEEP && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.NONE && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.LIBIDO && Children[0].GetComponent<NPCController>().CurrentNeed != Needs.Need.TOILET && Children[0].GetComponent<NPCController>().Following == transform)
            {
                if (EnvController.LastKnownPlaces.ContainsKey(Children[0].GetComponent<NPCController>().CurrentNeed))
                {
                    if (EnvController.LastKnownPlaces[Children[0].GetComponent<NPCController>().CurrentNeed].gameObject.activeSelf)
                    {
                        WalkAndAnim(EnvController.LastKnownPlaces[Children[0].GetComponent<NPCController>().CurrentNeed].position, State.GOING_TO_NEED);
                        return;
                    }
                }
            }
        }

        if((CurrentState == State.GOING_TO_FOOD || CurrentState == State.GOING_TO_NEED || CurrentState == State.GOING_TO_WATER || CurrentState == State.SEEKING_NEED) && CurrentNeed == Needs.Need.NONE && !isKidFollowing())
        {
            CurrentState = State.DEFAULT;
        }
        if (CurrentNeed != Needs.Need.NONE && CurrentState != State.SLEEPING && CurrentState != State.DEAD && CurrentState != State.RUNNING_AWAY)
        {
            if (CurrentNeed == Needs.Need.TOILET)
            {
                Poop.gameObject.SetActive(true);
                Instantiate(Poop, transform.position, transform.rotation, transform.parent);
                Needs.SetGains(Needs.Need.TOILET, 50f, 50f);
                ChangeNeeds();
                Poop.gameObject.SetActive(false);
            }
            else
            {
                if (EnvController.LastKnownPlaces.ContainsKey(CurrentNeed) && CurrentState != State.FOLLOWING)
                {
                    if (EnvController.LastKnownPlaces[CurrentNeed].gameObject.activeSelf)
                    {
                        WalkAndAnim(EnvController.LastKnownPlaces[CurrentNeed].position, State.GOING_TO_NEED);
                    }
                    else
                    {
                        CurrentState = State.SEEKING_NEED;
                    }
                }
                else
                {
                    CurrentState = State.SEEKING_NEED;
                }
            }
        }
        if(Needs.Age >= 10 || Needs.Health <= 0f)
        {
            Death();
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Sheep|Die"))
            {
                //Destroy(gameObject);
            }
            return;
        }
        if(Needs.Age >= MinAge)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }else if(Needs.Age < MinAge)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        Needs.Age = EnvController.CurrentYear - YearOfBirth;
        if ((EnvController.CurrentHour >= 20f || EnvController.CurrentHour >= 0f && EnvController.CurrentHour <= 6f) && (CurrentNeed == Needs.Need.NONE || CurrentNeed == Needs.Need.SLEEP) && CurrentState != State.RUNNING_AWAY && !isKidFollowing())
        {
            if (EnvController.SafePlaces.Count > 0 && Vector3.Distance(transform.position, EnvController.SafePlaces[0]) > 1.5f)
            {
                WalkAndAnim(EnvController.SafePlaces[0], State.GOING_TO_NEED);
            }
            else
            {
                Agent.isStopped = true;
                CurrentState = State.SLEEPING;
                Needs.SetGains(Needs.Need.SLEEP, 50f, 50f);
                Sleep();
            }
        }else if (EnvController.CurrentHour > 6f)
        {
            if (CurrentState == State.SLEEPING)//&& CurrentNeed == Needs.Need.NONE)
            {
                CurrentState = State.DEFAULT;
            }
            Anim.SetBool("sleeping", false);
            Zs.gameObject.SetActive(false);
        }
        if (CurrentState == State.SLEEPING && CurrentNeed != Needs.Need.NONE && CurrentNeed != Needs.Need.SLEEP)
        {
            Debug.Log("No time for sleeping");
            CurrentState = State.SEEKING_NEED;
            Needs.SetGains(Needs.Need.SLEEP, 0f, 0f);
            Anim.SetBool("sleeping", false);
            Anim.SetBool("walk", true);
            Zs.gameObject.SetActive(false);
        }

    }

    public void Death()
    {
        CurrentState = State.DEAD;
        Anim.SetTrigger("die");
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Zs.gameObject.SetActive(false);
        CancelInvoke();
        InvokeRepeating("WaitBeforeDestroy", 0f, 1f);
    }

    public void WaitBeforeDestroy()
    {
        if(Ticks > 10)
        {
            Rip.gameObject.SetActive(true);
            Instantiate(Rip, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
        }
        Ticks++;
    }

    public void Sleep()
    {
        Zs.gameObject.SetActive(true);
        Anim.SetBool("sleeping", true);
    }

    public void Baah()
    {
        if (!SheepAudioSource.isPlaying && CurrentState != State.SLEEPING)
        {
            SheepAudioSource.clip = Baahs[r(0, 4)];
            SheepAudioSource.Play();
        }
    }

    private void WalkAndAnim(Vector3 dest, State state)
    {
        Agent.isStopped = false;
        Agent.SetDestination(dest);
        CurrentState = state;
        Anim.SetBool("walk", true);
    }

    public static int r(int min, int max)
    {
        lock (syncLock)
        { // synchronize
            return random.Next(min, max);
        }
    }


}
