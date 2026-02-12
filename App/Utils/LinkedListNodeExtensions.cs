using System;
using System.Collections.Generic;

namespace Utils
{
	// Token: 0x0200027B RID: 635
	public static class LinkedListNodeExtensions
	{
		// Token: 0x06000FC0 RID: 4032 RVA: 0x00035464 File Offset: 0x00033664
		public static LinkedListNode<T> LoopingNext<T>(this LinkedListNode<T> node)
		{
			bool flag;
			return node.LoopingNext(out flag);
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x00035479 File Offset: 0x00033679
		public static LinkedListNode<T> LoopingNext<T>(this LinkedListNode<T> node, out bool didLoop)
		{
			if (node.Next == null)
			{
				didLoop = true;
				return node.List.First;
			}
			didLoop = false;
			return node.Next;
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x0003549C File Offset: 0x0003369C
		public static LinkedListNode<T> LoopingPrevious<T>(this LinkedListNode<T> node)
		{
			bool flag;
			return node.LoopingPrevious(out flag);
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x000354B1 File Offset: 0x000336B1
		public static LinkedListNode<T> LoopingPrevious<T>(this LinkedListNode<T> node, out bool didLoop)
		{
			if (node.Previous == null)
			{
				didLoop = true;
				return node.List.Last;
			}
			didLoop = false;
			return node.Previous;
		}
	}
}
