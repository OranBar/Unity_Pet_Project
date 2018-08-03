using System;				 
using UnityEngine.Events;

[Serializable]
public class OBEvent : UnityEvent{

	public event Action Event;


	public static OBEvent operator +(OBEvent e, Action a) {
		e.Event += a;
		return e; 		
	}

	public static OBEvent operator -(OBEvent e, Action a) {
		e.Event -= a;
		return e; 
	}

	public new void Invoke() {
		if (Event != null) {
			Event();
		}
		base.Invoke ();
	}
}

[Serializable]
public class OBEvent<K, T> : UnityEvent<T> where K : OBEvent<K, T>{

	public event Action<T> Event;
	
	public static K operator +(OBEvent<K, T> e, Action<T> a) {
		e.Event += a;
		return (K) e; 		
	}

	public static K operator -(OBEvent<K, T> e, Action<T> a) {
		e.Event -= a;
		return (K) e; 
	}

	public new void Invoke(T arg) {
		if (Event != null) {
			Event (arg);
		}
		base.Invoke (arg);
	}
}

[Serializable]
public class OBEvent<K, T, U> : UnityEvent<T, U> where K : OBEvent<K, T, U> {

	public event Action<T, U> Event;


	public static K operator +(OBEvent<K, T, U> e, Action<T, U> a) {
		e.Event += a;
		return (K)e;
	}

	public static K operator -(OBEvent<K, T, U> e, Action<T, U> a) {
		e.Event -= a;
		return (K)e;
	}

	public new void Invoke(T arg1, U arg2) {
		if (Event != null) {
			Event(arg1, arg2);
		}
		base.Invoke(arg1, arg2);
	}
}

[Serializable]
public class OBEvent<K, T, U, V> : UnityEvent<T, U, V> where K : OBEvent<K, T, U, V> {

	public event Action<T, U, V> Event;


	public static K operator +(OBEvent<K, T, U, V> e, Action<T, U, V> a) {
		e.Event += a;
		return (K)e;
	}

	public static K operator -(OBEvent<K, T, U, V> e, Action<T, U, V> a) {
		e.Event -= a;
		return (K)e;
	}

	public new void Invoke(T arg1, U arg2, V arg3) {
		if (Event != null) {
			Event(arg1, arg2, arg3);
		}
		base.Invoke(arg1, arg2, arg3);
	}
}
	  