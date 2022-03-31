using UnityEngine;

namespace Menu
{
    public class MenuPage : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Activates the menu page and shows the fade in animation.
        /// </summary>
        internal void Show()
        {
            gameObject.SetActive(true);
            anim.ResetTrigger("FadeOut");
            anim.SetTrigger("FadeIn");
        }

        /// <summary>
        /// Shows the fade out animation.
        /// </summary>
        internal void Hide()
        {
            anim.ResetTrigger("FadeIn");
            anim.SetTrigger("FadeOut");
        }

        /// <summary>
        /// Hides the page and calls MenuSystem's PageHidden() method. Called when the fade out animation is complete.
        /// </summary>
        public void HideAnimationEnded()
        {
            gameObject.SetActive(false);
            MenuSystem.GetInstance().PageHidden();
        }
    }
}