using UnityEngine;

public class Ch2 : MonoBehaviour
{
    private Animator animator;

    public bool nearBox = false;  // kutu algılama için

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ISIRMA (sol click)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("bite");
        }

        // ZIPLAMA (space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
        }

        // EĞİLME (ctrl)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("crouch", true);

            // eğer kutuya yakınsa
            if (nearBox)
            {
                animator.SetBool("nearBox", true);
            }
            else
            {
                animator.SetBool("nearBox", false);
            }
        }
        else
        {
            animator.SetBool("crouch", false);
            animator.SetBool("nearBox", false);
        }
    }
}
