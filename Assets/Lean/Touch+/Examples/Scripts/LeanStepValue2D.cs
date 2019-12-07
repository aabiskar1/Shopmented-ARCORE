using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Lean.Touch
{
	/// <summary>This component allows you to accumilate value changes until they reach a threshold value, and then output them in fixed steps.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanStepValue2D")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Step Value 2D")]
	public class LeanStepValue2D : MonoBehaviour
	{
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		/// <summary>The current accumilated value type.</summary>
		[Tooltip("The current accumilated value type.")]
		public Vector2 Value;

		/// <summary>When any dimension of Value exceeds this, OnStep will be called, and Value will be rolled back.</summary>
		[Tooltip("When any dimension of Value exceeds this, OnStep will be called, and Value will be rolled back.")]
		public Vector2 Step = Vector2.one;

		public Vector2Event OnStep { get { if (onStep == null) onStep = new Vector2Event(); return onStep; } } [FormerlySerializedAs("OnStep")] [SerializeField] private Vector2Event onStep;

		/// <summary>This method allows you to increment Value.</summary>
		public void AddValue(Vector2 delta)
		{
			Value += delta;
		}

		/// <summary>This method allows you to increment Value.x.</summary>
		public void AddValueX(float delta)
		{
			Value.x += delta;
		}

		/// <summary>This method allows you to increment Value.y.</summary>
		public void AddValueY(float delta)
		{
			Value.y += delta;
		}

		protected virtual void Update()
		{
			var stepX = (int)(Value.x / Step.x);
			var stepY = (int)(Value.y / Step.y);

			if (stepX != 0 || stepY != 0)
			{
				var deltaX = stepX * Step.x;
				var deltaY = stepY * Step.y;

				Value.x -= deltaX;
				Value.y -= deltaY;

				if (onStep != null)
				{
					onStep.Invoke(new Vector2(deltaX, deltaY));
				}
			}
		}
	}
}