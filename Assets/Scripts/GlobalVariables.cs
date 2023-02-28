using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static Color healthColor = new Color(1, 0.3098039f, 0.3098039f);

    public static string GetTimeStr(double seconds)
    {
        int minutes = (int)seconds / 60;
        int restOfSeconds = (int)seconds % 60;

        string minutesNonsignificantZero = "";
        string secondsNonsignificantZero = "";

        if (Math.Abs(minutes) < 10)
            minutesNonsignificantZero = "0";
        if (Math.Abs(restOfSeconds) < 10)
            secondsNonsignificantZero = "0";

        return minutesNonsignificantZero + minutes + ":" + secondsNonsignificantZero + restOfSeconds;
    }

    public class DelayClock
    {
        public double time;
        public double timer;

        public DelayClock(double time, double timer)
        {
            this.time = time;
            this.timer = timer;
        }
    }
    public delegate void DelayFunction();

    static private Dictionary<DelayFunction, DelayClock> delays = new Dictionary<DelayFunction, DelayClock>();
    
    public static void Delay(double seconds, DelayFunction delayFunction)
    {
        if (!delays.ContainsKey(delayFunction))
            delays.Add(delayFunction, new DelayClock(seconds, 0));
        else
        {
            DelayClock delayClock;
            delays.TryGetValue(delayFunction, out delayClock);

            if (delayClock != null)
            {
                delayClock.timer += Time.deltaTime;

                if (delayClock.timer >= delayClock.time)
                {
                    delayFunction();
                    delays.Remove(delayFunction);
                }
            }
        }     
    }
}
