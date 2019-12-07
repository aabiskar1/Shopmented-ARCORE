using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Lean.Touch
{
	/// <summary>This component allows you to get the pinch of all fingers.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiPinch")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Pinch")]
	public class LeanMultiPinch : MonoBehaviour
	{
		public enum CoordinateType
		{
			OneBased,
			OneBasedReciprocal,
			ZeroBased
		}

		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>If there is no pinching, ignore it?</summary>
		[Tooltip("If there is no pinching, ignore it?")]
		public bool IgnoreIfStatic;

		[Space]

		/// <summary>OneBased = Scale (1 = no change, 2 = double size, 0.5 = half size).
		/// OneBasedReciprocal = 1 / Scale (1 = no change, 2 = half size, 0.5 = double size).
		/// ZeroBased = Scale - 1 (0 = no change, 1 = double size, - 0.5 = half size).</summary>
		[Tooltip("OneBased = Scale (1 = no change, 2 = double size, 0.5 = half size).\n\nOneBasedReciprocal = 1 / Scale (1 = no change, 2 = half size, 0.5 = double size).\n\nZeroBased = Scale - 1 (0 = no change, 1 = double size, - 0.5 = half size).")]
		[FormerlySerializedAs("Scale")] public CoordinateType Coordinate;

		/// <summary>The swipe delta will be multiplied by this value.</summary>
		[Tooltip("The swipe delta will be multiplied by this value.")]
		public float Multiplier = 1.0f;

		/// <summary>This event is invoked when the requirements are met.
		/// Float = Pinch value based on your Scale setting.</summary>
		public FloatEvent OnPinch { get { if (onPinch == null) onPinch = new FloatEvent(); return onPinch; } } [FormerlySerializedAs("OnPinch")] [SerializeField] private FloatEvent onPinch;

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
			// Get fingers
			var fingers = Use.GetFingers();

			// Get pinch
			var pinch = Coordinate == CoordinateType.OneBasedReciprocal == true ? LeanGesture.GetPinchRatio(fingers) : LeanGesture.GetPinchScale(fingers);

			// Ignore?
			if (pinch == 1.0f)
			{
				return;
			}

			// This gives you a 0 based pinch value, allowing usage with translation and rotation
			if (Coordinate == CoordinateType.ZeroBased)
			{
				pinch -= 1.0f; pinch *= Multiplier;
			}
			else
			{
				pinch = Mathf.Pow(pinch, Multiplier);
			}

			// Call events
			if (onPinch != null)
			{
				onPinch.Invoke(pinch);
			}
		}
	}
}