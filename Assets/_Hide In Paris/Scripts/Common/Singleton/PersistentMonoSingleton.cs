using UnityEngine;

namespace inkolorgames
{
    /// <summary>
    /// This singleton is persistent across scenes by calling <see cref="Object.DontDestroyOnLoad(Object)"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PersistentMonoSingleton<T> : MonoSingleton<T> where T : MonoSingleton<T>
    {

        #region Protected Methods

        protected override void OnInitializing()
        {
            base.OnInitializing();
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }

        #endregion

    }
}