using UnityEngine;

public partial class Player : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();
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

}
