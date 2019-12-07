using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This script allows you to twist the selected object around like a dial or knob.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableDial")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Dial")]
	public class LeanSelectableDial : LeanSelectableBehaviour
	{
		[System.Serializable]
		public class Trigger
		{
			[Tooltip("The central Angle of this trigger in degrees.")]
			public float Angle;

			[Tooltip("The angle range of this trigger in degrees.\n\n90 = Quarter circle.\n180 = Half circle.")]
			public float Arc;

			[HideInInspector]
			public bool Inside;

			public UnityEvent OnEnter { get { if (onEnter == null) onEnter = new UnityEvent(); return onEnter; } } [SerializeField] private UnityEvent onEnter;

			public UnityEvent OnExit { get { if (onExit == null) onExit = new UnityEvent(); return onExit; } } [SerializeField] private UnityEvent onExit;

			public bool IsInside(float angle, bool clamp)
			{
				var range = Arc * 0.5f;

				if (clamp == false)
				{
					var delta  = Mathf.Abs(Mathf.DeltaAngle(Angle, angle));

					return delta < range;
				}

				return angle >= Angle - range && angle <= Angle + range;
			}
		}

		/// <summary>The camera we will be used.
		/// None = MainCamera.</summary>
		[Tooltip("The camera we will be used.\n\nNone = MainCamera.")]
		public Camera Camera;

		/// <summary>The axis of the rotation in local space.</summary>
		[Tooltip("The axis of the rotation in local space.")]
		public Vector3 Tilt;

		/// <summary>The angle of the dial in degrees.</summary>
		[Tooltip("The angle of the dial in degrees.")]
		public float Angle;

		[Space]

		/// <summary>Should the Angle value be clamped?</summary>
		[Tooltip("Should the Angle value be clamped?")]
		public bool Clamp;

		/// <summary>The minimum Angle value.</summary>
		[Tooltip("The minimum Angle value.")]
		public float ClampMin = -45.0f;

		/// <summary>The maximum Angle value.</summary>
		[Tooltip("The maximum Angle value.")]
		public float ClampMax = 45.0f;

		[Space]

		/// <summary>This allows you to perform a custom event when the dial is within a specifid angle range.</summary>
		[Tooltip("This allows you to perform a custom event when the dial is within a specifid angle range.")]
		public List<Trigger> Triggers;

		private Vector3 oldPoint;

		private bool oldPointSet;

		protected virtual void Update()
		{
			// Reset rotation and get axis
			transform.localEulerAngles = Tilt;

			var axis = transform.up;

			// Is this GameObject selected?
			if (Selectable.IsSelected == true)
			{
				// Does it have a selected finger?
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					var newPoint = GetPoint(axis, finger.ScreenPosition);

					if (oldPointSet == true)
					{
						var oldVector = oldPoint - transform.position;
						var newVector = newPoint - transform.position;
						var cross     = Vector3.Cross(oldVector, newVector);
						var delta     = Vector3.Angle(oldVector, newVector);

						if (Vector3.Dot(cross, axis) >= 0.0f)
						{
							Angle += delta;
						}
						else
						{
							Angle -= delta;
						}
					}

					oldPoint    = newPoint;
					oldPointSet = true;
				}
			}
			else
			{
				oldPointSet = false;
			}

			if (Clamp == true)
			{
				Angle = Mathf.Clamp(Angle, ClampMin, ClampMax);
			}

			transform.Rotate(axis, Angle, Space.World);

			if (Triggers != null)
			{
				for (var i = 0; i < Triggers.Count; i++)
				{
					var trigger = Triggers[i];

					if (trigger.IsInside(Angle, Clamp) == true)
					{
						if (trigger.Inside == false)
						{
							trigger.Inside = true;

							trigger.OnEnter.Invoke();
						}
					}
					else
					{
						if (trigger.Inside == true)
						{
							trigger.Inside = false;

							trigger.OnExit.Invoke();
						}
					}
				}
			}
		}

		private Vector3 GetPoint(Vector3 axis, Vector2 screenPoint)
		{
			// Make sure the camera exists
			var camera = LeanTouch.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var ray      = camera.ScreenPointToRay(screenPoint);
				var plane    = new Plane(axis, transform.position);
				var distance = default(float);

				if (plane.Raycast(ray, out distance) == true)
				{
					return ray.GetPoint(distance);
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}

			return oldPoint;
		}
	}
}