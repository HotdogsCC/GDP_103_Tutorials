using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the same GameObject
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method to handle the Clap action started
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetBool("jumping", true);
        }
        else if (context.canceled)
        {
            animator.SetBool("jumping", false);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            animator.SetBool("running", true);
        }
        else if(context.canceled)
        {
            animator.SetBool("running", false);
        }
    }
}
