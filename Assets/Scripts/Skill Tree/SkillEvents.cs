using System;
using UnityEngine;

public static class SkillEvents
{
    public static event Action OnDoubleDashSkillUnlocked;
    public static void UnlockDoubleDash() => OnDoubleDashSkillUnlocked?.Invoke();

    public static event Action OnCounterSkillUnlocked;
    public static void UnlockCounter() => OnCounterSkillUnlocked?.Invoke();

}
