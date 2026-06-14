using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ACOSettingsPanelUI : MonoBehaviour
{
    [SerializeField] private Transform ACOSettingsPanel;
    [SerializeField] private Transform activePositionTransform;
    [SerializeField] private Transform disabledPositionTransform;

    [SerializeField] private GameObject informationPanel;

    [SerializeField] private GameObject downButton;
    [SerializeField] private GameObject upButton;

    [Header("Alpha Bölümü")]
    [SerializeField] private Slider alphaSlider;
    [SerializeField] private TextMeshProUGUI alphaText;
    private float alpha;

    [Header("Beta Bölümü")]
    [SerializeField] private Slider betaSlider;
    [SerializeField] private TextMeshProUGUI betaText;
    private float beta;

    [Header("StartPheromoneLevel Bölümü")]
    [SerializeField] private Slider startPheromoneLevelSlider;
    [SerializeField] private TextMeshProUGUI startPheromoneLevelText;
    private float startPheromoneLevel;

    [Header("EvaporationRate Bölümü")]
    [SerializeField] private Slider evaporationRateSlider;
    [SerializeField] private TextMeshProUGUI evaporationRateText;
    private float evaporationRate;

    [Header("MinPheromoneLevel Bölümü")]
    [SerializeField] private Slider minPheromoneLevelSlider;
    [SerializeField] private TextMeshProUGUI minPheromoneLevelText;
    private float minPheromoneLevel;

    [Header("Q Bölümü")]
    [SerializeField] private Slider qSlider;
    [SerializeField] private TextMeshProUGUI qText;
    private int qValue;

    private Tween panelTween;
    private float windowAnimationSpeed = .6f;


    private void Start() {
        GetInitialSettings();

        VisualManager.Instance.OnExitedSelectionMode += Instance_OnExitedSelectionMode;

        //HideACOSettingsPanel();
        //HideInformationPanel();
    }

    private void OnDestroy() {
        VisualManager.Instance.OnExitedSelectionMode -= Instance_OnExitedSelectionMode;
    }

    private void Instance_OnExitedSelectionMode(object sender, System.EventArgs e) {
        HideACOSettingsPanel();
    }

    //ACOManager'da tanýmlý deðerleri alýr
    private void GetInitialSettings() {
        //alpha
        alpha = ACOManager.Instance.GetAlpha();
        alphaSlider.SetValueWithoutNotify(alpha / 0.25f);
        alphaText.text = alpha.ToString("F2");

        //beta
        beta = ACOManager.Instance.GetBeta();
        betaSlider.SetValueWithoutNotify(beta / 0.25f);
        betaText.text = beta.ToString("F2");

        //startPheromoneLevel
        startPheromoneLevel = ACOManager.Instance.GetStartPheromoneLevel();
        startPheromoneLevelSlider.SetValueWithoutNotify(startPheromoneLevel / 0.05f);
        startPheromoneLevelText.text = startPheromoneLevel.ToString("F2");

        //evaporationRate
        evaporationRate = ACOManager.Instance.GetEvaporationRate();
        evaporationRateSlider.SetValueWithoutNotify(evaporationRate / 0.05f);
        evaporationRateText.text = evaporationRate.ToString("F2");

        //minPheromoneLevel
        minPheromoneLevel = ACOManager.Instance.GetMinPheromoneLevel();
        minPheromoneLevelSlider.SetValueWithoutNotify(minPheromoneLevel / 0.05f);
        minPheromoneLevelText.text = minPheromoneLevel.ToString("F2");

        //Q
        qValue = (int)ACOManager.Instance.GetQ();
        qSlider.SetValueWithoutNotify(qValue);
        qText.text = qValue.ToString();
    }

    public void ShowACOSettingsPanel() {
        downButton.SetActive(false);
        upButton.SetActive(true);

        panelTween?.Kill(); 

        ACOSettingsPanel.gameObject.SetActive(true);

        panelTween = transform.DOMoveY(activePositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
            });

        AudioManager.Instance.PlayButtonClickSound();
    }

    public void HideACOSettingsPanel() {
        upButton.SetActive(false);
        downButton.SetActive(true);

        panelTween?.Kill();

        panelTween = transform.DOMoveY(disabledPositionTransform.position.y, windowAnimationSpeed)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => {
                ACOSettingsPanel.gameObject.SetActive(false);
                //settingsButton.enabled = true;
            });

        AudioManager.Instance.PlayButtonBackClickSound();
    }

    public void ShowInformationPanel() {
        informationPanel.SetActive(true);
        AudioManager.Instance.PlayButtonClickSound();
    }

    public void HideInformationPanel() {
        informationPanel.SetActive(false);
        AudioManager.Instance.PlayButtonBackClickSound();
    }

    public void UpdateAlpha() {
        alpha = alphaSlider.value * 0.25f;
        alphaText.text = alpha.ToString("F2");

        ACOManager.Instance.SetAlpha(alpha);
    }

    public void UpdateBeta() {
        beta = betaSlider.value * 0.25f;
        betaText.text = beta.ToString("F2");

        ACOManager.Instance.SetBeta(beta);
    }

    public void UpdateStartPheromoneLevel() {
        startPheromoneLevel = startPheromoneLevelSlider.value * 0.05f;
        startPheromoneLevelText.text = startPheromoneLevel.ToString("F2");

        ACOManager.Instance.SetStartPheromoneLevel(startPheromoneLevel);
    }

    public void UpdateEvaporationRate() {
        evaporationRate = evaporationRateSlider.value * 0.05f;
        evaporationRateText.text = evaporationRate.ToString("F2");

        ACOManager.Instance.SetEvaporationRate(evaporationRate);
    }

    public void UpdateMinPheromoneLevel() {
        minPheromoneLevel = minPheromoneLevelSlider.value * 0.05f;
        minPheromoneLevelText.text = minPheromoneLevel.ToString("F2");

        ACOManager.Instance.SetMinPheromoneLevel(minPheromoneLevel);
    }

    public void UpdateQ() {
        qValue = (int)qSlider.value;
        qText.text = qValue.ToString();

        ACOManager.Instance.SetQ(qValue);
    }
}
