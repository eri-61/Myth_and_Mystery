using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace TS.PageSlider
{
    public class PageDotsIndicator : MonoBehaviour
    {
        #region Variables

        [Header("References")]
        [SerializeField] private PageDot _prefab;

        [Header("Children")]
        [SerializeField] private List<PageDot> _dots;

        [Header("Events")]
        public UnityEvent<int> OnDotPressed;


        public bool IsVisible
        {
            get { return gameObject.activeInHierarchy; }
            set { gameObject.SetActive(value); }
        }

        #endregion

        private void Awake()
        {
            if (_dots.Count == 0) return;
            for (int i = 0; i < _dots.Count; i++)
            {
                _dots[i].ChangeActiveState(i == 0);
            }
        }

        public void Add()
        {
            PageDot dot = null;

            if (dot == null)
            {
                dot = Instantiate(_prefab, transform);
            }

            dot.Index = _dots.Count;
            dot.ChangeActiveState(_dots.Count == 0);

            _dots.Add(dot);
        }

        public void Clear()
        {
            for (int i = 0; i < _dots.Count; i++)
            {
                if (_dots[i] == null) { continue; }
            }

            _dots.Clear();
        }


        public void ChangeActiveDot(int fromIndex, int toIndex)
        {
            _dots[fromIndex].ChangeActiveState(false);
            _dots[toIndex].ChangeActiveState(true);
        }

    }
}