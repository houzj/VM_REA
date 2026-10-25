using System.Collections;
using System.Collections.Generic;

namespace Apps.Data;

public class ItemData : NotifyPropertyChangedBaseNode
{
	public string name;

	public bool isVisible = true;

	public bool isEnabled = true;

	public bool isGroupData = false;

	public List<ItemData> items = new List<ItemData>();

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (name != value)
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}
	}

	public virtual bool IsVisible
	{
		get
		{
			return isVisible;
		}
		set
		{
			if (isVisible != value)
			{
				isVisible = value;
				OnPropertyChanged("IsVisible");
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			if (isEnabled != value)
			{
				isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}

	public bool IsGroupData => isGroupData;

	public virtual IList Items => items;

	public ItemData()
	{
	}

	public ItemData(string nodeName)
		: base(nodeName)
	{
	}

	public IList<ItemData> GetAllChildren()
	{
		List<ItemData> list = new List<ItemData>();
		if (items != null)
		{
			list.AddRange(items);
			foreach (ItemData item in items)
			{
				if (!item.isGroupData)
				{
					list.AddRange(item.GetAllChildren());
				}
			}
		}
		return list;
	}
}
