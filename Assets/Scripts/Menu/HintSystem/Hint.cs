using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator animator;
    [SerializeField]  private RectTransform rectTransform;

    int ID;

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetID(int ID)
    {
        this.ID = ID;
    }

    public int GetID()
    {
        return ID;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public RectTransform GetTransform()
    {
        return rectTransform;
    }

    public void HintHideAnimationEnded()
    {
        HintSystem.GetInstance().HintRemoveEnd(this);
    }
}
