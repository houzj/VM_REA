using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace VM.UnRedo;

public class PoolStack<T> : IEnumerable<T>, IEnumerable, INotifyPropertyChanged
{
	private ObservableCollection<T> sl;

	private uint length = 15u;

	public int Count => sl.Count;

	public ObservableCollection<T> EnumEntrysA
	{
		get
		{
			return sl;
		}
		set
		{
			if (sl != value)
			{
				sl = value;
				OnPropertyChanged("EnumEntrysA");
			}
		}
	}

	public string DisplayNameA { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	public PoolStack()
	{
		sl = new ObservableCollection<T>();
	}

	public void Clear()
	{
		sl.Clear();
	}

	public void Push(T item)
	{
		sl.Insert(0, item);
		if (Count > length)
		{
			sl.RemoveAt(Count - 1);
		}
		OnPropertyChanged("EnumEntrysA");
	}

	public List<T> ToList()
	{
		return sl.ToList();
	}

	public bool TryPush(T item, out object removeItem)
	{
		sl.Insert(0, item);
		bool result = false;
		if (Count > length)
		{
			removeItem = sl[Count - 1];
			sl.RemoveAt(Count - 1);
			result = true;
		}
		else
		{
			removeItem = null;
		}
		OnPropertyChanged("EnumEntrysA");
		return result;
	}

	public T TryRemoveLast()
	{
		T result = sl[Count - 1];
		sl.RemoveAt(Count - 1);
		return result;
	}

	public T Pop()
	{
		if (Count == 0)
		{
			throw new IndexOutOfRangeException();
		}
		T result = sl[0];
		sl.RemoveAt(0);
		OnPropertyChanged("EnumEntrysA");
		return result;
	}

	public bool Contains(T item)
	{
		return sl.Contains(item);
	}

	public T Peek()
	{
		if (Count == 0)
		{
			throw new IndexOutOfRangeException();
		}
		return sl[0];
	}

	public void SetLenght(uint len)
	{
		length = len;
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return sl.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return sl.GetEnumerator();
	}
}
