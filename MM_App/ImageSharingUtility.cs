using System;
using System.IO;
using UnityEngine;

// Token: 0x02000260 RID: 608
public static class ImageSharingUtility
{
	// Token: 0x06000E63 RID: 3683 RVA: 0x00030C20 File Offset: 0x0002EE20
	public static bool SaveScreenshotToPictures(string name, string parentFolder, int superSize = 1)
	{
		string screenshotPath = ImageSharingUtility.GetUniqueImagePath(name, parentFolder);
		if (screenshotPath == null)
		{
			return false;
		}
		ScreenCapture.CaptureScreenshot(screenshotPath, superSize);
		return true;
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00030C44 File Offset: 0x0002EE44
	public static bool SaveGIF(byte[] gifData, string name, string parentFolder)
	{
		string gifPath = ImageSharingUtility.GetUniqueImagePath(name, parentFolder);
		ImageSharingUtility.Log.Info("Saving gif to {0}", new object[]
		{
			gifPath
		});
		bool result;
		try
		{
			using (FileStream fileStream = File.Open(gifPath, FileMode.Create))
			{
				if (fileStream == null)
				{
					result = false;
				}
				else
				{
					fileStream.Write(gifData, 0, gifData.Length);
					fileStream.Close();
					ImageSharingUtility.Log.Info("Saved gif to {0}!", new object[]
					{
						gifPath
					});
					result = true;
				}
			}
		}
		catch (Exception e)
		{
			ImageSharingUtility.Log.Warn("Failed to save GIF. {0}", new object[]
			{
				e
			});
			result = false;
		}
		return result;
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x00030CF4 File Offset: 0x0002EEF4
	public static bool SaveScreenshotToPictures(Texture2D screenshot, string name, string parentFolder)
	{
		string screenshotPath = ImageSharingUtility.GetUniqueImagePath(name, parentFolder);
		if (screenshotPath == null)
		{
			return false;
		}
		byte[] bytes = screenshot.EncodeToPNG();
		bool result;
		try
		{
			using (FileStream pngStream = File.Open(screenshotPath, FileMode.Create))
			{
				if (pngStream == null)
				{
					result = false;
				}
				else
				{
					using (BinaryWriter pngWriter = new BinaryWriter(pngStream))
					{
						if (pngWriter == null)
						{
							result = false;
						}
						else
						{
							pngWriter.Write(bytes);
							ImageSharingUtility.Log.Info("Wrote screenshot to {0}", new object[]
							{
								screenshotPath
							});
							result = true;
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			ImageSharingUtility.Log.Info("Failed to save screenshot.", Array.Empty<object>());
			ImageSharingUtility.Log.Info(e.ToString(), Array.Empty<object>());
			result = false;
		}
		return result;
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x00030DC8 File Offset: 0x0002EFC8
	public static string GetUniqueImagePath(string filename, string parentFolder)
	{
		string originalFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), parentFolder);
		string folderPath = originalFolderPath;
		int index = 1;
		for (;;)
		{
			if (Directory.Exists(folderPath))
			{
				if (ImageSharingUtility.HasWriteAccessToFolder(folderPath))
				{
					break;
				}
			}
			else
			{
				try
				{
					Directory.CreateDirectory(folderPath);
					break;
				}
				catch (Exception e)
				{
					ImageSharingUtility.Log.Info("Unable to create a directory at {0}", new object[]
					{
						folderPath
					});
					ImageSharingUtility.Log.Info("Failed due to: {0}", new object[]
					{
						e
					});
				}
			}
			folderPath = string.Format("{0} {1}", originalFolderPath, index);
			index++;
			if (index > ImageSharingUtility.MaximumNumberofSaveAttempts)
			{
				goto Block_4;
			}
		}
		goto IL_85;
		Block_4:
		return null;
		IL_85:
		string extension = "";
		int extensionIndex = filename.LastIndexOf('.');
		if (extensionIndex > 0)
		{
			extension = filename.Substring(extensionIndex);
			filename = filename.Substring(0, extensionIndex);
		}
		index = 0;
		for (;;)
		{
			string imageFilename = string.Format("{0}{1}{2}", filename, (index == 0) ? "" : (" " + index.ToString()), extension);
			string imagePath = Path.Combine(folderPath, imageFilename);
			index++;
			if (index > ImageSharingUtility.MaximumNumberofSaveAttempts)
			{
				break;
			}
			if (!File.Exists(imagePath))
			{
				return imagePath;
			}
		}
		return null;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00030EE4 File Offset: 0x0002F0E4
	private static bool HasWriteAccessToFolder(string folderPath)
	{
		try
		{
			string testFilename = Path.Combine(folderPath, "accessCheck");
			FileStream file = File.Create(testFilename);
			if (file == null)
			{
				return false;
			}
			file.Close();
			File.Delete(testFilename);
			return true;
		}
		catch (Exception e)
		{
			ImageSharingUtility.Log.Info("Unable to write to this directory! {0}", new object[]
			{
				folderPath
			});
			ImageSharingUtility.Log.Info("Failed due to: {0}", new object[]
			{
				e
			});
		}
		return false;
	}

	// Token: 0x04000881 RID: 2177
	public static Diagnostics.Log.Channel Log = new Diagnostics.Log.Channel("ImageSharingUtility");

	// Token: 0x04000882 RID: 2178
	public static readonly string PNG = ".png";

	// Token: 0x04000883 RID: 2179
	public static readonly string GIF = ".gif";

	// Token: 0x04000884 RID: 2180
	public static readonly int MaximumNumberofSaveAttempts = 256;
}
