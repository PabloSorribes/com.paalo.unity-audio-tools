using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paalo.UnityAudioTools
{
	public static class AudioMiscHelper
	{

		/// <summary>
		/// Use this to get a super precise calculation of the length of an audioClip, mainly for use with <see cref="AudioSource.PlayScheduled(double)"/>.
		/// </summary>
		/// <remarks>
		/// Source: 10 Unity Audio Tips (That You Won’t Find in the Tutorials) - #6 Stitch Audio Clips together seamlessly
		/// https://johnleonardfrench.com/articles/10-unity-audio-tips-that-you-wont-find-in-the-tutorials/#playscheduled
		/// </remarks>
		/// <param name="audioClip"></param>
		/// <returns></returns>
		public static double ClipLengthForPlayScheduled(AudioClip audioClip)
		{
			// This gets the exact duration of the first clip, note that you have to cast the samples value as a double
			double clipDuration = (double)audioClip.samples / audioClip.frequency;
			return clipDuration;
		}

		/// <summary>
        /// This method will tell you the percentage of the clip that is done Playing (0-100).
        /// </summary>
        /// <param name="source">The Audio Source to calculate for.</param>
        /// <returns>(0-100 float)</returns>
        public static float GetAudioPlayedPercentage(AudioSource source) {
            if (source.clip == null || source.time == 0f) {
                return 0f;
            }

            var playedPercentage = (source.time / source.clip.length) * 100;
            return playedPercentage;
        }

	}
}
