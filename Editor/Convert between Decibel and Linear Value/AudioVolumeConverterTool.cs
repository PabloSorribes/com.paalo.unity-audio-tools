using Paalo.UnityAudioTools;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Paalo.UnityAudioTools.Editor
{
	public class AudioVolumeConverterTool : EditorWindow
	{
		#region ToolName and SetupWindow
		private const string toolName = "Audio Volume Converter Tool";

		[MenuItem(CurrentPackageConstants.packageRightClickMenuPath + toolName, false, CurrentPackageConstants.packageMenuIndexPosition)]
		public static void RightClickMenu()
		{
			SetupWindow();
		}

		[MenuItem(CurrentPackageConstants.packageWindowMenuPath + toolName, false, CurrentPackageConstants.packageMenuIndexPosition)]
		public static void ToolsMenu()
		{
			SetupWindow();
		}

		public static void SetupWindow()
		{
			//Instantiate the window and set its size.
			var window = GetWindow<AudioVolumeConverterTool>(utility: false, title: toolName, focus: true);
			window.minSize = new Vector2(400, 65);
			window.maxSize = new Vector2(window.minSize.x + 10, window.minSize.y + 10);
			window.Show();
		}
		#endregion ToolName and SetupWindow


		public void OnEnable()
		{
			var root = this.rootVisualElement;
			root.style.paddingTop = new StyleLength(10f);
			root.style.paddingBottom = new StyleLength(10f);
			root.style.paddingLeft = new StyleLength(10f);
			root.style.paddingRight = new StyleLength(10f);

			UnityEngine.UIElements.Toggle performanceToggle = new UnityEngine.UIElements.Toggle("Use approximation for performance: ");
			performanceToggle.value = false;
			root.Add(performanceToggle);

			UnityEditor.UIElements.FloatField decibelField = new UnityEditor.UIElements.FloatField("Decibel (dB)");
			root.Add(decibelField);

			UnityEditor.UIElements.FloatField amplitudeField = new UnityEditor.UIElements.FloatField("Normalized Value (0-1)");
			root.Add(amplitudeField);

			amplitudeField.value = AudioVolumeConverter.ConvertDecibelVolumeToLinearVolume(decibelField.value, performanceToggle.value);

			decibelField.RegisterCallback<ChangeEvent<float>>(evt =>
			{
				amplitudeField.value = AudioVolumeConverter.ConvertDecibelVolumeToLinearVolume(decibelField.value, performanceToggle.value);
				amplitudeField.value = Mathf.Clamp01(amplitudeField.value);
				decibelField.value = Mathf.Clamp(decibelField.value, -80f, 0f);
			});


			amplitudeField.RegisterCallback<ChangeEvent<float>>(evt =>
			{
				decibelField.value = AudioVolumeConverter.ConvertLinearVolumeToDecibelVolume(amplitudeField.value, performanceToggle.value);
				decibelField.value = Mathf.Clamp(decibelField.value, -80f, 0f);
				amplitudeField.value = Mathf.Clamp01(amplitudeField.value);
			});
		}
	}
}