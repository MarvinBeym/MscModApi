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
		
		/// <summary>
		/// Checks if any ReplacedGameParts need to be initialized.
		/// Returns false if modsParts is null or empty.
		/// </summary>
		private bool initializingRequired
		{
			get
			{
				// FIX: Added null check for modsParts to prevent NullReferenceException
				if (ReplacedGameParts.modsParts == null)
				{
					return false;
				}
				return ReplacedGameParts.modsParts.Any(keyValuePair => keyValuePair.Value != null && keyValuePair.Value.Count > 0);
			}
		}

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

			// FIX: Added null check for modsParts
			if (ReplacedGameParts.modsParts == null || ReplacedGameParts.modsParts.Count == 0)
			{
				yield break;
			}

			float currentTime = 0;
			
			// FIX: Added null checks for FsmVariables.GlobalVariables and playerStop
			FsmBool playerStop = null;
			try
			{
				if (FsmVariables.GlobalVariables != null)
				{
					playerStop = FsmVariables.GlobalVariables.FindFsmBool("PlayerStop");
				}
			}
			catch (Exception ex)
			{
				ModConsole.Error($"[MscModApi] Failed to find PlayerStop FsmBool: {ex.Message}");
			}
			
			// Only set playerStop if it was found successfully
			if (playerStop != null)
			{
				playerStop.Value = true;
			}
			else
			{
				ModConsole.Warning("[MscModApi] PlayerStop FsmBool not found, skipping movement lock during initialization");
			}

			while (currentTime < SECONDS_TO_WAIT)
			{
				currentTime += Time.deltaTime;
				// FIX: Added null check before accessing playerStop.Value
				if (playerStop != null && playerStop.Value)
				{
					int secondsLeft = (int) Math.Floor(SECONDS_TO_WAIT - currentTime);
					if (secondsLeft <= 0)
					{
						secondsLeft = 0;
					}
					UserInteraction.GuiInteraction(UserInteraction.Type.None, $"MscModApi waiting for game finished loading ~{secondsLeft} seconds. Press [{cInput.GetText("Use")}] to force unlock movement");
					// FIX: Added null check for MscModApi.disableLoadingMovementLock
					bool disableMovementLock = MscModApi.disableLoadingMovementLock != null && MscModApi.disableLoadingMovementLock.GetValue();
					if (UserInteraction.UseButtonDown || disableMovementLock)
					{
						playerStop.Value = false;
					}
				}
				yield return null;
			}
			// FIX: Added null check before setting playerStop.Value
			if (playerStop != null)
			{
				playerStop.Value = false;
			}

			// FIX: Added null check for modsParts before iterating
			if (ReplacedGameParts.modsParts == null)
			{
				yield break;
			}

			foreach (KeyValuePair<string, List<ReplacedGameParts>> keyValuePair in ReplacedGameParts.modsParts)
			{
				// FIX: Skip if value is null
				if (keyValuePair.Value == null)
				{
					continue;
				}
				
				int modInitializedCounter = 0;
				int modInitializedFailureCounter = 0;
				foreach (ReplacedGameParts replacedGameParts in keyValuePair.Value)
				{
					// FIX: Skip if replacedGameParts is null
					if (replacedGameParts == null)
					{
						continue;
					}
					
					if (!replacedGameParts.initialized)
					{
						try
						{
							var events = replacedGameParts.GetEvents(ReplacedGamePartsEvent.Type.Initialized);
							// FIX: Added null check for events
							if (events != null)
							{
								events.InvokeAll();
							}
							modInitializedCounter++;
						}
						catch (Exception ex)
						{
							ModConsole.Print($"Executing Initializing events for ReplacedGamePart with id '{replacedGameParts.id}' failed. Check your Events");
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
			
			try
			{
				StartCoroutine(AwaitGameProperlyInitialized());
			}
			catch (Exception ex)
			{
				ModConsole.Error($"[MscModApi] Failed to start initialization coroutine: {ex.Message}");
			}
		}
	}
}