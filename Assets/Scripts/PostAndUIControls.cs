using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class PostAndUIControls : MonoBehaviour
{
	public PostProcessVolume postProcessVolume1;
	public PostProcessVolume postProcessVolume2;

	public Renderer voronoiRenderer1;
	public Renderer voronoiRenderer2;

	public Image hueBG;
	public Image saturationBG;
	public Image valueBG;

	public BubbleButton bubbleButton;
	public RectTransform bubbleRect;
	public Text panelText;

	public Screensaver screensaver;

	private PostProcessVolume activePostProcessVolume;
	private Renderer activeVoronoiRenderer;

	private Bloom bloom;
	private ColorGrading colorGrading;


	[Header("Bloom")]
	public InputField intensity;
	public InputField threshold;
	public Slider softKnee;
	public InputField softKneeValue;
	public Slider anamorphicRatio;
	public InputField anamorphicRatioValue;
	public Slider bloomH;
	public Slider bloomS;
	public Slider bloomV;

	[Header("Colour Grading")]
	public Slider temperature;
	public InputField temperatureValue;
	public Slider tint;
	public InputField tintValue;
	public Slider saturation;
	public InputField saturationValue;
	public Slider brightness;
	public InputField brightnessValue;
	public Slider contrast;
	public InputField contrastValue;
	public Slider channelMixerR;
	public InputField channelMixerRValue;
	public Slider channelMixerG;
	public InputField channelMixerGValue;
	public Slider channelMixerB;
	public InputField channelMixerBValue;

	[Header("UI Size")]
	public InputField bubbleFontSize;
	public InputField bubbleWidth;
	public InputField panelFontSize;
	public InputField voronoiScale;
	public InputField voronoiSpeed;

	public int bubbleFontSizeFloat { get; set; }
	public float bubbleWidthFloat { get; set; }

	public void ChangeIntensityValue()
	{
		if (!string.IsNullOrEmpty(intensity.text))
			bloom.intensity.Override(Convert.ToSingle(intensity.text));
	}

	public void ChangeThresholdValue()
	{
		if (!string.IsNullOrEmpty(threshold.text))
			bloom.threshold.Override(Convert.ToSingle(threshold.text));
	}

	public void ChangeSoftKneeValueSlider()
	{
		bloom.softKnee.Override(softKnee.value);
		softKneeValue.text = bloom.softKnee.value.ToString();
	}

	public void ChangeSoftKneeValueText()
	{
		if (!string.IsNullOrEmpty(softKneeValue.text))
		{
			bloom.softKnee.Override(Convert.ToSingle(softKneeValue.text));
			softKnee.SetValueWithoutNotify(bloom.softKnee.value);
		}
	}

	public void ChangeAnamorphicRatioValueSlider()
	{
		bloom.anamorphicRatio.Override(anamorphicRatio.value);
		anamorphicRatioValue.text = anamorphicRatio.value.ToString();
	}

	public void ChangeAnamorphicRatioValueText()
	{
		if (!string.IsNullOrEmpty(anamorphicRatioValue.text))
		{
			bloom.anamorphicRatio.Override(Convert.ToSingle(anamorphicRatioValue.text));
			anamorphicRatio.SetValueWithoutNotify(bloom.anamorphicRatio.value);
		}
	}

	public void ChangeBloomColourValue()
	{
		bloom.color.Override(Color.HSVToRGB(bloomH.value, bloomS.value, bloomV.value));

		hueBG.color = bloom.color.value;
		saturationBG.color = bloom.color.value;
		valueBG.color = bloom.color.value;
	}



	public void ChangeTemperatureValueSlider()
	{
		colorGrading.temperature.Override(temperature.value);
		temperatureValue.text = temperature.value.ToString();
	}

	public void ChangeTemperatureValueText()
	{
		if (!string.IsNullOrEmpty(temperatureValue.text))
		{
			colorGrading.temperature.Override(Convert.ToSingle(temperatureValue.text));
			temperature.SetValueWithoutNotify(colorGrading.temperature.value);
		}
	}

	public void ChangeTintValueSlider()
	{
		colorGrading.tint.Override(tint.value);
		tintValue.text = tint.value.ToString();
	}

	public void ChangeTintValueText()
	{
		if (!string.IsNullOrEmpty(tintValue.text))
		{
			colorGrading.tint.Override(Convert.ToSingle(tintValue.text));
			tint.SetValueWithoutNotify(colorGrading.tint.value);
		}
	}

	public void ChangeSaturationValueSlider()
	{
		colorGrading.saturation.Override(saturation.value);
		saturationValue.text = saturation.value.ToString();
	}

	public void ChangeSaturationValueText()
	{
		if (!string.IsNullOrEmpty(saturationValue.text))
		{
			colorGrading.saturation.Override(Convert.ToSingle(saturationValue.text));
			saturation.SetValueWithoutNotify(colorGrading.saturation.value);
		}
	}

	public void ChangeBrightnessValueSlider()
	{
		colorGrading.brightness.Override(brightness.value);
		brightnessValue.text = brightness.value.ToString();
	}

	public void ChangeBrightnessValueText()
	{
		if (!string.IsNullOrEmpty(brightnessValue.text))
		{
			colorGrading.brightness.Override(Convert.ToSingle(brightnessValue.text));
			brightness.SetValueWithoutNotify(colorGrading.brightness.value);
		}
	}

	public void ChangeContrastValueSlider()
	{
		colorGrading.contrast.Override(contrast.value);
		contrastValue.text = contrast.value.ToString();
	}

	public void ChangeContrastValueText()
	{
		if (!string.IsNullOrEmpty(contrastValue.text))
		{
			colorGrading.contrast.Override(Convert.ToSingle(contrastValue.text));
			contrast.SetValueWithoutNotify(colorGrading.contrast.value);
		}
	}

	public void ChangeChannelMixerRValueSlider()
	{
		colorGrading.mixerRedOutRedIn.Override(channelMixerR.value);
		channelMixerRValue.text = channelMixerR.value.ToString();
	}

	public void ChangeChannelMixerRValueText()
	{
		if (!string.IsNullOrEmpty(channelMixerRValue.text))
		{
			colorGrading.mixerRedOutRedIn.Override(Convert.ToSingle(channelMixerRValue.text));
			channelMixerR.SetValueWithoutNotify(colorGrading.mixerRedOutRedIn.value);
		}
	}

	public void ChangeChannelMixerGValueSlider()
	{
		colorGrading.mixerRedOutGreenIn.Override(channelMixerG.value);
		channelMixerGValue.text = channelMixerG.value.ToString();
	}

	public void ChangeChannelMixerGValueText()
	{
		if (!string.IsNullOrEmpty(channelMixerGValue.text))
		{
			colorGrading.mixerRedOutGreenIn.Override(Convert.ToSingle(channelMixerGValue.text));
			channelMixerG.SetValueWithoutNotify(colorGrading.mixerRedOutGreenIn.value);
		}
	}

	public void ChangeChannelMixerBValueSlider()
	{
		colorGrading.mixerRedOutBlueIn.Override(channelMixerB.value);
		channelMixerBValue.text = channelMixerB.value.ToString();
	}

	public void ChangeChannelMixerBValueText()
	{
		if (!string.IsNullOrEmpty(channelMixerBValue.text))
		{
			colorGrading.mixerRedOutBlueIn.Override(Convert.ToSingle(channelMixerBValue.text));
			channelMixerB.SetValueWithoutNotify(colorGrading.mixerRedOutBlueIn.value);
		}
	}



	public void ChangeBubbleFontSizeValue()
	{
		if (!string.IsNullOrEmpty(bubbleFontSize.text))
			bubbleFontSizeFloat = Convert.ToInt32(bubbleFontSize.text);
	}

	public void ChangeBubbleWidthValue()
	{
		if (!string.IsNullOrEmpty(bubbleWidth.text))
			bubbleWidthFloat = Convert.ToSingle(bubbleWidth.text);
	}

	public void ChangePanelFontSizeValue()
	{
		if (!string.IsNullOrEmpty(panelFontSize.text))
			panelText.fontSize = Convert.ToInt32(panelFontSize.text);
	}

	public void ChangeVoronoiScaleValue()
	{
		if (!string.IsNullOrEmpty(voronoiScale.text))
			activeVoronoiRenderer.material.SetFloat("_RippleScale", Convert.ToSingle(voronoiScale.text));
	}

	public void ChangeVoronoiSpeedValue()
	{
		if (!string.IsNullOrEmpty(voronoiSpeed.text))
			activeVoronoiRenderer.material.SetFloat("_RippleSpeed", Convert.ToSingle(voronoiSpeed.text));
	}

	// Start is called before the first frame update
	void Start()
	{
		screensaver = FindObjectOfType<Screensaver>();
		screensaver.switchEvent.AddListener(RefreshUI);

		RefreshUI();
	}

	public void RefreshUI()
	{
		if (screensaver.primaryCamera.activeSelf)
		{
			activePostProcessVolume = postProcessVolume1;
			activeVoronoiRenderer = voronoiRenderer1;
		}
		else if (screensaver.secondaryCamera.activeSelf)
		{
			activePostProcessVolume = postProcessVolume2;
			activeVoronoiRenderer = voronoiRenderer2;
		}

		bloom = activePostProcessVolume.profile.GetSetting<Bloom>();

		intensity.text = bloom.intensity.value.ToString();
		threshold.text = bloom.threshold.value.ToString();

		softKnee.value = bloom.softKnee.value;
		softKneeValue.text = bloom.softKnee.value.ToString();

		anamorphicRatio.value = bloom.anamorphicRatio.value;
		anamorphicRatioValue.text = bloom.anamorphicRatio.value.ToString();

		float[] hsv = new float[3];
		Color.RGBToHSV(bloom.color.value, out hsv[0], out hsv[1], out hsv[2]);
		bloomH.value = hsv[0];
		bloomS.value = hsv[1];
		bloomV.value = hsv[2];



		colorGrading = activePostProcessVolume.profile.GetSetting<ColorGrading>();

		temperature.value = colorGrading.temperature.value;
		temperatureValue.text = colorGrading.temperature.value.ToString();

		tint.value = colorGrading.tint.value;
		tintValue.text = colorGrading.tint.value.ToString();

		saturation.value = colorGrading.saturation.value;
		saturationValue.text = colorGrading.saturation.value.ToString();

		brightness.value = colorGrading.brightness.value;
		brightnessValue.text = colorGrading.brightness.value.ToString();

		contrast.value = colorGrading.contrast.value;
		contrastValue.text = colorGrading.contrast.value.ToString();

		channelMixerR.value = colorGrading.mixerRedOutRedIn.value;
		channelMixerRValue.text = colorGrading.mixerRedOutRedIn.value.ToString();

		channelMixerG.value = colorGrading.mixerRedOutGreenIn.value;
		channelMixerGValue.text = colorGrading.mixerRedOutGreenIn.value.ToString();

		channelMixerB.value = colorGrading.mixerRedOutBlueIn.value;
		channelMixerBValue.text = colorGrading.mixerRedOutBlueIn.value.ToString();



		bubbleFontSize.text = bubbleButton.text.fontSize.ToString();

		bubbleWidth.text = bubbleRect.sizeDelta.x.ToString();

		panelFontSize.text = panelText.fontSize.ToString();

		voronoiScale.text = activeVoronoiRenderer.material.GetFloat("_RippleScale").ToString();

		voronoiSpeed.text = activeVoronoiRenderer.material.GetFloat("_RippleSpeed").ToString();
	}
}
