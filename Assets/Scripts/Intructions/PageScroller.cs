using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TS.PageSlider
{
    public class PageScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        #region Variables
        [Header("Configuration")]
        [SerializeField] private float _minDeltaDrag = 0.1f;
        [SerializeField] private float _snapDuration = 0.3f;

        [Header("Events")]
        public UnityEvent<int, int> OnPageChangeStarted;
        public UnityEvent<int, int> OnPageChangeEnded;
        public Rect Rect
        {
            get
            {
                return ((RectTransform)_scrollRect.transform).rect;
            }
        }

        public RectTransform Content
        {
            get
            {
                return _scrollRect.content;
            }
        }

        private ScrollRect _scrollRect;

        private int _currentPage; 
        private int _targetPage; 

        private float _startNormalizedPosition; 
        private float _targetNormalizedPosition;
        private float _moveSpeed; 

        #endregion

        private void Awake()
        {
            _scrollRect = FindScrollRect();
        }

        private void Update()
        {
            if (_moveSpeed == 0) { return; }

            var position = _scrollRect.horizontalNormalizedPosition;
            position += _moveSpeed * Time.deltaTime;

            var min = _moveSpeed > 0 ? position : _targetNormalizedPosition;
            var max = _moveSpeed > 0 ? _targetNormalizedPosition : position;
            position = Mathf.Clamp(position, min, max);

            _scrollRect.horizontalNormalizedPosition = position;

            if (Mathf.Abs(_targetNormalizedPosition - position) < Mathf.Epsilon)
            {
                _moveSpeed = 0;

                OnPageChangeEnded?.Invoke(_currentPage, _targetPage);

                _currentPage = _targetPage;
            }
        }

        public void SetPage(int index)
        {
            _scrollRect.horizontalNormalizedPosition = GetTargetPagePosition(index);

            _targetPage = index;
            _currentPage = index;
            OnPageChangeEnded?.Invoke(0, _currentPage);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startNormalizedPosition = _scrollRect.horizontalNormalizedPosition;

            if (_targetPage != _currentPage)
            {
                OnPageChangeEnded?.Invoke(_currentPage, _targetPage);

                _currentPage = _targetPage;
            }

            _moveSpeed = 0;
        }
        public void OnEndDrag(PointerEventData eventData)
        {

            var pageWidth = 1f / GetPageCount();

            var pagePosition = _currentPage * pageWidth;

            var currentPosition = _scrollRect.horizontalNormalizedPosition;

            var minPageDrag = pageWidth * _minDeltaDrag;

            var isForwardDrag = _scrollRect.horizontalNormalizedPosition > _startNormalizedPosition;

            var switchPageBreakpoint = pagePosition + (isForwardDrag ? minPageDrag : -minPageDrag);

            var page = _currentPage;
            if (isForwardDrag && currentPosition > switchPageBreakpoint)
            {
                page++;
            }
            else if (!isForwardDrag && currentPosition < switchPageBreakpoint)
            {
                page--;
            }

            ScrollToPage(page);
        }

        private void ScrollToPage(int page)
        {
            _targetNormalizedPosition = GetTargetPagePosition(page);

            _moveSpeed = (_targetNormalizedPosition - _scrollRect.horizontalNormalizedPosition) / _snapDuration;

            _targetPage = page;

            if (_targetPage != _currentPage)
            {
                OnPageChangeStarted?.Invoke(_currentPage, _targetPage);
            }
        }

        private int GetPageCount()
        {
            var contentWidth = _scrollRect.content.rect.width;
            var rectWidth = ((RectTransform)_scrollRect.transform).rect.size.x;
            return Mathf.RoundToInt(contentWidth / rectWidth) - 1;
        }

        private float GetTargetPagePosition(int page)
        {
            return page * (1f / GetPageCount());
        }

        private ScrollRect FindScrollRect()
        {
            var scrollRect = GetComponentInChildren<ScrollRect>();

            return scrollRect;
        }
    }
}