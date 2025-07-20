using System;
using UnityEngine;

public static class SkillEvents
{
    //
    public static event Action OnDoubleDashSkillUnlocked;
    public static void UnlockDoubleDash() => OnDoubleDashSkillUnlocked?.Invoke();
    //

    //
    public static event Action OnCounterSkillUnlocked;
    public static void UnlockCounter() => OnCounterSkillUnlocked?.Invoke();
    //

    //
    public static event Action OnPerfectTimingSkillUnlocked;
    public static void UnlockPerfectTiming() => OnPerfectTimingSkillUnlocked?.Invoke();
    //

    //
    public static event Action OnEchoingSteelSkillUnlocked;
    public static void UnlockEchoingSteel() => OnEchoingSteelSkillUnlocked?.Invoke();
    //

    //
    public static event Action OnMomentumShiftSkillUnlocked;
    public static void UnlockMomentumShift() => OnMomentumShiftSkillUnlocked?.Invoke();
    //

    //
    public static event Action OnCounterSurgeSkillUnlocked;
    public static void UnlockCounterSurge() => OnCounterSurgeSkillUnlocked?.Invoke();
    //

    //
    public static event Action OnBladeReflectionSkillUnlocked;
    public static void UnlockBladeReflection() => OnBladeReflectionSkillUnlocked?.Invoke();
    //

}
