using System.Collections;
using UnityEngine;

namespace Paalo.UnityAudioTools
{
	public class AudioLoadingHelper : MonoBehaviour
	{
		private void LoadAudioExample(AudioClip audioClip, System.Action OnLoadedAction = null)
		{
			StartCoroutine(LoadAudioDataCoroutine(audioClip, OnLoadedAction));
		}

		public IEnumerator LoadAudioDataCoroutine(AudioClip audioClip, System.Action OnLoadedAction = null)
		{
			//Clip is null, exiting early
			if (audioClip == null)
				yield break;

			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			audioClip.LoadAudioData();

			//Wait until loading is done
			while (audioClip.loadState == AudioDataLoadState.Loading)
				yield return null;

			if (audioClip.loadState == AudioDataLoadState.Failed)
			{
				Debug.LogWarning($"FAILED to load AudioClip '{audioClip.name}'!");
				yield break;
			}

			if (audioClip.loadState == AudioDataLoadState.Loaded)
			{
				//Debug.Log($"AudioClip '{clip.name}' is loaded!");
				OnLoadedAction?.Invoke();
			}

			stopwatch.Stop();
			Debug.Log($"Time to load AudioClip '{audioClip.name}': {stopwatch.ElapsedMilliseconds} ms");
		}
	}
}