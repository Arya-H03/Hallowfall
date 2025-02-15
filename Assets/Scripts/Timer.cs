public class Timer
{
    private float remainingTime;
    private bool isActive = false;

    public float RemainingTime { get => remainingTime; set => remainingTime = value; }
    public bool IsActive { get => remainingTime > 0;}

    public Timer()
    {
        remainingTime = 0;
    }
}