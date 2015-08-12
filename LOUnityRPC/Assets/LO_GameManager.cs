using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class LO_GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		LO_GameServer.InitServer("192.168.203.78",23466);
	}

	void OnGUI()
	{
		if (GUILayout.Button("StartServer")) 
		{
			LO_GameServer.StartServer();
		}

		if (GUILayout.Button("RequestRoom")) 
		{
			LO_GameServer.StartRequestRoom((HostData[] list)=>{
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

	/// <summary>
	/// some event notification from master server
	/// </summary>
	/// <param name="ev">Ev.</param>
	public void OnMasterServerEvent(MasterServerEvent ev)
	{
		switch (ev) {
		case MasterServerEvent.RegistrationSucceeded:
		{
			break;
		}
			
		case MasterServerEvent.RegistrationFailedNoServer:
		{
			break;
		}
		case MasterServerEvent.RegistrationFailedGameType:
		{
			break;
		}
		case MasterServerEvent.RegistrationFailedGameName:
		{
			break;
		}
		case MasterServerEvent.HostListReceived:
		{
			LO_GameServer.CompleteBlock(MasterServer.PollHostList());
			break;
		}
		default:
			break;
		}
	}
}
