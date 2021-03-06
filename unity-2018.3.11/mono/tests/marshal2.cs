//
// Tests for Marshal.StructureToPtr and PtrToStructure
//

using System;
using System.Runtime.InteropServices;

public class Test {


	[StructLayout (LayoutKind.Sequential)]
	public class SimpleObj {
		public int a;
		public int b;

		public void test () {}
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct SimpleStruct2 {
		public int a;
		public int b;
	}
	
	[StructLayout (LayoutKind.Sequential, CharSet=CharSet.Ansi)]
	public struct SimpleStruct {
		public int a;
		public bool bool1;
		public bool bool2;
		public int b;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst=2)] public short[] a1;
		[MarshalAs (UnmanagedType.ByValTStr, SizeConst=4)] public string s1;
		public SimpleStruct2 emb1;
		public SimpleObj emb2;
		public string s2;
		public double x;
		[MarshalAs (UnmanagedType.ByValArray, SizeConst=2)] public char[] a2;
	}

	[StructLayout (LayoutKind.Sequential, CharSet=CharSet.Unicode)]
	public struct ByValWStrStruct {
		[MarshalAs (UnmanagedType.ByValTStr, SizeConst=4)] public string s1;
		public int i;
	}
	
	public unsafe static int Main () {
		SimpleStruct ss = new SimpleStruct ();
		int size = Marshal.SizeOf (typeof (SimpleStruct));
		
		//if (size != 52)
		//return 1;
		
		IntPtr p = Marshal.AllocHGlobal (size);
		ss.a = 1;
		ss.bool1 = true;
		ss.bool2 = false;
		ss.b = 2;
		ss.a1 = new short [2];
		ss.a1 [0] = 6;
		ss.a1 [1] = 5;
		ss.s1 = "abcd";
		ss.emb1 = new SimpleStruct2 ();
		ss.emb1.a = 3;
		ss.emb1.b = 4;
		ss.emb2 = new SimpleObj ();
		ss.emb2.a = 10;
		ss.emb2.b = 11;
		ss.s2 = "just a test";
		ss.x = 1.5;
		ss.a2 = new char [2];
		ss.a2 [0] = 'a';
		ss.a2 [1] = 'b';
		
		Marshal.StructureToPtr (ss, p, false);
		Type t = ss.GetType ();
		
		if (Marshal.ReadInt32 (p, (int)Marshal.OffsetOf (t, "a")) != 1)
			return 1;
		if (Marshal.ReadInt32 (p, (int)Marshal.OffsetOf (t, "bool1")) != 1)
			return 2;
		if (Marshal.ReadInt32 (p, (int)Marshal.OffsetOf (t, "bool2")) != 0)
			return 3;
		if (Marshal.ReadInt32 (p, (int)Marshal.OffsetOf (t, "b")) != 2)
			return 4;
		if (Marshal.ReadInt16 (p, 16) != 6)
			return 5;
		if (Marshal.ReadInt16 (p, 18) != 5)
			return 6;
		if (Marshal.ReadByte (p, 20) != 97)
			return 7;
		if (Marshal.ReadByte (p, 21) != 98)
			return 8;
		if (Marshal.ReadByte (p, 22) != 99)
			return 9;
		if (Marshal.ReadByte (p, 23) != 0)
			return 10;
		if (Marshal.ReadInt32 (p, 24) != 3)
			return 11;
		if (Marshal.ReadInt32 (p, 28) != 4)
			return 12;
		if (Marshal.ReadInt32 (p, 32) != 10)
			return 13;
		if (Marshal.ReadInt32 (p, 36) != 11)
			return 14;
		if (Marshal.ReadByte (p, (int)Marshal.OffsetOf (t, "a2")) != 97)
			return 15;
		if (Marshal.ReadByte (p, (int)Marshal.OffsetOf (t, "a2") + 1) != 98)
			return 16;

		SimpleStruct cp = (SimpleStruct)Marshal.PtrToStructure (p, ss.GetType ());

		if (cp.a != 1)
			return 16;

		if (cp.bool1 != true)
			return 17;

		if (cp.bool2 != false)
			return 18;

		if (cp.b != 2)
			return 19;

		if (cp.a1 [0] != 6)
			return 20;
		
		if (cp.a1 [1] != 5)
			return 21;

		if (cp.s1 != "abc")
			return 22;
		
		if (cp.emb1.a != 3)
			return 23;

		if (cp.emb1.b != 4)
			return 24;

		if (cp.emb2.a != 10)
			return 25;

		if (cp.emb2.b != 11)
			return 26;

		if (cp.s2 != "just a test")
			return 27;

		if (cp.x != 1.5)
			return 28;

		if (cp.a2 [0] != 'a')
			return 29;

		if (cp.a2 [1] != 'b')
			return 30;

		/* ByValTStr with Unicode */
		ByValWStrStruct s = new ByValWStrStruct ();

		IntPtr p2 = Marshal.AllocHGlobal (Marshal.SizeOf (typeof (ByValWStrStruct)));
		Marshal.StructureToPtr(s, p2, false);

		/* Check that the ByValWStr is initialized correctly */
		for (int i = 0; i < 8; ++i)
			if (Marshal.ReadByte (p2, i) != 0)
				return 31;

		s.s1 = "ABCD";
		s.i = 55;

		Marshal.StructureToPtr(s, p2, false);

		ByValWStrStruct s2 = (ByValWStrStruct)Marshal.PtrToStructure (p2, typeof (ByValWStrStruct));

		/* The fourth char is lost because of null-termination */
		if (s2.s1 != "ABC")
			return 32;

		if (s2.i != 55)
			return 33;

		return 0;
	}
}

