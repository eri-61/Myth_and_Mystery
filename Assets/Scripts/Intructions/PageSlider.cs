using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;


namespace TS.PageSlider
{
    public class PageSlider : MonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] private PageDotsIndicator _dotsIndicator;

        [Header("Children")]
        [SerializeField] private List<PageContainer> _pages;

        [Header("Configuration")]
        [SerializeField] private int _startPageIndex;

        [Header("Events")]
        public UnityEvent<PageContainer> OnPageChanged;

        public Rect Rect { get { return ((RectTransform)transform).rect; } }
        private PageScroller _scroller;

        #endregion

        private void Awake()
        {
            _scroller = FindScroller();
        }

        private IEnumerator Start()
        {
            _scroller.OnPageChangeStarted.AddListener(PageScroller_PageChangeStarted);
            _scroller.OnPageChangeEnded.AddListener(PageScroller_PageChangeEnded);

            yield return new WaitForEndOfFrame();

            if (_startPageIndex == 0) yield break;
            _scroller.SetPage(_startPageIndex);
        }

        public void AddPage(RectTransform content)
        {
            if (_scroller == null)
            {
                _scroller = FindScroller();
            }

            _pages ??= new List<PageContainer>();

            var page = new GameObject(string.Format("Page_{0}", _pages.Count), typeof(RectTransform), typeof(PageContainer));
            page.transform.SetParent(_scroller.Content);

            var rectTransform = page.GetComponent<RectTransform>();
            rectTransform.sizeDelta = _scroller.Rect.size;
            rectTransform.localScale = Vector3.one;

            var pageView = page.GetComponent<PageContainer>();
            pageView.AssignContent(content);

            if (_pages.Count == 0)
            {
                pageView.ChangingToActiveState();
                pageView.ChangeActiveState(true);
            }

            _pages.Add(pageView);

            if (_dotsIndicator != null)
            {
                _dotsIndicator.Add();
                _dotsIndicator.IsVisible = _pages.Count > 1;
            }
        }
        public void Clear()
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                if (_pages[i] == null) { continue; }
            }
            _pages.Clear();

            if (_dotsIndicator != null)
            {
                _dotsIndicator.Clear();
            }
        }

        private void PageScroller_PageChangeStarted(int fromIndex, int toIndex)
        {
            _pages[fromIndex].ChangingToInactiveState();
            _pages[toIndex].ChangingToActiveState();
        }
        private void PageScroller_PageChangeEnded(int fromIndex, int toIndex)
        {
            _pages[fromIndex].ChangeActiveState(false);
            _pages[toIndex].ChangeActiveState(true);

            if (_dotsIndicator != null)
            {
                _dotsIndicator.ChangeActiveDot(fromIndex, toIndex);
            }

            OnPageChanged?.Invoke(_pages[toIndex]);
        }

        private PageScroller FindScroller()
        {
            var scroller = GetComponentInChildren<PageScroller>();

            return scroller;
        }
    }

}