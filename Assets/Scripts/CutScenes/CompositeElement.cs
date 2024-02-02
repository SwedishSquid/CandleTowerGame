using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class CompositeElement : MonoBehaviour, ICutElement
{
    [SerializeField]
    public List<MonoBehaviour> ChildrenCutElements;

    private Lazy<IEnumerator<bool>> state;

    public void Awake()
    {
        state = new Lazy<IEnumerator<bool>>(() => GetShowEnumerable().GetEnumerator());
    }

    public bool TryShowNext()
    {
        if (state.Value.MoveNext())
        {
            return true;
        }
        return false;
    }

    private IEnumerable<bool> GetShowEnumerable()
    {
        var childrenElements = ChildrenCutElements
            .Where(e => e.TryGetComponent<ICutElement>(out _))
            .Select(e => e.GetComponent<ICutElement>());
        foreach (var child in childrenElements)
        {
            while (child.TryShowNext())
            {
                yield return true;
            }
        }
    }
}
