using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicElement : MonoBehaviour, ICutElement
{
    [SerializeField]
    public List<GameObject> ToActivate;

    [SerializeField]
    public List<GameObject> ToDeactivate;

    private Lazy<IEnumerator<bool>> state;

    public void Awake()
    {
        state = new Lazy<IEnumerator<bool>>(() => GetShowEnumerable().GetEnumerator());
    }

    public bool TryShowNext()
    {
        return state.Value.MoveNext();
    }

    public IEnumerable<bool> GetShowEnumerable()
    {
        foreach (var item in ToActivate)
        {
            item.SetActive(true);
        }
        yield return true;

        foreach (var item in ToDeactivate)
        {
            item.gameObject.SetActive(false);
        }
    }
}
