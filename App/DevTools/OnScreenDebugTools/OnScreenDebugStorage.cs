using System;
using System.IO;
using Factory;
using Factory.Pools;

namespace DevTools.OnScreenDebugTools
{
	// Token: 0x02000791 RID: 1937
	public class OnScreenDebugStorage : IReusable
	{
		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06003581 RID: 13697 RVA: 0x000F9C09 File Offset: 0x000F7E09
		private string StoragePath
		{
			get
			{
				return Path.Combine(this._hardwareCapabilities.PersistentStoragePath, "OnScreenDebugData");
			}
		}

		// Token: 0x06003582 RID: 13698 RVA: 0x000F9C20 File Offset: 0x000F7E20
		public string[] LoadAll()
		{
			if (Directory.Exists(this.StoragePath))
			{
				return Directory.GetFiles(this.StoragePath);
			}
			return null;
		}

		// Token: 0x06003583 RID: 13699 RVA: 0x000F9C3C File Offset: 0x000F7E3C
		public bool Exists(string filename)
		{
			return Directory.Exists(Path.Combine(this.StoragePath, filename));
		}

		// Token: 0x06003584 RID: 13700 RVA: 0x000F9C4F File Offset: 0x000F7E4F
		public bool Store(string filename, byte[] data)
		{
			if (!this.Exists(this.StoragePath))
			{
				Directory.CreateDirectory(this.StoragePath);
			}
			return OnScreenDebugStorage.Write(Path.Combine(this.StoragePath, filename), data);
		}

		// Token: 0x06003585 RID: 13701 RVA: 0x000F9C7D File Offset: 0x000F7E7D
		public bool Store(string filename, string[] data)
		{
			if (!this.Exists(this.StoragePath))
			{
				Directory.CreateDirectory(this.StoragePath);
			}
			return OnScreenDebugStorage.WriteLines(Path.Combine(this.StoragePath, filename), data);
		}

		// Token: 0x06003586 RID: 13702 RVA: 0x000F9CAB File Offset: 0x000F7EAB
		public void Delete(string filename)
		{
			File.Delete(filename);
		}

		// Token: 0x06003587 RID: 13703 RVA: 0x000F9CB4 File Offset: 0x000F7EB4
		public static bool LoadBytesFromFile(string filePath, out byte[] bytes)
		{
			bool result;
			try
			{
				bytes = File.ReadAllBytes(filePath);
				result = true;
			}
			catch (Exception e)
			{
				OnScreenDebugStorage.Log.Warn("Unable to read from {0}.\n{1}", new object[]
				{
					filePath,
					e
				});
				bytes = null;
				result = false;
			}
			return result;
		}

		// Token: 0x06003588 RID: 13704 RVA: 0x000F9D04 File Offset: 0x000F7F04
		private static bool Write(string filepath, byte[] data)
		{
			bool result;
			try
			{
				File.WriteAllBytes(filepath, data);
				result = true;
			}
			catch (Exception exception)
			{
				OnScreenDebugStorage.Log.Warn("Unable to write to {0}.\n{1}", new object[]
				{
					filepath,
					exception
				});
				result = false;
			}
			return result;
		}

		// Token: 0x06003589 RID: 13705 RVA: 0x000F9D50 File Offset: 0x000F7F50
		private static bool WriteLines(string filepath, string[] lines)
		{
			bool result;
			try
			{
				File.WriteAllLines(filepath, lines);
				result = true;
			}
			catch (Exception exception)
			{
				OnScreenDebugStorage.Log.Warn("Unable to write to {0}.\n{1}", new object[]
				{
					filepath,
					exception
				});
				result = false;
			}
			return result;
		}

		// Token: 0x0600358A RID: 13706 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x04002D7A RID: 11642
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("StartupScreen");

		// Token: 0x04002D7B RID: 11643
		[Dependency]
		private IHardwareCapabilities _hardwareCapabilities;

		// Token: 0x04002D7C RID: 11644
		private const string DataStorageDirectory = "OnScreenDebugData";
	}
}
