using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Touch
{
	/// <summary>This component allows you to perform events while fingers are on top of the current UI element.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMultiSetCanvas")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Multi Set Canvas")]
	public class LeanMultiSetCanvas : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerListEvent : UnityEvent<List<LeanFinger>> {}

		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(false);

		/// <summary>If a finger is currently off the current UI element, ignore it?</summary>
		[Tooltip("If a finger is currently off the current UI element, ignore it?")]
		public bool IgnoreIfOff = true;

		/// <summary>This event is invoked when the requirements are met.
		/// List<LeanFinger> = The fingers that are touching the screen.</summary>
		public LeanFingerListEvent OnFingers { get { if (onFingers == null) onFingers = new LeanFingerListEvent(); return onFingers; } } [SerializeField] private LeanFingerListEvent onFingers;

		[System.NonSerialized]
		private List<LeanFinger> downFingers = new List<LeanFinger>();

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

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += HandleFingerDown;
			LeanTouch.OnFingerUp   += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= HandleFingerDown;
			LeanTouch.OnFingerUp   -= HandleFingerUp;
		}

		protected virtual void Update()
		{
			// Get an initial list of fingers
			var fingers = Use.GetFingers();

			// Remove fingers that didn't begin on this UI element
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (downFingers.Contains(finger) == false)
				{
					fingers.RemoveAt(i);
				}
			}

			// Remove fingers that currently aren't on this UI element?
			if (IgnoreIfOff == true)
			{
				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = fingers[i];
					
					if (ElementOverlapped(finger) == false)
					{
						fingers.RemoveAt(i);
					}
				}
			}

			if (fingers.Count > 0)
			{
				if (onFingers != null)
				{
					onFingers.Invoke(fingers);
				}
			}
		}

		private void HandleFingerDown(LeanFinger finger)
		{
			if (ElementOverlapped(finger) == true)
			{
				downFingers.Add(finger);
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			downFingers.Remove(finger);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanMultiSetCanvas))]
	public class LeanMultiSetCanvas_Inspector : LeanInspector<LeanMultiSetCanvas>
	{
		protected override void DrawInspector()
		{
			Draw("Use");
			Draw("IgnoreIfOff");

			EditorGUILayout.Separator();

			Draw("onFingers");
		}
	}
}
#endif