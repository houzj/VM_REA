using System;

namespace VMControls.Interface;

public class ValueEventArgs : EventArgs
{
	public object Value;

	public uint ExecuteCount;

	public string ValueType;
}
