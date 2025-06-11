using System;
using UnityEngine;

public static class SkillEvents
{
    public static event Action OnDoubleDashSkillUnlocked;
    public static void UnlockDoubleDash() => OnDoubleDashSkillUnlocked?.Invoke();   

}
