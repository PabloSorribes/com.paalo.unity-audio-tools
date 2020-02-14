// Source: "FPSSample"-project by Unity, in the SoundSystem-class
// https://github.com/Unity-Technologies/FPSSample/blob/master/Assets/Scripts/Audio/SoundSystem.cs

using UnityEngine;

namespace Paalo.UnityAudioTools
{
	/// <summary>
	/// Helper class for helping to input Sound Designer-friendly Decibel values, 
	/// which get converted into Parametric values (0-1f), which are better for Unity's built in Audio Classes.
	/// Also contains a function for converting from Parametric to Decibel.
	/// </summary>
	public static class AudioVolumeConverter
	{
		public static readonly float SOUND_DB_CUTOFF = -60.0f;
		public static readonly float SOUND_LINEAR_CUTOFF = Mathf.Pow(2.0f, SOUND_DB_CUTOFF / 6.02f);

		/// <summary>
		/// Returns a decibel value based on a parametric value (0-1f).
		/// </summary>
		/// <param name="linearVolume"></param>
		/// <returns></returns>
		public static float ConvertLinearVolumeToDecibelVolume(float linearVolume, bool usePerformanceApproximation = true)
		{
			float dBVolume = 0f;

			if (usePerformanceApproximation)
			{
				if (linearVolume < SOUND_LINEAR_CUTOFF)
					return -60.0f;

				dBVolume = 6.02f * Mathf.Log(linearVolume) / Mathf.Log(2.0f);
			}
			else
			{
				dBVolume = 20f * Mathf.Log10(linearVolume);
			}
			return dBVolume;
		}

		/// <summary>
		/// Returns the parametric value (0-1f) corresponding to the inputted decibel value.
		/// </summary>
		/// <param name="dBVolume"></param>
		/// <returns></returns>
		public static float ConvertDecibelVolumeToLinearVolume(float dBVolume, bool usePerformanceApproximation = true)
		{
			float linearVolume = 0f;

			if (usePerformanceApproximation)
			{
				if (dBVolume <= SOUND_DB_CUTOFF)
					return 0;
				linearVolume = Mathf.Pow(2.0f, dBVolume / 6.02f);
			}
			else
			{
				linearVolume = Mathf.Pow(10f, dBVolume / 20f);
			}
			return linearVolume;
		}
	}
}
