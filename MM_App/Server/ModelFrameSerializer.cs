using System;
using System.Collections.Generic;
using Factory;

namespace Server
{
	// Token: 0x02000292 RID: 658
	public class ModelFrameSerializer : PrimitiveSerializer
	{
		// Token: 0x06001021 RID: 4129 RVA: 0x000360CC File Offset: 0x000342CC
		public override bool Serialize(object obj, ExportContext context)
		{
			ISerializer frameSerializer = this.GetFrameSerializer(obj);
			if (frameSerializer == null)
			{
				return true;
			}
			int currentFrameIndex = 0;
			if (context.Scope != null)
			{
				currentFrameIndex = context.Scope.Get<Clock>().ModelFrameIndex;
			}
			return frameSerializer.Serialize((obj as Array).GetValue(1 - currentFrameIndex), context);
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x00036118 File Offset: 0x00034318
		public override object Deserialize(object existingObj, ImportContext context)
		{
			ISerializer frameSerializer = this.GetFrameSerializer(existingObj);
			if (frameSerializer == null)
			{
				return existingObj;
			}
			Array array = existingObj as Array;
			IFrame frame0 = array.GetValue(0) as IFrame;
			IFrame frame = array.GetValue(1) as IFrame;
			frameSerializer.Deserialize(frame0, context);
			frame0.CloneInto(frame, context.Scope);
			return existingObj;
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x0003616C File Offset: 0x0003436C
		private ISerializer GetFrameSerializer(object stateArrayObj)
		{
			Type frameType = stateArrayObj.GetType().GetElementType();
			ISerializer frameSerializer;
			if (this._frameSerializers.TryGetValue(frameType, out frameSerializer))
			{
				return frameSerializer;
			}
			if (frameType != typeof(EmptyModelFrame))
			{
				frameSerializer = new CompositeSerializer(frameType);
			}
			this._frameSerializers[frameType] = frameSerializer;
			return frameSerializer;
		}

		// Token: 0x04000E48 RID: 3656
		private Dictionary<Type, ISerializer> _frameSerializers = new Dictionary<Type, ISerializer>();
	}
}
