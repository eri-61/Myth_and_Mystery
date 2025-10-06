using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TS.PageSlider
{
    public class PageDot : MonoBehaviour
    {
        #region Variables

        [Header("Configuration")]
        [SerializeField] private bool _useImageComponent;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;

        [Header("Events")]
        public UnityEvent<bool> OnActiveStateChanged;
        public UnityEvent<int> OnPressed;
        public bool IsActive { get; private set; }
        public int Index { get; set; }

        private Image _image;

        #endregion

        private void Awake()
        {
            if (_useImageComponent && !TryGetComponent(out _image))
            {
                Debug.LogError("No Image Component found");
            }
        }
        private void Start()
        {
            ChangeActiveState(IsActive);
        }

        public virtual void ChangeActiveState(bool active)
        {
            IsActive = active;

            if (_useImageComponent && _image != null)
            {
                _image.color = active ? _selectedColor : _defaultColor;
            }

            OnActiveStateChanged?.Invoke(active);
        }

        public void Press()
        {
            OnPressed?.Invoke(Index);
        }
    }
}