using System.Collections;
using UnityEngine;

namespace Paalo.UnityAudioTools
{
	/// <summary>
	/// Useful component to set on a normal Unity Audio Source.
	/// Gives functionality such as fade in/out, play at position
	/// and play follow transform.
	/// </summary>
	public class AudioSourceUtility : MonoBehaviour
	{
		[SerializeField] AudioSource audioSourceToAffect;
		AudioSource AudioSource => audioSourceToAffect == null ? audioSourceToAffect = GetComponent<AudioSource>() : audioSourceToAffect;

		CoroutineCancellationToken cancellationToken;

		//private void Update()
		//{
		//	if (Input.GetKeyDown(KeyCode.Alpha5))
		//	{
		//		PlayAudioSource(AudioSource, 2f, () => Debug.Log("Audio source at full volume"));
		//	}
		//	if (Input.GetKeyDown(KeyCode.Alpha6))
		//	{
		//		StopAudioSource(AudioSource, 2f, () => Debug.Log("Stopped audio source"));
		//	}
		//}

		public void PlayAudioSourceAtPosition(Vector3 position, AudioSource audioSource, float fadeTime = 0f, System.Action completedFadeCallback = null)
		{
			audioSource.transform.position = position;
			PlayAudioSource(audioSource, fadeTime, completedFadeCallback);
		}

		public void PlayAudioSourceFollowTransform(Transform transform, AudioSource audioSource, float fadeTime = 0f, System.Action completedFadeCallback = null)
		{
			audioSource.transform.parent = transform;
			audioSource.transform.localPosition = Vector3.zero;
			PlayAudioSource(audioSource, fadeTime, completedFadeCallback);
		}

		/// <summary> Method suited for calling through the Inspector </summary>
		public void PlayAudioSource(float fadeTime = 0f)
		{
			PlayAudioSource(AudioSource, fadeTime);
		}

		public void PlayAudioSource(AudioSource audioSource, float fadeTime = 0f, System.Action completedFadeCallback = null)
		{
			fadeTime = Mathf.Clamp(fadeTime, 0f, fadeTime);

			if (Mathf.Approximately(0.000f, fadeTime))
			{
				audioSource.Play();
			}
			else
			{
				float originalInspectorVolume = audioSource.volume;
				audioSource.volume = 0f;
				audioSource.Play();
				FadeAudioSourceSmoothStep(audioSource, 0f, originalInspectorVolume, fadeTime, completedFadeCallback);
			}
		}

		/// <summary> Method suited for calling through the Inspector </summary>
		public void StopAudioSource(float fadeTime = 0f)
		{
			StopAudioSource(AudioSource, fadeTime);
		}

		public void StopAudioSource(AudioSource audioSource, float fadeTime = 0f, System.Action completedFadeCallback = null)
		{
			fadeTime = Mathf.Clamp(fadeTime, 0f, fadeTime);

			if (Mathf.Approximately(0.000f, fadeTime))
			{
				audioSource.Stop();
			}
			else
			{
				FadeAudioSourceSmoothStep(audioSource, audioSource.volume, 0f, fadeTime, audioSource.Stop);
				StartCoroutine(DoActionAfterTime(fadeTime, completedFadeCallback));
			}
		}

		/// <summary> Method suited for calling through the Inspector </summary>
		public void FadeInAudioSource(float fadeTime = 2f)
		{
			try
			{
				FadeAudioSourceSmoothStep(AudioSource, AudioSource.volume, 1f, fadeTime, () => Debug.Log("Fading IN complete!"));
			}
			catch (System.Exception)
			{
				Debug.LogError($"This '{nameof(AudioSourceUtility)}' is not on a gameObject with an AudioSource or hasn't had its '{nameof(audioSourceToAffect)}' variable set.", gameObject);
			}
		}

		/// <summary> Method suited for calling through the Inspector </summary>
		public void FadeOutAudioSource(float fadeTime = 2f)
		{
			try
			{
				FadeAudioSourceSmoothStep(AudioSource, AudioSource.volume, 0f, fadeTime, () => Debug.Log("Fading OUT complete!"));
			}
			catch (System.Exception)
			{
				Debug.LogError($"This '{nameof(AudioSourceUtility)}' is not on a gameObject with an AudioSource or hasn't had its '{nameof(audioSourceToAffect)}'-variable set.", gameObject);
			}
		}

		/// <summary> Generic method for fading an AudioSource along a SmoothStep-curve. </summary>
		public void FadeAudioSourceSmoothStep(AudioSource audioSource, float from, float to, float fadeTime, System.Action completedCallback = null)
		{
			if (!gameObject.activeSelf)
			{
				Debug.LogError($"This gameObject '{gameObject.name}' has been deactivated before calling the FadeCoroutine, which will not work on a disabled object. \n" +
					$"Try moving this '{nameof(AudioSourceUtility)}'-component to a different object, and set its audiosource to the object you tried to disable.", gameObject);
				return;
			}

			if (cancellationToken == null)
			{
				cancellationToken = new CoroutineCancellationToken();
			}
			else if (!cancellationToken.IsFinished)
			{
				cancellationToken.Cancel();
				cancellationToken = new CoroutineCancellationToken();
			}

			StartCoroutine(GenericFadeToVolumeCoroutine(audioSource, Mathf.SmoothStep, from, to, fadeTime, cancellationToken, completedCallback));
		}

		/// <summary> Generic method for fading an AudioSource along a Linear-curve (Lerp). </summary>
		public void FadeAudioSourceLinear(AudioSource audioSource, float from, float to, float fadeTime, System.Action completedCallback = null)
		{
			if (!gameObject.activeSelf)
			{
				Debug.LogError($"This gameObject '{gameObject.name}' has been deactivated before calling the FadeCoroutine, which will not work on a disabled object. \n" +
					$"Try moving this '{nameof(AudioSourceUtility)}'-component to a different object, and set its audiosource to the object you tried to disable.", gameObject);
				return;
			}

			if (cancellationToken == null)
			{
				cancellationToken = new CoroutineCancellationToken();
			}
			else if (!cancellationToken.IsFinished)
			{
				cancellationToken.Cancel();
				cancellationToken = new CoroutineCancellationToken();
			}

			StartCoroutine(GenericFadeToVolumeCoroutine(audioSource, Mathf.Lerp, from, to, fadeTime, cancellationToken, completedCallback));
		}

		/// <summary> Generic Coroutine which does all of the actual fading of the AudioSource along an interpolationMethod-curve (SmoothStep, Lerp, etc). </summary>
		private static IEnumerator GenericFadeToVolumeCoroutine(AudioSource audioSource, System.Func<float, float, float, float> interpolationMethod, float from, float to, float fadeTime, CoroutineCancellationToken token = null, System.Action completedCallback = null)
		{
			token?.CoroutineStart();

			var elapsedTime = 0f;

			while (elapsedTime < fadeTime && (token == null || !token.IsCanceled))
			{
				audioSource.volume = 1f * interpolationMethod(from, to, elapsedTime / fadeTime);

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			if (token == null || !token.IsCanceled)
			{
				audioSource.volume = 1f * to;
			}

			token?.CoroutineFinish();
			completedCallback?.Invoke();
		}

		private static IEnumerator DoActionAfterTime(float time, System.Action action)
		{
			yield return new WaitForSeconds(time);
			action?.Invoke();
		}
	}
}
