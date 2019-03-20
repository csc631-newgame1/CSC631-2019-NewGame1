using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/* The Network class serves as an abstraction layer on top of NetworkBase 
 * Where NetworkBase deals with raw I/O, Network deals with higher level game commands
 * Network is also the API visible to the rest of the code, whereas NetworkBase is exclusively private
 */
public class Network : NetworkBase
{
	// callback dictionary for parsing incoming commands
	private static Dictionary<int, Func<string, NetworkCommand>> decoderCallbacks = new Dictionary<int, Func<string, NetworkCommand>>()
	{
		[MoveCommand.ID] 		= MoveCommand		.ConvertFromString,
		[ReadyCommand.ID] 		= ReadyCommand		.ConvertFromString,
		[NicknameCommand.ID] 	= NicknameCommand	.ConvertFromString,
		[JoinCommand.ID] 		= JoinCommand		.ConvertFromString,
		[StartCommand.ID]		= StartCommand		.ConvertFromString,
		[EndCommand.ID] 		= EndCommand		.ConvertFromString,
		[DisconnectCommand.ID] 	= DisconnectCommand	.ConvertFromString
	};
	
    public static void submitCommand(NetworkCommand cmd)
	{
		submit(NetworkCommand.assembleCommandBytes(cmd));
	}
	
	public static List<NetworkCommand> getPendingCommands()
	{
		if (hasPending()) {
			
			List<NetworkCommand> cmds = new List<NetworkCommand>();
			foreach(byte[] command in getReceived()) {
				string[] cmdBits = Encoding.ASCII.GetString(command).Split('$');
				int ID = int.Parse(cmdBits[0]);
				NetworkCommand decodedCmd = decoderCallbacks[ID](cmdBits[1]);
				cmds.Add(decodedCmd);
			}
			return cmds;
		}
		else return null;
	}
	
	public static void connectToServer(string ip, int port)
	{
		connect(ip, port);
	}
	
	public static void disconnectFromServer()
	{
		disconnect();
	}
	
	public static void enable() 
	{
		networkEnabled = true;
	}
	
	public static void disable() 
	{
		networkEnabled = false;
	}
}