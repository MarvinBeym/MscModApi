using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using MSCLoader;
using MscModApi.Caching;
using MscModApi.Parts.ReplacePart.EventSystem;
using MscModApi.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MscModApi.Parts.ReplacePart
{
	/// <summary>
	/// Class handling the loading/initializing of ReplacedGameParts classes, waiting for the game to be safely loaded
	/// </summary>
	public class ReplacedGamePartsDelayedInitializer : MonoBehaviour
	{
		private const int FRAMES_TO_WAIT = 50;
		private const int SECONDS_TO_WAIT = 6;

		private bool initialized;
		private bool initializingRequired => ReplacedGameParts.modsParts.Any(keyValuePair => keyValuePair.Value.Count > 0);

		private IEnumerator AwaitGameProperlyInitialized()
		{
			if (!initializingRequired)
			{
				yield break;
			}

			for (int frameCounter = 0; frameCounter < FRAMES_TO_WAIT; frameCounter++)
			{
				yield return null;
			}

			float currentTime = 0;
			FsmBool playerStop = FsmVariables.GlobalVariables.FindFsmBool("PlayerStop");
			playerStop.Value = true;

			while (currentTime < SECONDS_TO_WAIT)
			{
				currentTime += Time.deltaTime;
				if (playerStop.Value)
				{
					int secondsLeft = (int) Math.Floor(SECONDS_TO_WAIT - currentTime);
					if (secondsLeft <= 0)
					{
						secondsLeft = 0;
					}
					UserInteraction.GuiInteraction(UserInteraction.Type.None, $"MscModApi waiting for game finished loading ~{secondsLeft} seconds. Press [{cInput.GetText("Use")}] to force unlock movement");
					if (UserInteraction.UseButtonDown || MscModApi.disableLoadingMovementLock.GetValue())
					{
						playerStop.Value = false;
					}
				}
				yield return null;
			}
			playerStop.Value = false;

			foreach (KeyValuePair<string, List<ReplacedGameParts>> keyValuePair in ReplacedGameParts.modsParts)
			{
				int modInitializedCounter = 0;
				int modInitializedFailureCounter = 0;
				foreach (ReplacedGameParts replacedGameParts in keyValuePair.Value)
				{
					if (!replacedGameParts.initialized)
					{
						try
						{
							replacedGameParts.GetEvents(ReplacedGamePartsEvent.Type.Initialized).InvokeAll();
							modInitializedCounter++;
						}
						catch (Exception ex)
						{
							ModConsole.Print($"Executing Initializing events for ReplacedGamePart with id '{replacedGameParts.id} failed. Check your Events'");
							ModConsole.Error(ex.Message);
							modInitializedFailureCounter++;
						}
						
					}
				}

				if (modInitializedCounter > 0)
				{
					ModConsole.Print($"Initialized {modInitializedCounter} ({modInitializedFailureCounter} failures) ReplacedGameParts for mod <color=blue>{keyValuePair.Key}</color>");
				} 
			}
		}

		/// <summary>
		/// Called once in the Update loop to startup the coroutine to wait for the game to be loaded.
		/// </summary>
		public void InitOnceByUpdateFrame()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			StartCoroutine(AwaitGameProperlyInitialized());
		}
	}
}