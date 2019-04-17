using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using MapUtils;

public class NetworkManager : MonoBehaviour
{
    public static int clientID = 0;
    public static MapManager mapManager = null; // networkmanager needs to interface with the map

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        List<NetworkCommand> cmds = Network.getPendingCommands();

        if (cmds != null)
        {
            foreach (NetworkCommand cmd in cmds)
            {

                /*** IN-GAME ACTIONS ***/
                if (cmd is MoveCommand)
                {
                    MoveCommand move = (MoveCommand)cmd;
                    Debug.Log("Move received! From " + move.a + " to " + move.b);

                    mapManager.move(move.a, move.b);
                }
                if (cmd is AttackCommand)
                {
                    AttackCommand attack = (AttackCommand)cmd;
                    Debug.Log("Attack received! From " + attack.a + " to " + attack.b);

                    mapManager.attack(attack.a, attack.b);
                }
                if (cmd is WaitCommand)
                {
                    WaitCommand wait = (WaitCommand)cmd;
                    Debug.Log("Wait received! From client#" + wait.clientID);

                    Client waitingClient = Network.getPeer(wait.clientID);
                    waitingClient.playerObject.wait();
                }
                /*** LOBBY ACTIONS ***/
                if (cmd is ReadyCommand)
                {
                    ReadyCommand ready = (ReadyCommand)cmd;
                    Debug.Log("Received READY");

                    Client client = Network.getPeer(ready.clientID);
                    if (client == null)
                    {
                        Network.setPeer(ready.clientID);
                        client = Network.getPeer(ready.clientID);
                    }
                    client.ready = !client.ready;
                }
                if (cmd is NicknameCommand)
                {
                    NicknameCommand nickname = (NicknameCommand)cmd;
                    Debug.Log("Received nickname: " + nickname.nickname);

                    Client client = Network.getPeer(nickname.clientID);
                    if (client == null)
                    {
                        Network.setPeer(nickname.clientID);
                        client = Network.getPeer(nickname.clientID);
                    }
                    client.nickname = nickname.nickname;
                }
                if (cmd is ClassnameCommand)
                {
                    ClassnameCommand classname = (ClassnameCommand)cmd;
                    Debug.Log("Received classname: " + classname.classname);

                    Client client = Network.getPeer(classname.clientID);
                    if (client == null)
                    {
                        Network.setPeer(classname.clientID);
                        client = Network.getPeer(classname.clientID);
                    }
                    client.classname = classname.classname;
                }
                if (cmd is UpdateClientInfoCommand)
                { // called when a new player joins and needs to be synchronized
                    UpdateClientInfoCommand update = (UpdateClientInfoCommand)cmd;
                    Debug.Log("Received update from client#" + update.clientID);

                    Client client = Network.getPeer(update.clientID);
                    if (client == null)
                    {
                        Network.setPeer(update.clientID);
                        client = Network.getPeer(update.clientID);
                    }
                    client.nickname = update.nickname;
                    client.classname = update.classname;
                    client.ready = update.ready;
                }
                if (cmd is SetSeedCommand)
                {
                    SetSeedCommand seed = (SetSeedCommand)cmd;
                    Debug.Log("Received seed!");

                    Settings.Seed = seed.seed;
                }
                /*** NETWORK ACTIONS ***/
                if (cmd is JoinCommand)
                {
                    JoinCommand _join = (JoinCommand)cmd; // to avoid conflict with 'join' keyword
                    if (clientID == 0)
                    {
                        clientID = _join.clientID;
                        Debug.Log("Joined game! Client ID #" + clientID + " assigned!");
                    }
                    else
                    {
                        Debug.Log("New player with client ID #" + _join.clientID + " joined!");
                        Network.submitCommand(new UpdateClientInfoCommand(Network.getPeer(clientID)));
                        Network.submitCommand(new SetSeedCommand(Settings.Seed));
                    }
                    Network.setPeer(_join.clientID);
                }
                if (cmd is StartCommand)
                {
                    StartCommand start = (StartCommand)cmd;
                    Debug.Log("Received START");
                    if (SceneManager.GetActiveScene().name == "NewMenu")
                    { // only loads new scene if we're in the lobby
                        SceneManager.LoadScene("Procedural");
                    }
                }
                if (cmd is EndCommand)
                {
                    EndCommand end = (EndCommand)cmd;
                    Debug.Log("Received END");
                }
                if (cmd is DisconnectCommand)
                {
                    DisconnectCommand disconnect = (DisconnectCommand)cmd;
                    Debug.Log("Received DISCONNECT from client #" + disconnect.clientID);
                    Network.removePeer(disconnect.clientID);

                    if (clientID >= disconnect.clientID)
                    {
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