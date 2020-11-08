using UnityEngine;

public class KnobEffect : MonoBehaviour
{
    // Cached component
    private Animator animator;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        animator = this.GetComponent<Animator>();
    }

    //--------------------------------------------------------------------------------//

    public void HitButton ()
    {
        animator.SetTrigger ("Hit");
    }

    public void TurnDirection (string triggerName)
    {
        animator.SetTrigger (triggerName);
    }
}