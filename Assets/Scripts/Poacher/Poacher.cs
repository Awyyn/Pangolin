using UnityEngine;

/*

Int → Facing
0 = Right
1 = Up
2 = Left
3 = Down

From Turn → Idle states
4 transitions:
To Idle_Right when Facing == 0
To Idle_Up when Facing == 1
To Idle_Left when Facing == 2
To Idle_Down when Facing == 3

*/

public class Poacher : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private Direction currentDirection = Direction.Right;

    private void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        UpdateIdleAnimation();
    }


    // This will be called whenever the PLAYER moves
    public void RotateCounterClockwise()
    {
        currentDirection = GetNextDirectionCCW(currentDirection);
        PlayTurnAnimation(currentDirection);
    }

    private Direction GetNextDirectionCCW(Direction dir)
    {
        switch (dir)
        {
            case Direction.Right: return Direction.Up;
            case Direction.Up:    return Direction.Left;
            case Direction.Left:  return Direction.Down;
            case Direction.Down:  return Direction.Right;
            default:              return Direction.Right;
        }
    }

    private void PlayTurnAnimation(Direction newDir)
    {
        currentDirection = newDir;
        animator.SetInteger("Facing", (int)newDir);

        // Force restart of the correct turn animation
        switch (newDir)
        {
            case Direction.Right:
                animator.Play("PoacherTurnRight", 0, 0f);
                break;
            case Direction.Up:
                animator.Play("PoacherTurnUp", 0, 0f);
                break;
            case Direction.Left:
                animator.Play("PoacherTurnLeft", 0, 0f);
                break;
            case Direction.Down:
                animator.Play("PoacherTurnDown", 0, 0f);
                break;
        }

        animator.Update(0f); // apply immediately this frame
    }



    // Called at the end of the turn animation using an Animation Event
    public void OnTurnAnimationFinished()
    {
        UpdateIdleAnimation();
    }

    private void UpdateIdleAnimation()
    {
        animator.SetInteger("Facing", (int)currentDirection);
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerStepComplete += RotateCounterClockwise;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerStepComplete -= RotateCounterClockwise;
    }

}
