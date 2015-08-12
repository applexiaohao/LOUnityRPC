# 蓝鸥Untiy-RPC-Sample
## 使用UnityEngine中的NetWork创建的服务器类

### LO_GameServer.cs

** 通过该工具类能够在Unity程序中快速的创建一个游戏房间服务器，查询房间列表，及发送消息功能 **
```
using System;
using UnityEngine;

namespace LO_Tool
{
	public class LO_GameServer:MonoBehaviour
	{
    	#region 单例化
		private LO_GameServer ()
		{
		}

		private static GameObject s_LO_GameServer_object;
		private static LO_GameServer s_LO_GameServer = null;
		private static NetworkView s_LO_NetworkView = null;

		public static LO_GameServer DefaultServer
		{
			get{
				if (s_LO_GameServer == null) 
				{
					s_LO_GameServer_object = new GameObject("DefaultServer");
					s_LO_GameServer = s_LO_GameServer_object.AddComponent<LO_GameServer>();
					s_LO_NetworkView = s_LO_GameServer_object.AddComponent<NetworkView>();
				}

				return s_LO_GameServer;
			}
		}

		private static NetworkView DefalutNetworkView
		{
			get{
				return s_LO_NetworkView;
			}
		}
		#endregion

		/// <summary>
		/// init server...
		/// </summary>
		/// <param name="ip">Ip.</param>
		/// <param name="port">Port.</param>
		public bool InitServer(string ip,int port)
		{
			//set property
			MasterServer.ipAddress = ip;
			MasterServer.port = port;

			return true;
		}

		/// <summary>
		/// Starts the server.
		/// </summary>
		/// <returns><c>true</c>, if server was started, <c>false</c> otherwise.</returns>
		public bool StartServer()
		{
			//start...
			Network.InitializeServer(1000,25000,!Network.HavePublicAddress());

			//register a game
			MasterServer.RegisterHost("Card","XiaoHao's Doudizhu");

			return true;
		}


		public delegate void RequestRoomComplete(HostData[] list);
		private RequestRoomComplete complete_block = null;
		public RequestRoomComplete CompleteBlock{
			set{
				complete_block = value;
			}
			get{
				return complete_block;
			}
		}

		public void StartRequestRoom(RequestRoomComplete block)
		{
			LO_GameServer.DefaultServer.CompleteBlock = block;

			MasterServer.RequestHostList("Card");
		}


		public delegate void JoinHostRoomDelegate(int state);

		private JoinHostRoomDelegate join_delegate = null;
		public void JoinHostRoom(HostData room,JoinHostRoomDelegate block)
		{
			this.join_delegate = block;

			NetworkConnectionError error = Network.Connect(room.ip[0],room.port);

			Debug.Log(error);
		}

		public void SendGameMessage(string message)
		{
			LO_GameServer.DefalutNetworkView.RPC("RemoteReceiveMessage",RPCMode.All,message);
		}

		[RPC]
		public void RemoteReceiveMessage(string message)
		{
			Debug.Log(message);
		}

		#region Behaviour Actions


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
				LO_GameServer.DefaultServer.CompleteBlock(MasterServer.PollHostList());
				break;
			}
			default:
				break;
			}

		}


		public void OnPlayerConnected(NetworkPlayer player)
		{

		}

		public void OnConnectedToServer()
		{
			this.join_delegate(0);
			Debug.Log("OnConnectedToServer");
		}

		#endregion
	}
}

```
**我们发现几点与通常单例类不同的地方**
- LO_GameServer类继承于MonoBehaviour脚本类
- 静态对象s_LO_GameServer,是通过AddComponent函数实例化的,这与Unity引擎脚本类的实例化机制有关
- LO_GameServer单例类与其他单例类不同的地方在于，需要多创建一个静态的GameObject变量，用来存储该单例脚本对象

**除了以上几点不同之处，在该类中同样定义了几个委托类型，用来做回调功能的处理**
- RequestRoomComplete委托类型，当请求房间列表成功后就会调用该委托类型的变量complete_block
- JoinHostRoomDelegate委托类型，当加入房间成功后就会调用该委托类型的变量join_delegate

#### 应用举例
```
	private HostData[] room_list = null;
	private bool  isConnected = false;
	void OnGUI()
	{
		if (GUILayout.Button("StartServer")) 
		{
			LO_GameServer.DefaultServer.StartServer();
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
```


_ _ _



##使用C#语言中的System.Xml与System.IO库完成实体对象与XML转换的工具类
### LO_XMLTool.cs
**通过该工具类，能够快速的将C#中的实体对象与XML进行转换,方便大家在编写代码完成进行数据交互的功能**

```
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace AssemblyCSharp
{
	public class LO_XMLTool
	{

		#region 反序列化
		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="xml">XML字符串</param>
		/// <returns></returns>
		public static object Deserialize(Type type, string xml)
		{
			try
			{
				using (StringReader sr = new StringReader(xml))
				{
					XmlSerializer xmldes = new XmlSerializer(type);
					return xmldes.Deserialize(sr);
				}
			}
			catch (Exception e)
			{
				
				return null;
			}
		}
		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="type"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static object Deserialize(Type type, Stream stream)
		{
			XmlSerializer xmldes = new XmlSerializer(type);
			return xmldes.Deserialize(stream);
		}
		#endregion
		
		#region 序列化
		/// <summary>
		/// 序列化
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="obj">对象</param>
		/// <returns></returns>
		public static string Serializer(Type type, object obj)
		{
			MemoryStream Stream = new MemoryStream();
			XmlSerializer xml = new XmlSerializer(type);
			try
			{
				//序列化对象
				xml.Serialize(Stream, obj);
			}
			catch (InvalidOperationException)
			{
				throw;
			}
			Stream.Position = 0;
			StreamReader sr = new StreamReader(Stream);
			string str = sr.ReadToEnd();
			
			sr.Dispose();
			Stream.Dispose();
			
			return str;
		}
		
		#endregion
	}
}

```