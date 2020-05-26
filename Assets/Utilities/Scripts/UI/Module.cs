using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Module : MonoBehaviour {

    protected CanvasGroup CanvasGroup;
    private bool _Active;

    public bool Active
    {
        get { return _Active; }
        set
        {
            if (_Active == value) return;

            _Active = value;

            if (_Active)
            {
                OnActivated();
            }
            else
            {
                OnDeactivated();
            }
        }
    }


    protected virtual void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void OnActivated()
    {
        AnimateIn().Play();
    }

    protected virtual void OnDeactivated()
    {
        AnimateOut().Play();
    }

    public Sequence AnimateIn()
    {
        Sequence sequence = DOTween.Sequence();

        if(CanvasGroup != null)
        {
            sequence.Append(CanvasGroup.DOFade(1f, .3f));
        }
        return sequence;
    }
    
    public Sequence AnimateOut()
    {
        Sequence sequence = DOTween.Sequence();
        if (CanvasGroup != null)
        {
            sequence.Append(CanvasGroup.DOFade(0f, .3f));
        }
        return sequence;
    }

    public void SetPosition(Vector2 position)
    {
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = position;
    }
}
