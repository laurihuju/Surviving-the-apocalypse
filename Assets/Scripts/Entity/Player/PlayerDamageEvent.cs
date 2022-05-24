using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageEvent : MonoBehaviour, DamageEvent
{
    [Header("Image")]
    [SerializeField] private Image redScreenImage;

    [Header("Settings")]
    [SerializeField] private AnimationCurve redScreenAlpha;
    [SerializeField] private float redScreenShowTime;

    private float timeShownScreen;

    private void Start()
    {
        timeShownScreen = -redScreenShowTime;
    }


    private void FixedUpdate()
    {
        if (Time.time - timeShownScreen > redScreenShowTime)
            return;

        Color newColor = redScreenImage.color;
        newColor.a = redScreenAlpha.Evaluate((Time.time - timeShownScreen) / redScreenShowTime);
        redScreenImage.color = newColor;
    }

    public void OnDamage()
    {
        timeShownScreen = Time.time;
    }
}
