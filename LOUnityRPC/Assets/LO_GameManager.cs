using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;

public class LO_GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		user.Name = "Xiaohao";
		user.Age = 18;
		user.UserID = 100001;
		LO_GameServer.DefaultServer.InitServer("115.28.227.1",23466);
	}

	private RX_UserInfo user = new RX_UserInfo();

	private HostData[] room_list = null;
	private bool  isConnected = false;
	void OnGUI()
	{
		if (GUILayout.Button("StartServer")) 
		{
			LO_GameServer.DefaultServer.StartServer(user.Name);
		}

		if (GUILayout.Button("RequestRoom")) 
		{
			LO_GameServer.DefaultServer.StartRequestRoom((HostData[] list)=>{
				this.room_list = list;
			});
		}

		if (this.room_list != null) {
			GUILayout.BeginVertical();
			
			foreach (HostData item in this.room_list) 
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(item.ip[0],GUILayout.Width(200f),GUILayout.Height(40f));
				GUILayout.Label(item.gameName,GUILayout.Width(200f),GUILayout.Height(40f));

				string title = null;
				//Aciton
				Action<HostData> action = null;

				Action<HostData> state_connect = (HostData data)=>{
					LO_GameServer.DefaultServer.SendGameMessage(user.ToString());
				};

				Action<HostData> state_no_connect = (HostData data) => 
				{
					LO_GameServer.DefaultServer.JoinHostRoom(data,(int state)=>{

						isConnected = state == 0;
						
					});
				};


				if (isConnected) {
					title = "Send";
					action = state_connect;
				}
				else
				{
					title = "Connect";
					action = state_no_connect;
				}

				if (GUILayout.Button(title,GUILayout.Width(60f),GUILayout.Height(40f))) 
				{
					action(item);
				}
				
				GUILayout.EndHorizontal();
			}
			
			
			GUILayout.EndVertical();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
