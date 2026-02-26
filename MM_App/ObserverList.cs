using System;
using System.Collections.Generic;

// Token: 0x020001FB RID: 507
public class ObserverList<T>
{
	// Token: 0x06000BDD RID: 3037 RVA: 0x0002874A File Offset: 0x0002694A
	public ObserverList(int capacity = 1)
	{
		this._observers = new List<T>(capacity);
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00028760 File Offset: 0x00026960
	public void Subscribe(T observer)
	{
		if (this._lockCount == 0)
		{
			if (!this._observers.Contains(observer))
			{
				this._observers.Add(observer);
				return;
			}
		}
		else
		{
			if (this._lockedSubscriptions != null && this._lockedSubscriptions.Contains(observer))
			{
				return;
			}
			if (this._lockedUnsubscriptions != null && this._lockedUnsubscriptions.Remove(observer))
			{
				return;
			}
			if (this._lockedSubscriptions == null)
			{
				this._lockedSubscriptions = new List<T>();
			}
			this._lockedSubscriptions.Add(observer);
		}
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x000287E0 File Offset: 0x000269E0
	public bool Unsubscribe(T observer)
	{
		if (this._lockCount == 0)
		{
			return this._observers.Remove(observer);
		}
		if (this._lockedUnsubscriptions != null && this._lockedUnsubscriptions.Contains(observer))
		{
			return false;
		}
		if (this._lockedSubscriptions != null && this._lockedSubscriptions.Remove(observer))
		{
			return true;
		}
		if (this._lockedUnsubscriptions == null)
		{
			this._lockedUnsubscriptions = new List<T>();
		}
		this._lockedUnsubscriptions.Add(observer);
		return true;
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00028852 File Offset: 0x00026A52
	public void UnsubscribeAll()
	{
		if (this._lockCount == 0)
		{
			this._observers.Clear();
			return;
		}
		if (this._lockedUnsubscriptions == null)
		{
			this._lockedUnsubscriptions = new List<T>();
		}
		this._lockedUnsubscriptions.AddRange(this._observers);
	}

	// Token: 0x06000BE1 RID: 3041 RVA: 0x0002888C File Offset: 0x00026A8C
	public ObserverList<T>.Enumerator GetEnumerator()
	{
		return new ObserverList<T>.Enumerator(this);
	}

	// Token: 0x06000BE2 RID: 3042 RVA: 0x00028894 File Offset: 0x00026A94
	private void Lock()
	{
		this._lockCount++;
	}

	// Token: 0x06000BE3 RID: 3043 RVA: 0x000288A4 File Offset: 0x00026AA4
	private void Unlock()
	{
		this._lockCount--;
		if (this._lockCount > 0)
		{
			return;
		}
		if (this._lockedUnsubscriptions != null)
		{
			foreach (T observer in this._lockedUnsubscriptions)
			{
				this.Unsubscribe(observer);
			}
			this._lockedUnsubscriptions.Clear();
		}
		if (this._lockedSubscriptions != null)
		{
			foreach (T observer2 in this._lockedSubscriptions)
			{
				this.Subscribe(observer2);
			}
			this._lockedSubscriptions.Clear();
		}
	}

	// Token: 0x040006D9 RID: 1753
	private readonly List<T> _observers;

	// Token: 0x040006DA RID: 1754
	private List<T> _lockedSubscriptions;

	// Token: 0x040006DB RID: 1755
	private List<T> _lockedUnsubscriptions;

	// Token: 0x040006DC RID: 1756
	private int _lockCount;

	// Token: 0x020001FC RID: 508
	public struct Enumerator
	{
		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x00028978 File Offset: 0x00026B78
		public T Current
		{
			get
			{
				return this._list._observers[this._index];
			}
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x00028990 File Offset: 0x00026B90
		public Enumerator(ObserverList<T> list)
		{
			this._list = list;
			this._index = -1;
			list.Lock();
			this._hasLock = true;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x000289B0 File Offset: 0x00026BB0
		public bool MoveNext()
		{
			if (this._index + 1 < this._list._observers.Count)
			{
				this._index++;
				return true;
			}
			if (this._hasLock)
			{
				this._list.Unlock();
				this._hasLock = false;
			}
			return false;
		}

		// Token: 0x040006DD RID: 1757
		private readonly ObserverList<T> _list;

		// Token: 0x040006DE RID: 1758
		private int _index;

		// Token: 0x040006DF RID: 1759
		private bool _hasLock;
	}
}
