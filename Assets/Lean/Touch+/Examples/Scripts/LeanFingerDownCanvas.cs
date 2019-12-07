using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Lean.Touch
{
	/// <summary>This component calls OnDown when a finger begins touching the screen on top of this UI element.
	/// NOTE: This requires you to enable the RaycastTarget setting on your UI graphic.</summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanFingerDownCanvas")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Down Canvas")]
	public class LeanFingerDownCanvas : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}

		/// <summary>Ignore fingers with StartedOverGui?</summary>
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreStartedOverGui;

		/// <summary>Do nothing if this LeanSelectable isn't selected?</summary>
		[Tooltip("Do nothing if this LeanSelectable isn't selected?")]
		public LeanSelectable RequiredSelectable;

		/// <summary>Called on the first frame the conditions are met.</summary>
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [FormerlySerializedAs("onDown")] [FormerlySerializedAs("OnDown")] [SerializeField] private LeanFingerEvent onFinger;

		public bool ElementOverlapped(LeanFinger finger)
		{
			var results = LeanTouch.RaycastGui(finger.ScreenPosition, -1);

			if (results != null && results.Count > 0)
			{
				if (results[0].gameObject == gameObject)
				{
					return true;
				}
			}

			return false;
		}
		
		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += HandleFingerDown;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= HandleFingerDown;
		}

		private void HandleFingerDown(LeanFinger finger)
		{
			if (IgnoreStartedOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			if (ElementOverlapped(finger) == true)
			{
				if (onFinger != null)
				{
					onFinger.Invoke(finger);
				}
			}
		}
	}
}