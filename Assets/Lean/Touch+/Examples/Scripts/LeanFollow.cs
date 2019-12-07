using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component causes the current Transform to follow the specified trail of positions.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanFollow")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Follow")]
	public class LeanFollow : MonoBehaviour
	{
		public enum RotateType
		{
			None,
			Forward,
			TopDown,
			Side2D
		}

		/// <summary>The world space position that will be followed.
		/// NOTE: If Destination is set, then this value will be overridden.</summary>
		[Tooltip("The world space position that will be followed.\n\nNOTE: If Destination is set, then this value will be overridden.")]
		public Vector3 Position;

		/// <summary>The world space offset that will be followed.</summary>
		[Tooltip("The world space offset that will be followed.")]
		public Vector3 Offset;

		[Tooltip("When this object is within this many world space units of the next point, it will be removed.")]
		public float Threshold = 0.001f;

		[Tooltip("The speed of the following in units per seconds.")]
		public float Speed = 1.0f;

		[System.NonSerialized]
		private LinkedList<Vector3> positions;

		/// <summary>This method will remove all follow positions, and stop movement.</summary>
		[ContextMenu("Clear Position")]
		public void ClearPosition()
		{
			if (positions != null)
			{
				positions.Clear();
			}
		}

		/// <summary>This method adds a new position to the follow path.</summary>
		public void AddPosition(Vector3 newPosition)
		{
			if (positions == null)
			{
				positions = new LinkedList<Vector3>();
			}

			// Only add newPosition if it's far enough away from the last added point
			if (positions.Count == 0 || Vector3.Distance(positions.Last.Value, newPosition) > Threshold)
			{
				positions.AddLast(newPosition);
			}
		}

		protected virtual void Update()
		{
			if (positions != null)
			{
				TrimPositions();

				if (positions.Count > 0)
				{
					var currentPosition = transform.position;
					var targetPosition  = positions.First.Value;

					currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, Speed * Time.deltaTime);

					transform.position = currentPosition;
				}
			}
		}

		protected void TrimPositions()
		{
			var currentPosition = transform.position;

			while (positions.Count > 0)
			{
				var first    = positions.First;
				var distance = Vector3.Distance(currentPosition, first.Value);

				if (distance > Threshold)
				{
					break;
				}

				positions.Remove(first);
			}
		}
	}
}