using System.Collections.Generic;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation.Animation
{
    internal delegate float CalcDeltaTime(float deltaTime);

    internal class UpdateDispatcher : MonoBehaviour
    {
        private static UpdateDispatcher _instance;
        private readonly Dictionary<IUpdatable, CalcDeltaTime> _targets = new Dictionary<IUpdatable, CalcDeltaTime>();

        public static UpdateDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObject = new GameObject(nameof(UpdateDispatcher));
                    var component = gameObject.AddComponent<UpdateDispatcher>();
                    DontDestroyOnLoad(gameObject);
                    _instance = component;
                }

                return _instance;
            }
        }

        private void Update()
        {
            foreach (var target in _targets)
            {
                var deltaTime = target.Value?.Invoke(Time.deltaTime) ?? Time.deltaTime;
                target.Key.Update(deltaTime);
            }
        }

        public void Register(IUpdatable target, CalcDeltaTime calcDeltaTime = null)
        {
            _targets.Add(target, calcDeltaTime);
        }

        public void Unregister(IUpdatable target)
        {
            _targets.Remove(target);
        }
    }
}