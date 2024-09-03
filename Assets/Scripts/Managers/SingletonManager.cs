using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Managers
{
    public class SingletonManager <T>: MonoBehaviour where T:SingletonManager<T>
    {
        #region Fields

        private static volatile T _Instance = null;

        #endregion
    
        #region Properties

        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindFirstObjectByType(typeof(T)) as T;
                }

                return _Instance;
            }
            
        }
        
        #endregion
    }
}

