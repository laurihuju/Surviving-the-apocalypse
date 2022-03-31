using UnityEngine;

namespace Menu
{
    public class MenuSystem : MonoBehaviour
    {
        private static MenuSystem instance;

        [SerializeField] private MenuPage[] pages;
        private int currentPage = -1;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Opens the menu page in the given index. If a menu page is already open, the method closes it first.
        /// </summary>
        /// <param name="index"></param>
        public void ShowPage(int index)
        {
            if (index >= pages.Length || index < 0)
                return;
            if (pages[index] == null)
                return;
            
            if (currentPage != -1)
            {
                pages[currentPage].Hide();
                currentPage = index;
                return;
            }

            currentPage = index;
            pages[index].Show();

            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Called by MenuPages when the page is hidden.
        /// </summary>
        internal void PageHidden()
        {
            if (currentPage == -1)
                return;
            pages[currentPage].Show();

            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Hides the menu system and closes open page.
        /// </summary>
        public void Hide()
        {
            if (currentPage == -1)
                return;
            pages[currentPage].Hide();
            currentPage = -1;

            Cursor.lockState = CursorLockMode.Locked;
        }

        public int GetOpenPage()
        {
            return currentPage;
        }

        /// <summary>
        /// Returns the singleton instance of MenuSystem.
        /// </summary>
        /// <returns></returns>
        public static MenuSystem GetInstance()
        {
            return instance;
        }
    }

}