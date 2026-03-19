using NaughtyAttributes;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public enum Animation
    {
        IDLE,
        JUMP,
        RUN
    }

    [Header("Reference")]
    [SerializeField] private Animator animator;

    [Header("Animations")]
    [SerializeField] private bool useRandomAnimation = false;
    [SerializeField, HideIf("useRandomAnimation")] private Animation startingAnimation;

    private readonly int JUMP_HASH = Animator.StringToHash("jump");
    private readonly int RUN_HASH = Animator.StringToHash("run");

    private void Start()
    {
        if (useRandomAnimation)
        {
            Animation animation = ChooseRandomAnimation();
            SetAnimation(animation);

            return;
        }

        SetAnimation(startingAnimation);
    }

    private void SetAnimation(Animation animation)
    {
        switch (animation)
        {
            case Animation.IDLE:
                break;
            case Animation.JUMP:
                animator.SetTrigger(JUMP_HASH);
                break;
            case Animation.RUN:
                animator.SetTrigger(RUN_HASH);
                break;
            default:
                break;
        }
    }

    private Animation ChooseRandomAnimation()
    {
        int index = Random.Range(0, 3);
        if(index == 1)
            return Animation.JUMP;
        else if(index == 2)
            return Animation.RUN;
        return Animation.IDLE;
    }
}
