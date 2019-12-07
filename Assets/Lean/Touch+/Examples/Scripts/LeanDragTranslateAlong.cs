using UnityEngine;

namespace Lean.Touch
{
	/// <summary>This component alows you to translate the current GameObject along the specified surface.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDragTranslateAlong")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Translate Along")]
	public class LeanDragTranslateAlong : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>If your ScreenDepth settings cause the position values to clamp, there will be a difference between where the finger is and where the object is. Should this difference be tracked?</summary>
		[Tooltip("If your ScreenDepth settings cause the position values to clamp, there will be a difference between where the finger is and where the object is. Should this difference be tracked?")]
		public bool TrackScreenPosition = true;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		public float Dampening = -1.0f;

		[System.NonSerialized]
		private Vector2 deltaDifference;

		[SerializeField]
		private Vector3 remainingDelta;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}
#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif
		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Update()
		{
			// Store smoothed position
			var smoothPosition = transform.localPosition;

			// Snap to target
			transform.localPosition = transform.localPosition + remainingDelta;

			// Store old position
			var oldPosition = transform.localPosition;

			// Update to new position
			UpdateTranslation();

			// Shift delta by old new delta
			remainingDelta += transform.localPosition - oldPosition;

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Dampen remainingDelta
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			// Shift this position by the change in delta
			transform.localPosition = smoothPosition + remainingDelta - newDelta;

			// Update remainingDelta with the dampened value
			remainingDelta = newDelta;
		}

		private void UpdateTranslation()
		{
			// Get the fingers we want to use
			var fingers = Use.GetFingers();

			// Calculate the screenDelta value based on these fingers
			var screenDelta = LeanGesture.GetScreenDelta(fingers);

			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(ScreenDepth.Camera, gameObject);

			if (fingers.Count == 0)
			{
				deltaDifference = Vector2.zero;
			}

			if (camera != null)
			{
				if (TrackScreenPosition == true)
				{
					var oldScreenPoint = camera.WorldToScreenPoint(transform.position);
					var worldPosition  = default(Vector3);

					if (ScreenDepth.TryConvert(ref worldPosition, oldScreenPoint + (Vector3)(screenDelta + deltaDifference), gameObject) == true)
					{
						transform.position = worldPosition;
					}

					var newScreenPoint = camera.WorldToScreenPoint(transform.position);
					var oldNewDelta    = (Vector2)(newScreenPoint - oldScreenPoint);

					deltaDifference += screenDelta - oldNewDelta;
				}
				else
				{
					var oldScreenPoint = camera.WorldToScreenPoint(transform.position);
					var worldPosition  = default(Vector3);

					if (ScreenDepth.TryConvert(ref worldPosition, oldScreenPoint + (Vector3)screenDelta, gameObject) == true)
					{
						transform.position = worldPosition;
					}
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}