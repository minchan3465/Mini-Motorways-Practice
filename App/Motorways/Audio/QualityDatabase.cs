using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x0200066B RID: 1643
	public static class QualityDatabase
	{
		// Token: 0x06002DAC RID: 11692 RVA: 0x000D2D80 File Offset: 0x000D0F80
		public static Quality Find(string name)
		{
			Quality q = QualityDatabase.ALL.Find((Quality x) => x.Name == name);
			if (q == null)
			{
				Dbug.Log.Error("Quality {0} Not Found.", new object[]
				{
					name
				});
			}
			return q;
		}

		// Token: 0x06002DAD RID: 11693 RVA: 0x000D2DD4 File Offset: 0x000D0FD4
		public static List<Quality> Gather(params string[] names)
		{
			List<Quality> q = new List<Quality>();
			for (int i = 0; i < names.Length; i++)
			{
				q.Add(QualityDatabase.Find(names[i]));
			}
			return q;
		}

		// Token: 0x040027A5 RID: 10149
		public static readonly Quality MAJOR_TETRA = new Quality("Major Tetra", Liszt.From<int>(new int[]
		{
			2,
			2,
			1,
			7
		}), Liszt.From<int>(new int[]
		{
			0,
			12,
			24
		})).Modal(new string[]
		{
			"Major Lower Tetra",
			"Minor Cross Tetra",
			"Phrygian Cross Tetra",
			"Major Upper Tetra"
		});

		// Token: 0x040027A6 RID: 10150
		public static readonly Quality MINOR_TETRA = new Quality("Minor Tetra", Liszt.From<int>(new int[]
		{
			2,
			1,
			2,
			7
		}), Liszt.From<int>(new int[]
		{
			0,
			12,
			24
		})).Modal(new string[]
		{
			"Minor Lower Tetra",
			"Locrian Cross Tetra",
			"Ionian Cross Tetra",
			"Minor Upper Tetra"
		});

		// Token: 0x040027A7 RID: 10151
		public static readonly Quality PHRYGIAN_TETRA = new Quality("Phrygian Tetra", Liszt.From<int>(new int[]
		{
			1,
			2,
			2,
			7
		}), Liszt.From<int>(new int[]
		{
			0,
			12,
			24
		})).Modal(new string[]
		{
			"Phrygian Lower Tetra",
			"Lydian Cross Tetra",
			"Mixolydian Upper Cross Tetra",
			"Phrygian Upper Tetra"
		});

		// Token: 0x040027A8 RID: 10152
		public static readonly Quality ALTERED_TETRA = new Quality("Altered Tetra", Liszt.From<int>(new int[]
		{
			1,
			2,
			1,
			8
		}), Liszt.From<int>(new int[]
		{
			0,
			12,
			24
		})).Modal(new string[]
		{
			"Altered Lower Tetra",
			"Harmonic Minor Cross Tetra",
			"Mixo b2 Cross Tetra",
			"Altered Upper Tetra"
		});

		// Token: 0x040027A9 RID: 10153
		public static readonly Quality HARMONIC_TETRA = new Quality("Harmonic Tetra", Liszt.From<int>(new int[]
		{
			1,
			3,
			1,
			7
		}), Liszt.From<int>(new int[]
		{
			0,
			12,
			24
		})).Modal(new string[]
		{
			"Harmonic Lower Tetra",
			"Lydian #2 Cross Tetra",
			"Locrian bb7 Cross Tetra",
			"Harmonic Upper Tetra"
		});

		// Token: 0x040027AA RID: 10154
		public static readonly Quality LYDIAN_TETRA = new Quality("Lydian Tetra", Liszt.From<int>(new int[]
		{
			2,
			2,
			2,
			6
		}), Liszt.From<int>(new int[]
		{
			0,
			12,
			24
		})).Modal(new string[]
		{
			"Lydian Lower Tetra",
			"Mixolydian Lower Cross Tetra",
			"Aeolian Cross Tetra",
			"Lydian Upper Tetra"
		});

		// Token: 0x040027AB RID: 10155
		public static readonly List<Quality> HEXATONIC_PREINIT = Liszt.From<Quality>(new Quality[]
		{
			new Quality("Hexatonic no1", Liszt.From<int>(new int[]
			{
				2,
				1,
				2,
				2,
				2,
				3
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("Hexatonic no2", Liszt.From<int>(new int[]
			{
				4,
				1,
				2,
				2,
				2,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("Hexatonic no3", Liszt.From<int>(new int[]
			{
				2,
				3,
				2,
				2,
				2,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("Hexatonic no4", Liszt.From<int>(new int[]
			{
				2,
				2,
				3,
				2,
				2,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("Hexatonic no5", Liszt.From<int>(new int[]
			{
				2,
				2,
				1,
				4,
				2,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("Hexatonic no6", Liszt.From<int>(new int[]
			{
				2,
				2,
				1,
				2,
				4,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("Hexatonic no7", Liszt.From<int>(new int[]
			{
				2,
				2,
				1,
				2,
				2,
				3
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			}))
		});

		// Token: 0x040027AC RID: 10156
		public static readonly List<Quality> INTERVALS = Liszt.From<Quality>(new Quality[]
		{
			new Quality("Wholetone", Liszt.From<int>(new int[]
			{
				2
			}), null).Chromatic(""),
			new Quality("Diminished Triad", Liszt.From<int>(new int[]
			{
				3
			}), null).Chromatic(""),
			new Quality("Augmented Triad", Liszt.From<int>(new int[]
			{
				4
			}), null).Chromatic(""),
			new Quality("Quartal", Liszt.From<int>(new int[]
			{
				5
			}), null).Chromatic(""),
			new Quality("Overtone", Liszt.From<int>(new int[]
			{
				7
			}), null).Chromatic("")
		});

		// Token: 0x040027AD RID: 10157
		public static readonly List<Quality> HEXATONIC_CHROMATIC = QualityDatabase.HEXATONIC_PREINIT.Chromatic("");

		// Token: 0x040027AE RID: 10158
		public static readonly List<Quality> HEXATONIC_MODAL = QualityDatabase.HEXATONIC_PREINIT.Modal("Modal");

		// Token: 0x040027AF RID: 10159
		public static readonly List<Quality> HEXATONIC_CHROMODAL = QualityDatabase.HEXATONIC_MODAL.Chromatic("Chromodal");

		// Token: 0x040027B0 RID: 10160
		public static readonly List<Quality> SUHMM = Liszt.From<Quality>(new Quality[]
		{
			new Quality("SUHMM Mixolydian", Liszt.From<int>(new int[]
			{
				2,
				2,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("SUHMM Aeolian", Liszt.From<int>(new int[]
			{
				2,
				1,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("SUHMM Phrygian", Liszt.From<int>(new int[]
			{
				1,
				2,
				2,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("SUHMM Lydian", Liszt.From<int>(new int[]
			{
				2,
				2,
				2,
				1
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			})),
			new Quality("SUHMM Locrian", Liszt.From<int>(new int[]
			{
				1,
				2,
				2
			}), Liszt.From<int>(new int[]
			{
				0,
				12,
				24
			}))
		}).Chromatic("");

		// Token: 0x040027B1 RID: 10161
		public static readonly List<Quality> TETRA_MODES = Liszt.Flatten<Quality>(new List<Quality>[]
		{
			QualityDatabase.ALTERED_TETRA.ToModes(),
			QualityDatabase.HARMONIC_TETRA.ToModes(),
			QualityDatabase.LYDIAN_TETRA.ToModes(),
			QualityDatabase.MAJOR_TETRA.ToModes(),
			QualityDatabase.MINOR_TETRA.ToModes(),
			QualityDatabase.PHRYGIAN_TETRA.ToModes()
		});

		// Token: 0x040027B2 RID: 10162
		public static readonly List<Quality> TETRA = Liszt.From<Quality>(new Quality[]
		{
			QualityDatabase.MAJOR_TETRA,
			QualityDatabase.MINOR_TETRA,
			QualityDatabase.PHRYGIAN_TETRA,
			QualityDatabase.ALTERED_TETRA,
			QualityDatabase.HARMONIC_TETRA,
			QualityDatabase.LYDIAN_TETRA
		});

		// Token: 0x040027B3 RID: 10163
		public static readonly List<Quality> TETRA_CHROMODAL = QualityDatabase.TETRA.Chromatic("Chromodal");

		// Token: 0x040027B4 RID: 10164
		public static readonly Quality MAJOR = new Quality("Major", Liszt.From<int>(new int[]
		{
			2,
			2,
			1,
			2,
			2,
			2,
			1
		}), Liszt.From<int>(new int[]
		{
			0,
			12
		})).ModalVerbose(new Scale.Data[]
		{
			new Scale.Data("Ionian", new int[]
			{
				19
			}),
			new Scale.Data("Dorian", new int[]
			{
				19
			}),
			new Scale.Data("Phrygian", new int[]
			{
				19
			}),
			new Scale.Data("Lydian", new int[]
			{
				18
			}),
			new Scale.Data("Mixolydian", new int[]
			{
				19
			}),
			new Scale.Data("Aeolian", new int[]
			{
				19
			}),
			new Scale.Data("Locrian", new int[]
			{
				18
			})
		});

		// Token: 0x040027B5 RID: 10165
		public static readonly Quality MELODIC_MINOR = new Quality("Melodic Minor", Liszt.From<int>(new int[]
		{
			2,
			1,
			2,
			2,
			2,
			2,
			1
		}), Liszt.From<int>(new int[]
		{
			0,
			12
		})).Modal(new string[]
		{
			"Melodic Minor",
			"Dorian b2",
			"Lydian Augmented",
			"Lydian Dominant",
			"Aeolian Dominant",
			"Half Diminished",
			"Altered"
		});

		// Token: 0x040027B6 RID: 10166
		public static readonly Quality HARMONIC_MINOR = new Quality("Harmonic Minor", Liszt.From<int>(new int[]
		{
			2,
			1,
			2,
			2,
			1,
			3,
			1
		}), null).Modal(new string[]
		{
			"Harmonic Minor",
			"Locrian Natural 6",
			"Major #5",
			"Dorian #4",
			"Phrygian Dominant",
			"Lydian #2",
			"Altered Dominant bb7"
		});

		// Token: 0x040027B7 RID: 10167
		public static readonly Quality HARMONIC_MAJOR = new Quality("Harmonic Major", Liszt.From<int>(new int[]
		{
			2,
			2,
			1,
			2,
			1,
			3,
			1
		}), null).Modal(new string[]
		{
			"Harmonic Major",
			"Dorian b5",
			"Phrygian b4",
			"Lydian Minor",
			"Mixolydian b2",
			"Lydian Augmented #2",
			"Locrian bb7"
		});

		// Token: 0x040027B8 RID: 10168
		public static readonly Quality DOUBLE_HARMONIC = new Quality("Double Harmonic", Liszt.From<int>(new int[]
		{
			1,
			3,
			1,
			2,
			1,
			3,
			1
		}), null).Modal(new string[]
		{
			"Double Harmonic",
			"Lydian #2 #6",
			"Ultraphrygian",
			"Hungarian Minor",
			"Mixolydian b2 b5",
			"Ionian #2 #5",
			"Locrian bb3 bb7"
		});

		// Token: 0x040027B9 RID: 10169
		public static readonly Quality INSEN = new Quality("Insen", Liszt.From<int>(new int[]
		{
			1,
			4,
			2,
			3,
			2
		}), null).Modal(Array.Empty<string>());

		// Token: 0x040027BA RID: 10170
		public static readonly Quality IN = new Quality("In", Liszt.From<int>(new int[]
		{
			1,
			4,
			2,
			1,
			4
		}), null).Modal(Array.Empty<string>());

		// Token: 0x040027BB RID: 10171
		public static readonly Quality MAJOR_B6_PENTA = new Quality("Major b6 Penta", Liszt.From<int>(new int[]
		{
			2,
			2,
			3,
			1,
			4
		}), null).Modal(Array.Empty<string>());

		// Token: 0x040027BC RID: 10172
		public static readonly Quality SIX_NINE = new Quality("6/9", Liszt.From<int>(new int[]
		{
			2,
			2,
			3,
			2,
			3
		}), null);

		// Token: 0x040027BD RID: 10173
		public static readonly Quality PENTA = Quality.Clone(QualityDatabase.SIX_NINE, "Penta").Modal(new string[]
		{
			"Major Pentatonic",
			"Penta 2",
			"Penta 3",
			"Yo",
			"Minor Pentatonic"
		});

		// Token: 0x040027BE RID: 10174
		public static readonly Quality NINE = new Quality("9", Liszt.From<int>(new int[]
		{
			2,
			2,
			3,
			3,
			2
		}), null);

		// Token: 0x040027BF RID: 10175
		public static readonly Quality PENTA_DOM = Quality.Clone(QualityDatabase.NINE, "Dominant Penta").Modal(Array.Empty<string>());

		// Token: 0x040027C0 RID: 10176
		public static readonly List<Quality> ALL = Liszt.Flatten<Quality>(new List<Quality>[]
		{
			QualityDatabase.INTERVALS,
			QualityDatabase.SUHMM,
			QualityDatabase.HEXATONIC_MODAL,
			QualityDatabase.HEXATONIC_CHROMATIC,
			QualityDatabase.HEXATONIC_CHROMODAL,
			QualityDatabase.TETRA,
			QualityDatabase.TETRA_CHROMODAL,
			QualityDatabase.TETRA_MODES,
			QualityDatabase.MAJOR.ToModes(),
			QualityDatabase.MELODIC_MINOR.ToModes(),
			QualityDatabase.HARMONIC_MINOR.ToModes(),
			QualityDatabase.HARMONIC_MAJOR.ToModes(),
			QualityDatabase.DOUBLE_HARMONIC.ToModes(),
			QualityDatabase.INSEN.ToModes(),
			QualityDatabase.IN.ToModes(),
			QualityDatabase.MAJOR_B6_PENTA.ToModes(),
			QualityDatabase.PENTA.ToModes(),
			QualityDatabase.PENTA_DOM.ToModes(),
			Liszt.From<Quality>(new Quality[]
			{
				QualityDatabase.MAJOR,
				Quality.Clone(QualityDatabase.MAJOR, "Major Chromodal").Chromatic(""),
				QualityDatabase.DOUBLE_HARMONIC,
				Quality.Clone(QualityDatabase.DOUBLE_HARMONIC, "Double Harmonic Chromodal").Chromatic(""),
				QualityDatabase.HARMONIC_MAJOR,
				Quality.Clone(QualityDatabase.HARMONIC_MAJOR, "Harmonic Major Chromodal").Chromatic(""),
				QualityDatabase.HARMONIC_MINOR,
				Quality.Clone(QualityDatabase.HARMONIC_MINOR, "Harmonic Minor Chromodal").Chromatic(""),
				QualityDatabase.MELODIC_MINOR,
				Quality.Clone(QualityDatabase.MELODIC_MINOR, "Melodic Minor Chromodal").Chromatic(""),
				QualityDatabase.INSEN,
				Quality.Clone(QualityDatabase.INSEN, "Insen Chromodal").Chromatic(""),
				QualityDatabase.IN,
				Quality.Clone(QualityDatabase.IN, "In Chromodal").Chromatic(""),
				QualityDatabase.MAJOR_B6_PENTA,
				Quality.Clone(QualityDatabase.MAJOR_B6_PENTA, "Major b6 Penta Chromodal").Chromatic(""),
				QualityDatabase.PENTA,
				Quality.Clone(QualityDatabase.PENTA, "Penta Chromodal").Chromatic(""),
				QualityDatabase.PENTA_DOM,
				Quality.Clone(QualityDatabase.PENTA_DOM, "Dominant Penta Chromodal").Chromatic(""),
				QualityDatabase.SIX_NINE.Chromatic(""),
				QualityDatabase.NINE.Chromatic(""),
				QualityDatabase.HEXATONIC_MODAL[6].GetMode(2, "Ritsu"),
				new Quality("Diminished", Liszt.From<int>(new int[]
				{
					2,
					1
				}), null).Chromatic(""),
				new Quality("Aux Diminished", Liszt.From<int>(new int[]
				{
					1,
					2
				}), null).Chromatic(""),
				new Quality("Augmented", Liszt.From<int>(new int[]
				{
					3,
					1
				}), null).Chromatic(""),
				new Quality("Aux Augmented", Liszt.From<int>(new int[]
				{
					1,
					3
				}), null).Chromatic(""),
				new Quality("7b5", Liszt.From<int>(new int[]
				{
					4,
					2
				}), null).Chromatic(""),
				new Quality("5 (Power Chord)", Liszt.From<int>(new int[]
				{
					7,
					5
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("Petrushka", Liszt.From<int>(new int[]
				{
					1,
					3,
					2
				}), null).Chromatic(""),
				new Quality("5sus2", Liszt.From<int>(new int[]
				{
					2,
					5,
					5
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("Minor Triad", Liszt.From<int>(new int[]
				{
					3,
					4,
					5
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("Major Triad", Liszt.From<int>(new int[]
				{
					4,
					3,
					5
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("Sus", Liszt.From<int>(new int[]
				{
					5,
					2,
					5
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("Sus2Maj7", Liszt.From<int>(new int[]
				{
					2,
					5,
					4,
					1
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("Sus2Min7", Liszt.From<int>(new int[]
				{
					2,
					5,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("HalfDim", Liszt.From<int>(new int[]
				{
					3,
					3,
					2,
					4
				}), null).Chromatic(""),
				new Quality("Min7b5", Liszt.From<int>(new int[]
				{
					3,
					3,
					4,
					2
				}), null).Chromatic(""),
				new Quality("MinMaj6", Liszt.From<int>(new int[]
				{
					3,
					4,
					2,
					3
				}), null).Chromatic(""),
				new Quality("Min7", Liszt.From<int>(new int[]
				{
					3,
					4,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("MinMaj7", Liszt.From<int>(new int[]
				{
					3,
					4,
					4,
					1
				}), null).Chromatic(""),
				new Quality("Maj6", Liszt.From<int>(new int[]
				{
					4,
					3,
					2,
					3
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("7", Liszt.From<int>(new int[]
				{
					4,
					3,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("Maj7", Liszt.From<int>(new int[]
				{
					4,
					3,
					4,
					1
				}), Liszt.From<int>(new int[]
				{
					0,
					12
				})).Chromatic(""),
				new Quality("7#5", Liszt.From<int>(new int[]
				{
					4,
					4,
					2,
					2
				}), null).Chromatic(""),
				new Quality("b7sus", Liszt.From<int>(new int[]
				{
					5,
					2,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("7b9", Liszt.From<int>(new int[]
				{
					1,
					3,
					3,
					3,
					2
				}), null).Chromatic(""),
				new Quality("Min9", Liszt.From<int>(new int[]
				{
					2,
					1,
					4,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("7#9", Liszt.From<int>(new int[]
				{
					3,
					1,
					3,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("11", Liszt.From<int>(new int[]
				{
					4,
					1,
					2,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("Aug11", Liszt.From<int>(new int[]
				{
					4,
					2,
					1,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("13", Liszt.From<int>(new int[]
				{
					4,
					3,
					2,
					1,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("Istrian", Liszt.From<int>(new int[]
				{
					1,
					2,
					1,
					2,
					1,
					5
				}), null).Chromatic(""),
				new Quality("b9(13)", Liszt.From<int>(new int[]
				{
					1,
					3,
					3,
					2,
					1,
					2
				}), null).Chromatic(""),
				new Quality("Min11", Liszt.From<int>(new int[]
				{
					2,
					1,
					2,
					2,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					19
				})).Chromatic(""),
				new Quality("Mystic", Liszt.From<int>(new int[]
				{
					2,
					2,
					2,
					3,
					1,
					2
				}), null).Chromatic(""),
				new Quality("Blues", Liszt.From<int>(new int[]
				{
					3,
					2,
					1,
					1,
					3,
					2
				}), null).Chromatic("")
			})
		});
	}
}
