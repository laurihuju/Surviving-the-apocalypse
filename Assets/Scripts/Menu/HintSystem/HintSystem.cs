using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintSystem : MonoBehaviour
{
    private static HintSystem instance;

    [SerializeField] private GameObject hintPrefab;
    [SerializeField] private Transform hintParent;
    [SerializeField] private float hintDistance;

    private List<Hint> hints;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        hints = new List<Hint>();
    }

    public Hint ShowHint(string text)
    {
        Hint newHint = Instantiate(hintPrefab, hintParent).GetComponent<Hint>();
        newHint.SetText(text);
        newHint.GetTransform().position = new Vector3(transform.position.x, transform.position.y + (newHint.GetTransform().sizeDelta.y + hintDistance) * hints.Count, newHint.GetTransform().position.z);

        hints.Add(newHint);

        return newHint;
    }

    public void RemoveHint(Hint hint)
    {
        hint.GetAnimator().SetTrigger("FadeOut");
    }

    public void HintRemoveEnd(Hint hint)
    {
        int hintIndex = hints.IndexOf(hint);

        hints.Remove(hint);
        Destroy(hint);

        for(int i = hintIndex; i < hints.Count; i++)
        {
            hints[i].GetTransform().position = new Vector3(transform.position.x, transform.position.y + (hints[i].GetTransform().sizeDelta.y + hintDistance) * i, hints[i].GetTransform().position.z);
        }
    }

    public static HintSystem GetInstance()
    {
        return instance;
    }
}
