using System;
using UnityEngine;

// Token: 0x020001DD RID: 477
[RequireComponent(typeof(LineRenderer))]
public class VehicleTrailRenderer : MonoBehaviour
{
	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06000B5F RID: 2911 RVA: 0x00026D4A File Offset: 0x00024F4A
	public Renderer Renderer
	{
		get
		{
			return this._line;
		}
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06000B60 RID: 2912 RVA: 0x00026D52 File Offset: 0x00024F52
	// (set) Token: 0x06000B61 RID: 2913 RVA: 0x00026D5F File Offset: 0x00024F5F
	public Color Color
	{
		get
		{
			return this._line.startColor;
		}
		set
		{
			this._line.endColor = value;
			value.a = 0f;
			this._line.startColor = value;
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00026D85 File Offset: 0x00024F85
	private void OnEnable()
	{
		this._line.positionCount = 0;
		this._started = true;
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00026D9A File Offset: 0x00024F9A
	private void OnDisable()
	{
		this._line.positionCount = 0;
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00026DA8 File Offset: 0x00024FA8
	public void Tick(float deltaTime)
	{
		this._currentTime += deltaTime;
		if (this._started)
		{
			this._head = 0;
			this._tail = 0;
			this._points[this._tail] = base.transform.position;
			this._times[this._tail] = this._currentTime;
			this._lineChanged = true;
			this._started = false;
		}
		if (this._head < 199 && (base.transform.position - this._points[this._head]).sqrMagnitude > this._minSampleDistance * this._minSampleDistance)
		{
			this._head++;
			this._points[this._head] = base.transform.position;
			this._times[this._head] = this._currentTime;
			this._lineChanged = true;
			if (this._head >= 199)
			{
				this.RelocatePoints();
			}
		}
		while (this._currentTime - this._times[this._tail] > this._lifetime && this._tail < this._head)
		{
			this._tail++;
			this._lineChanged = true;
		}
		if (this._lineChanged)
		{
			this.AssignPointsToLineRenderer();
			return;
		}
		this._line.SetPosition(this._line.positionCount - 1, base.transform.position);
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00026F28 File Offset: 0x00025128
	private void AssignPointsToLineRenderer()
	{
		int numberOfPointsStored = this._head - this._tail + 1;
		Vector3[] currentPoints = new Vector3[numberOfPointsStored + 1];
		Array.Copy(this._points, this._tail, currentPoints, 0, numberOfPointsStored);
		currentPoints[numberOfPointsStored] = base.transform.position;
		this._line.positionCount = currentPoints.Length;
		this._line.SetPositions(currentPoints);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x00026F90 File Offset: 0x00025190
	private void RelocatePoints()
	{
		int numberOfPoints = this._head - this._tail + 1;
		if (numberOfPoints < 100)
		{
			Array.Copy(this._points, this._tail, this._points, 0, numberOfPoints);
			Array.Copy(this._times, this._tail, this._times, 0, numberOfPoints);
			this._tail = 0;
			this._head = numberOfPoints - 1;
			return;
		}
		Vector3[] currentPoints = new Vector3[numberOfPoints];
		Array.Copy(this._points, this._tail, currentPoints, 0, numberOfPoints);
		Array.Copy(currentPoints, this._points, currentPoints.Length);
		float[] currentTimes = new float[numberOfPoints];
		Array.Copy(this._times, this._tail, currentTimes, 0, numberOfPoints);
		Array.Copy(currentTimes, this._times, currentTimes.Length);
		this._tail = 0;
		this._head = numberOfPoints - 1;
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00027059 File Offset: 0x00025259
	public void SetLifetime(float newLifetime)
	{
		this._lifetime = newLifetime;
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00027062 File Offset: 0x00025262
	public float GetTimeForPoint(int index)
	{
		return this._times[index];
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0002706C File Offset: 0x0002526C
	public int GetTailIndex()
	{
		return this._tail;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00027074 File Offset: 0x00025274
	public int GetHeadIndex()
	{
		return this._head;
	}

	// Token: 0x04000683 RID: 1667
	[SerializeField]
	private LineRenderer _line;

	// Token: 0x04000684 RID: 1668
	[SerializeField]
	private float _minSampleDistance = 0.1f;

	// Token: 0x04000685 RID: 1669
	[SerializeField]
	private float _lifetime = 0.5f;

	// Token: 0x04000686 RID: 1670
	private const int MaxPoints = 200;

	// Token: 0x04000687 RID: 1671
	private Vector3[] _points = new Vector3[200];

	// Token: 0x04000688 RID: 1672
	private float[] _times = new float[200];

	// Token: 0x04000689 RID: 1673
	private int _head;

	// Token: 0x0400068A RID: 1674
	private int _tail;

	// Token: 0x0400068B RID: 1675
	private bool _started;

	// Token: 0x0400068C RID: 1676
	private bool _lineChanged;

	// Token: 0x0400068D RID: 1677
	private float _currentTime;
}
