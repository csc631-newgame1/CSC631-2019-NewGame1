using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
	private static System.Random rng = new System.Random(MasterSeed);
	private static int _MasterSeed = 1111111;
	public static int MasterSeed {
		get {
			return _MasterSeed;
		}
		set {
			_MasterSeed = value;
			rng = new System.Random(_MasterSeed);
		}
	}
	public static int MapSeed {
		get {
			return rng.Next();
		}
	}
	public static float Volume = 0;
	public static float AnimationSpeed = 10;
}
