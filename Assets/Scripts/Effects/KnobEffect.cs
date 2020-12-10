using UnityEngine;

public class KnobEffect : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    public void HitButton()
    {
        if (!animator) return;
        animator.SetTrigger("Hit");
    }

    public void TurnDirection(string triggerName)
    {
        if (!animator) return;
        animator.SetTrigger(triggerName);
    }
}