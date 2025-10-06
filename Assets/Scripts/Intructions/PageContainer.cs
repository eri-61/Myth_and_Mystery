using UnityEngine;

namespace TS.PageSlider
{
    public class PageContainer : MonoBehaviour
    {
        #region Variables

        [Header("Children")]
        [SerializeField] private PageView _page;

        #endregion

        public void AssignContent(RectTransform content)
        {
            if (content == null)
            {
                // Create a new GameObject with required components if content is not provided.
                var contentObject = new GameObject("Content", typeof(RectTransform), typeof(PageView));
                content = contentObject.GetComponent<RectTransform>();
            }

            content.SetParent(transform);

            content.anchorMin = Vector2.zero;
            content.anchorMax = Vector2.one;
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;
            content.anchoredPosition = Vector2.zero;

            content.localScale = Vector3.one;

            _page = content.GetComponent<PageView>();
        }

        public void ChangingToActiveState()
        {
            _page.ChangingToActiveState();
        }

        public void ChangingToInactiveState()
        {
            _page.ChangingToInactiveState();
        }

        public void ChangeActiveState(bool active)
        {
            _page.ChangeActiveState(active);
        }
    }

}