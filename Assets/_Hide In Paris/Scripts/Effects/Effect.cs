using UnityEngine;

namespace inkolorgames.effects
{
    public abstract class Effect : MonoBehaviour
    {
        [SerializeField] private bool doEffectOnEnable = false;

        protected virtual void OnEnable()
        {
            if (doEffectOnEnable)
                DoEffect();
        }
        public abstract void DoEffect();
    }
}