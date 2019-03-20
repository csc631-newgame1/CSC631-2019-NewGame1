using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class NetworkManager : MonoBehaviour
{
	public int clientID = 0;
	
    // Start is called before the first frame update
    void Start()
    {
		Network.enable();
        Network.connectToServer("18.223.24.205", 1337); // remote network
        //Network.connectToServer("127.0.0.1", 1337); // local network
		
		Thread.Sleep(1000);
		Network.submitCommand(new MoveCommand(new Pos(2, 2), new Pos(3, 3)));
		Network.submitCommand(new StartCommand());
		Network.submitCommand(new EndCommand());
		Network.submitCommand(new NicknameCommand("Big Boy", clientID));
    }

    // Update is called once per frame
    void Update()
    {
        List<NetworkCommand> cmds = Network.getPendingCommands();
		
		if (cmds != null) {
			Debug.Log("Received " + cmds.Count + " commands");
			foreach (NetworkCommand cmd in cmds) {
				
				if (cmd is MoveCommand) {
					MoveCommand move = (MoveCommand) cmd;
					Debug.Log("Move received! From " + move.a + " to " + move.b);
				}
				if (cmd is ReadyCommand) {
					ReadyCommand ready = (ReadyCommand) cmd;
					Debug.Log("Received READY");
				}
				if (cmd is NicknameCommand) {
					NicknameCommand nickname = (NicknameCommand) cmd;
					Debug.Log("Received nickname: " + nickname.nickname);
				}
				if (cmd is JoinCommand) {
					JoinCommand _join = (JoinCommand) cmd; // to avoid conflict with 'join' keyword
					if (clientID == 0) {
						clientID = _join.clientID;
						Debug.Log("Joined game! Client ID #" + clientID + " assigned!");
					} else {
						Debug.Log("New player with client ID #" + _join.clientID + " joined!");
					}
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
					Debug.Log("Received DISCONNECT from cient #" + disconnect.clientID);
				}
			}
		}
    }
	
	void OnApplicationQuit()
	{
		Network.disconnectFromServer();
	}
}
