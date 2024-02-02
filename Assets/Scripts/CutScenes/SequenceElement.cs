using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SequenceElement : MonoBehaviour, ICutElement
{
    [SerializeField]
    public List<GameObject> Items;

    [SerializeField]
    public List<bool> ActivationValue;

    private Lazy<IEnumerator<bool>> state;

    public void Awake()
    {
        if (Items.Count != ActivationValue.Count)
        {
            throw new System.ArgumentException("Items count must be equal to activation values count");
        }
        state = new Lazy<IEnumerator<bool>>(() => GetShowEnumerable().GetEnumerator());
    }

    public bool TryShowNext()
    {
        return state.Value.MoveNext();
    }

    public IEnumerable<bool> GetShowEnumerable()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].SetActive(ActivationValue[i]);
            yield return true;
        }
    }
}
