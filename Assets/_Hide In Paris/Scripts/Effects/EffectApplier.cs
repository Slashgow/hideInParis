using System.Collections.Generic;
using UnityEngine;

namespace inkolorgames.effects
{
    public class EffectApplier : MonoBehaviour
    {
        [SerializeField] private List<Effect> effects = new List<Effect>();

        public virtual void DoEffects()
        {
            foreach (Effect effect in effects)
            {
                effect.DoEffect();
            }
        }

    }
}