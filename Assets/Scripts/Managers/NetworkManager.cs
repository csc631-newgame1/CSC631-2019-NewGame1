using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class NetworkManager : MonoBehaviour
{
	public static int clientID = 0;
	
    void Awake()
    {
		DontDestroyOnLoad(this.gameObject);

        /*Network.connectToServer("18.223.24.205", 1337); // remote network
        //Network.connectToServer("127.0.0.1", 1337); // local network
		
		Thread.Sleep(1000);
		Network.submitCommand(new MoveCommand(new Pos(2, 2), new Pos(3, 3)));
		Network.submitCommand(new StartCommand());
		Network.submitCommand(new EndCommand());
		Network.submitCommand(new NicknameCommand("Big Boy", clientID));*/
    }

    // Update is called once per frame
    void Update()
    {
        List<NetworkCommand> cmds = Network.getPendingCommands();
		
		if (cmds != null) {
			foreach (NetworkCommand cmd in cmds) {
				
				if (cmd is MoveCommand) {
					MoveCommand move = (MoveCommand) cmd;
					Debug.Log("Move received! From " + move.a + " to " + move.b);
				}
				if (cmd is ReadyCommand) {
					ReadyCommand ready = (ReadyCommand) cmd;
					Debug.Log("Received READY");
					
					Client peer = Network.getPeer(ready.clientID);
					if (client == null) {
						Network.setPeer(ready.clientID);
						client = Network.getPeer(ready.clientID);
					}
					peer.ready = !peer.ready;
				}
				if (cmd is NicknameCommand) {
					NicknameCommand nickname = (NicknameCommand) cmd;
					Debug.Log("Received nickname: " + nickname.nickname);
					
					Client client = Network.getPeer(nickname.clientID);
					if (client == null) {
						Network.setPeer(nickname.clientID);
						client = Network.getPeer(nickname.clientID);
					}
					client.nickname = nickname.nickname;
				}
				if (cmd is ClassnameCommand) {
					ClassnameCommand classname = (ClassnameCommand) cmd;
					Debug.Log("Received classname: " + classname.classname);
					
					Client client = Network.getPeer(classname.clientID);
					if (client == null) {
						Network.setPeer(classname.clientID);
						client = Network.getPeer(classname.clientID);
					}
					client.classname = classname.classname;
				}
				if (cmd is UpdateClientInfoCommand) { // called when a new player joins and needs to be synchronized
					UpdateClientInfoCommand update = (UpdateClientInfoCommand) cmd;
					Debug.Log("Received update from client#" + update.clientID);
					
					Client client = Network.getPeer(update.clientID);
					if (client == null) {
						Network.setPeer(update.clientID);
						client = Network.getPeer(update.clientID);
					}
					client.nickname = update.nickname;
					client.classname = update.classname;
					client.ready = update.ready;
				}
				if (cmd is JoinCommand) {
					JoinCommand _join = (JoinCommand) cmd; // to avoid conflict with 'join' keyword
					if (clientID == 0) {
						clientID = _join.clientID;
						Debug.Log("Joined game! Client ID #" + clientID + " assigned!");
					} else {
						Debug.Log("New player with client ID #" + _join.clientID + " joined!");
						Network.submitCommand(new UpdateClientInfoCommand(Network.getPeer(clientID)));
					}
					Network.setPeer(_join.clientID);
				}
				if (cmd is StartCommand) {
					StartCommand start = (StartCommand) cmd;
					Debug.Log("Received START");
				}
				if (cmd is EndCommand) {
					EndCommand end = (EndCommand) cmd;
					Debug.Log("Received END");
				}
				if (cmd is DisconnectCommand) {
					DisconnectCommand disconnect = (DisconnectCommand) cmd;
					Debug.Log("Received DISCONNECT from client #" + disconnect.clientID);
					Network.removePeer(disconnect.clientID);
					
					if (clientID >= disconnect.clientID) {
						clientID--;
						Debug.Log("Changed client ID to " + clientID);
					}
				}
			}
		}
		if (!Network.connected()) clientID = 0;
    }
	
	void OnApplicationQuit()
	{
		Network.disconnectFromServer();
	}
}
