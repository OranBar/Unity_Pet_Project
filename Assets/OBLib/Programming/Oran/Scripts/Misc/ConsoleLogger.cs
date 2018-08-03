using UnityEngine;

public class ConsoleLogger : MonoBehaviour {
	
    public void DebugLog(string msg)
    {
        Debug.Log(msg, this);
    }

}