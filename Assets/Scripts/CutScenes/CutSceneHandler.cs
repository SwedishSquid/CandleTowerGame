using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutSceneHandler : MonoBehaviour
{
    [SerializeField]
    public List<MonoBehaviour> cutElements;

    private List<ICutElement> _cutElements;

    public IEnumerator<bool> showState;

    public void Start()
    {
        _cutElements = cutElements
            .Where(e => e.TryGetComponent<ICutElement>(out _))
            .Select(e => e.GetComponent<ICutElement>())
            .ToList();

        Debug.Log($"_cutElements.Count = {_cutElements.Count}");
        Debug.Log($"cutElements.Count = {cutElements.Count}");

        showState = GetShowAttemts().GetEnumerator();
    }

    public bool TryShowNext()
    {
        return showState.MoveNext();
    }

    public IEnumerable<bool> GetShowAttemts()
    {
        foreach (var el in _cutElements)
        {
            while (el.TryShowNext())
            {
                yield return true;
            }
        }
    }
}
