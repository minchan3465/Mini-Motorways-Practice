using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200071C RID: 1820
	public class ButtonGroup : MonoBehaviour
	{
		// Token: 0x06003212 RID: 12818 RVA: 0x000ECCAC File Offset: 0x000EAEAC
		[Button("Assign all buttons")]
		private void GetAllButtons()
		{
			this.buttons.Clear();
			foreach (TouchButton button in base.GetComponentsInChildren<TouchButton>())
			{
				this.buttons.Add(button);
			}
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x000ECCE9 File Offset: 0x000EAEE9
		private void Start()
		{
			this.Initialize();
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x000ECCF4 File Offset: 0x000EAEF4
		public void Initialize()
		{
			if (!this._isInitialized)
			{
				using (List<TouchButton>.Enumerator enumerator = this.buttons.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TouchButton button = enumerator.Current;
						button.AddOnClickedEvent(delegate
						{
							this.OnButtonClicked(button);
						});
						button.AddOnSelectedEvent(delegate
						{
							this.OnButtonSelected(button);
						});
						button.AddOnDeselectedEvent(delegate
						{
							this.OnButtonDeselected(button);
						});
					}
				}
				this._isInitialized = true;
			}
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x000ECDAC File Offset: 0x000EAFAC
		private void OnEnable()
		{
			if (this.isToggleButtonGroup && this.activeButton != null)
			{
				this.OnButtonClicked(this.activeButton);
			}
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x000ECDD0 File Offset: 0x000EAFD0
		public void OnButtonClicked(TouchButton clickedButton)
		{
			if (this.isToggleButtonGroup)
			{
				this.activeButton = clickedButton;
				this.activeButton.GetComponent<Animator>().SetTrigger(ButtonGroup.Selected);
				this.activeButton.GetComponent<Animator>().ResetTrigger(ButtonGroup.Normal);
				foreach (TouchButton otherButton in this.buttons)
				{
					if (otherButton != clickedButton)
					{
						if (otherButton != null)
						{
							Animator component = otherButton.GetComponent<Animator>();
							if (component != null)
							{
								component.ResetTrigger(ButtonGroup.Normal);
							}
						}
						if (otherButton != null)
						{
							Animator component2 = otherButton.GetComponent<Animator>();
							if (component2 != null)
							{
								component2.SetTrigger(ButtonGroup.Lowlight);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x000ECE94 File Offset: 0x000EB094
		public void OnButtonSelected(TouchButton selectedButton)
		{
			foreach (TouchButton button in this.buttons)
			{
				if (button != selectedButton && (!this.isToggleButtonGroup || button != this.activeButton))
				{
					if (button != null)
					{
						Animator component = button.GetComponent<Animator>();
						if (component != null)
						{
							component.ResetTrigger(ButtonGroup.Normal);
						}
					}
					if (button != null)
					{
						Animator component2 = button.GetComponent<Animator>();
						if (component2 != null)
						{
							component2.SetTrigger(ButtonGroup.Lowlight);
						}
					}
				}
			}
			if (this.isToggleButtonGroup && this.activeButton == selectedButton && this.activeButton != null)
			{
				TouchButton touchButton = this.activeButton;
				if (touchButton != null)
				{
					Animator component3 = touchButton.GetComponent<Animator>();
					if (component3 != null)
					{
						component3.ResetTrigger(ButtonGroup.Lowlight);
					}
				}
				TouchButton touchButton2 = this.activeButton;
				if (touchButton2 != null)
				{
					Animator component4 = touchButton2.GetComponent<Animator>();
					if (component4 != null)
					{
						component4.ResetTrigger(ButtonGroup.Selected);
					}
				}
				TouchButton touchButton3 = this.activeButton;
				if (touchButton3 == null)
				{
					return;
				}
				Animator component5 = touchButton3.GetComponent<Animator>();
				if (component5 == null)
				{
					return;
				}
				component5.SetTrigger(ButtonGroup.Highlighted);
			}
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x000ECFB8 File Offset: 0x000EB1B8
		public void OnButtonDeselected(VariableDeviceSelectable deselectedButton)
		{
			if (this.isToggleButtonGroup)
			{
				if (this.activeButton != null)
				{
					Animator component = this.activeButton.GetComponent<Animator>();
					if (component != null)
					{
						component.ResetTrigger(ButtonGroup.Normal);
					}
					if (this.keepHighlightedOnDeselectForTouchInput && this.activeButton.DeviceInputType == DeviceInputType.Touch)
					{
						Animator component2 = this.activeButton.GetComponent<Animator>();
						if (component2 != null)
						{
							component2.SetTrigger(ButtonGroup.Highlighted);
						}
					}
					else
					{
						Animator component3 = this.activeButton.GetComponent<Animator>();
						if (component3 != null)
						{
							component3.SetTrigger(ButtonGroup.Selected);
						}
					}
				}
				if (deselectedButton != this.activeButton)
				{
					if (deselectedButton != null)
					{
						Animator component4 = deselectedButton.GetComponent<Animator>();
						if (component4 != null)
						{
							component4.ResetTrigger(ButtonGroup.Normal);
						}
					}
					if (deselectedButton != null)
					{
						Animator component5 = deselectedButton.GetComponent<Animator>();
						if (component5 == null)
						{
							return;
						}
						component5.SetTrigger(ButtonGroup.Lowlight);
						return;
					}
				}
			}
			else
			{
				foreach (VariableDeviceSelectable button in this.buttons)
				{
					if (!(button == null))
					{
						Animator component6 = button.GetComponent<Animator>();
						if (component6 != null)
						{
							component6.SetTrigger(ButtonGroup.Normal);
						}
					}
				}
			}
		}

		// Token: 0x04002AED RID: 10989
		private static readonly int Normal = Animator.StringToHash("Normal");

		// Token: 0x04002AEE RID: 10990
		private static readonly int Highlighted = Animator.StringToHash("Highlighted");

		// Token: 0x04002AEF RID: 10991
		private static readonly int Lowlight = Animator.StringToHash("Lowlight");

		// Token: 0x04002AF0 RID: 10992
		private static readonly int Selected = Animator.StringToHash("Selected");

		// Token: 0x04002AF1 RID: 10993
		public bool keepHighlightedOnDeselectForTouchInput = true;

		// Token: 0x04002AF2 RID: 10994
		public List<TouchButton> buttons = new List<TouchButton>();

		// Token: 0x04002AF3 RID: 10995
		public bool isToggleButtonGroup;

		// Token: 0x04002AF4 RID: 10996
		[ShowIf("isToggleButtonGroup")]
		public TouchButton activeButton;

		// Token: 0x04002AF5 RID: 10997
		private bool _isInitialized;
	}
}
