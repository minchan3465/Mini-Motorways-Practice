using System;
using System.Collections.Generic;
using Client;

// Token: 0x0200023C RID: 572
public interface IScreen
{
	// Token: 0x06000D85 RID: 3461
	void Tick(float deltaTime);

	// Token: 0x06000D86 RID: 3462
	void Enable(bool shouldBeVisible);

	// Token: 0x06000D87 RID: 3463
	void TransitionIn(ScreenStack.MotorwaysScreen outScreen);

	// Token: 0x06000D88 RID: 3464
	void TransitionOut(ScreenStack.MotorwaysScreen inScreen);

	// Token: 0x06000D89 RID: 3465
	float TransitionInPercentage();

	// Token: 0x06000D8A RID: 3466
	float TransitionOutPercentage();

	// Token: 0x06000D8B RID: 3467
	bool IsTransitioningIn();

	// Token: 0x06000D8C RID: 3468
	bool IsTransitioningOut();

	// Token: 0x06000D8D RID: 3469
	void OnTransitionedIn();

	// Token: 0x06000D8E RID: 3470
	void OnTransitionedOut();

	// Token: 0x06000D8F RID: 3471
	void OnGainedFocus();

	// Token: 0x06000D90 RID: 3472
	void OnLostFocus();

	// Token: 0x06000D91 RID: 3473
	void RegisterAdditionalThemeComponents(List<IThemeComponent> additionalThemeComponents);

	// Token: 0x06000D92 RID: 3474
	void UnregisterAdditionalThemeComponents(List<IThemeComponent> additionalThemeComponents);

	// Token: 0x06000D93 RID: 3475
	bool IsVisible();

	// Token: 0x06000D94 RID: 3476
	bool CanTransitionIn();

	// Token: 0x06000D95 RID: 3477
	void BackActivated();

	// Token: 0x06000D96 RID: 3478
	bool CanPopScreen();
}
