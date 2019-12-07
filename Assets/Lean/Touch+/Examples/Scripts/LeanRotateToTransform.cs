using UnityEngine;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component automatically rotates the current GameObject based on movement.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanRotateToTransform")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Rotate To Transform")]
	public class LeanRotateToTransform : MonoBehaviour
	{
		public enum RotateType
		{
			None,
			Forward,
			TopDown,
			Side2D
		}

		/// <summary>This allows you choose the method used to find the target rotation.
		/// Forward = </summary>
		[Tooltip("Should this GameObject be rotated to the follow path too?")]
		public RotateType RotateTo;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		public float Dampening = 10.0f;

		[HideInInspector]
		[SerializeField]
		private Vector3 previousPosition;

		[HideInInspector]
		[SerializeField]
		private Vector3 previousDelta;

		protected virtual void Start()
		{
			previousPosition = transform.position;
		}

		protected virtual void LateUpdate()
		{
			var currentPosition = transform.position;
			var delta           = currentPosition - previousPosition;

			if (delta.sqrMagnitude > 0.0f)
			{
				previousDelta = delta;
			}

			var currentRotation = transform.localRotation;
			var factor          = LeanHelper.DampenFactor(Dampening, Time.deltaTime);

			if (previousDelta.sqrMagnitude > 0.0f)
			{
				UpdateRotation(previousDelta);
			}

			transform.localRotation = Quaternion.Slerp(currentRotation, transform.localRotation, factor);

			previousPosition = currentPosition;
		}

		private void UpdateRotation(Vector3 vector)
		{
			switch (RotateTo)
			{
				case RotateType.Forward:
				{
					transform.forward = vector;
				}
				break;

				case RotateType.TopDown:
				{
					var yaw = Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg;

					transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
				}
				break;

				case RotateType.Side2D:
				{
					var roll = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

					transform.rotation = Quaternion.Euler(0.0f, 0.0f, -roll);
				}
				break;
			}
		}
	}
}