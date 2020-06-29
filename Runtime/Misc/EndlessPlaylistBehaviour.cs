//Source: https://gamedevbeginner.com/ultimate-guide-to-playscheduled-in-unity/#queue_clips

using UnityEngine;

namespace Paalo.UnityAudioTools
{
	/// <summary>
	/// Used to endlessly feed in clips that play back to back, perhaps for a dynamic music system or a never ending playlist.
	/// </summary>
	public class EndlessPlaylistBehaviour : MonoBehaviour
	{
		AudioSource[] audioSources = new AudioSource[2];
		[SerializeField] AudioClip[] audioClips = new AudioClip[2];

		/// <summary>
		/// Typically for dynamic music systems, we will have the system look one second ahead until just before the next clip is needed.
		/// </summary>
		private readonly int lookAheadTimeInSeconds = 1;

		private int nextClipIndex = 0;
		private int sourceToggle = 0;
		private double nextStartTime;

		private void Awake()
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i] = gameObject.AddComponent<AudioSource>();
			}
		}

		void Update()
		{
			if (AudioSettings.dspTime > nextStartTime - lookAheadTimeInSeconds)
			{
				PlayNextPlaylistClip();
			}
		}

		private void PlayNextPlaylistClip()
		{
			AudioClip clipToPlay = audioClips[nextClipIndex];
			AudioSource nextSource = audioSources[sourceToggle];

			//TODO @Paalo: There's a bug here somewhere, which makes all the playlist clips 
			//play at the same time on the first frame, and since it's done in Update() it basically 
			//creates a sine wave for a random amount of time until kinda sorts itself out somehow.

			// Loads the next Clip to play and schedules when it will start
			nextSource.clip = clipToPlay;
			nextSource.PlayScheduled(nextStartTime);

			// Checks how long the Clip will last and updates the Next Start Time with a new value
			double duration = (double)clipToPlay.samples / clipToPlay.frequency;
			nextStartTime = nextStartTime + duration;

			// Switches the toggle to use the other Audio Source next
			sourceToggle = 1 - sourceToggle;

			// Increase the clip index number - else reset it if it runs out of clips
			nextClipIndex = nextClipIndex < audioClips.Length - 1 ? nextClipIndex + 1 : 0;
		}
	}
}
