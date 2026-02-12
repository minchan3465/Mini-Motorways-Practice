using System;
using System.Collections.Generic;
using System.Globalization;
using Factory;

// Token: 0x02000203 RID: 515
public abstract class ForwardCompatibleJsonSaveData : IJsonSerializableSaveData, IStorable
{
	// Token: 0x06000C30 RID: 3120 RVA: 0x0002965D File Offset: 0x0002785D
	public void InitializeWithJson(JSON.Dictionary jsonDictionary)
	{
		this.UtcTimestamp = DateTimeExtensions.FromInts(jsonDictionary.GetInt("_utcSaveTime_low", 0), jsonDictionary.GetInt("_utcSaveTime_high", 0));
		this.LoadFromJson(jsonDictionary);
		this._sourceDictionary = jsonDictionary.Clone();
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x00029698 File Offset: 0x00027898
	public Dictionary<string, object> SerializeToJson()
	{
		Dictionary<string, object> jsonDictionary;
		if (this._sourceDictionary != null)
		{
			jsonDictionary = this._sourceDictionary.Clone().RawDictionary;
		}
		else
		{
			jsonDictionary = new Dictionary<string, object>();
		}
		int timestampLow;
		int timestampHigh;
		this.UtcTimestamp.ToInts(out timestampLow, out timestampHigh);
		jsonDictionary["_utcSaveTime_low"] = timestampLow;
		jsonDictionary["_utcSaveTime_high"] = timestampHigh;
		this.SaveToJson(jsonDictionary);
		return jsonDictionary;
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06000C32 RID: 3122 RVA: 0x000296FF File Offset: 0x000278FF
	// (set) Token: 0x06000C33 RID: 3123 RVA: 0x00029707 File Offset: 0x00027907
	public DateTime UtcTimestamp { get; set; } = DateTime.MinValue;

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06000C34 RID: 3124 RVA: 0x00029710 File Offset: 0x00027910
	// (set) Token: 0x06000C35 RID: 3125 RVA: 0x00029718 File Offset: 0x00027918
	public bool IsAuthoritative { get; set; }

	// Token: 0x06000C36 RID: 3126 RVA: 0x00029724 File Offset: 0x00027924
	public void Merge(IJsonSerializableSaveData otherData, bool autosave = true)
	{
		ForwardCompatibleJsonSaveData.Log.Info("Merging {0} with {1}.", new object[]
		{
			this,
			otherData
		});
		ForwardCompatibleJsonSaveData otherSaveData = otherData as ForwardCompatibleJsonSaveData;
		if (otherSaveData != null)
		{
			if (!this._isSourceAuthoritative || otherSaveData.IsAuthoritative)
			{
				JSON.Dictionary sourceDictionary = otherSaveData._sourceDictionary;
				this._sourceDictionary = ((sourceDictionary != null) ? sourceDictionary.Clone() : null);
				this._isSourceAuthoritative = otherSaveData.IsAuthoritative;
			}
			this._isMerging = true;
			this._changedDuringMerge = false;
			this.MergeValues(otherSaveData);
			this._isMerging = false;
			if (this._changedDuringMerge)
			{
				this._changedDuringMerge = false;
				if (otherSaveData.UtcTimestamp > this.UtcTimestamp)
				{
					ForwardCompatibleJsonSaveData.Log.Info("Data changed on merge, updating timestamp from {0} to {1}.", new object[]
					{
						this.UtcTimestamp.ToString(CultureInfo.InvariantCulture),
						otherSaveData.UtcTimestamp.ToString(CultureInfo.InvariantCulture)
					});
					this.UtcTimestamp = otherSaveData.UtcTimestamp;
				}
				if ((this._hasUnstoredChanges || !otherData.IsAuthoritative) && autosave)
				{
					ForwardCompatibleJsonSaveData.Log.Info("Rescheduling a store to persist the merged changes.", Array.Empty<object>());
					this._storage.Store(this, new StoreCompleted(this.OnStoreCompleted));
				}
				Action dataChanged = this.DataChanged;
				if (dataChanged == null)
				{
					return;
				}
				dataChanged();
			}
		}
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x00029870 File Offset: 0x00027A70
	public override string ToString()
	{
		return string.Format("[{0} UtcTimestamp={1}]", base.GetType().Name, this.UtcTimestamp.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06000C38 RID: 3128 RVA: 0x000298A8 File Offset: 0x00027AA8
	// (remove) Token: 0x06000C39 RID: 3129 RVA: 0x000298E0 File Offset: 0x00027AE0
	public event Action DataChanged;

	// Token: 0x06000C3A RID: 3130 RVA: 0x00029918 File Offset: 0x00027B18
	protected void OnValueChanged()
	{
		if (this._isMerging)
		{
			this._changedDuringMerge = true;
			return;
		}
		this.UtcTimestamp = DateTime.UtcNow;
		this._hasUnstoredChanges = true;
		this._storage.Store(this, new StoreCompleted(this.OnStoreCompleted));
		ForwardCompatibleJsonSaveData.Log.Info("Data changed on {0}, updating timestamp and scheduling a store.", new object[]
		{
			this
		});
		Action dataChanged = this.DataChanged;
		if (dataChanged == null)
		{
			return;
		}
		dataChanged();
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x00029989 File Offset: 0x00027B89
	protected T ChooseMax<T>(T ours, T theirs) where T : IComparable
	{
		if (ours.CompareTo(theirs) < 0)
		{
			return theirs;
		}
		return ours;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x000299A4 File Offset: 0x00027BA4
	protected T ChooseLatest<T>(T ours, T theirs, DateTime theirTimestamp)
	{
		if (!(this.UtcTimestamp >= theirTimestamp))
		{
			return theirs;
		}
		return ours;
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x000299B7 File Offset: 0x00027BB7
	private void OnStoreCompleted(StoreOperationResult result)
	{
		if (result != StoreOperationResult.Failed)
		{
			this._hasUnstoredChanges = false;
		}
	}

	// Token: 0x06000C3E RID: 3134
	protected abstract void LoadFromJson(JSON.Dictionary jsonDictionary);

	// Token: 0x06000C3F RID: 3135
	protected abstract void SaveToJson(Dictionary<string, object> jsonDictionary);

	// Token: 0x06000C40 RID: 3136
	protected abstract void MergeValues(ForwardCompatibleJsonSaveData otherSaveData);

	// Token: 0x04000712 RID: 1810
	private bool _isMerging;

	// Token: 0x04000713 RID: 1811
	private bool _changedDuringMerge;

	// Token: 0x04000714 RID: 1812
	private bool _hasUnstoredChanges;

	// Token: 0x04000715 RID: 1813
	private JSON.Dictionary _sourceDictionary;

	// Token: 0x04000716 RID: 1814
	private bool _isSourceAuthoritative;

	// Token: 0x04000717 RID: 1815
	[Dependency]
	private IPersistentStorageService _storage;

	// Token: 0x04000718 RID: 1816
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("JsonStorable");

	// Token: 0x04000719 RID: 1817
	private const string TimestampLowKey = "_utcSaveTime_low";

	// Token: 0x0400071A RID: 1818
	private const string TimestampHighKey = "_utcSaveTime_high";
}
