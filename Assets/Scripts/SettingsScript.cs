using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    public static SettingsScript instance;
    #region Variables
    [Header ("Panel")]
    public GameObject settingsPanel;

    [Header("Slider")]
    public Slider musicSlider;     
    public Slider vfxSlider;       
    public Slider textSpeedSlider;

    [Header("TMP_InputField")]
    public TMP_InputField musicInput;
    public TMP_InputField vfxInput;
    public TMP_InputField textSpeedInput;

    [Header("Text Speed")]
    public TextMeshProUGUI textSpeedSample;

    [Header("Button")]
    public Button applyChanges;
    public Button quit;
    public Button closeBtn;
    #endregion

    Coroutine sampleCoroutine;
    bool suppressChangeEvents = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(CloseSettings);
        quit.onClick.AddListener(QuitGame);
        applyChanges.onClick.AddListener(ApplySettingsChanges);

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (vfxSlider != null) vfxSlider.onValueChanged.AddListener(OnVFXSliderChanged);
        if (textSpeedSlider != null) textSpeedSlider.onValueChanged.AddListener(OnTextSpeedSliderChanged);

        if (musicInput != null) musicInput.onEndEdit.AddListener(OnMusicInputEndEdit);
        if (vfxInput != null) vfxInput.onEndEdit.AddListener(OnVFXInputEndEdit);
        if (textSpeedInput != null) textSpeedInput.onEndEdit.AddListener(OnTextSpeedInputEndEdit);

        suppressChangeEvents = true;
        if (AudioManager.Instance != null)
        {
            if (musicSlider != null) musicSlider.value = AudioManager.Instance.GetMusicVolumePercent();
            if (vfxSlider != null) vfxSlider.value = AudioManager.Instance.GetVFXVolumePercent();

            if (musicInput != null) musicInput.text = Mathf.RoundToInt(AudioManager.Instance.GetMusicVolumePercent()).ToString();
            if (vfxInput != null) vfxInput.text = Mathf.RoundToInt(AudioManager.Instance.GetVFXVolumePercent()).ToString();
        }

        if (textSpeedSlider != null)
        {
            textSpeedInput.text = textSpeedSlider.value.ToString("0.###");
            UpdateTextSpeedSample(textSpeedSlider.value);
        }
        suppressChangeEvents = false;
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(CloseSettings);
        quit.onClick.RemoveListener(QuitGame);
        applyChanges.onClick.RemoveListener(ApplySettingsChanges);

        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (vfxSlider != null) vfxSlider.onValueChanged.RemoveListener(OnVFXSliderChanged);
        if (textSpeedSlider != null) textSpeedSlider.onValueChanged.RemoveListener(OnTextSpeedSliderChanged);

        if (musicInput != null) musicInput.onEndEdit.RemoveListener(OnMusicInputEndEdit);
        if (vfxInput != null) vfxInput.onEndEdit.RemoveListener(OnVFXInputEndEdit);
        if (textSpeedInput != null) textSpeedInput.onEndEdit.RemoveListener(OnTextSpeedInputEndEdit);
    }

    public void OpenSettings()
    {
        Time.timeScale = 0f;
        if (settingsPanel == null) return;

        settingsPanel.transform.SetAsLastSibling();

        var cg = settingsPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Time.timeScale = 1f;
        if (settingsPanel == null) return;

        settingsPanel.SetActive(false);
    }

    void QuitGame() => Application.Quit();

    void ApplySettingsChanges()
    {
        float musicPercent = musicSlider != null ? musicSlider.value : 100f;
        float vfxPercent = vfxSlider != null ? vfxSlider.value : 100f;
        float textSpeedValue = textSpeedSlider != null ? textSpeedSlider.value : 0.04f;

        if (musicInput != null) musicInput.text = Mathf.RoundToInt(musicPercent).ToString();
        if (vfxInput != null) vfxInput.text = Mathf.RoundToInt(vfxPercent).ToString();
        if (textSpeedInput != null) textSpeedInput.text = textSpeedValue.ToString("0.###");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolumePercent(musicPercent);
            AudioManager.Instance.SetVFXVolumePercent(vfxPercent);
        }

        if (DialogController.instance != null)
            DialogController.instance.SetTypingSpeed(textSpeedValue);

        CloseSettings();
    }

    #region Slider/Input 
    void OnMusicSliderChanged(float percent)
    {
        if (suppressChangeEvents) return;
        if (musicInput != null) musicInput.text = Mathf.RoundToInt(percent).ToString();
    }

    void OnVFXSliderChanged(float percent)
    {
        if (suppressChangeEvents) return;
        if (vfxInput != null) vfxInput.text = Mathf.RoundToInt(percent).ToString();
    }

    void OnTextSpeedSliderChanged(float v)
    {
        if (suppressChangeEvents) return;
        if (textSpeedInput != null) textSpeedInput.text = v.ToString("0.###");
        UpdateTextSpeedSample(v);
    }

    void OnMusicInputEndEdit(string s)
    {
        if (suppressChangeEvents) return;
        if (int.TryParse(s, out int v))
        {
            v = Mathf.Clamp(v, 0, 100);
            if (musicSlider != null) musicSlider.value = v;
            musicInput.text = v.ToString();
        }
    }

    void OnVFXInputEndEdit(string s)
    {
        if (suppressChangeEvents) return;
        if (int.TryParse(s, out int v))
        {
            v = Mathf.Clamp(v, 0, 100);
            if (vfxSlider != null) vfxSlider.value = v;
            vfxInput.text = v.ToString();
        }
    }

    void OnTextSpeedInputEndEdit(string s)
    {
        if (suppressChangeEvents) return;
        if (float.TryParse(s, out float v))
        {
            v = Mathf.Max(0f, v);
            if (textSpeedSlider != null) textSpeedSlider.value = v;
            UpdateTextSpeedSample(v);
        }
    }
    #endregion

    void UpdateTextSpeedSample(float speed)
    {
        if (textSpeedSample == null) return;

        if (sampleCoroutine != null) StopCoroutine(sampleCoroutine);
        sampleCoroutine = StartCoroutine(PlaySample(speed));
    }

    IEnumerator PlaySample(float speed)
    {
        string sample = "Sample typing speed...";
        textSpeedSample.text = "";
        float delay = Mathf.Max(0.001f, speed); 
        foreach (char c in sample)
        {
            textSpeedSample.text += c;
            yield return new WaitForSecondsRealtime(delay);
        }
        yield return new WaitForSecondsRealtime(0.5f);
    }
}
