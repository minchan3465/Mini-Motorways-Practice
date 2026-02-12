using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200009F RID: 159
public static class Diagnostics
{
	// Token: 0x060002AD RID: 685 RVA: 0x0000C95B File Offset: 0x0000AB5B
	[DebuggerHidden]
	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition)
	{
		if (!condition)
		{
			Diagnostics.FailAssert("Assertion failed!", Array.Empty<object>());
		}
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0000C96F File Offset: 0x0000AB6F
	[DebuggerHidden]
	[StringFormatMethod("message")]
	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string message, params object[] args)
	{
		if (!condition)
		{
			Diagnostics.FailAssert(message, args);
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0000C97B File Offset: 0x0000AB7B
	[StringFormatMethod("message")]
	[DebuggerHidden]
	public static void FailAssert(string message, params object[] args)
	{
		Diagnostics.Log.Critical("Assert", message, args);
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0000C989 File Offset: 0x0000AB89
	[StringFormatMethod("message")]
	[DebuggerHidden]
	public static void FailAssert(UnityEngine.Object contextObject, string message, params object[] args)
	{
		Diagnostics.Log.Critical("Assert", message, args);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0000C998 File Offset: 0x0000AB98
	[DebuggerHidden]
	[Conditional("UNITY_EDITOR")]
	private static void Break()
	{
		if (Diagnostics._breakCount == 0)
		{
			bool isFirst = false;
			object breakMutex = Diagnostics._breakMutex;
			lock (breakMutex)
			{
				Diagnostics._breakCount++;
				if (Diagnostics._breakCount == 1)
				{
					isFirst = true;
				}
			}
			if (isFirst && Debugger.IsAttached)
			{
				Debugger.Break();
			}
			breakMutex = Diagnostics._breakMutex;
			lock (breakMutex)
			{
				Diagnostics._breakCount--;
			}
		}
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000CA34 File Offset: 0x0000AC34
	// (set) Token: 0x060002B3 RID: 691 RVA: 0x0000CA3C File Offset: 0x0000AC3C
	public static bool IsTrackingExceptions
	{
		get
		{
			return Diagnostics._isTrackingExceptions;
		}
		set
		{
			if (Diagnostics._isTrackingExceptions != value)
			{
				Diagnostics._isTrackingExceptions = value;
				if (Diagnostics._isTrackingExceptions)
				{
					Application.logMessageReceived += Diagnostics.Exception.OnLogMessageReceived;
					return;
				}
				if (Diagnostics._isTrackingExceptions)
				{
					Application.logMessageReceived -= Diagnostics.Exception.OnLogMessageReceived;
				}
			}
		}
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[DebuggerHidden]
	[ContractAnnotation("false => false")]
	[ContractAnnotation("true => true")]
	public static bool Verify(bool condition)
	{
		return condition;
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	[ContractAnnotation("condition:true => true")]
	public static bool Verify(bool condition, string message)
	{
		return condition;
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:true => true")]
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	public static bool Verify(bool condition, string message, object param0)
	{
		return condition;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	[ContractAnnotation("condition:true => true")]
	public static bool Verify(bool condition, string message, object param0, object param1)
	{
		return condition;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:false => false")]
	[ContractAnnotation("condition:true => true")]
	[DebuggerHidden]
	public static bool Verify(bool condition, string message, object param0, object param1, object param2)
	{
		return condition;
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:false => false")]
	[ContractAnnotation("condition:true => true")]
	[DebuggerHidden]
	public static bool Verify(bool condition, string message, object param0, object param1, object param2, object param3)
	{
		return condition;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	[ContractAnnotation("condition:true => true")]
	public static bool Verify(bool condition, string message, object param0, object param1, object param2, object param3, object param4)
	{
		return condition;
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:true => true")]
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	public static bool Verify(bool condition, string message, object param0, object param1, object param2, object param3, object param4, object param5)
	{
		return condition;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:true => true")]
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message)
	{
		return condition;
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[DebuggerHidden]
	[ContractAnnotation("condition:false => false")]
	[ContractAnnotation("condition:true => true")]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message, object param0)
	{
		return condition;
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:false => false")]
	[DebuggerHidden]
	[ContractAnnotation("condition:true => true")]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message, object param0, object param1)
	{
		return condition;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:true => true")]
	[ContractAnnotation("condition:false => false")]
	[DebuggerHidden]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message, object param0, object param1, object param2)
	{
		return condition;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:false => false")]
	[DebuggerHidden]
	[ContractAnnotation("condition:true => true")]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message, object param0, object param1, object param2, object param3)
	{
		return condition;
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:true => true")]
	[ContractAnnotation("condition:false => false")]
	[DebuggerHidden]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message, object param0, object param1, object param2, object param3, object param4)
	{
		return condition;
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0000CA88 File Offset: 0x0000AC88
	[ContractAnnotation("condition:true => true")]
	[ContractAnnotation("condition:false => false")]
	[DebuggerHidden]
	public static bool Verify(bool condition, UnityEngine.Object contextObject, string message, object param0, object param1, object param2, object param3, object param4, object param5)
	{
		return condition;
	}

	// Token: 0x04000105 RID: 261
	private static int _breakCount = 0;

	// Token: 0x04000106 RID: 262
	private static readonly object _breakMutex = new object();

	// Token: 0x04000107 RID: 263
	private static bool _isTrackingExceptions;

	// Token: 0x020000A0 RID: 160
	public class AuditTrail
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0000CA9D File Offset: 0x0000AC9D
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x0000CAA5 File Offset: 0x0000ACA5
		public bool IsRecordingEvents { get; set; }

		// Token: 0x060002C6 RID: 710 RVA: 0x0000CAAE File Offset: 0x0000ACAE
		public void RecordEvent(string name, Diagnostics.AuditTrail.PopulateMetadata populateMetadata = null)
		{
			if (!this.IsRecordingEvents)
			{
				return;
			}
			this.CreateEvent(name, populateMetadata);
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000CAC4 File Offset: 0x0000ACC4
		public Diagnostics.AuditTrail.EventBlock OpenEvent(string name, Diagnostics.AuditTrail.PopulateMetadata populateMetadata = null)
		{
			if (!this.IsRecordingEvents)
			{
				return this._emptyEventBlock;
			}
			Diagnostics.AuditTrailEvent newEvent = this.CreateEvent(name, populateMetadata);
			this._openEvents.Push(newEvent);
			return new Diagnostics.AuditTrail.EventBlock(this, newEvent);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000CAFC File Offset: 0x0000ACFC
		public string ToJson()
		{
			List<object> entries = new List<object>();
			foreach (Diagnostics.AuditTrailEvent auditTrailEvent in this._rootEvents)
			{
				entries.Add(auditTrailEvent.ToJson());
			}
			return Json.Serialize(entries, false);
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000CB64 File Offset: 0x0000AD64
		private Diagnostics.AuditTrailEvent CreateEvent(string name, Diagnostics.AuditTrail.PopulateMetadata populateMetadata)
		{
			Diagnostics.AuditTrailEvent newEvent = new Diagnostics.AuditTrailEvent(name, populateMetadata);
			if (this._openEvents.Count > 0)
			{
				this._openEvents.Peek().AddChild(newEvent);
			}
			else
			{
				this._rootEvents.Add(newEvent);
			}
			this._allEvents.Add(newEvent);
			return newEvent;
		}

		// Token: 0x04000109 RID: 265
		private Stack<Diagnostics.AuditTrailEvent> _openEvents = new Stack<Diagnostics.AuditTrailEvent>();

		// Token: 0x0400010A RID: 266
		private List<Diagnostics.AuditTrailEvent> _rootEvents = new List<Diagnostics.AuditTrailEvent>();

		// Token: 0x0400010B RID: 267
		private List<Diagnostics.AuditTrailEvent> _allEvents = new List<Diagnostics.AuditTrailEvent>();

		// Token: 0x0400010C RID: 268
		private Diagnostics.AuditTrail.EventBlock _emptyEventBlock = new Diagnostics.AuditTrail.EventBlock();

		// Token: 0x020000A1 RID: 161
		// (Invoke) Token: 0x060002CC RID: 716
		public delegate void PopulateMetadata(Dictionary<string, string> metadata);

		// Token: 0x020000A2 RID: 162
		public class EventBlock : IDisposable
		{
			// Token: 0x060002CF RID: 719 RVA: 0x0000CBE7 File Offset: 0x0000ADE7
			public EventBlock(Diagnostics.AuditTrail auditTrail, Diagnostics.AuditTrailEvent openEvent)
			{
				this._auditTrail = auditTrail;
				this._openEvent = openEvent;
			}

			// Token: 0x060002D0 RID: 720 RVA: 0x0000CBFD File Offset: 0x0000ADFD
			public EventBlock()
			{
				this._auditTrail = null;
				this._openEvent = null;
				this._hasClosed = true;
			}

			// Token: 0x060002D1 RID: 721 RVA: 0x0000CC1C File Offset: 0x0000AE1C
			public void Close()
			{
				if (!this._hasClosed)
				{
					this._hasClosed = true;
					if (Diagnostics.Verify(this._auditTrail._openEvents.Count > 0 && this._auditTrail._openEvents.Peek() == this._openEvent))
					{
						this._auditTrail._openEvents.Pop();
					}
				}
			}

			// Token: 0x060002D2 RID: 722 RVA: 0x0000CC7E File Offset: 0x0000AE7E
			public void Dispose()
			{
				this.Close();
			}

			// Token: 0x0400010D RID: 269
			private readonly Diagnostics.AuditTrail _auditTrail;

			// Token: 0x0400010E RID: 270
			private readonly Diagnostics.AuditTrailEvent _openEvent;

			// Token: 0x0400010F RID: 271
			private bool _hasClosed;
		}
	}

	// Token: 0x020000A3 RID: 163
	public class AuditTrailEvent
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0000CC86 File Offset: 0x0000AE86
		public int Id { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000CC8E File Offset: 0x0000AE8E
		public DateTime Timestamp { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x0000CC96 File Offset: 0x0000AE96
		public string Name { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000CC9E File Offset: 0x0000AE9E
		public IReadOnlyDictionary<string, string> Metadata
		{
			get
			{
				return this._metadata;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x0000CCA6 File Offset: 0x0000AEA6
		// (set) Token: 0x060002D8 RID: 728 RVA: 0x0000CCAE File Offset: 0x0000AEAE
		public Diagnostics.AuditTrailEvent Parent { get; private set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x0000CCB7 File Offset: 0x0000AEB7
		public IReadOnlyList<Diagnostics.AuditTrailEvent> Children
		{
			get
			{
				return this._children;
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
		public AuditTrailEvent(string name, Diagnostics.AuditTrail.PopulateMetadata populateMetadata)
		{
			this.Id = Diagnostics.AuditTrailEvent.NextId;
			Diagnostics.AuditTrailEvent.NextId++;
			this.Timestamp = DateTime.Now;
			this.Name = name;
			if (populateMetadata != null)
			{
				populateMetadata(this._metadata);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000CD21 File Offset: 0x0000AF21
		public void AddChild(Diagnostics.AuditTrailEvent childEvent)
		{
			childEvent.Parent = this;
			this._children.Add(childEvent);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000CD38 File Offset: 0x0000AF38
		public object ToJson()
		{
			Dictionary<string, object> jsonEvent = new Dictionary<string, object>();
			jsonEvent["name"] = this.Name;
			jsonEvent["timestamp"] = this.Timestamp.ToString(DateTimeFormatInfo.InvariantInfo);
			if (this._metadata.Count > 0)
			{
				Dictionary<string, object> jsonMetadata = new Dictionary<string, object>();
				jsonEvent["metadata"] = jsonMetadata;
				foreach (KeyValuePair<string, string> metadatum in this._metadata)
				{
					jsonMetadata[metadatum.Key] = metadatum.Value;
				}
			}
			if (this._children.Count > 0)
			{
				List<object> jsonChildren = new List<object>();
				jsonEvent["children"] = jsonChildren;
				foreach (Diagnostics.AuditTrailEvent child in this._children)
				{
					jsonChildren.Add(child.ToJson());
				}
			}
			return jsonEvent;
		}

		// Token: 0x04000114 RID: 276
		private readonly Dictionary<string, string> _metadata = new Dictionary<string, string>();

		// Token: 0x04000115 RID: 277
		private readonly List<Diagnostics.AuditTrailEvent> _children = new List<Diagnostics.AuditTrailEvent>();

		// Token: 0x04000116 RID: 278
		private static int NextId = 1;
	}

	// Token: 0x020000A4 RID: 164
	public class StorageAuditTrail : Diagnostics.AuditTrail
	{
	}

	// Token: 0x020000A5 RID: 165
	public static class Exception
	{
		// Token: 0x060002DF RID: 735 RVA: 0x0000CE6C File Offset: 0x0000B06C
		public static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				Diagnostics.Exception.LastException = condition;
				Diagnostics.Exception.LastExceptionStackTrace = stackTrace;
			}
		}

		// Token: 0x04000117 RID: 279
		public static string LastException;

		// Token: 0x04000118 RID: 280
		public static string LastExceptionStackTrace;
	}

	// Token: 0x020000A6 RID: 166
	public static class File
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000CE7E File Offset: 0x0000B07E
		public static bool CanWrite
		{
			get
			{
				if (!Diagnostics.File.hasCheckedDirectory)
				{
					Diagnostics.File.hasCheckedDirectory = true;
					Diagnostics.File.doesDirectoryExist = Directory.Exists(Diagnostics.File.DiagnosticsDirectory);
				}
				return Diagnostics.File.doesDirectoryExist;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000CEA1 File Offset: 0x0000B0A1
		public static string Path
		{
			get
			{
				return Diagnostics.File.DiagnosticsDirectory;
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000CEA8 File Offset: 0x0000B0A8
		public static string GetFullPath(string filename)
		{
			if (Diagnostics.File.CanWrite)
			{
				return System.IO.Path.Combine(Diagnostics.File.DiagnosticsDirectory, filename);
			}
			return null;
		}

		// Token: 0x04000119 RID: 281
		private static readonly string DiagnosticsDirectory = System.IO.Path.Combine(Application.persistentDataPath, "diagnostics");

		// Token: 0x0400011A RID: 282
		private static bool hasCheckedDirectory;

		// Token: 0x0400011B RID: 283
		private static bool doesDirectoryExist;
	}

	// Token: 0x020000A7 RID: 167
	public static class Hierarchy
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x0000CED4 File Offset: 0x0000B0D4
		public static Diagnostics.HierarchyNode Root
		{
			get
			{
				return Diagnostics.Hierarchy.root;
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000CEDB File Offset: 0x0000B0DB
		public static void Clear()
		{
			if (Diagnostics.Hierarchy.root != null)
			{
				UnityEngine.Object.DestroyImmediate(Diagnostics.Hierarchy.root.GameObject);
				Diagnostics.Hierarchy.root = null;
			}
		}

		// Token: 0x0400011C RID: 284
		private static Diagnostics.HierarchyNode root;
	}

	// Token: 0x020000A8 RID: 168
	public class HierarchyNode
	{
		// Token: 0x060002E6 RID: 742 RVA: 0x0000CEF9 File Offset: 0x0000B0F9
		public static Diagnostics.HierarchyNode CreateNode(string name, Diagnostics.HierarchyNode parent = null)
		{
			return new Diagnostics.HierarchyNode(name, parent);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000CF04 File Offset: 0x0000B104
		private HierarchyNode(string name, Diagnostics.HierarchyNode parent = null)
		{
			if (parent == null)
			{
				this._gameObject = GameObject.Find("/" + name);
			}
			else
			{
				Transform childTransform = parent.GameObject.transform.Find(name);
				if (childTransform != null)
				{
					this._gameObject = childTransform.gameObject;
				}
			}
			if (this._gameObject == null)
			{
				this._gameObject = new GameObject(name);
				if (parent != null)
				{
					Transform parentTransform = parent.GameObject.transform;
					int childIndex = 0;
					while (childIndex < parentTransform.childCount && name.CompareTo(parentTransform.GetChild(childIndex).name) >= 0)
					{
						childIndex++;
					}
					this._gameObject.transform.SetParent(parentTransform);
					this._gameObject.transform.SetSiblingIndex(childIndex);
				}
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000CFCA File Offset: 0x0000B1CA
		public Diagnostics.HierarchyNode GetChild(string name)
		{
			return Diagnostics.HierarchyNode.CreateNode(name, this);
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000CFD3 File Offset: 0x0000B1D3
		public GameObject GameObject
		{
			get
			{
				return this._gameObject;
			}
		}

		// Token: 0x0400011D RID: 285
		private GameObject _gameObject;
	}

	// Token: 0x020000A9 RID: 169
	public static class Log
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000CFDB File Offset: 0x0000B1DB
		// (set) Token: 0x060002EB RID: 747 RVA: 0x0000CFE8 File Offset: 0x0000B1E8
		public static bool IsRecordingLog
		{
			get
			{
				return Diagnostics.Log._recordedLogLines != null;
			}
			set
			{
				if (value)
				{
					if (Diagnostics.Log._recordedLogLines == null)
					{
						Diagnostics.Log._recordedLogLines = new List<string>();
						Application.logMessageReceived += Diagnostics.Log.OnLogMessageReceived;
						return;
					}
				}
				else if (Diagnostics.Log._recordedLogLines != null)
				{
					Diagnostics.Log._recordedLogLines = null;
					Application.logMessageReceived -= Diagnostics.Log.OnLogMessageReceived;
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000D039 File Offset: 0x0000B239
		[CanBeNull]
		public static byte[] RecordedLog
		{
			get
			{
				if (Diagnostics.Log._recordedLogLines != null)
				{
					return Encoding.UTF8.GetBytes(string.Join("\n", Diagnostics.Log._recordedLogLines));
				}
				return null;
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000D05D File Offset: 0x0000B25D
		[DebuggerHidden]
		public static void Info(string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(Diagnostics.Log.Level.Info, channel, message, args);
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000D068 File Offset: 0x0000B268
		[DebuggerHidden]
		public static void Warn(string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(Diagnostics.Log.Level.Warn, channel, message, args);
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000D073 File Offset: 0x0000B273
		[DebuggerHidden]
		public static void Error(string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(Diagnostics.Log.Level.Error, channel, message, args);
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000D07E File Offset: 0x0000B27E
		[DebuggerHidden]
		public static void Critical(string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(Diagnostics.Log.Level.Critical, channel, message, args);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x000022F5 File Offset: 0x000004F5
		[DebuggerHidden]
		public static void Message(Diagnostics.Log.Level level, string channel, string message, params object[] args)
		{
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000D089 File Offset: 0x0000B289
		[DebuggerHidden]
		public static void Info(UnityEngine.Object contextObject, string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(contextObject, Diagnostics.Log.Level.Info, channel, message, args);
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0000D095 File Offset: 0x0000B295
		[DebuggerHidden]
		public static void Warn(UnityEngine.Object contextObject, string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(contextObject, Diagnostics.Log.Level.Warn, channel, message, args);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000D0A1 File Offset: 0x0000B2A1
		[DebuggerHidden]
		public static void Error(UnityEngine.Object contextObject, string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(contextObject, Diagnostics.Log.Level.Error, channel, message, args);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000D0AD File Offset: 0x0000B2AD
		[DebuggerHidden]
		public static void Critical(UnityEngine.Object contextObject, string channel, string message, params object[] args)
		{
			Diagnostics.Log.Message(contextObject, Diagnostics.Log.Level.Critical, channel, message, args);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000D0B9 File Offset: 0x0000B2B9
		public static void MuteChannel(string channel)
		{
			if (!Diagnostics.Log.mutedChannels.Contains(channel))
			{
				Diagnostics.Log.mutedChannels.Add(channel);
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000D0D3 File Offset: 0x0000B2D3
		public static void UnmuteChannel(string channel)
		{
			if (Diagnostics.Log.mutedChannels.Contains(channel))
			{
				Diagnostics.Log.mutedChannels.Remove(channel);
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x000022F5 File Offset: 0x000004F5
		[DebuggerHidden]
		public static void Message(UnityEngine.Object contextObject, Diagnostics.Log.Level level, string channel, string message, params object[] args)
		{
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000D0EE File Offset: 0x0000B2EE
		public static Diagnostics.Log.Channel OpenChannel(string name)
		{
			return new Diagnostics.Log.Channel(name);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000D0F8 File Offset: 0x0000B2F8
		private static void OnLogMessageReceived(string condition, string trace, LogType type)
		{
			Diagnostics.Log._recordedLogLines.Add(condition);
			bool hasSkippedBoilerplateStackFrames = false;
			string[] array = trace.Split(Environment.NewLine.ToCharArray());
			int i = 0;
			while (i < array.Length)
			{
				string line = array[i];
				if (hasSkippedBoilerplateStackFrames)
				{
					goto IL_45;
				}
				if (!line.StartsWith("Diagnostics/Log") && !line.StartsWith("UnityEngine.Debug:"))
				{
					hasSkippedBoilerplateStackFrames = true;
					goto IL_45;
				}
				IL_50:
				i++;
				continue;
				IL_45:
				Diagnostics.Log._recordedLogLines.Add(line);
				goto IL_50;
			}
			while (Diagnostics.Log._recordedLogLines.Count > 131072)
			{
				Diagnostics.Log._recordedLogLines.RemoveAt(0);
			}
		}

		// Token: 0x0400011E RID: 286
		private static readonly List<string> mutedChannels = new List<string>();

		// Token: 0x0400011F RID: 287
		private const int MaxRecordedLogLines = 131072;

		// Token: 0x04000120 RID: 288
		private static List<string> _recordedLogLines;

		// Token: 0x020000AA RID: 170
		public enum Level
		{
			// Token: 0x04000122 RID: 290
			Info,
			// Token: 0x04000123 RID: 291
			Warn,
			// Token: 0x04000124 RID: 292
			Error,
			// Token: 0x04000125 RID: 293
			Critical
		}

		// Token: 0x020000AB RID: 171
		public class Channel
		{
			// Token: 0x060002FC RID: 764 RVA: 0x0000D189 File Offset: 0x0000B389
			public Channel(string name)
			{
				this._name = name;
			}

			// Token: 0x1700006A RID: 106
			// (get) Token: 0x060002FD RID: 765 RVA: 0x0000D198 File Offset: 0x0000B398
			// (set) Token: 0x060002FE RID: 766 RVA: 0x0000D1A0 File Offset: 0x0000B3A0
			public bool IsMuted
			{
				get
				{
					return this._isMuted;
				}
				set
				{
					if (value != this._isMuted)
					{
						this._isMuted = value;
						if (this._isMuted)
						{
							Diagnostics.Log.MuteChannel(this._name);
							return;
						}
						Diagnostics.Log.UnmuteChannel(this._name);
					}
				}
			}

			// Token: 0x060002FF RID: 767 RVA: 0x0000D1D1 File Offset: 0x0000B3D1
			[DebuggerHidden]
			public void Info(string message, params object[] args)
			{
				this.Message(Diagnostics.Log.Level.Info, message, args);
			}

			// Token: 0x06000300 RID: 768 RVA: 0x0000D1DC File Offset: 0x0000B3DC
			[DebuggerHidden]
			public void Warn(string message, params object[] args)
			{
				this.Message(Diagnostics.Log.Level.Warn, message, args);
			}

			// Token: 0x06000301 RID: 769 RVA: 0x0000D1E7 File Offset: 0x0000B3E7
			[DebuggerHidden]
			public void Error(string message, params object[] args)
			{
				this.Message(Diagnostics.Log.Level.Error, message, args);
			}

			// Token: 0x06000302 RID: 770 RVA: 0x0000D1F2 File Offset: 0x0000B3F2
			[DebuggerHidden]
			public void Critical(string message, params object[] args)
			{
				this.Message(Diagnostics.Log.Level.Critical, message, args);
			}

			// Token: 0x06000303 RID: 771 RVA: 0x0000D1FD File Offset: 0x0000B3FD
			[DebuggerHidden]
			public void Message(Diagnostics.Log.Level level, string message, params object[] args)
			{
				Diagnostics.Log.Message(level, this._name, message, args);
			}

			// Token: 0x06000304 RID: 772 RVA: 0x0000D20D File Offset: 0x0000B40D
			[DebuggerHidden]
			public void Info(UnityEngine.Object contextObject, string message, params object[] args)
			{
				this.Message(contextObject, Diagnostics.Log.Level.Info, message, args);
			}

			// Token: 0x06000305 RID: 773 RVA: 0x0000D219 File Offset: 0x0000B419
			[DebuggerHidden]
			public void Warn(UnityEngine.Object contextObject, string message, params object[] args)
			{
				this.Message(contextObject, Diagnostics.Log.Level.Warn, message, args);
			}

			// Token: 0x06000306 RID: 774 RVA: 0x0000D225 File Offset: 0x0000B425
			[DebuggerHidden]
			public void Error(UnityEngine.Object contextObject, string message, params object[] args)
			{
				this.Message(contextObject, Diagnostics.Log.Level.Error, message, args);
			}

			// Token: 0x06000307 RID: 775 RVA: 0x0000D231 File Offset: 0x0000B431
			[DebuggerHidden]
			public void Critical(UnityEngine.Object contextObject, string message, params object[] args)
			{
				this.Message(contextObject, Diagnostics.Log.Level.Critical, message, args);
			}

			// Token: 0x06000308 RID: 776 RVA: 0x0000D23D File Offset: 0x0000B43D
			[DebuggerHidden]
			public void Message(UnityEngine.Object contextObject, Diagnostics.Log.Level level, string message, params object[] args)
			{
				Diagnostics.Log.Message(contextObject, level, this._name, message, args);
			}

			// Token: 0x04000126 RID: 294
			private readonly string _name;

			// Token: 0x04000127 RID: 295
			private bool _isMuted;
		}
	}

	// Token: 0x020000AC RID: 172
	public enum ReportOrigin
	{
		// Token: 0x04000129 RID: 297
		Local,
		// Token: 0x0400012A RID: 298
		Remote
	}

	// Token: 0x020000AD RID: 173
	public enum ReportState
	{
		// Token: 0x0400012C RID: 300
		Searching,
		// Token: 0x0400012D RID: 301
		Downloading,
		// Token: 0x0400012E RID: 302
		Error,
		// Token: 0x0400012F RID: 303
		Ready
	}

	// Token: 0x020000AE RID: 174
	public class ReportAttachment
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000D24F File Offset: 0x0000B44F
		// (set) Token: 0x0600030A RID: 778 RVA: 0x0000D257 File Offset: 0x0000B457
		public string Filename { get; private set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000D260 File Offset: 0x0000B460
		// (set) Token: 0x0600030C RID: 780 RVA: 0x0000D268 File Offset: 0x0000B468
		public string LocalFilepath { get; private set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000D271 File Offset: 0x0000B471
		// (set) Token: 0x0600030E RID: 782 RVA: 0x0000D279 File Offset: 0x0000B479
		public byte[] Data { get; private set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000D282 File Offset: 0x0000B482
		public int Size
		{
			get
			{
				if (this.Data != null)
				{
					return this.Data.Length;
				}
				if (!string.IsNullOrEmpty(this.LocalFilepath) && System.IO.File.Exists(this.LocalFilepath))
				{
					return (int)new FileInfo(this.LocalFilepath).Length;
				}
				return 0;
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000D2C2 File Offset: 0x0000B4C2
		public ReportAttachment(string filename, byte[] data)
		{
			this.Filename = filename;
			this.LocalFilepath = null;
			this.Data = data;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0000D2DF File Offset: 0x0000B4DF
		public ReportAttachment(string filename, string localFilepath)
		{
			this.Filename = filename;
			this.LocalFilepath = localFilepath;
			this.Data = null;
		}
	}

	// Token: 0x020000AF RID: 175
	public class ReportUpload
	{
		// Token: 0x06000312 RID: 786 RVA: 0x0000D2FC File Offset: 0x0000B4FC
		public ReportUpload(Diagnostics.Report report)
		{
			this.Id = 0;
			this.IsComplete = false;
			this.BytesUploaded = 0;
			this.BytesToUpload = report.TotalAttachmentSize;
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0000D325 File Offset: 0x0000B525
		// (set) Token: 0x06000314 RID: 788 RVA: 0x0000D32D File Offset: 0x0000B52D
		public int Id { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0000D336 File Offset: 0x0000B536
		// (set) Token: 0x06000316 RID: 790 RVA: 0x0000D33E File Offset: 0x0000B53E
		public bool IsComplete { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0000D347 File Offset: 0x0000B547
		// (set) Token: 0x06000318 RID: 792 RVA: 0x0000D34F File Offset: 0x0000B54F
		public int BytesUploaded { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0000D358 File Offset: 0x0000B558
		// (set) Token: 0x0600031A RID: 794 RVA: 0x0000D360 File Offset: 0x0000B560
		public int BytesToUpload { get; private set; }
	}

	// Token: 0x020000B0 RID: 176
	public class Report
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000D369 File Offset: 0x0000B569
		// (set) Token: 0x0600031C RID: 796 RVA: 0x0000D371 File Offset: 0x0000B571
		public int Id { get; private set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000D37A File Offset: 0x0000B57A
		// (set) Token: 0x0600031E RID: 798 RVA: 0x0000D382 File Offset: 0x0000B582
		public string Motive { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000D38B File Offset: 0x0000B58B
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000D393 File Offset: 0x0000B593
		public Diagnostics.ReportOrigin Origin { get; private set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000D39C File Offset: 0x0000B59C
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000D3A4 File Offset: 0x0000B5A4
		public Diagnostics.ReportState State { get; private set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0000D3B0 File Offset: 0x0000B5B0
		public int TotalAttachmentSize
		{
			get
			{
				int size = 0;
				foreach (Diagnostics.ReportAttachment attachment in this._attachments)
				{
					size += attachment.Size;
				}
				return size;
			}
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000D408 File Offset: 0x0000B608
		public Report()
		{
			this.Id = -1;
			this.Origin = Diagnostics.ReportOrigin.Local;
			this.State = Diagnostics.ReportState.Ready;
			this.SetMetadata("deviceModel", SystemInfo.deviceModel, true);
			this.SetMetadata("deviceType", SystemInfo.deviceType.ToString(), false);
			if (Application.isEditor)
			{
				this.SetMetadata("deviceName", SystemInfo.deviceName, true);
			}
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000D499 File Offset: 0x0000B699
		public void SetMetadata(string key, string value, bool index = false)
		{
			this._metadata[key] = value;
			if (index)
			{
				this._metadataIndices.Add(key);
			}
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000D4B8 File Offset: 0x0000B6B8
		public void AttachFile(string filename, string localFilepath)
		{
			this._attachments.Add(new Diagnostics.ReportAttachment(filename, localFilepath));
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000D4CC File Offset: 0x0000B6CC
		public void AttachFile(string filename, byte[] data)
		{
			this._attachments.Add(new Diagnostics.ReportAttachment(filename, data));
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000D4E0 File Offset: 0x0000B6E0
		public Diagnostics.ReportAttachment GetAttachment(int attachmentIndex)
		{
			return this._attachments[attachmentIndex];
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000D4EE File Offset: 0x0000B6EE
		public IEnumerable<Diagnostics.ReportAttachment> Attachments
		{
			get
			{
				return this._attachments;
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000D4F8 File Offset: 0x0000B6F8
		public Diagnostics.ReportUpload Upload()
		{
			if (!Diagnostics.Verify(this.Origin == Diagnostics.ReportOrigin.Local))
			{
				return null;
			}
			Diagnostics.ReportUpload upload = new Diagnostics.ReportUpload(this);
			Diagnostics.Report.GetCoroutineHost().StartCoroutine(this.DoUpload(upload));
			return upload;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000D534 File Offset: 0x0000B734
		public static Diagnostics.Report Download(int id)
		{
			Diagnostics.Report report = new Diagnostics.Report(id);
			Diagnostics.Report.GetCoroutineHost().StartCoroutine(report.DoDownload());
			return report;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000D55C File Offset: 0x0000B75C
		public static Diagnostics.Report SearchAndDownload(string metadataSearchKey, string metadataSearchValue)
		{
			Diagnostics.Report report = new Diagnostics.Report(metadataSearchKey, metadataSearchValue);
			Diagnostics.Report.GetCoroutineHost().StartCoroutine(report.DoSearch());
			return report;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000D583 File Offset: 0x0000B783
		private Report(int id)
		{
			this.Id = id;
			this.Origin = Diagnostics.ReportOrigin.Remote;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000D5BA File Offset: 0x0000B7BA
		private Report(string metadataSearchKey, string metadataSearchValue)
		{
			this.Id = -1;
			this.Origin = Diagnostics.ReportOrigin.Remote;
			this.SetMetadata(metadataSearchKey, metadataSearchValue, false);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000D5FA File Offset: 0x0000B7FA
		private static MonoBehaviour GetCoroutineHost()
		{
			if (Diagnostics.Report._coroutineHost == null)
			{
				Diagnostics.Report._coroutineHost = new GameObject().AddComponent<Diagnostics.Report.CoroutineHost>();
			}
			return Diagnostics.Report._coroutineHost;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000D61D File Offset: 0x0000B81D
		private IEnumerator DoUpload(Diagnostics.ReportUpload upload)
		{
			Dictionary<string, string> formData = new Dictionary<string, string>();
			foreach (string key in this._metadata.Keys)
			{
				string value = this._metadata[key];
				if (this._metadataIndices.Contains(key))
				{
					formData[key + "*"] = value;
				}
				else
				{
					formData[key] = value;
				}
			}
			formData["motive*"] = this.Motive;
			Diagnostics.Report.Log.Info("Uploading new report with metadata:", Array.Empty<object>());
			foreach (string metadataKey in formData.Keys)
			{
				Diagnostics.Report.Log.Info("\t{0}: {1}", new object[]
				{
					metadataKey,
					formData[metadataKey]
				});
			}
			List<IMultipartFormSection> formSections = new List<IMultipartFormSection>();
			foreach (KeyValuePair<string, string> formDatum in formData)
			{
				formSections.Add(new MultipartFormDataSection(formDatum.Key, formDatum.Value));
			}
			UnityWebRequest www = UnityWebRequest.Post("https://api.dinopoloclub.com/1/diagnostics/report/new/", formSections);
			yield return www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				Diagnostics.Report.Log.Error("Failed to upload report.", Array.Empty<object>());
				Diagnostics.Report.Log.Error("{0}", new object[]
				{
					www.error
				});
			}
			else
			{
				JSON.Dictionary response = JSON.ToDictionary(JSON.LoadFromString(www.downloadHandler.text));
				if (response == null || response.GetString("result") != "ok")
				{
					Diagnostics.Report.Log.Error("Failed to upload report, result '{0}'.", new object[]
					{
						response.GetString("result")
					});
				}
				else
				{
					int reportId = response.GetInt("reportId", 0);
					if (reportId <= 0)
					{
						Diagnostics.Report.Log.Error("Failed to upload report, invalid id {0}.", new object[]
						{
							reportId
						});
					}
					else
					{
						upload.Id = reportId;
						this.Id = reportId;
						Diagnostics.Report.Log.Info("Filed report with id {0}, uploading attachments.", new object[]
						{
							this.Id
						});
						foreach (Diagnostics.ReportAttachment attachment in this._attachments)
						{
							string filename = attachment.Filename;
							byte[] filedata = attachment.Data;
							Diagnostics.Report.Log.Info("Uploading {0} ({1} bytes).", new object[]
							{
								filename,
								filedata.Length
							});
							int attachmentBytesUploaded = 0;
							while (attachmentBytesUploaded < filedata.Length)
							{
								int chunkLength = Mathf.Min(filedata.Length - attachmentBytesUploaded, 524288);
								byte[] chunk = new byte[chunkLength];
								Array.Copy(filedata, attachmentBytesUploaded, chunk, 0, chunkLength);
								attachmentBytesUploaded += chunkLength;
								upload.BytesUploaded += chunkLength;
								formSections.Clear();
								formSections.Add(new MultipartFormFileSection(filename, chunk));
								Diagnostics.Report.Log.Info("Uploading chunk of {0} bytes.", new object[]
								{
									chunkLength
								});
								UnityWebRequest attachmentRequest = UnityWebRequest.Post(string.Format("{0}diagnostics/report/{1}/attachment/", "https://api.dinopoloclub.com/1/", this.Id), formSections);
								yield return attachmentRequest.SendWebRequest();
								if (attachmentRequest.result != UnityWebRequest.Result.Success)
								{
									Diagnostics.Report.Log.Error("Failed to upload attachment.", Array.Empty<object>());
									Diagnostics.Report.Log.Error("{0}", new object[]
									{
										attachmentRequest.error
									});
									break;
								}
								Diagnostics.Report.Log.Info("{0}", new object[]
								{
									attachmentRequest.downloadHandler.text
								});
								JSON.Dictionary uploadResponse = JSON.ToDictionary(JSON.LoadFromString(attachmentRequest.downloadHandler.text));
								string uploadResult = null;
								if (uploadResponse != null)
								{
									uploadResult = uploadResponse.GetString("result");
								}
								if (string.IsNullOrEmpty(uploadResult) || uploadResult != "ok")
								{
									Diagnostics.Report.Log.Info("Failed to upload attachment, result '{0}'.", new object[]
									{
										uploadResult
									});
									break;
								}
								Diagnostics.Report.Log.Info("Uploaded {0} / {1} bytes.", new object[]
								{
									attachmentBytesUploaded,
									filedata.Length
								});
								attachmentRequest = null;
							}
							filename = null;
							filedata = null;
						}
						List<Diagnostics.ReportAttachment>.Enumerator enumerator3 = default(List<Diagnostics.ReportAttachment>.Enumerator);
					}
				}
			}
			upload.IsComplete = true;
			yield break;
			yield break;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000D633 File Offset: 0x0000B833
		private IEnumerator DoSearch()
		{
			this.State = Diagnostics.ReportState.Searching;
			string searchKey = null;
			string searchValue = null;
			foreach (string metadataKey in this._metadata.Keys)
			{
				searchKey = metadataKey;
				searchValue = this._metadata[searchKey];
			}
			if (searchKey == null || searchValue == null)
			{
				this.State = Diagnostics.ReportState.Error;
				yield break;
			}
			UnityWebRequest www = UnityWebRequest.Get(string.Format("{0}diagnostics/report/search/?{1}={2}", "https://api.dinopoloclub.com/1/", searchKey, searchValue));
			yield return www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				Diagnostics.Report.Log.Error("Failed to search for report matching metadata {0} = {1}.", new object[]
				{
					searchKey,
					searchValue
				});
				Diagnostics.Report.Log.Error("{0}", new object[]
				{
					www.error
				});
				this.State = Diagnostics.ReportState.Error;
			}
			else
			{
				JSON.Dictionary response = JSON.ToDictionary(JSON.LoadFromString(www.downloadHandler.text));
				if (response == null || response.GetString("result") != "ok")
				{
					Diagnostics.Report.Log.Error("Failed to search for report, result '{0}'.", new object[]
					{
						response.GetString("result")
					});
					this.State = Diagnostics.ReportState.Error;
				}
				else
				{
					JSON.Array reportIds = response.GetArray("reportIds");
					if (reportIds == null || reportIds.Count == 0)
					{
						Diagnostics.Report.Log.Error("Failed to find a report matching metadata {0} = {1}.", new object[]
						{
							searchKey,
							searchValue
						});
						this.State = Diagnostics.ReportState.Error;
						yield break;
					}
					this.Id = Convert.ToInt32(reportIds[0]);
					yield return Diagnostics.Report.GetCoroutineHost().StartCoroutine(this.DoDownload());
				}
				this.State = Diagnostics.ReportState.Ready;
			}
			yield break;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000D642 File Offset: 0x0000B842
		private IEnumerator DoDownload()
		{
			this.State = Diagnostics.ReportState.Downloading;
			UnityWebRequest www = UnityWebRequest.Get(string.Format("{0}diagnostics/report/{1}/", "https://api.dinopoloclub.com/1/", this.Id));
			yield return www.SendWebRequest();
			if (www.result != UnityWebRequest.Result.Success)
			{
				Diagnostics.Report.Log.Error("Failed to download report {0}.", new object[]
				{
					this.Id
				});
				Diagnostics.Report.Log.Error("{0}", new object[]
				{
					www.error
				});
				this.State = Diagnostics.ReportState.Error;
			}
			else
			{
				JSON.Dictionary response = JSON.ToDictionary(JSON.LoadFromString(www.downloadHandler.text));
				if (response == null || response.GetString("result") != "ok")
				{
					Diagnostics.Report.Log.Error("Failed to download report, result '{0}'.", new object[]
					{
						response.GetString("result")
					});
					this.State = Diagnostics.ReportState.Error;
				}
				else
				{
					this.Motive = response.GetString("name");
					response.GetDictionary("metadata");
					JSON.Array attachments = response.GetArray("attachments");
					if (attachments != null)
					{
						int num;
						for (int attachmentIndex = 0; attachmentIndex < attachments.Count; attachmentIndex = num)
						{
							string filename = attachments.GetString(attachmentIndex);
							if (!string.IsNullOrEmpty(filename))
							{
								www = new UnityWebRequest(string.Format("{0}diagnostics/report/{1}/attachment/?filename={2}", "https://api.dinopoloclub.com/1/", this.Id, filename));
								www.method = "GET";
								string localFilepath = Path.Combine(Application.temporaryCachePath, filename);
								if (System.IO.File.Exists(localFilepath))
								{
									System.IO.File.Delete(localFilepath);
								}
								www.downloadHandler = new DownloadHandlerFile(localFilepath)
								{
									removeFileOnAbort = true
								};
								yield return www.SendWebRequest();
								if (www.result != UnityWebRequest.Result.Success)
								{
									Diagnostics.Report.Log.Error("Failed to download attachment '{0}'.", new object[]
									{
										filename
									});
									Debug.Log(www.error);
								}
								else
								{
									Diagnostics.Report.Log.Info("Downloaded attachment '{0}' to {1}", new object[]
									{
										filename,
										localFilepath
									});
									this.AttachFile(filename, localFilepath);
								}
								filename = null;
								localFilepath = null;
							}
							num = attachmentIndex + 1;
						}
					}
					attachments = null;
				}
				this.State = Diagnostics.ReportState.Ready;
			}
			yield break;
		}

		// Token: 0x0400013B RID: 315
		private readonly Dictionary<string, string> _metadata = new Dictionary<string, string>();

		// Token: 0x0400013C RID: 316
		private readonly HashSet<string> _metadataIndices = new HashSet<string>();

		// Token: 0x0400013D RID: 317
		private readonly List<Diagnostics.ReportAttachment> _attachments = new List<Diagnostics.ReportAttachment>();

		// Token: 0x0400013E RID: 318
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Report");

		// Token: 0x0400013F RID: 319
		private const string ApiUrl = "https://api.dinopoloclub.com/1/";

		// Token: 0x04000140 RID: 320
		private const int MaxUploadSize = 524288;

		// Token: 0x04000141 RID: 321
		private static MonoBehaviour _coroutineHost;

		// Token: 0x020000B1 RID: 177
		private class CoroutineHost : MonoBehaviour
		{
		}
	}
}
