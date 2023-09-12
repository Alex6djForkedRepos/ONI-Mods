﻿using DecorPackA.UI;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

namespace DecorPackA.Buildings.MoodLamp
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class TintableLamp : KMonoBehaviour
	{
		[Serialize] public string colorHex;
		[Serialize] public int swatchIdx;
		[Serialize] public bool initialized;

		[MyCmpReq] private MoodLamp moodLamp;

		public bool IsActive { get; private set; }

		public HashSet<KAnimHashedString> tintTargetSymbols = new()
		{
			"tintable",
			"tintable_bloom",
			"tintable_fg"
		};

		public Color Color { get; private set; }

		public TintableLamp()
		{
			swatchIdx = SwatchSelector.Invalid;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();

			if (!initialized)
			{
				colorHex = Random.ColorHSV(0, 1, 0.4f, 1, 0.5f, 0.9f).ToHexString();
				initialized = true;
			}

			Subscribe(ModEvents.OnMoodlampChanged, OnMoodlampChanged);
			Subscribe((int)GameHashes.CopySettings, OnCopySettings);
		}

		private void OnMoodlampChanged(object data)
		{
			var isBeingTurnedOn = LampVariant.HasTag(data, LampVariants.TAGS.TINTABLE);

			if (isBeingTurnedOn)
			{
				var targetSymbols = LampVariant.GetCustomDataOrDefault<HashSet<KAnimHashedString>>(data, "Tintable_TargetSymbols", null);

				if (targetSymbols != null)
					tintTargetSymbols = targetSymbols;
			}
			else if (IsActive)
			{
				// if we are turning this component off, and was active before, reset tints to white
				TintKbacs(Color.white);
			}

			IsActive = isBeingTurnedOn;

			if (IsActive)
				RefreshColor();
		}

		private void OnCopySettings(object obj)
		{
			if (!IsActive)
				return;

			if (((GameObject)obj).TryGetComponent(out TintableLamp tintable))
			{
				SetColor(tintable.Color);
			}
		}

		private void RefreshColor()
		{
			if (swatchIdx != SwatchSelector.Invalid)
				SetColor(swatchIdx);
			else
				SetColor(Util.ColorFromHex(colorHex));
		}

		private Color GetLightColor(Color color) => (color * 2.55f) with { a = 1f };

		private void TintKbacs(Color color)
		{
			if (!IsActive)
				return;

			if (tintTargetSymbols == null)
			{
				Log.Warning($"Tintable lamp {moodLamp.currentVariantID} has no target symbols defined.");
				return;
			}

			foreach (var symbol in tintTargetSymbols)
			{
				moodLamp.lampKbac.SetSymbolTint(symbol, color);
			}
		}

		public void SetColor(int index)
		{
			if (index == SwatchSelector.Invalid)
				return;

			swatchIdx = index;
			SetColor(ModAssets.colors[index]);
		}

		public void SetColor(Color color)
		{
			colorHex = color.ToHexString();
			Color = color;

			if (IsActive)
			{
				moodLamp.SetLightColor(GetLightColor(color));
				TintKbacs(color);
			}

			Trigger(ModEvents.OnLampTinted, color);
		}
	}
}
