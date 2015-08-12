using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class LO_GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		LO_GameServer.DefaultServer.InitServer("192.168.203.78",23466);
	}

	void OnGUI()
	{
		if (GUILayout.Button("StartServer")) 
		{
			LO_GameServer.DefaultServer.StartServer();
		}

		if (GUILayout.Button("RequestRoom")) 
		{
			LO_GameServer.DefaultServer.StartRequestRoom((HostData[] list)=>{
				foreach (HostData item in list) 
				{
					Debug.Log(item.ip + " : " + item.gameName);
				}
			});
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
