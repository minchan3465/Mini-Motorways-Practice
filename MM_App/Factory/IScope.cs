using System;
using System.IO;
using JetBrains.Annotations;

namespace Factory
{
	// Token: 0x020002FB RID: 763
	public interface IScope
	{
		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060012AA RID: 4778
		Assembler Assembler { get; }

		// Token: 0x060012AB RID: 4779
		bool Release();

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060012AC RID: 4780
		IScope ParentScope { get; }

		// Token: 0x060012AD RID: 4781
		void AddChildScope(IScope childScope, object establishingObject);

		// Token: 0x060012AE RID: 4782
		object Get(Type type);

		// Token: 0x060012AF RID: 4783
		T Get<T>() where T : class;

		// Token: 0x060012B0 RID: 4784
		void Assemble([NotNull] object unboundObject);

		// Token: 0x060012B1 RID: 4785
		bool Release(object obj);

		// Token: 0x060012B2 RID: 4786
		void Set(Type type, object variable);

		// Token: 0x060012B3 RID: 4787
		void Set<T>(object variable);

		// Token: 0x060012B4 RID: 4788
		void Unset(Type type);

		// Token: 0x060012B5 RID: 4789
		T Import<T>(BinaryReader reader) where T : class;

		// Token: 0x060012B6 RID: 4790
		object Import(BinaryReader reader);

		// Token: 0x060012B7 RID: 4791
		bool Export(object obj, BinaryWriter writer);

		// Token: 0x060012B8 RID: 4792
		void Subscribe(IScopeObserver newObserver);

		// Token: 0x060012B9 RID: 4793
		void Unsubscribe(IScopeObserver oldObserver);
	}
}
