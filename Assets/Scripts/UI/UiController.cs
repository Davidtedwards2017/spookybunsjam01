using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiController<T> : Singleton<T> where T : MonoBehaviour
{
    private bool _Active;
    public bool Active
    {
        get { return _Active; }
        set
        {
            if (_Active == value) return;

            _Active = value;

            var modules = GetComponentsInChildren<Module>(true);
            foreach (var module in modules)
            {
                module.Active = _Active;
            }         
        }
    }
}