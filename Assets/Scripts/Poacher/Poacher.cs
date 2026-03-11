using UnityEngine;
using System.Collections;


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
    
    //start dir
    [SerializeField] private Direction startingDirection = Direction.Right;
    private Direction currentDirection;
    
    //for pangolin check
    [SerializeField] private Transform pangolin;
    [SerializeField] private PlayerMovement player;
    
    //flashlight sprites
    [SerializeField] private GameObject lightRight;
    [SerializeField] private GameObject lightLeft;
    [SerializeField] private GameObject lightUp;
    [SerializeField] private GameObject lightDown;

    private void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        currentDirection = startingDirection;
        UpdateIdleAnimation();
        UpdateLight();
    }
    private void UpdateLight()
    {
        lightRight.SetActive(currentDirection == Direction.Right);
        lightLeft.SetActive(currentDirection == Direction.Left);
        lightUp.SetActive(currentDirection == Direction.Up);
        lightDown.SetActive(currentDirection == Direction.Down);
    }
    
    private void CheckForPangolin()
    {
        Vector2 poacherPos = transform.position;
        Vector2 playerPos = pangolin.position;

        // convert world positions to grid coordinates
        int px = Mathf.RoundToInt(playerPos.x);
        int py = Mathf.RoundToInt(playerPos.y);

        int ox = Mathf.RoundToInt(poacherPos.x);
        int oy = Mathf.RoundToInt(poacherPos.y);

        bool caught = false;

        switch (currentDirection)
        {
            case Direction.Right:
                caught = (py == oy) && (px > ox);
                break;

            case Direction.Left:
                caught = (py == oy) && (px < ox);
                break;

            case Direction.Up:
                caught = (px == ox) && (py > oy);
                break;

            case Direction.Down:
                caught = (px == ox) && (py < oy);
                break;
        }

        if (caught)
        {
            Debug.Log("player caught");
            CatchPlayer();
        }
    }
    
    private void CatchPlayer()
    {
        animator.Play(currentDirection == Direction.Right 
            ? "PoacherAlertedRight" 
            : "PoacherAlertedLeft");

        player.animator.Play("ScaredSide");                 //idk if this works

        StartCoroutine(RestartLevelDelay());
    }
    private IEnumerator RestartLevelDelay()
    {
        yield return new WaitForSeconds(1.5f);
        LevelManager.Instance.ResetLevel();
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
        UpdateLight();
    }
    // Called at the end of the turn animation using an Animation Event
    public void OnTurnAnimationFinished()
    {
        UpdateIdleAnimation();
        //WaitABitAfterTurning();
        CheckForPangolin();
    }
    private IEnumerator WaitABitAfterTurning()
    {
        yield return new WaitForSeconds(0.1f);
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
    
    public void ResetPoacher()
    {
        currentDirection = startingDirection;
        UpdateIdleAnimation();
    }

}
