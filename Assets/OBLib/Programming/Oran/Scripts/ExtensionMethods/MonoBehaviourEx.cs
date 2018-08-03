using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace OranUnityUtils
{
	public static class MonoBehaviourEx
	{
		#region Coroutines: SuperCoroutine Timeline and SuperCoroutine
		/// <summary>
		/// Starts a SuperCoroutine timeline, executing each routine in order.
		/// Each routine waits for the previous one to finish before executing
		/// </summary>
		/// <returns>The SuperCoroutine timeline.</returns>
		/// <param name="routines">Routines.</param>
		public static Coroutine StartCoroutineTimeline(this MonoBehaviour monoBehaviour, params IEnumerator[] routines)
		{
			return monoBehaviour.StartCoroutineTimeline(true, routines);
			//return monoBehaviour.StartCoroutine( monoBehaviour.StartCoroutineTimeline_Coro(routines) );
		}

		/// <summary>
		/// Starts a SuperCoroutine timeline, executing each routine in order.
		/// Each routine waits for the previous one to finish before executing
		/// </summary>
		/// <returns>The SuperCoroutine timeline.</returns>
		/// <param name="routines">Routines.</param>
		public static Coroutine StartCoroutineTimeline(this MonoBehaviour monoBehaviour, bool executeIfInactive, params IEnumerator[] routines)
		{
			//If we are trying to run a coroutine from a inactive monobehaviour, we're gonna fail. 
			//So the solution is to instantiate a new GameObject (obviously active), and let it run the coroutine for us.
			if (monoBehaviour.gameObject.activeInHierarchy == false)
			{
				if (executeIfInactive == false)
				{
					return null;
				}

				GameObject tmpCoroutineHost = new GameObject(monoBehaviour.name + " Helper");
				var coroutineHelper = tmpCoroutineHost.AddComponent<CoroutineHelper>();

				//As the last action, destroy the Helper GameObject
				routines = routines.Concat(new IEnumerator[1] { monoBehaviour.ToIEnum(() => UnityEngine.Object.Destroy(tmpCoroutineHost)) }).ToArray();

				return coroutineHelper.StartCoroutine(coroutineHelper.StartCoroutineTimeline_Coro(routines));
			}
			return monoBehaviour.StartCoroutine(monoBehaviour.StartCoroutineTimeline_Coro(routines));
		}

		private static IEnumerator StartCoroutineTimeline_Coro(this MonoBehaviour monoBehaviour, params IEnumerator[] routines){
			foreach(IEnumerator currentRoutine in routines){
				yield return monoBehaviour.StartCoroutine( currentRoutine );
			}
		}
        
        public static StoppableCoroutine StartStoppableCoroutine(this MonoBehaviour monoBehaviour, IEnumerator coroutine) {
            return monoBehaviour.gameObject.StartStoppableCoroutine(coroutine);
        }
		#endregion

		public static void ExecuteDelayed(this MonoBehaviour monoBehaviour, Action action, float delay){
            monoBehaviour.gameObject.ExecuteDelayed(action, delay);
        }

        public static void ExecuteAfterXFrames(this MonoBehaviour monoBehaviour, int frames, Action action)
        {
            monoBehaviour.StartCoroutineTimeline(
                WaitFrames(frames),
                monoBehaviour.ToIEnum(()=> action())
            );
        }

        private static IEnumerator WaitFrames(int frames)
        {
            while (frames-- > 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

		/// <summary>
		/// This method will immediately throw an exception if the component is not found. Use this when the T component is necessary for the correct execution of the program, 
		/// as opposed to a normal GetComponent with a null check
		/// </summary>														
		public static T GetRequiredComponent<T>(this MonoBehaviour thisMonoBehaviour) where T : class {
			var retrievedComponent = thisMonoBehaviour.GetComponent<T>();
			Assert.IsNotNull(retrievedComponent,
				string.Format("Script {1} on GameObject \"{0}\" does not have the required component of type {2}", thisMonoBehaviour.name, thisMonoBehaviour.GetType(), typeof(T)));

			return retrievedComponent;
		}
		/// <summary>
		/// This method will immediately throw an exception if no component is found. Use this when the T component is necessary for the correct execution of the program, 
		/// as opposed to a normal GetComponent with a null check
		/// </summary>														
		public static T GetRequiredComponentInChildren<T>(this MonoBehaviour thisMonoBehaviour) where T : class {
			var retrievedComponent = thisMonoBehaviour.GetComponentInChildren<T>();
			Assert.IsNotNull(retrievedComponent,
				string.Format("Script {1} on GameObject \"{0}\" does not have a child with component of type {2}", thisMonoBehaviour.name, thisMonoBehaviour.GetType(), typeof(T)));

			return retrievedComponent;
		}
	}
}