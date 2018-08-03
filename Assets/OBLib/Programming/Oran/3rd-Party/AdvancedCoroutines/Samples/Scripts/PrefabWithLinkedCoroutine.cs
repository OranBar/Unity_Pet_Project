// <copyright file="PrefabWithLinkedCoroutine.cs" company="Parallax Pixels">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Michael Kulikov</author>
// <date>07/05/2016 19:09:58 AM </date>

using AdvancedCoroutines;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedCoroutines.Samples.Scripts
{
    public class PrefabWithLinkedCoroutine : MonoBehaviour
    {
        public new Transform transform;
		private Routine r;
		private int i = 0;

        public void Start()
        {
            transform = GetComponent<Transform>();
			//r = CoroutineManager.StartCoroutine(enumerator(), this.gameObject);
			r = this.StartAdvCoroutine(enumerator());
		}

        private IEnumerator enumerator()
        {
            while (true)
            {
                transform.Rotate(Vector3.up + Vector3.left, 20f * Time.deltaTime);
				this.Log("1");
				yield return this.StartAdvCoroutine(Wait());
				this.Log("2");
				yield return null;
			}
        }

		private IEnumerator Wait()
		{
			yield return new Wait(0.5f);
		}

		void OnDisable()
		{
			r?.Pause();
		}
	}
}
