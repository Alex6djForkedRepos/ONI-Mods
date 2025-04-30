﻿using ONITwitchLib;
using System.Linq;
using Twitchery.Content.Scripts;
using UnityEngine;

namespace Twitchery.Content.Events.EventTypes
{
	public class RainbowRainEvent() : TwitchEventBase(ID)
	{
		public const string ID = "RainbowRain";

		public override Danger GetDanger() => Danger.Medium;

		public override int GetWeight() => Consts.EventWeight.Common;


		public override void Run()
		{
			var go = new GameObject("rainbow cloud spawner");

			var rain = go.AddComponent<LiquidRainSpawner>();

			rain.totalAmountRangeKg = (3000, 9000);
			rain.durationInSeconds = 240;
			rain.dropletMassKg = 0.03f;
			rain.spawnRadius = 7;

			var potentialElements = AkisTwitchEvents.Instance.GetGenerallySafeLiquids();
			potentialElements.Shuffle();

			foreach (var element in potentialElements.Take(Mathf.Min(12, potentialElements.Count)))
				rain.AddElement(element);

			go.SetActive(true);

			GameScheduler.Instance.Schedule("rainbow rain", 1.5f, _ =>
			{
				rain.StartRaining();
				AudioUtil.PlaySound(ModAssets.Sounds.TEA, ModAssets.GetSFXVolume() * 0.15f);
			});

			ToastManager.InstantiateToast(
				STRINGS.AETE_EVENTS.RAINBOWRAIN.TOAST,
				STRINGS.AETE_EVENTS.RAINBOWRAIN.DESC);
		}
	}
}
