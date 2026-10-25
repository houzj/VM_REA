using System.ComponentModel;
using Apps.Interface;
using Hik.Xml;

namespace Apps.Data;

public class NotifyPropertyChangedBaseNode : BaseNode, INotifyPropertyChanged, IPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public NotifyPropertyChangedBaseNode()
	{
	}

	public NotifyPropertyChangedBaseNode(string nodeName)
		: base(nodeName)
	{
	}

	public virtual void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
