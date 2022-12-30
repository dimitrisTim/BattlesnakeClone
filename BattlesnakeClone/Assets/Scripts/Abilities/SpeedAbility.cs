using UnityEngine;

public class SpeedAbility : AbstractAbility
{
    public int Cooldown {get; private set;}
    private Snake snake;
    private float previousSpeed;

    private void Awake()
    {
        this.snake = GetComponent<Snake>();
        this.Cooldown = 3;
        this.Activate();
    }

    protected override void Activate()
    {
        previousSpeed = this.snake.GridMoveTimerMax;
        this.snake.GridMoveTimerMax = previousSpeed - 0.1f;   
        Invoke(nameof(Deactivate), Cooldown);
    } 

    protected override void Deactivate()
    {
        this.snake.GridMoveTimerMax = previousSpeed;
        Destroy(this);
    }
}
