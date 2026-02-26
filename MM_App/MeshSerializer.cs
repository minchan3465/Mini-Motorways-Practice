using System;
using Factory;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class MeshSerializer : PrimitiveSerializer
{
	// Token: 0x06000A6A RID: 2666 RVA: 0x000224D8 File Offset: 0x000206D8
	public override bool Serialize(object obj, ExportContext context)
	{
		Mesh mesh = obj as Mesh;
		if (mesh == null)
		{
			context.Writer.Write(0);
			context.Writer.Write(0);
			return obj == null;
		}
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector2[] uvs = mesh.uv;
		Color[] colors = mesh.colors;
		int vertexCount = vertices.Length;
		if (!Diagnostics.Verify(vertices.Length == normals.Length && vertices.Length == uvs.Length, "Expected mesh to have the same number of vertices, normals, and uvs."))
		{
			context.Writer.Write(0);
			context.Writer.Write(0);
			return false;
		}
		context.Writer.Write(vertexCount);
		foreach (Vector3 vertex in vertices)
		{
			context.Writer.Write(vertex.x);
			context.Writer.Write(vertex.y);
		}
		foreach (Vector3 normal in normals)
		{
			context.Writer.Write(normal.x);
			context.Writer.Write(normal.y);
		}
		foreach (Vector2 uv in uvs)
		{
			context.Writer.Write(uv.x);
			context.Writer.Write(uv.y);
		}
		foreach (Color color in colors)
		{
			context.Writer.Write(color.a);
		}
		int[] triangles = mesh.triangles;
		int indexCount = triangles.Length;
		context.Writer.Write(indexCount);
		foreach (int index in triangles)
		{
			context.Writer.Write(index);
		}
		return true;
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x000226C0 File Offset: 0x000208C0
	public override object Deserialize(object existingObj, ImportContext context)
	{
		int vertexCount = context.Reader.ReadInt32();
		Vector3[] vertices = null;
		Vector3[] normals = null;
		Vector2[] uvs = null;
		Color[] colors = null;
		if (vertexCount > 0)
		{
			vertices = new Vector3[vertexCount];
			for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
			{
				vertices[vertexIndex] = new Vector3(context.Reader.ReadSingle(), context.Reader.ReadSingle(), 0f);
			}
			normals = new Vector3[vertexCount];
			for (int normalIndex = 0; normalIndex < vertexCount; normalIndex++)
			{
				normals[normalIndex] = new Vector3(context.Reader.ReadSingle(), context.Reader.ReadSingle(), 0f);
			}
			uvs = new Vector2[vertexCount];
			for (int uvIndex = 0; uvIndex < vertexCount; uvIndex++)
			{
				uvs[uvIndex] = new Vector2(context.Reader.ReadSingle(), context.Reader.ReadSingle());
			}
			colors = new Color[vertexCount];
			for (int colorIndex = 0; colorIndex < vertexCount; colorIndex++)
			{
				colors[colorIndex] = new Color(1f, 1f, 1f, context.Reader.ReadSingle());
			}
		}
		int indexCount = context.Reader.ReadInt32();
		int[] indices = null;
		if (indexCount > 0)
		{
			indices = new int[indexCount];
			for (int indexIndex = 0; indexIndex < indexCount; indexIndex++)
			{
				indices[indexIndex] = context.Reader.ReadInt32();
			}
		}
		if (vertexCount > 0 && indexCount > 0)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.normals = normals;
			mesh.colors = colors;
			mesh.subMeshCount = 1;
			mesh.SetTriangles(indices, 0);
			return mesh;
		}
		return null;
	}
}
