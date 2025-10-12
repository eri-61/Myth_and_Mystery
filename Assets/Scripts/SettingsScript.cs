using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsScript : MonoBehaviour
{
    #region Variables
    [Header("Slider")]
    public Slider musicSlider;
    public Slider vfxSlider;
    public Slider textSpeedSlider;

    [Header("TMP_InputField")]
    public TMP_InputField musicInput;
    public TMP_InputField vfxInput;
    public TMP_InputField textSpeedInput;

    [Header("Text Speed")]
    public TextMeshProUGUI textSpeed;

    [Header("Button")]
    public Button applyChanges;
    public Button quit;
    public Button closeBtn;
    #endregion
}
