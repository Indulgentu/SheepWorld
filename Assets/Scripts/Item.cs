using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public String Name = "Insignificant Item";
    public Sprite ItemIcon;
    public int ItemID = 0;
    public enum ItemType
    {
        EDIBLE, USABLE, SELLABLE
    }
    public enum ItemModifiers
    {
        HEALTH, HUNGER, TIRED, THIRST, TOILET
    }
    public List<ItemType> Attributes;
    public Dictionary<Needs.Need, float> ItemMods = new Dictionary<Needs.Need, float>();

    private void OnTriggerStay(Collider c)
    {
        try
        {
            if (c.GetComponent<NPCController>().isKidFollowing() && Vector3.Distance(transform.position, c.transform.position) < 1f)
            {
                foreach (var obj in c.GetComponent<NPCController>().Children)
                {
                    if (ItemMods.ContainsKey(obj.GetComponent<NPCController>().CurrentNeed))
                    {
                        Use(obj.GetComponent<NPCController>().Needs);
                    }
                }
                Use(c.GetComponent<NPCController>().Needs);
            }
            else if (Vector3.Distance(transform.position, c.transform.position) < 1f && !c.GetComponent<NPCController>().isKidFollowing())
            {
                Use(c.GetComponent<NPCController>().Needs);
            }
        }
        catch
        {

        }

    }

    void OnDestroy()
    {
        try
        {
           // NPC.NPC.Instance.UsedObj.Remove(Name);
        }
        catch
        {

        }
    }

    void Start()
    {
        if (Attributes.Contains(ItemType.EDIBLE))
        {
            ItemMods.Add(Needs.Need.HEALTH, UnityEngine.Random.Range(5f, 20f));
            ItemMods.Add(Needs.Need.HUNGER, UnityEngine.Random.Range(40f, 90f));
        }
        if (Attributes.Contains(ItemType.USABLE))
        {
            ItemMods.Add(Needs.Need.THIRST, UnityEngine.Random.Range(40f, 90f));
        }
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
    }

    public void Use(Needs Target)
    {
        //Needs Target = NPCController.Instance.Needs;
        if (Attributes.Contains(ItemType.EDIBLE) || Attributes.Contains(ItemType.USABLE))
        {
            foreach (var Mods in ItemMods)
            {
                Debug.Log("Used item with mod: " + Mods.Key + ", adding " + Mods.Value);
                switch (Mods.Key)
                {
                    case Needs.Need.HEALTH:
                        if (Target.Health + Mods.Value <= 100 && NPCController.Instance.CurrentNeed == Mods.Key)
                        {
                            Target.Health += Mods.Value;
                        }
                        break;
                    case Needs.Need.HUNGER:
                        if (Target.CurrentLevels[Needs.Need.HUNGER] + Mods.Value >= 0f)
                        {
                            Target.SetGains(Mods.Key, Mods.Value, 10f);
                            try
                            {
                                EnvController.LastKnownPlaces.Add(Needs.Need.HUNGER, transform);
                            }
                            catch
                            {
                                EnvController.LastKnownPlaces[Needs.Need.HUNGER] = transform;
                            }
                        }
                        break;
                    case Needs.Need.THIRST:
                        if (Target.CurrentLevels[Needs.Need.THIRST] + Mods.Value >= 0f)
                        {
                            Target.SetGains(Mods.Key, Mods.Value, 10f);
                            try
                            {
                                EnvController.LastKnownPlaces.Add(Needs.Need.THIRST, transform);
                            }
                            catch
                            {
                                //EnvController.LastKnownPlaces[Needs.Need.THIRST] = transform;
                            }
                        }
                        break;
                    case Needs.Need.SLEEP:
                        if (Target.CurrentLevels[Needs.Need.SLEEP] + Mods.Value >= 0f)
                        {
                            Target.SetGains(Mods.Key, Mods.Value, 10f);
                            try
                            {
                                EnvController.LastKnownPlaces.Add(Needs.Need.SLEEP, transform);
                            }
                            catch
                            {
                                EnvController.LastKnownPlaces[Needs.Need.SLEEP] = transform;
                            }
                        }
                        break;
                    default: break;
                }
            }
            if (!Attributes.Contains(ItemType.USABLE))
            {
                Invoke("Respawn", 5f);
                gameObject.SetActive(false);
            }
            NPCController.Instance.ChangeNeeds();
            
        }
    }
}
