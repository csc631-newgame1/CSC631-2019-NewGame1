using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

/* Quick note on NetworkCommands;
 * Each network command MUST contain a character string describing it's command ID followed by a "$" character
 */
 
public abstract class NetworkCommand
{	
	protected enum Directive { START=0, END, DISCONNECT, ECHO, NONE }

    public abstract string getString();
	protected Directive directive;
	
	public static byte[] assembleCommandBytes(NetworkCommand cmd)
	{
		byte dirByte = (byte) cmd.directive;
		byte[] msgBytes = Encoding.ASCII.GetBytes(cmd.getString());
		byte[] lenBytes = new byte[2] { (byte)(msgBytes.Length >> 8), (byte)(msgBytes.Length) };
		byte[] msgBody = new byte[3 + msgBytes.Length];
		
		msgBody[0] = dirByte;
		msgBody[1] = lenBytes[0];
		msgBody[2] = lenBytes[1];
		Array.Copy(msgBytes, 0, msgBody, 3, msgBytes.Length);
		return msgBody;
	}
}

/* #############################
 * ## ECHO Directive commands ##
 * #############################
 * Echo commands simply echo their message to every client currently connected to the server
 */

// MOVE commands signal a move from location a to location b on the game map
public class MoveCommand : NetworkCommand
{
	public const int ID = 0;
	
	public Pos a, b;
	public MoveCommand(Pos a, Pos b)
	{
		this.directive = Directive.ECHO;
		this.a = a;
		this.b = b;
	}
	public override string getString() { return ID + "$" + a.x + "," + a.y + "," + b.x + "," + b.y; }
	// parse everything to the right of the '$' in the string returned by getString() for this object
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		int[] pts = Array.ConvertAll(cmdString.Split(','), int.Parse);
		return new MoveCommand(new Pos(pts[0], pts[1]), new Pos(pts[2], pts[3]));
	}
}

// READY commands signal player readiness, typically in the lobby or at the end of a turn
public class ReadyCommand : NetworkCommand
{
	public const int ID = 1;
	
	private int clientID;
	public ReadyCommand(int clientID)
	{
		this.directive = Directive.ECHO;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	// parse everything to the right of the '$' in the string returned by getString() for this object
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new ReadyCommand(Int32.Parse(cmdString));
	}
}

// NICKNAME commands signal a change in the nickname for a given player
public class NicknameCommand : NetworkCommand
{
	public const int ID = 4;
	
	public int clientID;
	public string nickname;
	public NicknameCommand(string nickname, int clientID)
	{
		this.directive = Directive.ECHO;
		this.nickname = nickname;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + nickname + "," + clientID; }
	
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		string[] nnID = cmdString.Split(',');
		string nickname = nnID[0];
		int clientID = Int32.Parse(nnID[1]);
		return new NicknameCommand(nickname, clientID);
	}
}

/* #######################
 * ## NON-ECHO Commands ##
 * #######################
 * Non-standard commands that modify the state of the server
 * e.g; adding/removing a client, or blocking/allowing new clients
 */

// JOIN commands are sent by the server to all clients when a new client connects 
// JOIN commands are never sent by the client, only received from the server
// the getString() method should not be invoked client-side, but is there for reference's sake
public class JoinCommand : NetworkCommand
{
	public const int ID = -2;
	
	public int clientID;
	public JoinCommand(int clientID) 
	{
		this.directive = Directive.NONE;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new JoinCommand(Int32.Parse(cmdString));
	}
}

// START commands signal the beginning of the game
// In addition, they tell the server to stop accepting new clients
public class StartCommand : NetworkCommand
{
	public const int ID = 2;
	
	public StartCommand() 
	{
		this.directive = Directive.START;
	}
	public override string getString() { return ID + "$Directive.START"; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new StartCommand();
	}
}

// END commands signal the end of the game
// In addition, they tell the server to begin accepting new clients (provided there is room)
public class EndCommand : NetworkCommand
{
	public const int ID = 3;
	
	public EndCommand() 
	{
		this.directive = Directive.END;
	}
	public override string getString() { return ID + "$Directive.END"; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new EndCommand();
	}
}

// DISCONNECT commands are sent by the server to all clients when a client disconnects
// Unlike the JOIN command, the client may send this command to the server
// Only when the client is not responding should the server take responsibility for disconnecting them
public class DisconnectCommand : NetworkCommand
{
	public const int ID = -1;
	
	public int clientID;
	public DisconnectCommand(int clientID) 
	{
		this.directive = Directive.DISCONNECT;
		this.clientID = clientID;
	}
	public override string getString() { return ID + "$" + clientID; }
	public static NetworkCommand ConvertFromString(string cmdString)
	{
		return new DisconnectCommand(Int32.Parse(cmdString));
	}
}
