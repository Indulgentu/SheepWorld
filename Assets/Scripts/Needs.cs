using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Base class for Needs. 
/// <para>Will benefit from further optimisation.</para>
/// </summary>
public class Needs
{
    /// <summary>
    /// Holds the levels of needs
    /// <para>
    /// Key: <c>Need</c>
    /// Value: <c>float</c>
    /// </para>
    ///</summary>
    public Dictionary<Need, float> CurrentLevels = new Dictionary<Need, float>();
    /// <summary>
    /// Holds the max tolerance levels
    /// <para>
    /// Key: <c>Need</c>
    /// Value: <c>float</c>
    /// </para>
    ///</summary>
    public Dictionary<Need, float> MaxAccLevels = new Dictionary<Need, float>();
    /// <summary>
    /// Holds temporary action gain for needs
    /// <para>
    /// Key: <c>Need</c>
    /// Value: <c>float</c>
    /// </para>
    ///</summary>
    public Dictionary<Need, float> ActionGain = new Dictionary<Need, float>();
    /// <summary>
    /// Holds temporary item gay for needs
    /// <para>
    /// Key: <c>Need</c>
    /// Value: <c>float</c>
    /// </para>
    ///</summary>
    public Dictionary<Need, float> ItemGain = new Dictionary<Need, float>();
    /// <summary>
    /// Time gain of actions
    ///</summary>
    private float TimeGain { get; set; }
    /// <summary>
    /// Pain Level increased by all needs
    /// </summary>
    public float PainLevel { get; set; } = 1f;
    /// <summary>
    /// The amount by which to increase pain levels over time
    /// </summary>
    public float PainConstant { get; set; } = 5f;
    /// <summary>
    /// Base health of agent
    /// </summary>
    public float Health = 100f;
    /// <summary>
    /// Age of agent
    /// </summary>
    public int Age = 0;
    private Random R;
    /// <summary>
    /// Current supported needs.
    /// </summary>
    public enum Need
    {
        HEALTH, HUNGER, THIRST, SLEEP, TOILET, LIBIDO, NONE 
    }

    public Needs()
    {
        R = new Random();
        for (int i = 0; i < Enum.GetValues(typeof(Need)).Length; i++)
        {
            if (!((Need)Enum.GetValues(typeof(Need)).GetValue(i) == Need.NONE) && !((Need)Enum.GetValues(typeof(Need)).GetValue(i) == Need.HEALTH))
            {
                MaxAccLevels.Add((Need)Enum.GetValues(typeof(Need)).GetValue(i), R.Next(0, 100));
                CurrentLevels.Add((Need)Enum.GetValues(typeof(Need)).GetValue(i), 0);
                ActionGain.Add((Need)Enum.GetValues(typeof(Need)).GetValue(i), R.Next(0, 100));
                ItemGain.Add((Need)Enum.GetValues(typeof(Need)).GetValue(i), 0);
            }
        }
        TimeGain = 5f;
    }

    public void TimePasses()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Need)).Length; i++)
        {
            Need v = (Need)Enum.GetValues(typeof(Need)).GetValue(i);
            if (!(v == Need.NONE) && !(v == Need.HEALTH))
            {
                if (v == Need.LIBIDO && Age < 5)
                {
                    continue;
                }
                if (CurrentLevels[v] <= 100f || ActionGain[v] > 0f && ItemGain[v] > 0f)
                {
                    CurrentLevels[v] = CurrentLevels[v] + TimeGain - (ActionGain[v] * ItemGain[v]); //0f - if no item is used
                    CurrentLevels[v] = CurrentLevels[v] < 0f ? 0f : CurrentLevels[v];
                    CurrentLevels[v] = CurrentLevels[v] > 100f ? 100f : CurrentLevels[v];
                    ActionGain[v] = 0f;
                    ItemGain[v] = 0f;
                }
                if (CurrentLevels[v] >= MaxAccLevels[v] && v != Need.LIBIDO && v != Need.SLEEP && v != Need.TOILET)
                {
                    Pain(v);
                }
            }
        }
    }
    /// <summary>
    /// Inflicts pain based on needs
    /// </summary>
    /// <param name="n">The need causing pain.</param>
    public void Pain(Need n)
    {
        if (CurrentLevels[n] >= MaxAccLevels[n] && PainLevel < 100)
        {
            PainLevel += PainConstant; // HERE SOMETHING DOESNT FEEL OK
            if (Health >= 0f && PainLevel > 80f)
            {
                Health--;
            }
            if (Health < 0f)
            {
                Health = 0f;
            }
        }
    }
    /// <summary>
    /// Computes and returns the current need.
    /// </summary>
    /// <returns>Returns the need that affects the agent the most.</returns>
    public Need Saturation()
    {
        TimePasses();
        var sortedDict = from entry in CurrentLevels orderby entry.Value descending select entry;
        if (sortedDict.ToList()[0].Value > MaxAccLevels[sortedDict.ToList()[0].Key])
        {
            return sortedDict.ToList()[0].Key;
        }
        else
        {
            Health++;
            PainLevel--;
            return Need.NONE;
        }
    }

    /// <summary>
    /// Set gains of the current on-going action.
    /// </summary>
    /// <param name="Need">The need affected by the current action.</param>
    /// <param name="arg1">The item gain for the current need.</param>
    /// <param name="arg2">The action gain for the current need.</param>
    public void SetGains(Need Need, float arg1, float arg2)
    {
        if (Need == NPCController.Instance.CurrentNeed)
        {
            PainLevel -= (PainLevel <= 0f) ? 0f : 50f;
        }
        ItemGain[Need] = arg1;
        ActionGain[Need] = arg2;
    }


}
