using UnityEngine;

public partial class Player : MonoBehaviour
{
    Animator animator;

    float idleTimer;
    float idleDelay;

    void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();

        idleDelay = Random.Range(0f, 1f);
    }

    void UpdateAnimatorStates()
    {
        animator.SetBool("IsGrounded", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsGliding", false);
        animator.SetBool("IsRunning", false);

        if (playerState == State.Grounded)
        {
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsRunning", isRunning);
        }
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsGliding", isRealyGliding);
    }

    void HandleIdleRandom()
    {
        if (playerState != State.Grounded || isRunning)
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
