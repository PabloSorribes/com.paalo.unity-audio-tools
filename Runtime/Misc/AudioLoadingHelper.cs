using System.Collections;
using UnityEngine;

namespace Paalo.UnityAudioTools
{
	public class AudioLoadingHelper : MonoBehaviour
	{
		//If-def for testing purposes
#if UNITY_EDITOR
		[SerializeField] AudioClip[] audioClips = default;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Debug.Log($"LOAD!");
				foreach (var clip in audioClips)
				{
					LoadAudio(clip);
				}
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Debug.Log($"UNLOAD!");
				foreach (var clip in audioClips)
				{
					UnloadAudio(clip);
				}
			}
		}
#endif

		public void LoadAudio(AudioClip audioClip, System.Action OnLoadedAction = null)
		{
			StartCoroutine(LoadAudioDataCoroutine(audioClip, OnLoadedAction));
		}

		private IEnumerator LoadAudioDataCoroutine(AudioClip audioClip, System.Action OnLoadedAction = null)
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

		public void UnloadAudio(AudioClip audioClip, System.Action OnUnloadedAction = null)
		{
			//There doesn't seem to be a way to check if a clip's state is currently "unloading", 
			//that's why the coroutine call is commented out
			audioClip.UnloadAudioData();
			OnUnloadedAction?.Invoke();

			//StartCoroutine(UnloadAudioDataCoroutine(audioClip, OnUnloadedAction));
		}

		private IEnumerator UnloadAudioDataCoroutine(AudioClip audioClip, System.Action OnUnloadedAction = null)
		{
			//Clip is null, exiting early
			if (audioClip == null)
				yield break;

			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

			audioClip.UnloadAudioData();

			//Wait until loading is done
			while (audioClip.loadState == AudioDataLoadState.Loading)
			{
				Debug.Log($"Loading: '{audioClip.name}'");
				yield return null;
			}

			if (audioClip.loadState == AudioDataLoadState.Failed)
			{
				Debug.LogWarning($"FAILED to load AudioClip '{audioClip.name}'!");
				yield break;
			}

			if (audioClip.loadState == AudioDataLoadState.Loaded)
			{
				Debug.Log($"AudioClip '{audioClip.name}' is LOADED!");
				yield return null;
			}

			if (audioClip.loadState == AudioDataLoadState.Unloaded)
			{
				//Debug.Log($"AudioClip '{audioClip.name}' is unloaded!");
				OnUnloadedAction?.Invoke();
			}

			stopwatch.Stop();
			Debug.Log($"Time to unload AudioClip '{audioClip.name}': {stopwatch.ElapsedMilliseconds} ms");
		}

	}
}