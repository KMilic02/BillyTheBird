using UnityEngine;

public partial class Player : MonoBehaviour
{
    private Animator animator;
    private Player player;

    private float idleTimer;
    private float idleDelay;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();

        idleDelay = Random.Range(0f, 1f);
    }

    void UpdateAnimatorStates()
    {
        animator.SetBool("IsGrounded", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsGliding", false);
        animator.SetBool("IsRunning", false);

        if (player.playerState == State.Grounded)
        {
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsRunning", player.isRunning);
        }
        animator.SetBool("IsJumping", player.isJumping);
        animator.SetBool("IsGliding", player.isRealyGliding);
        Debug.Log("jump");
        Debug.Log(isJumping);

    }

    void HandleIdleRandom()
    {
        if (player.playerState != State.Grounded || player.isRunning)
            return;

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDelay)
        {
            animator.SetFloat("IdleRandom", Random.value);
            idleTimer = 0f;
            idleDelay = Random.Range(0f, 1f);
        }
    }
}
