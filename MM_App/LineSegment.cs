using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public struct LineSegment
{
	// Token: 0x06000E69 RID: 3689 RVA: 0x00030F93 File Offset: 0x0002F193
	public LineSegment(Vector2 start, Vector2 end)
	{
		this._start = start;
		this._end = end;
		this._direction = Vector2.zero;
		this._length = 0f;
		this._dirty = true;
		this.Update();
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06000E6A RID: 3690 RVA: 0x00030FC8 File Offset: 0x0002F1C8
	public bool IsNull
	{
		get
		{
			return this._start.x == 0f && this._start.y == 0f && this._end.x == 0f && this._end.y == 0f;
		}
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0003101F File Offset: 0x0002F21F
	public Vector2 GetPosition(float t)
	{
		return this.Start + this.Direction * t;
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00031038 File Offset: 0x0002F238
	public float GetParametricCoordinate(Vector2 point)
	{
		if (!Mathf.Approximately(this._start.x, this._end.x))
		{
			return (point.x - this._start.x) / (this._end.x - this._start.x);
		}
		if (!Mathf.Approximately(this._start.y, this._end.y))
		{
			return (point.y - this._start.y) / (this._end.y - this._start.y);
		}
		return 0f;
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06000E6D RID: 3693 RVA: 0x000310DA File Offset: 0x0002F2DA
	// (set) Token: 0x06000E6E RID: 3694 RVA: 0x000310E2 File Offset: 0x0002F2E2
	public Vector2 Start
	{
		get
		{
			return this._start;
		}
		set
		{
			this._start = value;
			this._dirty = true;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06000E6F RID: 3695 RVA: 0x000310F2 File Offset: 0x0002F2F2
	// (set) Token: 0x06000E70 RID: 3696 RVA: 0x000310FA File Offset: 0x0002F2FA
	public Vector2 End
	{
		get
		{
			return this._end;
		}
		set
		{
			this._end = value;
			this._dirty = true;
		}
	}

	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06000E71 RID: 3697 RVA: 0x0003110A File Offset: 0x0002F30A
	public Vector2 Direction
	{
		get
		{
			if (this._dirty)
			{
				this.Update();
			}
			return this._direction;
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06000E72 RID: 3698 RVA: 0x00031120 File Offset: 0x0002F320
	public Vector2 Normal
	{
		get
		{
			if (this._dirty)
			{
				this.Update();
			}
			return this._direction.GetTangent();
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0003113B File Offset: 0x0002F33B
	public float Length
	{
		get
		{
			if (this._dirty)
			{
				this.Update();
			}
			return this._length;
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x00031151 File Offset: 0x0002F351
	public override string ToString()
	{
		return string.Format("[LineSegment: Start={0}, End={1}]", this._start, this._end);
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00031174 File Offset: 0x0002F374
	private void Update()
	{
		if (!this.IsNull)
		{
			this._direction = this.End - this.Start;
			this._length = this._direction.magnitude;
			this._direction /= this._length;
			this._dirty = false;
		}
	}

	// Token: 0x04000885 RID: 2181
	private Vector2 _start;

	// Token: 0x04000886 RID: 2182
	private Vector2 _end;

	// Token: 0x04000887 RID: 2183
	private Vector2 _direction;

	// Token: 0x04000888 RID: 2184
	private float _length;

	// Token: 0x04000889 RID: 2185
	private bool _dirty;

	// Token: 0x0400088A RID: 2186
	public static readonly LineSegment Null = new LineSegment(Vector2.zero, Vector2.zero);
}
