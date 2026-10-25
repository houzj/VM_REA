.class public auto ansi beforefieldinit VMControls.WPF.GlobalUI.GlobalVariableDialog
	extends [VM.Controls]VM.Controls.BaseWindow
	implements [VMControls.Interface]VMControls.Interface.IDecorate,
	           VMControls.WPF.GlobalUI.IVarGroup,
	           [System.Xaml]System.Windows.Markup.IComponentConnector,
	           [PresentationFramework]System.Windows.Markup.IStyleConnector
{
	// Nested Types
	.class nested public auto ansi sealed PageShowType
		extends [mscorlib]System.Enum
	{
		// Fields
		.field public specialname rtspecialname int32 value__
		.field public static literal valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType ShowFirstPage = int32(0)
		.field public static literal valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType NoChange = int32(1)
		.field public static literal valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType ImmediateChange = int32(2)

	} // end of class PageShowType

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass4_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public bool 'init'
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cad0
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass4_0'::.ctor

		.method assembly hidebysig 
			instance void '<.ctor>b__0' (
				object s,
				class [PresentationCore]System.Windows.RoutedEventArgs e
			) cil managed 
		{
			// Method begins at RVA 0x5cadc
			// Header size: 12
			// Code size: 242 (0xf2)
			.maxstack 3
			.locals init (
				[0] bool,
				[1] bool,
				[2] bool,
				[3] string
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_0007: ldarg.0
			IL_0008: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_000d: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0012: newobj instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::.ctor(object)
			IL_0017: stfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_001c: ldarg.0
			IL_001d: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'init'
			IL_0022: stloc.0
			IL_0023: ldloc.0
			IL_0024: brfalse.s IL_002c

			IL_0026: nop
			IL_0027: br IL_00f1

			IL_002c: ldarg.0
			IL_002d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_0032: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0037: isinst [VM.Core]VM.Core.VmModule
			IL_003c: dup
			IL_003d: brtrue.s IL_0043

			IL_003f: pop
			IL_0040: ldc.i4.0
			IL_0041: br.s IL_0074

			IL_0043: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
			IL_0048: ldsfld class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__4_1'
			IL_004d: dup
			IL_004e: brtrue.s IL_0067

			IL_0050: pop
			IL_0051: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
			IL_0056: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<.ctor>b__4_1'(class [VM.Core]VM.Core.VmIO)
			IL_005c: newobj instance void class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, bool>::.ctor(object, native int)
			IL_0061: dup
			IL_0062: stsfld class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__4_1'

			IL_0067: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class [VM.Core]VM.Core.VmIO>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_006c: call int32 [System.Core]System.Linq.Enumerable::Count<class [VM.Core]VM.Core.VmIO>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
			IL_0071: ldc.i4.0
			IL_0072: cgt

			IL_0074: stloc.1
			IL_0075: ldloc.1
			IL_0076: brfalse.s IL_00b2

			IL_0078: nop
			IL_0079: ldarg.0
			IL_007a: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_007f: ldstr "Lang_VarLoading"
			IL_0084: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0089: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ProcessbarMessage(string)
			IL_008e: nop
			IL_008f: ldarg.0
			IL_0090: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_0095: ldstr "Collapsed"
			IL_009a: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_EmptyStatus(string)
			IL_009f: nop
			IL_00a0: ldarg.0
			IL_00a1: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_00a6: ldstr "Visible"
			IL_00ab: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_00b0: nop
			IL_00b1: nop

			IL_00b2: ldarg.0
			IL_00b3: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_00b8: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_00bd: ldnull
			IL_00be: cgt.un
			IL_00c0: stloc.2
			IL_00c1: ldloc.2
			IL_00c2: brfalse.s IL_00db

			IL_00c4: nop
			IL_00c5: ldarg.0
			IL_00c6: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
			IL_00cb: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::GetDataAsync()
			IL_00d0: nop
			IL_00d1: ldarg.0
			IL_00d2: ldc.i4.1
			IL_00d3: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'init'
			IL_00d8: nop
			IL_00d9: br.s IL_00f1

			IL_00db: nop
			IL_00dc: ldstr "Lang_VarLoadFailed"
			IL_00e1: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00e6: stloc.3
			IL_00e7: ldloc.3
			IL_00e8: ldc.i4.3
			IL_00e9: ldc.i4.0
			IL_00ea: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_00ef: pop
			IL_00f0: nop

			IL_00f1: ret
		} // end of method '<>c__DisplayClass4_0'::'<.ctor>b__0'

	} // end of class <>c__DisplayClass4_0

	.class nested private auto ansi sealed serializable beforefieldinit '<>c'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public static initonly class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' '<>9'
		.field public static class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, bool> '<>9__4_1'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> '<>9__99_1'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> '<>9__100_1'
		.field public static class [mscorlib]System.Comparison`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> '<>9__107_0'
		.field public static class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, string> '<>9__107_1'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> '<>9__107_3'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> '<>9__117_2'
		.field public static class [mscorlib]System.Func`2<int32, int32> '<>9__117_4'
		.field public static class [mscorlib]System.Func`2<int32, int32> '<>9__118_5'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> '<>9__120_1'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> '<>9__122_1'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> '<>9__123_1'
		.field public static class [mscorlib]System.Action '<>9__128_2'
		.field public static class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.VarGroupItem, string> '<>9__131_0'

		// Methods
		.method private hidebysig specialname rtspecialname static 
			void .cctor () cil managed 
		{
			// Method begins at RVA 0x5cbda
			// Header size: 1
			// Code size: 11 (0xb)
			.maxstack 8

			IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::.ctor()
			IL_0005: stsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
			IL_000a: ret
		} // end of method '<>c'::.cctor

		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cbe6
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c'::.ctor

		.method assembly hidebysig 
			instance bool '<.ctor>b__4_1' (
				class [VM.Core]VM.Core.VmIO x
			) cil managed 
		{
			// Method begins at RVA 0x5cbef
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance bool [VM.Core]VM.Core.VmIO::get_CustomVisibleA()
			IL_0006: ret
		} // end of method '<>c'::'<.ctor>b__4_1'

		.method assembly hidebysig 
			instance int32 '<SeearchItem>b__99_1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cbf7
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ret
		} // end of method '<>c'::'<SeearchItem>b__99_1'

		.method assembly hidebysig 
			instance int32 '<SetBindData>b__100_1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cbff
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ret
		} // end of method '<>c'::'<SetBindData>b__100_1'

		.method assembly hidebysig 
			instance int32 '<GetData>b__107_0' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem b1,
				class VMControls.WPF.GlobalUI.GlobalVariableItem b2
			) cil managed 
		{
			// Method begins at RVA 0x5cc08
			// Header size: 12
			// Code size: 28 (0x1c)
			.maxstack 3
			.locals init (
				[0] int32
			)

			IL_0000: nop
			IL_0001: call class [mscorlib]System.Collections.Generic.Comparer`1<!0> class [mscorlib]System.Collections.Generic.Comparer`1<int32>::get_Default()
			IL_0006: ldarg.1
			IL_0007: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_000c: ldarg.2
			IL_000d: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0012: callvirt instance int32 class [mscorlib]System.Collections.Generic.Comparer`1<int32>::Compare(!0, !0)
			IL_0017: stloc.0
			IL_0018: br.s IL_001a

			IL_001a: ldloc.0
			IL_001b: ret
		} // end of method '<>c'::'<GetData>b__107_0'

		.method assembly hidebysig 
			instance string '<GetData>b__107_1' (
				class [VM.Core]VM.Core.VmIO p
			) cil managed 
		{
			// Method begins at RVA 0x5cc30
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string [VM.Core]VM.Core.VmFrameTreeBase::get_Name()
			IL_0006: ret
		} // end of method '<>c'::'<GetData>b__107_1'

		.method assembly hidebysig 
			instance bool '<GetData>b__107_3' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cc38
			// Header size: 1
			// Code size: 12 (0xc)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0006: call bool [mscorlib]System.String::IsNullOrEmpty(string)
			IL_000b: ret
		} // end of method '<>c'::'<GetData>b__107_3'

		.method assembly hidebysig 
			instance int32 '<SetHeadIndex>b__117_2' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cc45
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ret
		} // end of method '<>c'::'<SetHeadIndex>b__117_2'

		.method assembly hidebysig 
			instance int32 '<SetHeadIndex>b__117_4' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5cc4d
			// Header size: 1
			// Code size: 2 (0x2)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ret
		} // end of method '<>c'::'<SetHeadIndex>b__117_4'

		.method assembly hidebysig 
			instance int32 '<SwapIndex>b__118_5' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5cc50
			// Header size: 1
			// Code size: 2 (0x2)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ret
		} // end of method '<>c'::'<SwapIndex>b__118_5'

		.method assembly hidebysig 
			instance int32 '<UpdateBtnEnable>b__120_1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cc53
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ret
		} // end of method '<>c'::'<UpdateBtnEnable>b__120_1'

		.method assembly hidebysig 
			instance int32 '<BtnUp_Click>b__122_1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cc5b
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ret
		} // end of method '<>c'::'<BtnUp_Click>b__122_1'

		.method assembly hidebysig 
			instance int32 '<BtnDown_Click>b__123_1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cc63
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ret
		} // end of method '<>c'::'<BtnDown_Click>b__123_1'

		.method assembly hidebysig 
			instance void '<ExportGlobalVar>b__128_2' () cil managed 
		{
			// Method begins at RVA 0x5cc6b
			// Header size: 1
			// Code size: 20 (0x14)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldstr "Lang_ExportFailed"
			IL_0006: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_000b: ldc.i4.3
			IL_000c: ldc.i4.0
			IL_000d: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0012: pop
			IL_0013: ret
		} // end of method '<>c'::'<ExportGlobalVar>b__128_2'

		.method assembly hidebysig 
			instance string '<GetUniqueName>b__131_0' (
				class VMControls.WPF.GlobalUI.VarGroupItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cc80
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0006: ret
		} // end of method '<>c'::'<GetUniqueName>b__131_0'

	} // end of class <>c

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass5_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem varItem
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field public class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> '<>9__1'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cc88
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass5_0'::.ctor

		.method assembly hidebysig 
			instance void '<VarValueReportEvent>b__0' () cil managed 
		{
			// Method begins at RVA 0x5cc94
			// Header size: 12
			// Code size: 212 (0xd4)
			.maxstack 4
			.locals init (
				[0] class VMControls.WPF.GlobalUI.GlobalVariableItem,
				[1] class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>,
				[2] bool,
				[3] string[],
				[4] bool,
				[5] bool
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<>4__this'
			IL_0007: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_000c: ldarg.0
			IL_000d: ldfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<>9__1'
			IL_0012: dup
			IL_0013: brtrue.s IL_002b

			IL_0015: pop
			IL_0016: ldarg.0
			IL_0017: ldarg.0
			IL_0018: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<VarValueReportEvent>b__1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_001e: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_0023: dup
			IL_0024: stloc.1
			IL_0025: stfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<>9__1'
			IL_002a: ldloc.1

			IL_002b: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_0030: stloc.0
			IL_0031: ldloc.0
			IL_0032: brfalse.s IL_0044

			IL_0034: ldarg.0
			IL_0035: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::varItem
			IL_003a: callvirt instance object [Apps.XmlParser]Apps.XmlParser.Variable.VarItem::get_ValueArray()
			IL_003f: ldnull
			IL_0040: cgt.un
			IL_0042: br.s IL_0045

			IL_0044: ldc.i4.0

			IL_0045: stloc.2
			IL_0046: ldloc.2
			IL_0047: brfalse IL_00d3

			IL_004c: nop
			IL_004d: ldloc.0
			IL_004e: ldstr "PropertyChanged"
			IL_0053: ldarg.0
			IL_0054: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<>4__this'
			IL_0059: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ValueChanged(object, class [System]System.ComponentModel.PropertyChangedEventArgs)
			IL_005f: newobj instance void class [mscorlib]System.EventHandler`1<class [System]System.ComponentModel.PropertyChangedEventArgs>::.ctor(object, native int)
			IL_0064: call void class [WindowsBase]System.Windows.WeakEventManager`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, class [System]System.ComponentModel.PropertyChangedEventArgs>::RemoveHandler(!0, string, class [mscorlib]System.EventHandler`1<!1>)
			IL_0069: nop
			IL_006a: ldarg.0
			IL_006b: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::varItem
			IL_0070: callvirt instance object [Apps.XmlParser]Apps.XmlParser.Variable.VarItem::get_ValueArray()
			IL_0075: isinst string[]
			IL_007a: dup
			IL_007b: stloc.3
			IL_007c: brfalse.s IL_0085

			IL_007e: ldloc.3
			IL_007f: ldlen
			IL_0080: ldc.i4.0
			IL_0081: cgt.un
			IL_0083: br.s IL_0086

			IL_0085: ldc.i4.0

			IL_0086: stloc.s 4
			IL_0088: ldloc.s 4
			IL_008a: brfalse.s IL_00b5

			IL_008c: nop
			IL_008d: ldloc.3
			IL_008e: ldlen
			IL_008f: conv.i4
			IL_0090: ldc.i4.1
			IL_0091: cgt
			IL_0093: ldc.i4.0
			IL_0094: ceq
			IL_0096: stloc.s 5
			IL_0098: ldloc.s 5
			IL_009a: brfalse.s IL_00aa

			IL_009c: nop
			IL_009d: ldloc.0
			IL_009e: ldloc.3
			IL_009f: ldc.i4.0
			IL_00a0: ldelem.ref
			IL_00a1: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Value(string)
			IL_00a6: nop
			IL_00a7: nop
			IL_00a8: br.s IL_00b4

			IL_00aa: nop
			IL_00ab: ldloc.0
			IL_00ac: ldloc.3
			IL_00ad: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::UpdateArrayValue(string[])
			IL_00b2: nop
			IL_00b3: nop

			IL_00b4: nop

			IL_00b5: ldloc.0
			IL_00b6: ldstr "PropertyChanged"
			IL_00bb: ldarg.0
			IL_00bc: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<>4__this'
			IL_00c1: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ValueChanged(object, class [System]System.ComponentModel.PropertyChangedEventArgs)
			IL_00c7: newobj instance void class [mscorlib]System.EventHandler`1<class [System]System.ComponentModel.PropertyChangedEventArgs>::.ctor(object, native int)
			IL_00cc: call void class [WindowsBase]System.Windows.WeakEventManager`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, class [System]System.ComponentModel.PropertyChangedEventArgs>::AddHandler(!0, string, class [mscorlib]System.EventHandler`1<!1>)
			IL_00d1: nop
			IL_00d2: nop

			IL_00d3: ret
		} // end of method '<>c__DisplayClass5_0'::'<VarValueReportEvent>b__0'

		.method assembly hidebysig 
			instance bool '<VarValueReportEvent>b__1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cd74
			// Header size: 1
			// Code size: 50 (0x32)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0006: ldarg.0
			IL_0007: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::varItem
			IL_000c: callvirt instance string [Apps.XmlParser]Apps.XmlParser.Variable.VarItem::get_Name()
			IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0016: brfalse.s IL_0030

			IL_0018: ldarg.1
			IL_0019: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_001e: ldarg.0
			IL_001f: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::varItem
			IL_0024: callvirt instance string [Apps.XmlParser]Apps.XmlParser.Variable.VarItem::get_Type()
			IL_0029: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_002e: br.s IL_0031

			IL_0030: ldc.i4.0

			IL_0031: ret
		} // end of method '<>c__DisplayClass5_0'::'<VarValueReportEvent>b__1'

	} // end of class <>c__DisplayClass5_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass94_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class [mscorlib]System.Text.StringBuilder stringBuilder

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cda7
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass94_0'::.ctor

		.method assembly hidebysig 
			instance bool '<GetDefaultName>b__0' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cdb0
			// Header size: 1
			// Code size: 38 (0x26)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0006: ldstr "%"
			IL_000b: ldarg.0
			IL_000c: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
			IL_0011: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0016: ldstr "%"
			IL_001b: call string [mscorlib]System.String::Concat(string, string, string)
			IL_0020: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0025: ret
		} // end of method '<>c__DisplayClass94_0'::'<GetDefaultName>b__0'

	} // end of class <>c__DisplayClass94_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass96_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string PropertyName

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cdd7
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass96_0'::.ctor

		.method assembly hidebysig 
			instance bool '<ExcudePropertyName>b__0' (
				string x
			) cil managed 
		{
			// Method begins at RVA 0x5cde0
			// Header size: 1
			// Code size: 13 (0xd)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ldarg.0
			IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass96_0'::PropertyName
			IL_0007: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_000c: ret
		} // end of method '<>c__DisplayClass96_0'::'<ExcudePropertyName>b__0'

	} // end of class <>c__DisplayClass96_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass99_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string searchName
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cdee
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass99_0'::.ctor

		.method assembly hidebysig 
			instance bool '<SeearchItem>b__0' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cdf7
			// Header size: 1
			// Code size: 50 (0x32)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0006: ldarg.0
			IL_0007: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::searchName
			IL_000c: callvirt instance bool [mscorlib]System.String::Contains(string)
			IL_0011: brfalse.s IL_0030

			IL_0013: ldarg.1
			IL_0014: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0019: ldarg.0
			IL_001a: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::'<>4__this'
			IL_001f: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0024: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0029: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_002e: br.s IL_0031

			IL_0030: ldc.i4.0

			IL_0031: ret
		} // end of method '<>c__DisplayClass99_0'::'<SeearchItem>b__0'

		.method assembly hidebysig 
			instance bool '<SeearchItem>b__2' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5ce2a
			// Header size: 1
			// Code size: 20 (0x14)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::'<>4__this'
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_000c: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0011: ceq
			IL_0013: ret
		} // end of method '<>c__DisplayClass99_0'::'<SeearchItem>b__2'

	} // end of class <>c__DisplayClass99_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass103_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[] vARIABLEs
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5ce3f
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass103_0'::.ctor

		.method assembly hidebysig 
			instance void '<GetDataAsync>b__1' () cil managed 
		{
			// Method begins at RVA 0x5ce48
			// Header size: 1
			// Code size: 20 (0x14)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::'<>4__this'
			IL_0007: ldarg.0
			IL_0008: ldfld valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[] VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::vARIABLEs
			IL_000d: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::GetData(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[])
			IL_0012: nop
			IL_0013: ret
		} // end of method '<>c__DisplayClass103_0'::'<GetDataAsync>b__1'

	} // end of class <>c__DisplayClass103_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass107_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string item

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5ce5d
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass107_0'::.ctor

		.method assembly hidebysig 
			instance bool '<GetData>b__2' (
				string x
			) cil managed 
		{
			// Method begins at RVA 0x5ce66
			// Header size: 1
			// Code size: 13 (0xd)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ldarg.0
			IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0'::item
			IL_0007: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_000c: ret
		} // end of method '<>c__DisplayClass107_0'::'<GetData>b__2'

	} // end of class <>c__DisplayClass107_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass108_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string varname

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5ce74
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass108_0'::.ctor

		.method assembly hidebysig 
			instance bool '<InitDynamicIO>b__0' (
				class [VM.Core]VM.Core.VmIO x
			) cil managed 
		{
			// Method begins at RVA 0x5ce7d
			// Header size: 1
			// Code size: 18 (0x12)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string [VM.Core]VM.Core.VmFrameTreeBase::get_Name()
			IL_0006: ldarg.0
			IL_0007: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass108_0'::varname
			IL_000c: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0011: ret
		} // end of method '<>c__DisplayClass108_0'::'<InitDynamicIO>b__0'

	} // end of class <>c__DisplayClass108_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass109_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 nDelIndex
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5ce90
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass109_0'::.ctor

		.method assembly hidebysig 
			instance void '<Delete_OnHandler>b__0' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5ce9c
			// Header size: 12
			// Code size: 22 (0x16)
			.maxstack 2
			.locals init (
				[0] bool
			)

			IL_0000: nop
			IL_0001: ldarg.1
			IL_0002: ldarg.0
			IL_0003: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::nDelIndex
			IL_0008: cgt
			IL_000a: stloc.0
			IL_000b: ldloc.0
			IL_000c: brfalse.s IL_0015

			IL_000e: nop
			IL_000f: ldarg.1
			IL_0010: ldc.i4.1
			IL_0011: sub
			IL_0012: starg.s x
			IL_0014: nop

			IL_0015: ret
		} // end of method '<>c__DisplayClass109_0'::'<Delete_OnHandler>b__0'

		.method assembly hidebysig 
			instance bool '<Delete_OnHandler>b__1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cebe
			// Header size: 1
			// Code size: 28 (0x1c)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0006: ldarg.0
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::'<>4__this'
			IL_000c: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0011: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0016: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_001b: ret
		} // end of method '<>c__DisplayClass109_0'::'<Delete_OnHandler>b__1'

	} // end of class <>c__DisplayClass109_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass109_1'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 nAddIndex
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0' 'CS$<>8__locals1'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cedb
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass109_1'::.ctor

		.method assembly hidebysig 
			instance bool '<Delete_OnHandler>b__2' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5cee4
			// Header size: 1
			// Code size: 36 (0x24)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ldarg.0
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::'CS$<>8__locals1'
			IL_000c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::'<>4__this'
			IL_0011: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_0016: ldarg.0
			IL_0017: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::nAddIndex
			IL_001c: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
			IL_0021: ceq
			IL_0023: ret
		} // end of method '<>c__DisplayClass109_1'::'<Delete_OnHandler>b__2'

	} // end of class <>c__DisplayClass109_1

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass117_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 currentIndex
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cf09
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass117_0'::.ctor

		.method assembly hidebysig 
			instance bool '<SetHeadIndex>b__0' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cf12
			// Header size: 1
			// Code size: 15 (0xf)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ldarg.0
			IL_0007: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::currentIndex
			IL_000c: ceq
			IL_000e: ret
		} // end of method '<>c__DisplayClass117_0'::'<SetHeadIndex>b__0'

		.method assembly hidebysig 
			instance bool '<SetHeadIndex>b__1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cf22
			// Header size: 1
			// Code size: 28 (0x1c)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0006: ldarg.0
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::'<>4__this'
			IL_000c: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0011: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0016: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_001b: ret
		} // end of method '<>c__DisplayClass117_0'::'<SetHeadIndex>b__1'

		.method assembly hidebysig 
			instance bool '<SetHeadIndex>b__3' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5cf3f
			// Header size: 1
			// Code size: 10 (0xa)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ldarg.0
			IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::currentIndex
			IL_0007: ceq
			IL_0009: ret
		} // end of method '<>c__DisplayClass117_0'::'<SetHeadIndex>b__3'

	} // end of class <>c__DisplayClass117_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass118_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 souceIndex
		.field public int32 destIndex
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cf4a
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass118_0'::.ctor

		.method assembly hidebysig 
			instance bool '<SwapIndex>b__0' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cf53
			// Header size: 1
			// Code size: 15 (0xf)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ldarg.0
			IL_0007: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
			IL_000c: ceq
			IL_000e: ret
		} // end of method '<>c__DisplayClass118_0'::'<SwapIndex>b__0'

		.method assembly hidebysig 
			instance bool '<SwapIndex>b__1' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cf63
			// Header size: 1
			// Code size: 15 (0xf)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0006: ldarg.0
			IL_0007: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
			IL_000c: ceq
			IL_000e: ret
		} // end of method '<>c__DisplayClass118_0'::'<SwapIndex>b__1'

		.method assembly hidebysig 
			instance bool '<SwapIndex>b__2' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5cf73
			// Header size: 1
			// Code size: 28 (0x1c)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0006: ldarg.0
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<>4__this'
			IL_000c: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0011: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0016: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_001b: ret
		} // end of method '<>c__DisplayClass118_0'::'<SwapIndex>b__2'

		.method assembly hidebysig 
			instance bool '<SwapIndex>b__3' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5cf90
			// Header size: 1
			// Code size: 10 (0xa)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ldarg.0
			IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
			IL_0007: ceq
			IL_0009: ret
		} // end of method '<>c__DisplayClass118_0'::'<SwapIndex>b__3'

		.method assembly hidebysig 
			instance bool '<SwapIndex>b__4' (
				int32 x
			) cil managed 
		{
			// Method begins at RVA 0x5cf9b
			// Header size: 1
			// Code size: 10 (0xa)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: ldarg.0
			IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
			IL_0007: ceq
			IL_0009: ret
		} // end of method '<>c__DisplayClass118_0'::'<SwapIndex>b__4'

	} // end of class <>c__DisplayClass118_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass126_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field public string filePath
		.field public int32 nRet
		.field public class [mscorlib]System.Action '<>9__5'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5cfa6
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass126_0'::.ctor

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__0' () cil managed 
		{
			// Method begins at RVA 0x5cfb0
			// Header size: 12
			// Code size: 301 (0x12d)
			.maxstack 6
			.locals init (
				[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1',
				[1] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2',
				[2] class [mscorlib]System.Exception,
				[3] bool
			)

			IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::.ctor()
			IL_0005: stloc.0
			IL_0006: ldloc.0
			IL_0007: ldarg.0
			IL_0008: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::'CS$<>8__locals1'
			IL_000d: nop
			.try
			{
				IL_000e: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::.ctor()
				IL_0013: stloc.1
				IL_0014: ldloc.1
				IL_0015: ldloc.0
				IL_0016: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::'CS$<>8__locals2'
				IL_001b: nop
				IL_001c: ldloc.1
				IL_001d: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]>::.ctor()
				IL_0022: stfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::varRunParamDict
				IL_0027: ldarg.0
				IL_0028: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
				IL_002d: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_0032: dup
				IL_0033: brtrue.s IL_0038

				IL_0035: pop
				IL_0036: br.s IL_004a

				IL_0038: ldloc.1
				IL_0039: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::'<ImportGlobalVar>b__3'()
				IL_003f: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0044: call instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_0049: nop

				IL_004a: ldarg.0
				IL_004b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
				IL_0050: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
				IL_0055: ldarg.0
				IL_0056: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::filePath
				IL_005b: ldc.i4.1
				IL_005c: ldloc.1
				IL_005d: ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::varRunParamDict
				IL_0062: ldarg.0
				IL_0063: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<ImportGlobalVar>b__4'()
				IL_0069: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_006e: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ImportVar(string, bool, class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]>, class [mscorlib]System.Action)
				IL_0073: nop
				IL_0074: nop
				IL_0075: leave.s IL_00b3
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_0077: stloc.2
				IL_0078: nop
				IL_0079: ldarg.0
				IL_007a: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
				IL_007f: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_0084: ldarg.0
				IL_0085: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<ImportGlobalVar>b__6'()
				IL_008b: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0090: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_0095: nop
				IL_0096: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
				IL_009b: ldstr "导入失败:"
				IL_00a0: ldloc.2
				IL_00a1: callvirt instance string [mscorlib]System.Object::ToString()
				IL_00a6: call string [mscorlib]System.String::Concat(string, string)
				IL_00ab: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
				IL_00b0: nop
				IL_00b1: leave.s IL_012c
			} // end handler

			IL_00b3: ldarg.0
			IL_00b4: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::nRet
			IL_00b9: ldc.i4.0
			IL_00ba: ceq
			IL_00bc: stloc.3
			IL_00bd: ldloc.3
			IL_00be: brfalse.s IL_00eb

			IL_00c0: nop
			IL_00c1: ldarg.0
			IL_00c2: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_00c7: ldnull
			IL_00c8: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_00cd: ldarg.0
			IL_00ce: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_00d3: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
			IL_00d8: ldarg.0
			IL_00d9: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<ImportGlobalVar>b__1'()
			IL_00df: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
			IL_00e4: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
			IL_00e9: nop
			IL_00ea: nop

			IL_00eb: ldloc.0
			IL_00ec: ldarg.0
			IL_00ed: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::nRet
			IL_00f2: brfalse.s IL_0100

			IL_00f4: ldstr "Lang_LoadFail"
			IL_00f9: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00fe: br.s IL_010a

			IL_0100: ldstr "Lang_LoadSuccess"
			IL_0105: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)

			IL_010a: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::msg
			IL_010f: ldarg.0
			IL_0110: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0115: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
			IL_011a: ldloc.0
			IL_011b: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::'<ImportGlobalVar>b__2'()
			IL_0121: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
			IL_0126: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
			IL_012b: nop

			IL_012c: ret
		} // end of method '<>c__DisplayClass126_0'::'<ImportGlobalVar>b__0'

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__4' () cil managed 
		{
			// Method begins at RVA 0x5d0fc
			// Header size: 12
			// Code size: 56 (0x38)
			.maxstack 4
			.locals init (
				[0] class [mscorlib]System.Action
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0007: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
			IL_000c: dup
			IL_000d: brtrue.s IL_0012

			IL_000f: pop
			IL_0010: br.s IL_0037

			IL_0012: ldarg.0
			IL_0013: ldfld class [mscorlib]System.Action VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>9__5'
			IL_0018: dup
			IL_0019: brtrue.s IL_0031

			IL_001b: pop
			IL_001c: ldarg.0
			IL_001d: ldarg.0
			IL_001e: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<ImportGlobalVar>b__5'()
			IL_0024: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
			IL_0029: dup
			IL_002a: stloc.0
			IL_002b: stfld class [mscorlib]System.Action VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>9__5'
			IL_0030: ldloc.0

			IL_0031: call instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
			IL_0036: nop

			IL_0037: ret
		} // end of method '<>c__DisplayClass126_0'::'<ImportGlobalVar>b__4'

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__5' () cil managed 
		{
			// Method begins at RVA 0x5d140
			// Header size: 12
			// Code size: 80 (0x50)
			.maxstack 2
			.locals init (
				[0] class [VM.Core]VM.Core.VmModule,
				[1] bool,
				[2] int32,
				[3] bool
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0007: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_000c: isinst [VM.Core]VM.Core.VmModule
			IL_0011: dup
			IL_0012: stloc.0
			IL_0013: ldnull
			IL_0014: cgt.un
			IL_0016: stloc.1
			IL_0017: ldloc.1
			IL_0018: brfalse.s IL_004f

			IL_001a: nop
			IL_001b: ldloc.0
			IL_001c: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
			IL_0021: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class [VM.Core]VM.Core.VmIO>::get_Count()
			IL_0026: ldc.i4.1
			IL_0027: sub
			IL_0028: stloc.2
			IL_0029: br.s IL_0043
			// loop start (head: IL_0043)
				IL_002b: nop
				IL_002c: ldloc.0
				IL_002d: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
				IL_0032: ldloc.2
				IL_0033: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class [VM.Core]VM.Core.VmIO>::get_Item(int32)
				IL_0038: callvirt instance void [VM.Core]VM.Core.VmIO::Dispose()
				IL_003d: nop
				IL_003e: nop
				IL_003f: ldloc.2
				IL_0040: ldc.i4.1
				IL_0041: sub
				IL_0042: stloc.2

				IL_0043: ldloc.2
				IL_0044: ldc.i4.0
				IL_0045: clt
				IL_0047: ldc.i4.0
				IL_0048: ceq
				IL_004a: stloc.3
				IL_004b: ldloc.3
				IL_004c: brtrue.s IL_002b
			// end loop

			IL_004e: nop

			IL_004f: ret
		} // end of method '<>c__DisplayClass126_0'::'<ImportGlobalVar>b__5'

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__6' () cil managed 
		{
			// Method begins at RVA 0x5d19c
			// Header size: 1
			// Code size: 37 (0x25)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0007: ldstr "Collapsed"
			IL_000c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_0011: nop
			IL_0012: ldstr "Lang_LoadFail"
			IL_0017: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_001c: ldc.i4.3
			IL_001d: ldc.i4.0
			IL_001e: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0023: pop
			IL_0024: ret
		} // end of method '<>c__DisplayClass126_0'::'<ImportGlobalVar>b__6'

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__1' () cil managed 
		{
			// Method begins at RVA 0x5d1c4
			// Header size: 12
			// Code size: 179 (0xb3)
			.maxstack 2
			.locals init (
				[0] valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[]
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0007: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_000c: dup
			IL_000d: brtrue.s IL_0012

			IL_000f: pop
			IL_0010: br.s IL_0018

			IL_0012: call instance void VMControls.WPF.GlobalUI.VarGroupItem::Clear()
			IL_0017: nop

			IL_0018: ldarg.0
			IL_0019: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_001e: ldnull
			IL_001f: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectGroupItem(class VMControls.WPF.GlobalUI.VarGroupItem)
			IL_0024: nop
			IL_0025: ldarg.0
			IL_0026: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_002b: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
			IL_0030: dup
			IL_0031: brtrue.s IL_0036

			IL_0033: pop
			IL_0034: br.s IL_003c

			IL_0036: call instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::Clear()
			IL_003b: nop

			IL_003c: ldarg.0
			IL_003d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0042: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0047: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Clear()
			IL_004c: nop
			IL_004d: ldarg.0
			IL_004e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0053: ldc.i4.0
			IL_0054: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
			IL_0059: nop
			IL_005a: ldarg.0
			IL_005b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0060: ldc.i4.0
			IL_0061: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
			IL_0066: nop
			IL_0067: ldarg.0
			IL_0068: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_006d: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
			IL_0072: nop
			IL_0073: ldarg.0
			IL_0074: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0079: ldc.i4.1
			IL_007a: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::bFirstGetData
			IL_007f: ldarg.0
			IL_0080: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0085: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::GetVarGroupMsg()
			IL_008a: pop
			IL_008b: ldc.i4.0
			IL_008c: newarr [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE
			IL_0091: stloc.0
			IL_0092: ldarg.0
			IL_0093: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0098: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_009d: ldloca.s 0
			IL_009f: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::GetAllVar(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[]&)
			IL_00a4: nop
			IL_00a5: ldarg.0
			IL_00a6: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_00ab: ldloc.0
			IL_00ac: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::GetData(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[])
			IL_00b1: nop
			IL_00b2: ret
		} // end of method '<>c__DisplayClass126_0'::'<ImportGlobalVar>b__1'

	} // end of class <>c__DisplayClass126_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass126_1'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string msg
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' 'CS$<>8__locals1'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d283
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass126_1'::.ctor

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__2' () cil managed 
		{
			// Method begins at RVA 0x5d28c
			// Header size: 1
			// Code size: 54 (0x36)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::'CS$<>8__locals1'
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_000c: ldstr "Collapsed"
			IL_0011: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_0016: nop
			IL_0017: ldarg.0
			IL_0018: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::msg
			IL_001d: ldarg.0
			IL_001e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::'CS$<>8__locals1'
			IL_0023: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::nRet
			IL_0028: brfalse.s IL_002d

			IL_002a: ldc.i4.3
			IL_002b: br.s IL_002e

			IL_002d: ldc.i4.4

			IL_002e: ldc.i4.0
			IL_002f: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0034: pop
			IL_0035: ret
		} // end of method '<>c__DisplayClass126_1'::'<ImportGlobalVar>b__2'

	} // end of class <>c__DisplayClass126_1

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass126_2'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]> varRunParamDict
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1' 'CS$<>8__locals2'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d2c3
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass126_2'::.ctor

		.method assembly hidebysig 
			instance void '<ImportGlobalVar>b__3' () cil managed 
		{
			// Method begins at RVA 0x5d2cc
			// Header size: 12
			// Code size: 91 (0x5b)
			.maxstack 3
			.locals init (
				[0] class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
				[1] class VMControls.WPF.GlobalUI.GlobalVariableItem
			)

			IL_0000: nop
			IL_0001: nop
			IL_0002: ldarg.0
			IL_0003: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::'CS$<>8__locals2'
			IL_0008: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_1'::'CS$<>8__locals1'
			IL_000d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
			IL_0012: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0017: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::GetEnumerator()
			IL_001c: stloc.0
			.try
			{
				IL_001d: br.s IL_0045
				// loop start (head: IL_0045)
					IL_001f: ldloc.0
					IL_0020: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Current()
					IL_0025: stloc.1
					IL_0026: nop
					IL_0027: ldarg.0
					IL_0028: ldfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_2'::varRunParamDict
					IL_002d: ldloc.1
					IL_002e: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
					IL_0033: ldloc.1
					IL_0034: callvirt instance class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode> VMControls.WPF.GlobalUI.GlobalVariableItem::get_SelectedRunParams()
					IL_0039: call class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVarExtern::GetLinkModuleInfoByRunParams(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode>)
					IL_003e: callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[]>::Add(!0, !1)
					IL_0043: nop
					IL_0044: nop

					IL_0045: ldloc.0
					IL_0046: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_004b: brtrue.s IL_001f
				// end loop

				IL_004d: leave.s IL_005a
			} // end .try
			finally
			{
				IL_004f: ldloc.0
				IL_0050: brfalse.s IL_0059

				IL_0052: ldloc.0
				IL_0053: callvirt instance void [mscorlib]System.IDisposable::Dispose()
				IL_0058: nop

				IL_0059: endfinally
			} // end handler

			IL_005a: ret
		} // end of method '<>c__DisplayClass126_2'::'<ImportGlobalVar>b__3'

	} // end of class <>c__DisplayClass126_2

	.class nested private auto ansi sealed beforefieldinit '<ImportGlobalVar>d__126'
		extends [mscorlib]System.Object
		implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 '<>1__state'
		.field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
		.field public string filePath
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field private class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' '<>8__1'
		.field private class [mscorlib]System.Threading.Tasks.Task '<importTask>5__2'
		.field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d344
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<ImportGlobalVar>d__126'::.ctor

		.method private final hidebysig newslot virtual 
			instance void MoveNext () cil managed 
		{
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
			// Method begins at RVA 0x5d350
			// Header size: 12
			// Code size: 248 (0xf8)
			.maxstack 4
			.locals init (
				[0] int32,
				[1] valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,
				[2] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126',
				[3] class [mscorlib]System.Exception
			)

			IL_0000: ldarg.0
			IL_0001: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>1__state'
			IL_0006: stloc.0
			.try
			{
				IL_0007: ldloc.0
				IL_0008: brfalse.s IL_000c

				IL_000a: br.s IL_0011

				IL_000c: br IL_00a5

				IL_0011: ldarg.0
				IL_0012: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::.ctor()
				IL_0017: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>8__1'
				IL_001c: ldarg.0
				IL_001d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>8__1'
				IL_0022: ldarg.0
				IL_0023: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>4__this'
				IL_0028: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<>4__this'
				IL_002d: ldarg.0
				IL_002e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>8__1'
				IL_0033: ldarg.0
				IL_0034: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::filePath
				IL_0039: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::filePath
				IL_003e: nop
				IL_003f: ldarg.0
				IL_0040: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>8__1'
				IL_0045: ldc.i4.0
				IL_0046: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::nRet
				IL_004b: ldarg.0
				IL_004c: call class [mscorlib]System.Threading.Tasks.TaskFactory [mscorlib]System.Threading.Tasks.Task::get_Factory()
				IL_0051: ldarg.0
				IL_0052: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>8__1'
				IL_0057: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass126_0'::'<ImportGlobalVar>b__0'()
				IL_005d: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0062: callvirt instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.TaskFactory::StartNew(class [mscorlib]System.Action)
				IL_0067: stfld class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<importTask>5__2'
				IL_006c: ldarg.0
				IL_006d: ldfld class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<importTask>5__2'
				IL_0072: callvirt instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
				IL_0077: stloc.1
				IL_0078: ldloca.s 1
				IL_007a: call instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
				IL_007f: brtrue.s IL_00c1

				IL_0081: ldarg.0
				IL_0082: ldc.i4.0
				IL_0083: dup
				IL_0084: stloc.0
				IL_0085: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>1__state'
				IL_008a: ldarg.0
				IL_008b: ldloc.1
				IL_008c: stfld valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>u__1'
				IL_0091: ldarg.0
				IL_0092: stloc.2
				IL_0093: ldarg.0
				IL_0094: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>t__builder'
				IL_0099: ldloca.s 1
				IL_009b: ldloca.s 2
				IL_009d: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter, class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'>(!!0&, !!1&)
				IL_00a2: nop
				IL_00a3: leave.s IL_00f7

				IL_00a5: ldarg.0
				IL_00a6: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>u__1'
				IL_00ab: stloc.1
				IL_00ac: ldarg.0
				IL_00ad: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>u__1'
				IL_00b2: initobj [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
				IL_00b8: ldarg.0
				IL_00b9: ldc.i4.m1
				IL_00ba: dup
				IL_00bb: stloc.0
				IL_00bc: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>1__state'

				IL_00c1: ldloca.s 1
				IL_00c3: call instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
				IL_00c8: nop
				IL_00c9: leave.s IL_00e3
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_00cb: stloc.3
				IL_00cc: ldarg.0
				IL_00cd: ldc.i4.s -2
				IL_00cf: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>1__state'
				IL_00d4: ldarg.0
				IL_00d5: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>t__builder'
				IL_00da: ldloc.3
				IL_00db: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
				IL_00e0: nop
				IL_00e1: leave.s IL_00f7
			} // end handler

			IL_00e3: ldarg.0
			IL_00e4: ldc.i4.s -2
			IL_00e6: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>1__state'
			IL_00eb: ldarg.0
			IL_00ec: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>t__builder'
			IL_00f1: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
			IL_00f6: nop

			IL_00f7: ret
		} // end of method '<ImportGlobalVar>d__126'::MoveNext

		.method private final hidebysig newslot virtual 
			instance void SetStateMachine (
				class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine
			) cil managed 
		{
			.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
			// Method begins at RVA 0x5d464
			// Header size: 1
			// Code size: 1 (0x1)
			.maxstack 8

			IL_0000: ret
		} // end of method '<ImportGlobalVar>d__126'::SetStateMachine

	} // end of class <ImportGlobalVar>d__126

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass128_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field public string filePath
		.field public string downLoadPath

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d466
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass128_0'::.ctor

		.method assembly hidebysig 
			instance void '<ExportGlobalVar>b__0' () cil managed 
		{
			// Method begins at RVA 0x5d470
			// Header size: 12
			// Code size: 352 (0x160)
			.maxstack 5
			.locals init (
				[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1',
				[1] class [mscorlib]System.Exception,
				[2] bool,
				[3] bool
			)

			IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::.ctor()
			IL_0005: stloc.0
			IL_0006: nop
			IL_0007: ldloc.0
			IL_0008: ldc.i4.0
			IL_0009: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::nRet
			.try
			{
				IL_000e: nop
				IL_000f: ldarg.0
				IL_0010: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::'<>4__this'
				IL_0015: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
				IL_001a: ldarg.0
				IL_001b: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::filePath
				IL_0020: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ExportVar(string)
				IL_0025: nop
				IL_0026: nop
				IL_0027: leave.s IL_007b
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_0029: stloc.1
				IL_002a: nop
				IL_002b: ldarg.0
				IL_002c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::'<>4__this'
				IL_0031: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_0036: ldsfld class [mscorlib]System.Action VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__128_2'
				IL_003b: dup
				IL_003c: brtrue.s IL_0055

				IL_003e: pop
				IL_003f: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
				IL_0044: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<ExportGlobalVar>b__128_2'()
				IL_004a: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_004f: dup
				IL_0050: stsfld class [mscorlib]System.Action VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__128_2'

				IL_0055: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_005a: nop
				IL_005b: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
				IL_0060: ldstr "导出全局变量失败:"
				IL_0065: ldloc.1
				IL_0066: callvirt instance string [mscorlib]System.Object::ToString()
				IL_006b: call string [mscorlib]System.String::Concat(string, string)
				IL_0070: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
				IL_0075: nop
				IL_0076: leave IL_015f
			} // end handler

			IL_007b: ldarg.0
			IL_007c: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::downLoadPath
			IL_0081: ldstr ""
			IL_0086: call bool [mscorlib]System.String::op_Inequality(string, string)
			IL_008b: stloc.2
			IL_008c: ldloc.2
			IL_008d: brfalse IL_0128

			IL_0092: nop
			IL_0093: ldsfld native int [VM.PlatformSDKCS]VM.PlatformSDKCS.CVmObjectBase::mhandle
			IL_0098: ldarg.0
			IL_0099: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::downLoadPath
			IL_009e: ldarg.0
			IL_009f: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::filePath
			IL_00a4: ldc.i4.1
			IL_00a5: call bool [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKAPI::FtpDownloadFile(native int, string, string, bool)
			IL_00aa: brfalse.s IL_00bc

			IL_00ac: ldarg.0
			IL_00ad: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::downLoadPath
			IL_00b2: call bool [mscorlib]System.IO.File::Exists(string)
			IL_00b7: ldc.i4.0
			IL_00b8: ceq
			IL_00ba: br.s IL_00bd

			IL_00bc: ldc.i4.1

			IL_00bd: stloc.3
			IL_00be: ldloc.3
			IL_00bf: brfalse.s IL_0116

			IL_00c1: nop
			IL_00c2: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_00c7: ldc.i4.5
			IL_00c8: newarr [mscorlib]System.String
			IL_00cd: dup
			IL_00ce: ldc.i4.0
			IL_00cf: ldstr "Down load file error, local file path = "
			IL_00d4: stelem.ref
			IL_00d5: dup
			IL_00d6: ldc.i4.1
			IL_00d7: ldarg.0
			IL_00d8: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::downLoadPath
			IL_00dd: stelem.ref
			IL_00de: dup
			IL_00df: ldc.i4.2
			IL_00e0: ldstr ", remote file path = "
			IL_00e5: stelem.ref
			IL_00e6: dup
			IL_00e7: ldc.i4.3
			IL_00e8: ldarg.0
			IL_00e9: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::filePath
			IL_00ee: stelem.ref
			IL_00ef: dup
			IL_00f0: ldc.i4.4
			IL_00f1: ldstr "."
			IL_00f6: stelem.ref
			IL_00f7: call string [mscorlib]System.String::Concat(string[])
			IL_00fc: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_0101: nop
			IL_0102: ldstr "导出失败"
			IL_0107: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_010c: ldc.i4.3
			IL_010d: ldc.i4.0
			IL_010e: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0113: pop
			IL_0114: br.s IL_015f

			IL_0116: ldsfld native int [VM.PlatformSDKCS]VM.PlatformSDKCS.CVmObjectBase::mhandle
			IL_011b: ldarg.0
			IL_011c: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::filePath
			IL_0121: call bool [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKAPI::FtpRemoveFile(native int, string)
			IL_0126: pop
			IL_0127: nop

			IL_0128: ldloc.0
			IL_0129: ldloc.0
			IL_012a: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::nRet
			IL_012f: brfalse.s IL_0138

			IL_0131: ldstr "导出失败"
			IL_0136: br.s IL_013d

			IL_0138: ldstr "Lang_ExportSuccess"

			IL_013d: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::msg
			IL_0142: ldarg.0
			IL_0143: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::'<>4__this'
			IL_0148: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
			IL_014d: ldloc.0
			IL_014e: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::'<ExportGlobalVar>b__1'()
			IL_0154: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
			IL_0159: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
			IL_015e: nop

			IL_015f: ret
		} // end of method '<>c__DisplayClass128_0'::'<ExportGlobalVar>b__0'

	} // end of class <>c__DisplayClass128_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass128_1'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string msg
		.field public int32 nRet

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d5ec
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass128_1'::.ctor

		.method assembly hidebysig 
			instance void '<ExportGlobalVar>b__1' () cil managed 
		{
			// Method begins at RVA 0x5d5f5
			// Header size: 1
			// Code size: 32 (0x20)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::msg
			IL_0007: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_000c: ldarg.0
			IL_000d: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_1'::nRet
			IL_0012: brfalse.s IL_0017

			IL_0014: ldc.i4.3
			IL_0015: br.s IL_0018

			IL_0017: ldc.i4.4

			IL_0018: ldc.i4.0
			IL_0019: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_001e: pop
			IL_001f: ret
		} // end of method '<>c__DisplayClass128_1'::'<ExportGlobalVar>b__1'

	} // end of class <>c__DisplayClass128_1

	.class nested private auto ansi sealed beforefieldinit '<ExportGlobalVar>d__128'
		extends [mscorlib]System.Object
		implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 '<>1__state'
		.field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
		.field public string filePath
		.field public string downLoadPath
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field private class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0' '<>8__1'
		.field private class [mscorlib]System.Threading.Tasks.Task '<exportTask>5__2'
		.field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d616
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<ExportGlobalVar>d__128'::.ctor

		.method private final hidebysig newslot virtual 
			instance void MoveNext () cil managed 
		{
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
			// Method begins at RVA 0x5d620
			// Header size: 12
			// Code size: 253 (0xfd)
			.maxstack 4
			.locals init (
				[0] int32,
				[1] valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,
				[2] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128',
				[3] class [mscorlib]System.Exception
			)

			IL_0000: ldarg.0
			IL_0001: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>1__state'
			IL_0006: stloc.0
			.try
			{
				IL_0007: ldloc.0
				IL_0008: brfalse.s IL_000c

				IL_000a: br.s IL_0011

				IL_000c: br IL_00aa

				IL_0011: ldarg.0
				IL_0012: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::.ctor()
				IL_0017: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>8__1'
				IL_001c: ldarg.0
				IL_001d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>8__1'
				IL_0022: ldarg.0
				IL_0023: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>4__this'
				IL_0028: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::'<>4__this'
				IL_002d: ldarg.0
				IL_002e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>8__1'
				IL_0033: ldarg.0
				IL_0034: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::filePath
				IL_0039: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::filePath
				IL_003e: ldarg.0
				IL_003f: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>8__1'
				IL_0044: ldarg.0
				IL_0045: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::downLoadPath
				IL_004a: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::downLoadPath
				IL_004f: nop
				IL_0050: ldarg.0
				IL_0051: call class [mscorlib]System.Threading.Tasks.TaskFactory [mscorlib]System.Threading.Tasks.Task::get_Factory()
				IL_0056: ldarg.0
				IL_0057: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>8__1'
				IL_005c: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass128_0'::'<ExportGlobalVar>b__0'()
				IL_0062: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0067: callvirt instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.TaskFactory::StartNew(class [mscorlib]System.Action)
				IL_006c: stfld class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<exportTask>5__2'
				IL_0071: ldarg.0
				IL_0072: ldfld class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<exportTask>5__2'
				IL_0077: callvirt instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
				IL_007c: stloc.1
				IL_007d: ldloca.s 1
				IL_007f: call instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
				IL_0084: brtrue.s IL_00c6

				IL_0086: ldarg.0
				IL_0087: ldc.i4.0
				IL_0088: dup
				IL_0089: stloc.0
				IL_008a: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>1__state'
				IL_008f: ldarg.0
				IL_0090: ldloc.1
				IL_0091: stfld valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>u__1'
				IL_0096: ldarg.0
				IL_0097: stloc.2
				IL_0098: ldarg.0
				IL_0099: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>t__builder'
				IL_009e: ldloca.s 1
				IL_00a0: ldloca.s 2
				IL_00a2: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter, class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'>(!!0&, !!1&)
				IL_00a7: nop
				IL_00a8: leave.s IL_00fc

				IL_00aa: ldarg.0
				IL_00ab: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>u__1'
				IL_00b0: stloc.1
				IL_00b1: ldarg.0
				IL_00b2: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>u__1'
				IL_00b7: initobj [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
				IL_00bd: ldarg.0
				IL_00be: ldc.i4.m1
				IL_00bf: dup
				IL_00c0: stloc.0
				IL_00c1: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>1__state'

				IL_00c6: ldloca.s 1
				IL_00c8: call instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
				IL_00cd: nop
				IL_00ce: leave.s IL_00e8
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_00d0: stloc.3
				IL_00d1: ldarg.0
				IL_00d2: ldc.i4.s -2
				IL_00d4: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>1__state'
				IL_00d9: ldarg.0
				IL_00da: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>t__builder'
				IL_00df: ldloc.3
				IL_00e0: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
				IL_00e5: nop
				IL_00e6: leave.s IL_00fc
			} // end handler

			IL_00e8: ldarg.0
			IL_00e9: ldc.i4.s -2
			IL_00eb: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>1__state'
			IL_00f0: ldarg.0
			IL_00f1: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>t__builder'
			IL_00f6: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
			IL_00fb: nop

			IL_00fc: ret
		} // end of method '<ExportGlobalVar>d__128'::MoveNext

		.method private final hidebysig newslot virtual 
			instance void SetStateMachine (
				class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine
			) cil managed 
		{
			.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
			// Method begins at RVA 0x5d73c
			// Header size: 1
			// Code size: 1 (0x1)
			.maxstack 8

			IL_0000: ret
		} // end of method '<ExportGlobalVar>d__128'::SetStateMachine

	} // end of class <ExportGlobalVar>d__128

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass135_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field public class VMControls.WPF.GlobalUI.VarGroupItem varGroup
		.field public class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> '<>9__5'
		.field public class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> '<>9__6'
		.field public class [mscorlib]System.Action`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> '<>9__8'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d73e
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass135_0'::.ctor

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__0' () cil managed 
		{
			// Method begins at RVA 0x5d748
			// Header size: 12
			// Code size: 421 (0x1a5)
			.maxstack 4
			.locals init (
				[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1',
				[1] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
				[2] class VMControls.WPF.GlobalUI.GlobalVariableItem,
				[3] class [mscorlib]System.Exception,
				[4] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'
			)

			IL_0000: nop
			.try
			{
				IL_0001: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::.ctor()
				IL_0006: stloc.0
				IL_0007: ldloc.0
				IL_0008: ldarg.0
				IL_0009: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
				IL_000e: nop
				IL_000f: ldloc.0
				IL_0010: ldarg.0
				IL_0011: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_0016: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_001b: isinst [VM.Core]VM.Core.VmModule
				IL_0020: stfld class [VM.Core]VM.Core.VmModule VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::vmModule
				IL_0025: ldarg.0
				IL_0026: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_002b: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
				IL_0030: ldarg.0
				IL_0031: ldfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::varGroup
				IL_0036: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
				IL_003b: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::DeleteGroup(string)
				IL_0040: nop
				IL_0041: ldloc.0
				IL_0042: newobj instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor()
				IL_0047: stfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::tmplist
				IL_004c: ldarg.0
				IL_004d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_0052: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_0057: ldloc.0
				IL_0058: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<DelVarGroup>b__1'()
				IL_005e: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0063: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_0068: nop
				IL_0069: nop
				IL_006a: ldloc.0
				IL_006b: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::tmplist
				IL_0070: callvirt instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::GetEnumerator()
				IL_0075: stloc.1
				.try
				{
					IL_0076: br.s IL_00a5
					// loop start (head: IL_00a5)
						IL_0078: ldloca.s 1
						IL_007a: call instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Current()
						IL_007f: stloc.2
						IL_0080: nop
						IL_0081: ldarg.0
						IL_0082: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
						IL_0087: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
						IL_008c: ldloc.2
						IL_008d: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
						IL_0092: ldc.i4.1
						IL_0093: ldloc.2
						IL_0094: callvirt instance class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode> VMControls.WPF.GlobalUI.GlobalVariableItem::get_SelectedRunParams()
						IL_0099: call class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVarExtern::GetLinkModuleInfoByRunParams(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode>)
						IL_009e: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::DeleteVarFromGroup(string, bool, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[])
						IL_00a3: nop
						IL_00a4: nop

						IL_00a5: ldloca.s 1
						IL_00a7: call instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class VMControls.WPF.GlobalUI.GlobalVariableItem>::MoveNext()
						IL_00ac: brtrue.s IL_0078
					// end loop

					IL_00ae: leave.s IL_00bf
				} // end .try
				finally
				{
					IL_00b0: ldloca.s 1
					IL_00b2: constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<class VMControls.WPF.GlobalUI.GlobalVariableItem>
					IL_00b8: callvirt instance void [mscorlib]System.IDisposable::Dispose()
					IL_00bd: nop
					IL_00be: endfinally
				} // end handler

				IL_00bf: ldarg.0
				IL_00c0: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_00c5: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_00ca: ldloc.0
				IL_00cb: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<DelVarGroup>b__2'()
				IL_00d1: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_00d6: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_00db: nop
				IL_00dc: ldloc.0
				IL_00dd: newobj instance void class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem>::.ctor()
				IL_00e2: stfld class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::allVarList
				IL_00e7: ldarg.0
				IL_00e8: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_00ed: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
				IL_00f2: ldloc.0
				IL_00f3: ldflda class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::allVarList
				IL_00f8: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::GetAllVar(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem>&)
				IL_00fd: nop
				IL_00fe: ldarg.0
				IL_00ff: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_0104: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_0109: ldloc.0
				IL_010a: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<DelVarGroup>b__3'()
				IL_0110: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0115: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_011a: nop
				IL_011b: ldloc.0
				IL_011c: ldstr "Lang_DeleteSucceed"
				IL_0121: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
				IL_0126: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::msg
				IL_012b: ldarg.0
				IL_012c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_0131: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_0136: ldloc.0
				IL_0137: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<DelVarGroup>b__4'()
				IL_013d: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0142: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_0147: nop
				IL_0148: nop
				IL_0149: leave.s IL_01a4
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_014b: stloc.3
				IL_014c: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'::.ctor()
				IL_0151: stloc.s 4
				IL_0153: ldloc.s 4
				IL_0155: ldarg.0
				IL_0156: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'::'CS$<>8__locals2'
				IL_015b: nop
				IL_015c: ldstr "DelVarGroup is exception:"
				IL_0161: ldloc.3
				IL_0162: callvirt instance string [mscorlib]System.Object::ToString()
				IL_0167: call string [mscorlib]System.String::Concat(string, string)
				IL_016c: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
				IL_0171: nop
				IL_0172: ldloc.s 4
				IL_0174: ldstr "Lang_DeleteFail"
				IL_0179: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
				IL_017e: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'::msg
				IL_0183: ldarg.0
				IL_0184: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_0189: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
				IL_018e: ldloc.s 4
				IL_0190: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'::'<DelVarGroup>b__10'()
				IL_0196: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_019b: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
				IL_01a0: nop
				IL_01a1: nop
				IL_01a2: leave.s IL_01a4
			} // end handler

			IL_01a4: ret
		} // end of method '<>c__DisplayClass135_0'::'<DelVarGroup>b__0'

		.method assembly hidebysig 
			instance bool '<DelVarGroup>b__5' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5d930
			// Header size: 1
			// Code size: 35 (0x23)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0006: ldarg.0
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
			IL_000c: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0011: dup
			IL_0012: brtrue.s IL_0018

			IL_0014: pop
			IL_0015: ldnull
			IL_0016: br.s IL_001d

			IL_0018: call instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()

			IL_001d: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0022: ret
		} // end of method '<>c__DisplayClass135_0'::'<DelVarGroup>b__5'

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__6' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5d954
			// Header size: 1
			// Code size: 19 (0x13)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
			IL_0006: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_000b: ldarg.1
			IL_000c: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Remove(!0)
			IL_0011: pop
			IL_0012: ret
		} // end of method '<>c__DisplayClass135_0'::'<DelVarGroup>b__6'

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__8' (
				class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem m
			) cil managed 
		{
			// Method begins at RVA 0x5d968
			// Header size: 12
			// Code size: 72 (0x48)
			.maxstack 3
			.locals init (
				[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_2',
				[1] class VMControls.WPF.GlobalUI.GlobalVariableItem,
				[2] bool
			)

			IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_2'::.ctor()
			IL_0005: stloc.0
			IL_0006: ldloc.0
			IL_0007: ldarg.1
			IL_0008: stfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_2'::m
			IL_000d: nop
			IL_000e: ldarg.0
			IL_000f: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
			IL_0014: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0019: ldloc.0
			IL_001a: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_2'::'<DelVarGroup>b__9'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_0020: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_0025: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_002a: stloc.1
			IL_002b: ldloc.1
			IL_002c: ldnull
			IL_002d: cgt.un
			IL_002f: stloc.2
			IL_0030: ldloc.2
			IL_0031: brfalse.s IL_0047

			IL_0033: nop
			IL_0034: ldloc.1
			IL_0035: ldloc.0
			IL_0036: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_2'::m
			IL_003b: callvirt instance int32 [Apps.XmlParser]Apps.XmlParser.Variable.VarItem::get_Index()
			IL_0040: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
			IL_0045: nop
			IL_0046: nop

			IL_0047: ret
		} // end of method '<>c__DisplayClass135_0'::'<DelVarGroup>b__8'

	} // end of class <>c__DisplayClass135_0

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass135_1'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> tmplist
		.field public class [VM.Core]VM.Core.VmModule vmModule
		.field public class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> allVarList
		.field public string msg
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' 'CS$<>8__locals1'
		.field public class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> '<>9__7'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5d9bc
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass135_1'::.ctor

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__1' () cil managed 
		{
			// Method begins at RVA 0x5d9c8
			// Header size: 12
			// Code size: 80 (0x50)
			.maxstack 5
			.locals init (
				[0] class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldarg.0
			IL_0003: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_0008: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
			IL_000d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0012: ldarg.0
			IL_0013: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_0018: ldfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>9__5'
			IL_001d: dup
			IL_001e: brtrue.s IL_0040

			IL_0020: pop
			IL_0021: ldarg.0
			IL_0022: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_0027: ldarg.0
			IL_0028: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_002d: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<DelVarGroup>b__5'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_0033: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_0038: dup
			IL_0039: stloc.0
			IL_003a: stfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>9__5'
			IL_003f: ldloc.0

			IL_0040: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_0045: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
			IL_004a: stfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::tmplist
			IL_004f: ret
		} // end of method '<>c__DisplayClass135_1'::'<DelVarGroup>b__1'

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__2' () cil managed 
		{
			// Method begins at RVA 0x5da24
			// Header size: 12
			// Code size: 103 (0x67)
			.maxstack 4
			.locals init (
				[0] class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::tmplist
			IL_0007: ldarg.0
			IL_0008: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_000d: ldfld class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>9__6'
			IL_0012: dup
			IL_0013: brtrue.s IL_0035

			IL_0015: pop
			IL_0016: ldarg.0
			IL_0017: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_001c: ldarg.0
			IL_001d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_0022: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<DelVarGroup>b__6'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_0028: newobj instance void class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor(object, native int)
			IL_002d: dup
			IL_002e: stloc.0
			IL_002f: stfld class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>9__6'
			IL_0034: ldloc.0

			IL_0035: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::ForEach(class [mscorlib]System.Action`1<!0>)
			IL_003a: nop
			IL_003b: ldarg.0
			IL_003c: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::tmplist
			IL_0041: ldarg.0
			IL_0042: ldfld class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<>9__7'
			IL_0047: dup
			IL_0048: brtrue.s IL_0060

			IL_004a: pop
			IL_004b: ldarg.0
			IL_004c: ldarg.0
			IL_004d: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<DelVarGroup>b__7'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_0053: newobj instance void class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor(object, native int)
			IL_0058: dup
			IL_0059: stloc.0
			IL_005a: stfld class [mscorlib]System.Action`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'<>9__7'
			IL_005f: ldloc.0

			IL_0060: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::ForEach(class [mscorlib]System.Action`1<!0>)
			IL_0065: nop
			IL_0066: ret
		} // end of method '<>c__DisplayClass135_1'::'<DelVarGroup>b__2'

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__7' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5da98
			// Header size: 12
			// Code size: 37 (0x25)
			.maxstack 2
			.locals init (
				[0] class [VM.Core]VM.Core.VmIO
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class [VM.Core]VM.Core.VmModule VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::vmModule
			IL_0007: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
			IL_000c: ldarg.1
			IL_000d: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0012: callvirt instance class [VM.Core]VM.Core.VmIO [VM.Core]VM.Core.VmIOCollection::get_Item(string)
			IL_0017: stloc.0
			IL_0018: ldloc.0
			IL_0019: brtrue.s IL_001d

			IL_001b: br.s IL_0024

			IL_001d: ldloc.0
			IL_001e: call instance void [VM.Core]VM.Core.VmIO::RemoveSelf()
			IL_0023: nop

			IL_0024: ret
		} // end of method '<>c__DisplayClass135_1'::'<DelVarGroup>b__7'

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__3' () cil managed 
		{
			// Method begins at RVA 0x5dacc
			// Header size: 12
			// Code size: 60 (0x3c)
			.maxstack 4
			.locals init (
				[0] class [mscorlib]System.Action`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem>
			)

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::allVarList
			IL_0007: ldarg.0
			IL_0008: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_000d: ldfld class [mscorlib]System.Action`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>9__8'
			IL_0012: dup
			IL_0013: brtrue.s IL_0035

			IL_0015: pop
			IL_0016: ldarg.0
			IL_0017: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_001c: ldarg.0
			IL_001d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_0022: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<DelVarGroup>b__8'(class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem)
			IL_0028: newobj instance void class [mscorlib]System.Action`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem>::.ctor(object, native int)
			IL_002d: dup
			IL_002e: stloc.0
			IL_002f: stfld class [mscorlib]System.Action`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>9__8'
			IL_0034: ldloc.0

			IL_0035: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem>::ForEach(class [mscorlib]System.Action`1<!0>)
			IL_003a: nop
			IL_003b: ret
		} // end of method '<>c__DisplayClass135_1'::'<DelVarGroup>b__3'

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__4' () cil managed 
		{
			// Method begins at RVA 0x5db14
			// Header size: 1
			// Code size: 38 (0x26)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::'CS$<>8__locals1'
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
			IL_000c: ldstr "Collapsed"
			IL_0011: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_0016: nop
			IL_0017: ldarg.0
			IL_0018: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_1'::msg
			IL_001d: ldc.i4.4
			IL_001e: ldc.i4.0
			IL_001f: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0024: pop
			IL_0025: ret
		} // end of method '<>c__DisplayClass135_1'::'<DelVarGroup>b__4'

	} // end of class <>c__DisplayClass135_1

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass135_2'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem m

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5db3b
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass135_2'::.ctor

		.method assembly hidebysig 
			instance bool '<DelVarGroup>b__9' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem p
			) cil managed 
		{
			// Method begins at RVA 0x5db44
			// Header size: 1
			// Code size: 23 (0x17)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0006: ldarg.0
			IL_0007: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_2'::m
			IL_000c: callvirt instance string [Apps.XmlParser]Apps.XmlParser.Variable.VarItem::get_Name()
			IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0016: ret
		} // end of method '<>c__DisplayClass135_2'::'<DelVarGroup>b__9'

	} // end of class <>c__DisplayClass135_2

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass135_3'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string msg
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' 'CS$<>8__locals2'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5db5c
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass135_3'::.ctor

		.method assembly hidebysig 
			instance void '<DelVarGroup>b__10' () cil managed 
		{
			// Method begins at RVA 0x5db65
			// Header size: 1
			// Code size: 38 (0x26)
			.maxstack 8

			IL_0000: nop
			IL_0001: ldarg.0
			IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'::'CS$<>8__locals2'
			IL_0007: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
			IL_000c: ldstr "Collapsed"
			IL_0011: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_0016: nop
			IL_0017: ldarg.0
			IL_0018: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_3'::msg
			IL_001d: ldc.i4.3
			IL_001e: ldc.i4.0
			IL_001f: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0024: pop
			IL_0025: ret
		} // end of method '<>c__DisplayClass135_3'::'<DelVarGroup>b__10'

	} // end of class <>c__DisplayClass135_3

	.class nested private auto ansi sealed beforefieldinit '<DelVarGroup>d__135'
		extends [mscorlib]System.Object
		implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 '<>1__state'
		.field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
		.field public class VMControls.WPF.GlobalUI.VarGroupItem varGroup
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field private class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' '<>8__1'
		.field private class [mscorlib]System.Threading.Tasks.Task '<delTask>5__2'
		.field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5db8c
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<DelVarGroup>d__135'::.ctor

		.method private final hidebysig newslot virtual 
			instance void MoveNext () cil managed 
		{
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
			// Method begins at RVA 0x5db98
			// Header size: 12
			// Code size: 392 (0x188)
			.maxstack 4
			.locals init (
				[0] int32,
				[1] valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,
				[2] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135',
				[3] bool,
				[4] bool,
				[5] class [mscorlib]System.Exception
			)

			IL_0000: ldarg.0
			IL_0001: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>1__state'
			IL_0006: stloc.0
			.try
			{
				IL_0007: ldloc.0
				IL_0008: brfalse.s IL_000c

				IL_000a: br.s IL_0011

				IL_000c: br IL_009c

				IL_0011: ldarg.0
				IL_0012: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::.ctor()
				IL_0017: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>8__1'
				IL_001c: ldarg.0
				IL_001d: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>8__1'
				IL_0022: ldarg.0
				IL_0023: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_0028: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<>4__this'
				IL_002d: ldarg.0
				IL_002e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>8__1'
				IL_0033: ldarg.0
				IL_0034: ldfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::varGroup
				IL_0039: stfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::varGroup
				IL_003e: nop
				IL_003f: ldarg.0
				IL_0040: call class [mscorlib]System.Threading.Tasks.TaskFactory [mscorlib]System.Threading.Tasks.Task::get_Factory()
				IL_0045: ldarg.0
				IL_0046: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>8__1'
				IL_004b: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::'<DelVarGroup>b__0'()
				IL_0051: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
				IL_0056: callvirt instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.TaskFactory::StartNew(class [mscorlib]System.Action)
				IL_005b: stfld class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<delTask>5__2'
				IL_0060: ldarg.0
				IL_0061: ldfld class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<delTask>5__2'
				IL_0066: callvirt instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
				IL_006b: stloc.1
				IL_006c: ldloca.s 1
				IL_006e: call instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
				IL_0073: brtrue.s IL_00b8

				IL_0075: ldarg.0
				IL_0076: ldc.i4.0
				IL_0077: dup
				IL_0078: stloc.0
				IL_0079: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>1__state'
				IL_007e: ldarg.0
				IL_007f: ldloc.1
				IL_0080: stfld valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>u__1'
				IL_0085: ldarg.0
				IL_0086: stloc.2
				IL_0087: ldarg.0
				IL_0088: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>t__builder'
				IL_008d: ldloca.s 1
				IL_008f: ldloca.s 2
				IL_0091: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter, class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'>(!!0&, !!1&)
				IL_0096: nop
				IL_0097: leave IL_0187

				IL_009c: ldarg.0
				IL_009d: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>u__1'
				IL_00a2: stloc.1
				IL_00a3: ldarg.0
				IL_00a4: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>u__1'
				IL_00a9: initobj [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
				IL_00af: ldarg.0
				IL_00b0: ldc.i4.m1
				IL_00b1: dup
				IL_00b2: stloc.0
				IL_00b3: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>1__state'

				IL_00b8: ldloca.s 1
				IL_00ba: call instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
				IL_00bf: nop
				IL_00c0: ldarg.0
				IL_00c1: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_00c6: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
				IL_00cb: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Count()
				IL_00d0: ldc.i4.1
				IL_00d1: cgt
				IL_00d3: stloc.3
				IL_00d4: ldloc.3
				IL_00d5: brfalse.s IL_0109

				IL_00d7: nop
				IL_00d8: ldarg.0
				IL_00d9: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_00de: ldarg.0
				IL_00df: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_00e4: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
				IL_00e9: ldarg.0
				IL_00ea: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_00ef: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
				IL_00f4: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Count()
				IL_00f9: ldc.i4.2
				IL_00fa: sub
				IL_00fb: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Item(int32)
				IL_0100: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectGroupItem(class VMControls.WPF.GlobalUI.VarGroupItem)
				IL_0105: nop
				IL_0106: nop
				IL_0107: br.s IL_0118

				IL_0109: nop
				IL_010a: ldarg.0
				IL_010b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_0110: ldnull
				IL_0111: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectGroupItem(class VMControls.WPF.GlobalUI.VarGroupItem)
				IL_0116: nop
				IL_0117: nop

				IL_0118: ldarg.0
				IL_0119: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_011e: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
				IL_0123: ldarg.0
				IL_0124: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>8__1'
				IL_0129: ldfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::varGroup
				IL_012e: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::Contains(!0)
				IL_0133: stloc.s 4
				IL_0135: ldloc.s 4
				IL_0137: brfalse.s IL_0157

				IL_0139: nop
				IL_013a: ldarg.0
				IL_013b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
				IL_0140: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
				IL_0145: ldarg.0
				IL_0146: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>8__1'
				IL_014b: ldfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass135_0'::varGroup
				IL_0150: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::Remove(!0)
				IL_0155: pop
				IL_0156: nop

				IL_0157: leave.s IL_0173
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_0159: stloc.s 5
				IL_015b: ldarg.0
				IL_015c: ldc.i4.s -2
				IL_015e: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>1__state'
				IL_0163: ldarg.0
				IL_0164: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>t__builder'
				IL_0169: ldloc.s 5
				IL_016b: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
				IL_0170: nop
				IL_0171: leave.s IL_0187
			} // end handler

			IL_0173: ldarg.0
			IL_0174: ldc.i4.s -2
			IL_0176: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>1__state'
			IL_017b: ldarg.0
			IL_017c: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>t__builder'
			IL_0181: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
			IL_0186: nop

			IL_0187: ret
		} // end of method '<DelVarGroup>d__135'::MoveNext

		.method private final hidebysig newslot virtual 
			instance void SetStateMachine (
				class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine
			) cil managed 
		{
			.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
			// Method begins at RVA 0x5dd48
			// Header size: 1
			// Code size: 1 (0x1)
			.maxstack 8

			IL_0000: ret
		} // end of method '<DelVarGroup>d__135'::SetStateMachine

	} // end of class <DelVarGroup>d__135

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass138_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string newName
		.field public string oldName

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5dd4a
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<>c__DisplayClass138_0'::.ctor

		.method assembly hidebysig 
			instance bool '<ReName>b__1' (
				class VMControls.WPF.GlobalUI.VarGroupItem x
			) cil managed 
		{
			// Method begins at RVA 0x5dd53
			// Header size: 1
			// Code size: 18 (0x12)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0006: ldarg.0
			IL_0007: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::newName
			IL_000c: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0011: ret
		} // end of method '<>c__DisplayClass138_0'::'<ReName>b__1'

		.method assembly hidebysig 
			instance bool '<ReName>b__0' (
				class VMControls.WPF.GlobalUI.GlobalVariableItem x
			) cil managed 
		{
			// Method begins at RVA 0x5dd66
			// Header size: 1
			// Code size: 18 (0x12)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
			IL_0006: ldarg.0
			IL_0007: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::oldName
			IL_000c: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0011: ret
		} // end of method '<>c__DisplayClass138_0'::'<ReName>b__0'

	} // end of class <>c__DisplayClass138_0

	.class nested private auto ansi sealed beforefieldinit '<<GetDataAsync>b__103_0>d'
		extends [mscorlib]System.Object
		implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public int32 '<>1__state'
		.field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
		.field public class VMControls.WPF.GlobalUI.GlobalVariableDialog '<>4__this'
		.field private class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' '<>8__1'
		.field private class [mscorlib]System.Exception '<ex>5__2'
		.field private string '<errorMsg>5__3'
		.field private class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException '<vmex>5__4'

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x5dd79
			// Header size: 1
			// Code size: 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: nop
			IL_0007: ret
		} // end of method '<<GetDataAsync>b__103_0>d'::.ctor

		.method private final hidebysig newslot virtual 
			instance void MoveNext () cil managed 
		{
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext()
			// Method begins at RVA 0x5dd84
			// Header size: 12
			// Code size: 650 (0x28a)
			.maxstack 4
			.locals init (
				[0] int32,
				[1] bool,
				[2] bool,
				[3] bool,
				[4] bool,
				[5] class [mscorlib]System.Exception,
				[6] bool
			)

			IL_0000: ldarg.0
			IL_0001: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>1__state'
			IL_0006: stloc.0
			.try
			{
				IL_0007: nop
				.try
				{
					IL_0008: ldarg.0
					IL_0009: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::.ctor()
					IL_000e: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_0013: ldarg.0
					IL_0014: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_0019: ldarg.0
					IL_001a: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_001f: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::'<>4__this'
					IL_0024: nop
					IL_0025: ldstr "GetDataAsync start"
					IL_002a: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
					IL_002f: nop
					IL_0030: ldarg.0
					IL_0031: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0036: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsLoading()
					IL_003b: stloc.1
					IL_003c: ldloc.1
					IL_003d: brfalse.s IL_0050

					IL_003f: nop
					IL_0040: ldstr "solution is loading"
					IL_0045: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
					IL_004a: nop
					IL_004b: leave IL_0275

					IL_0050: ldarg.0
					IL_0051: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0056: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
					IL_005b: ldc.i4.1
					IL_005c: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetReportState(bool)
					IL_0061: nop
					IL_0062: ldarg.0
					IL_0063: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_0068: ldc.i4.0
					IL_0069: newarr [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE
					IL_006e: stfld valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[] VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::vARIABLEs
					IL_0073: ldarg.0
					IL_0074: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0079: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
					IL_007e: ldarg.0
					IL_007f: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_0084: ldflda valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[] VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::vARIABLEs
					IL_0089: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::GetAllVar(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[]&)
					IL_008e: nop
					IL_008f: ldarg.0
					IL_0090: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0095: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsLoading()
					IL_009a: stloc.2
					IL_009b: ldloc.2
					IL_009c: brfalse.s IL_00af

					IL_009e: nop
					IL_009f: ldstr "solution is loading"
					IL_00a4: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
					IL_00a9: nop
					IL_00aa: leave IL_0275

					IL_00af: ldarg.0
					IL_00b0: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_00b5: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
					IL_00ba: ldarg.0
					IL_00bb: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_00c0: ldfld valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[] VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::vARIABLEs
					IL_00c5: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::GetGlobalBindingRelation(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[])
					IL_00ca: nop
					IL_00cb: ldarg.0
					IL_00cc: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_00d1: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsLoading()
					IL_00d6: stloc.3
					IL_00d7: ldloc.3
					IL_00d8: brfalse.s IL_00eb

					IL_00da: nop
					IL_00db: ldstr "solution is loading"
					IL_00e0: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
					IL_00e5: nop
					IL_00e6: leave IL_0275

					IL_00eb: ldarg.0
					IL_00ec: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_00f1: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
					IL_00f6: ldarg.0
					IL_00f7: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_00fc: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::GetVarGroupMsg()
					IL_0102: newobj instance void class [mscorlib]System.Func`1<bool>::.ctor(object, native int)
					IL_0107: callvirt instance !!0 [WindowsBase]System.Windows.Threading.Dispatcher::Invoke<bool>(class [mscorlib]System.Func`1<!!0>)
					IL_010c: pop
					IL_010d: ldarg.0
					IL_010e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0113: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsLoading()
					IL_0118: stloc.s 4
					IL_011a: ldloc.s 4
					IL_011c: brfalse.s IL_012f

					IL_011e: nop
					IL_011f: ldstr "solution is loading"
					IL_0124: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
					IL_0129: nop
					IL_012a: leave IL_0275

					IL_012f: ldarg.0
					IL_0130: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0135: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
					IL_013a: ldarg.0
					IL_013b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_0140: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0'::'<GetDataAsync>b__1'()
					IL_0146: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
					IL_014b: callvirt instance void [WindowsBase]System.Windows.Threading.Dispatcher::Invoke(class [mscorlib]System.Action)
					IL_0150: nop
					IL_0151: ldarg.0
					IL_0152: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0157: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
					IL_015c: isinst [VM.PlatformSDKCS]VM.PlatformSDKCS.IVarModule
					IL_0161: ldarg.0
					IL_0162: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_0167: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::VarValueReportEvent(object, class [mscorlib]System.EventArgs)
					IL_016d: newobj instance void [mscorlib]System.EventHandler::.ctor(object, native int)
					IL_0172: callvirt instance void [VM.PlatformSDKCS]VM.PlatformSDKCS.IVarModule::add_VarValueReportEvent(class [mscorlib]System.EventHandler)
					IL_0177: nop
					IL_0178: ldstr "GetDataAsync start end"
					IL_017d: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
					IL_0182: nop
					IL_0183: nop
					IL_0184: ldarg.0
					IL_0185: ldnull
					IL_0186: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass103_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>8__1'
					IL_018b: leave IL_0259
				} // end .try
				catch [mscorlib]System.Exception
				{
					IL_0190: stloc.s 5
					IL_0192: ldarg.0
					IL_0193: ldloc.s 5
					IL_0195: stfld class [mscorlib]System.Exception VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<ex>5__2'
					IL_019a: nop
					IL_019b: ldarg.0
					IL_019c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
					IL_01a1: ldstr "Collapsed"
					IL_01a6: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
					IL_01ab: nop
					IL_01ac: ldarg.0
					IL_01ad: ldstr "Lang_VarLoadFailed"
					IL_01b2: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
					IL_01b7: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_01bc: ldarg.0
					IL_01bd: ldarg.0
					IL_01be: ldfld class [mscorlib]System.Exception VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<ex>5__2'
					IL_01c3: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
					IL_01c8: stfld class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<vmex>5__4'
					IL_01cd: ldarg.0
					IL_01ce: ldfld class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<vmex>5__4'
					IL_01d3: ldnull
					IL_01d4: cgt.un
					IL_01d6: stloc.s 6
					IL_01d8: ldloc.s 6
					IL_01da: brfalse.s IL_0201

					IL_01dc: nop
					IL_01dd: ldarg.0
					IL_01de: ldarg.0
					IL_01df: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_01e4: ldstr "\r\n"
					IL_01e9: ldarg.0
					IL_01ea: ldfld class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<vmex>5__4'
					IL_01ef: callvirt instance string [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException::get_errorMessage()
					IL_01f4: call string [mscorlib]System.String::Concat(string, string, string)
					IL_01f9: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_01fe: nop
					IL_01ff: br.s IL_0224

					IL_0201: nop
					IL_0202: ldarg.0
					IL_0203: ldarg.0
					IL_0204: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_0209: ldstr "\r\n"
					IL_020e: ldarg.0
					IL_020f: ldfld class [mscorlib]System.Exception VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<ex>5__2'
					IL_0214: callvirt instance string [mscorlib]System.Object::ToString()
					IL_0219: call string [mscorlib]System.String::Concat(string, string, string)
					IL_021e: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_0223: nop

					IL_0224: ldstr "GetDataAsync is error:"
					IL_0229: ldarg.0
					IL_022a: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_022f: call string [mscorlib]System.String::Concat(string, string)
					IL_0234: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
					IL_0239: nop
					IL_023a: ldarg.0
					IL_023b: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_0240: ldc.i4.3
					IL_0241: ldc.i4.0
					IL_0242: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
					IL_0247: pop
					IL_0248: nop
					IL_0249: ldarg.0
					IL_024a: ldnull
					IL_024b: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<errorMsg>5__3'
					IL_0250: ldarg.0
					IL_0251: ldnull
					IL_0252: stfld class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<vmex>5__4'
					IL_0257: leave.s IL_0259
				} // end handler

				IL_0259: leave.s IL_0275
			} // end .try
			catch [mscorlib]System.Exception
			{
				IL_025b: stloc.s 5
				IL_025d: ldarg.0
				IL_025e: ldc.i4.s -2
				IL_0260: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>1__state'
				IL_0265: ldarg.0
				IL_0266: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>t__builder'
				IL_026b: ldloc.s 5
				IL_026d: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
				IL_0272: nop
				IL_0273: leave.s IL_0289
			} // end handler

			IL_0275: ldarg.0
			IL_0276: ldc.i4.s -2
			IL_0278: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>1__state'
			IL_027d: ldarg.0
			IL_027e: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>t__builder'
			IL_0283: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
			IL_0288: nop

			IL_0289: ret
		} // end of method '<<GetDataAsync>b__103_0>d'::MoveNext

		.method private final hidebysig newslot virtual 
			instance void SetStateMachine (
				class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine
			) cil managed 
		{
			.custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = (
				01 00 00 00
			)
			.override method instance void [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
			// Method begins at RVA 0x5e050
			// Header size: 1
			// Code size: 1 (0x1)
			.maxstack 8

			IL_0000: ret
		} // end of method '<<GetDataAsync>b__103_0>d'::SetStateMachine

	} // end of class <<GetDataAsync>b__103_0>d


	// Fields
	.field private static literal string REMOTE_TEMP_FILE_DIRECTORY = "/root/vmtempfiles/ftp/ModelFiles/"
	.field private static literal string REMOTE_TEMP_FILE_NAME = "temp.gvar"
	.field private static literal float64 CONFIG_WIDTH = float64(1090)
	.field public static literal float64 COMINIT_WIDTH = float64(93)
	.field private class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager varManager
	.field public static literal int32 MAX_GROUP_COUNT = int32(10)
	.field private static literal int32 MAX_VAR_COUNT = int32(2048)
	.field private static literal string COLLAPSED = "Collapsed"
	.field private static literal string VISIBLE = "Visible"
	.field private object _lock
	.field private static literal string _preGroupFix = "Lang_VarGroup"
	.field private static literal string _prefix = "var"
	.field private int32 _variableIndex
	.field public int32 NodeId
	.field private int32 totalPageNum
	.field private class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> _varItemGroups
	.field private class VMControls.WPF.GlobalUI.GlobalVariableItem oldSelectedItem
	.field private bool IsMoveItem
	.field private bool IsChangeVarValue
	.field private class [mscorlib]System.Collections.Generic.List`1<string> ExcudePropertyNameList
	.field private class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> sortItemsList
	.field private class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot _paramConfig
	.field private object decorateSource
	.field private class VMControls.WPF.GlobalUI.GlobalVariableItem _selectedVarItem
	.field private class VMControls.WPF.GlobalUI.VarGroupItem _selectGroupItem
	.field public static initonly class [WindowsBase]System.Windows.DependencyProperty ItemsTotalCountProperty
	.field private int32 _CurrentPageNum
	.field private string _EmptyStatus
	.field private string _PageNumShowStatus
	.field private string _IsShowProcessbar
	.field private string _ProcessbarMessage
	.field private bool bFirstGetData
	.field private bool IsSearchState
	.field private class [mscorlib]System.Collections.Generic.List`1<int32> iSearchCount
	.field private int32 _pageCount
	.field public static initonly class [WindowsBase]System.Windows.DependencyProperty ItemsProperty
	.field private bool _IsAllowOperat
	.field private object moveLock
	.field private bool CanScroll
	.field assembly class VMControls.WPF.GlobalUI.GlobalVariableDialog source
	.field assembly class [PresentationFramework]System.Windows.Controls.Image img
	.field assembly class [PresentationFramework]System.Windows.Controls.TextBox txtFocus
	.field assembly class [PresentationFramework]System.Windows.Controls.Button btnImport
	.field assembly class [PresentationFramework]System.Windows.Controls.Primitives.Popup PopImport
	.field assembly class [PresentationFramework]System.Windows.Controls.Button btnExport
	.field assembly class [PresentationFramework]System.Windows.Controls.Primitives.Popup PopExport
	.field assembly class [PresentationFramework]System.Windows.Controls.TextBox SearchText
	.field assembly class [PresentationFramework]System.Windows.Controls.Button btnTop
	.field assembly class [PresentationFramework]System.Windows.Controls.Button btnUp
	.field assembly class [PresentationFramework]System.Windows.Controls.Button btnDown
	.field assembly class [PresentationFramework]System.Windows.Controls.Button LightSave
	.field assembly class [PresentationFramework]System.Windows.Controls.TabControl tabControl
	.field assembly class [Apps.PcUI]Apps.PcUI.Controls.ProgressMask windowShadow
	.field private bool _contentLoaded

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x26a40
		// Header size: 12
		// Code size: 382 (0x17e)
		.maxstack 4
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'
		)

		IL_0000: ldarg.0
		IL_0001: newobj instance void [mscorlib]System.Object::.ctor()
		IL_0006: stfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::_lock
		IL_000b: ldarg.0
		IL_000c: ldc.i4.m1
		IL_000d: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::NodeId
		IL_0012: ldarg.0
		IL_0013: ldc.i4.0
		IL_0014: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::totalPageNum
		IL_0019: ldarg.0
		IL_001a: newobj instance void class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::.ctor()
		IL_001f: stfld class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::_varItemGroups
		IL_0024: ldarg.0
		IL_0025: ldc.i4.0
		IL_0026: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsMoveItem
		IL_002b: ldarg.0
		IL_002c: ldc.i4.0
		IL_002d: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsChangeVarValue
		IL_0032: ldarg.0
		IL_0033: newobj instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
		IL_0038: dup
		IL_0039: ldstr "Index"
		IL_003e: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_0043: nop
		IL_0044: dup
		IL_0045: ldstr "IsOdd"
		IL_004a: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_004f: nop
		IL_0050: dup
		IL_0051: ldstr "DisplayTooltipName"
		IL_0056: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_005b: nop
		IL_005c: dup
		IL_005d: ldstr "IsBindTarget"
		IL_0062: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_0067: nop
		IL_0068: dup
		IL_0069: ldstr "IsBindMoudleResult"
		IL_006e: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_0073: nop
		IL_0074: dup
		IL_0075: ldstr "Capture"
		IL_007a: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_007f: nop
		IL_0080: dup
		IL_0081: ldstr "DisplayParamsName"
		IL_0086: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_008b: nop
		IL_008c: dup
		IL_008d: ldstr "IsSelected"
		IL_0092: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_0097: nop
		IL_0098: dup
		IL_0099: ldstr ""
		IL_009e: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
		IL_00a3: nop
		IL_00a4: stfld class [mscorlib]System.Collections.Generic.List`1<string> VMControls.WPF.GlobalUI.GlobalVariableDialog::ExcudePropertyNameList
		IL_00a9: ldarg.0
		IL_00aa: newobj instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor()
		IL_00af: stfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::sortItemsList
		IL_00b4: ldarg.0
		IL_00b5: ldnull
		IL_00b6: stfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_00bb: ldarg.0
		IL_00bc: ldnull
		IL_00bd: stfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::decorateSource
		IL_00c2: ldarg.0
		IL_00c3: ldc.i4.0
		IL_00c4: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_CurrentPageNum
		IL_00c9: ldarg.0
		IL_00ca: ldstr "Visible"
		IL_00cf: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_EmptyStatus
		IL_00d4: ldarg.0
		IL_00d5: ldstr "Visible"
		IL_00da: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_PageNumShowStatus
		IL_00df: ldarg.0
		IL_00e0: ldstr "Collapsed"
		IL_00e5: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsShowProcessbar
		IL_00ea: ldarg.0
		IL_00eb: ldstr "Lang_Import"
		IL_00f0: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_00f5: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_ProcessbarMessage
		IL_00fa: ldarg.0
		IL_00fb: ldc.i4.1
		IL_00fc: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::bFirstGetData
		IL_0101: ldarg.0
		IL_0102: ldc.i4.0
		IL_0103: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_0108: ldarg.0
		IL_0109: newobj instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
		IL_010e: stfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_0113: ldarg.0
		IL_0114: ldc.i4.s 12
		IL_0116: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_pageCount
		IL_011b: ldarg.0
		IL_011c: ldc.i4.1
		IL_011d: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsAllowOperat
		IL_0122: ldarg.0
		IL_0123: newobj instance void [mscorlib]System.Object::.ctor()
		IL_0128: stfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::moveLock
		IL_012d: ldarg.0
		IL_012e: ldc.i4.1
		IL_012f: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::CanScroll
		IL_0134: ldarg.0
		IL_0135: call instance void [VM.Controls]VM.Controls.BaseWindow::.ctor()
		IL_013a: nop
		IL_013b: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::.ctor()
		IL_0140: stloc.0
		IL_0141: ldloc.0
		IL_0142: ldarg.0
		IL_0143: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<>4__this'
		IL_0148: nop
		IL_0149: ldarg.0
		IL_014a: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::InitializeComponent()
		IL_014f: nop
		IL_0150: ldarg.0
		IL_0151: newobj instance void class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor()
		IL_0156: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_Items(class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>)
		IL_015b: nop
		IL_015c: ldloc.0
		IL_015d: ldc.i4.0
		IL_015e: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'init'
		IL_0163: ldarg.0
		IL_0164: ldloc.0
		IL_0165: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass4_0'::'<.ctor>b__0'(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_016b: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_0170: call instance void [PresentationFramework]System.Windows.FrameworkElement::add_Loaded(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_0175: nop
		IL_0176: ldarg.0
		IL_0177: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::GetAuthority()
		IL_017c: nop
		IL_017d: ret
	} // end of method GlobalVariableDialog::.ctor

	.method private hidebysig 
		instance void VarValueReportEvent (
			object sender,
			class [mscorlib]System.EventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x26bcc
		// Header size: 12
		// Code size: 109 (0x6d)
		.maxstack 3
		.locals init (
			[0] class [VMControls.Interface]VMControls.Interface.ValueEventArgs,
			[1] bool,
			[2] bool,
			[3] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0',
			[4] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsChangeVarValue
		IL_0007: stloc.1
		IL_0008: ldloc.1
		IL_0009: brfalse.s IL_000e

		IL_000b: nop
		IL_000c: br.s IL_006c

		IL_000e: ldarg.2
		IL_000f: isinst [VMControls.Interface]VMControls.Interface.ValueEventArgs
		IL_0014: dup
		IL_0015: stloc.0
		IL_0016: ldnull
		IL_0017: cgt.un
		IL_0019: stloc.2
		IL_001a: ldloc.2
		IL_001b: brfalse.s IL_006c

		IL_001d: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::.ctor()
		IL_0022: stloc.3
		IL_0023: ldloc.3
		IL_0024: ldarg.0
		IL_0025: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<>4__this'
		IL_002a: nop
		IL_002b: ldloc.3
		IL_002c: ldloc.0
		IL_002d: ldfld object [VMControls.Interface]VMControls.Interface.ValueEventArgs::Value
		IL_0032: castclass [Apps.XmlParser]Apps.XmlParser.Variable.VarItem
		IL_0037: stfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::varItem
		IL_003c: ldloc.3
		IL_003d: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VarItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::varItem
		IL_0042: ldnull
		IL_0043: ceq
		IL_0045: stloc.s 4
		IL_0047: ldloc.s 4
		IL_0049: brfalse.s IL_004e

		IL_004b: nop
		IL_004c: br.s IL_006c

		IL_004e: ldarg.0
		IL_004f: call instance class [WindowsBase]System.Windows.Threading.Dispatcher [WindowsBase]System.Windows.Threading.DispatcherObject::get_Dispatcher()
		IL_0054: ldloc.3
		IL_0055: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass5_0'::'<VarValueReportEvent>b__0'()
		IL_005b: newobj instance void [mscorlib]System.Action::.ctor(object, native int)
		IL_0060: call !!0[] [mscorlib]System.Array::Empty<object>()
		IL_0065: callvirt instance class [WindowsBase]System.Windows.Threading.DispatcherOperation [WindowsBase]System.Windows.Threading.Dispatcher::BeginInvoke(class [mscorlib]System.Delegate, object[])
		IL_006a: pop
		IL_006b: nop

		IL_006c: ret
	} // end of method GlobalVariableDialog::VarValueReportEvent

	.method public hidebysig specialname 
		instance class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot get_ParamsConfig () cil managed 
	{
		// Method begins at RVA 0x26c48
		// Header size: 12
		// Code size: 77 (0x4d)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_0007: brtrue.s IL_0014

		IL_0009: ldarg.0
		IL_000a: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::decorateSource
		IL_000f: ldnull
		IL_0010: cgt.un
		IL_0012: br.s IL_0015

		IL_0014: ldc.i4.0

		IL_0015: stloc.0
		IL_0016: ldloc.0
		IL_0017: brfalse.s IL_0042

		IL_0019: nop
		IL_001a: ldarg.0
		IL_001b: ldarg.0
		IL_001c: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::decorateSource
		IL_0021: isinst [VMControls.Interface]VMControls.Interface.IVmModule
		IL_0026: callvirt instance class [VMControls.BaseInterface]VMControls.BaseInterface.IParamsConfig [VMControls.Interface]VMControls.Interface.IVmModule::get_Params()
		IL_002b: newobj instance void [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot::.ctor(class [VMControls.BaseInterface]VMControls.BaseInterface.IParamsConfig)
		IL_0030: stfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_0035: ldarg.0
		IL_0036: ldfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_003b: callvirt instance bool [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot::GetItemsValueUnit()
		IL_0040: pop
		IL_0041: nop

		IL_0042: ldarg.0
		IL_0043: ldfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_0048: stloc.1
		IL_0049: br.s IL_004b

		IL_004b: ldloc.1
		IL_004c: ret
	} // end of method GlobalVariableDialog::get_ParamsConfig

	.method public hidebysig specialname 
		instance void set_ParamsConfig (
			class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26ca4
		// Header size: 12
		// Code size: 39 (0x27)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_0007: ldarg.1
		IL_0008: ceq
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0026

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::_paramConfig
		IL_0019: ldarg.0
		IL_001a: ldstr "ParamsConfig"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: nop

		IL_0026: ret
	} // end of method GlobalVariableDialog::set_ParamsConfig

	.method public final hidebysig specialname newslot virtual 
		instance object get_DecorateSource () cil managed 
	{
		// Method begins at RVA 0x26cd8
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] object
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::decorateSource
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_DecorateSource

	.method public final hidebysig specialname newslot virtual 
		instance void set_DecorateSource (
			object 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26cf0
		// Header size: 1
		// Code size: 45 (0x2d)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: stfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::decorateSource
		IL_0008: ldarg.0
		IL_0009: ldstr "DisplayName"
		IL_000e: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0013: nop
		IL_0014: ldarg.0
		IL_0015: ldstr "IsVisibleComInit"
		IL_001a: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_001f: nop
		IL_0020: ldarg.0
		IL_0021: ldstr "ConfigWidth"
		IL_0026: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_002b: nop
		IL_002c: ret
	} // end of method GlobalVariableDialog::set_DecorateSource

	.method public hidebysig specialname 
		instance string get_DisplayName () cil managed 
	{
		// Method begins at RVA 0x26d20
		// Header size: 12
		// Code size: 55 (0x37)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
		IL_0007: ldnull
		IL_0008: ceq
		IL_000a: stloc.0
		IL_000b: ldloc.0
		IL_000c: brfalse.s IL_001c

		IL_000e: nop
		IL_000f: ldstr "Lang_GlobalVariable"
		IL_0014: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_0019: stloc.1
		IL_001a: br.s IL_0035

		IL_001c: nop
		IL_001d: ldarg.0
		IL_001e: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
		IL_0023: isinst [VM.Core]VM.Core.VmModule
		IL_0028: callvirt instance string [VM.Core]VM.Core.VmModule::get_StrModuleName()
		IL_002d: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_0032: stloc.1
		IL_0033: br.s IL_0035

		IL_0035: ldloc.1
		IL_0036: ret
	} // end of method GlobalVariableDialog::get_DisplayName

	.method public hidebysig specialname 
		instance float64 get_ConfigWidth () cil managed 
	{
		// Method begins at RVA 0x26d64
		// Header size: 12
		// Code size: 34 (0x22)
		.maxstack 1
		.locals init (
			[0] float64
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsVisibleComInit()
		IL_0007: brtrue.s IL_0014

		IL_0009: ldc.r8 997
		IL_0012: br.s IL_001d

		IL_0014: ldc.r8 1090

		IL_001d: stloc.0
		IL_001e: br.s IL_0020

		IL_0020: ldloc.0
		IL_0021: ret
	} // end of method GlobalVariableDialog::get_ConfigWidth

	.method public hidebysig specialname 
		instance float64 get_ComInitWidth () cil managed 
	{
		// Method begins at RVA 0x26d94
		// Header size: 12
		// Code size: 34 (0x22)
		.maxstack 1
		.locals init (
			[0] float64
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsVisibleComInit()
		IL_0007: brtrue.s IL_0014

		IL_0009: ldc.r8 0.0
		IL_0012: br.s IL_001d

		IL_0014: ldc.r8 93

		IL_001d: stloc.0
		IL_001e: br.s IL_0020

		IL_0020: ldloc.0
		IL_0021: ret
	} // end of method GlobalVariableDialog::get_ComInitWidth

	.method public hidebysig specialname 
		instance bool get_IsVisibleComInit () cil managed 
	{
		// Method begins at RVA 0x26dc4
		// Header size: 12
		// Code size: 40 (0x28)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
		IL_0007: brfalse.s IL_0022

		IL_0009: ldarg.0
		IL_000a: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
		IL_000f: isinst [VM.Core]VM.Core.VmModule
		IL_0014: callvirt instance uint32 [VM.Core]VM.Core.VmModule::get_ID()
		IL_0019: ldc.i4 13000
		IL_001e: ceq
		IL_0020: br.s IL_0023

		IL_0022: ldc.i4.1

		IL_0023: stloc.0
		IL_0024: br.s IL_0026

		IL_0026: ldloc.0
		IL_0027: ret
	} // end of method GlobalVariableDialog::get_IsVisibleComInit

	.method public hidebysig specialname 
		instance class VMControls.WPF.GlobalUI.GlobalVariableItem get_SelectedVarItem () cil managed 
	{
		// Method begins at RVA 0x26df8
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableItem
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_SelectedVarItem

	.method public hidebysig specialname 
		instance void set_SelectedVarItem (
			class VMControls.WPF.GlobalUI.GlobalVariableItem 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26e10
		// Header size: 12
		// Code size: 138 (0x8a)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_0008: ldarg.0
		IL_0009: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_000e: ldnull
		IL_000f: cgt.un
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: brfalse.s IL_007d

		IL_0015: nop
		IL_0016: ldarg.0
		IL_0017: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_001c: ldc.i4.1
		IL_001d: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Capture(bool)
		IL_0022: nop
		IL_0023: ldarg.0
		IL_0024: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_0029: ldc.i4.1
		IL_002a: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_002f: nop
		IL_0030: ldarg.0
		IL_0031: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0036: brfalse.s IL_0055

		IL_0038: ldarg.0
		IL_0039: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_003e: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0043: ldarg.0
		IL_0044: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_0049: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_004e: call bool [mscorlib]System.String::op_Inequality(string, string)
		IL_0053: br.s IL_0056

		IL_0055: ldc.i4.0

		IL_0056: stloc.1
		IL_0057: ldloc.1
		IL_0058: brfalse.s IL_0069

		IL_005a: nop
		IL_005b: ldarg.0
		IL_005c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0061: ldc.i4.0
		IL_0062: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_0067: nop
		IL_0068: nop

		IL_0069: ldarg.0
		IL_006a: ldarg.0
		IL_006b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_0070: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0075: ldarg.0
		IL_0076: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_007b: nop
		IL_007c: nop

		IL_007d: ldarg.0
		IL_007e: ldstr "SelectedVarItem"
		IL_0083: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0088: nop
		IL_0089: ret
	} // end of method GlobalVariableDialog::set_SelectedVarItem

	.method public hidebysig specialname 
		instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> get_VarItemGroups () cil managed 
	{
		// Method begins at RVA 0x26ea8
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem>
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::_varItemGroups
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_VarItemGroups

	.method public hidebysig specialname 
		instance void set_VarItemGroups (
			class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26ec0
		// Header size: 12
		// Code size: 39 (0x27)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::_varItemGroups
		IL_0007: ldarg.1
		IL_0008: ceq
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0026

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::_varItemGroups
		IL_0019: ldarg.0
		IL_001a: ldstr "VarItemGroups"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: nop

		IL_0026: ret
	} // end of method GlobalVariableDialog::set_VarItemGroups

	.method public hidebysig specialname 
		instance class VMControls.WPF.GlobalUI.VarGroupItem get_SelectGroupItem () cil managed 
	{
		// Method begins at RVA 0x26ef4
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] class VMControls.WPF.GlobalUI.VarGroupItem
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectGroupItem
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_SelectGroupItem

	.method public hidebysig specialname 
		instance void set_SelectGroupItem (
			class VMControls.WPF.GlobalUI.VarGroupItem 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26f0c
		// Header size: 12
		// Code size: 109 (0x6d)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool,
			[2] bool,
			[3] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectGroupItem
		IL_0007: ldarg.1
		IL_0008: ceq
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0060

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectGroupItem
		IL_0019: ldarg.1
		IL_001a: ldnull
		IL_001b: cgt.un
		IL_001d: stloc.1
		IL_001e: ldloc.1
		IL_001f: brfalse.s IL_005f

		IL_0021: nop
		IL_0022: ldarg.0
		IL_0023: ldc.i4.0
		IL_0024: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
		IL_0029: nop
		IL_002a: ldarg.0
		IL_002b: ldc.i4.0
		IL_002c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
		IL_0031: nop
		IL_0032: ldarg.0
		IL_0033: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_0038: stloc.2
		IL_0039: ldloc.2
		IL_003a: brfalse.s IL_0054

		IL_003c: nop
		IL_003d: ldarg.0
		IL_003e: ldfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText
		IL_0043: callvirt instance string [PresentationFramework]System.Windows.Controls.TextBox::get_Text()
		IL_0048: stloc.3
		IL_0049: ldarg.0
		IL_004a: ldloc.3
		IL_004b: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SeearchItem(string)
		IL_0050: nop
		IL_0051: nop
		IL_0052: br.s IL_005e

		IL_0054: nop
		IL_0055: ldarg.0
		IL_0056: ldc.i4.0
		IL_0057: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
		IL_005c: nop
		IL_005d: nop

		IL_005e: nop

		IL_005f: nop

		IL_0060: ldarg.0
		IL_0061: ldstr "SelectGroupItem"
		IL_0066: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_006b: nop
		IL_006c: ret
	} // end of method GlobalVariableDialog::set_SelectGroupItem

	.method public hidebysig specialname 
		instance int32 get_ItemsTotalCount () cil managed 
	{
		// Method begins at RVA 0x26f88
		// Header size: 12
		// Code size: 22 (0x16)
		.maxstack 2
		.locals init (
			[0] int32
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldsfld class [WindowsBase]System.Windows.DependencyProperty VMControls.WPF.GlobalUI.GlobalVariableDialog::ItemsTotalCountProperty
		IL_0007: call instance object [WindowsBase]System.Windows.DependencyObject::GetValue(class [WindowsBase]System.Windows.DependencyProperty)
		IL_000c: unbox.any [mscorlib]System.Int32
		IL_0011: stloc.0
		IL_0012: br.s IL_0014

		IL_0014: ldloc.0
		IL_0015: ret
	} // end of method GlobalVariableDialog::get_ItemsTotalCount

	.method public hidebysig specialname 
		instance void set_ItemsTotalCount (
			int32 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26faa
		// Header size: 1
		// Code size: 20 (0x14)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldsfld class [WindowsBase]System.Windows.DependencyProperty VMControls.WPF.GlobalUI.GlobalVariableDialog::ItemsTotalCountProperty
		IL_0007: ldarg.1
		IL_0008: box [mscorlib]System.Int32
		IL_000d: call instance void [WindowsBase]System.Windows.DependencyObject::SetValue(class [WindowsBase]System.Windows.DependencyProperty, object)
		IL_0012: nop
		IL_0013: ret
	} // end of method GlobalVariableDialog::set_ItemsTotalCount

	.method public hidebysig specialname 
		instance int32 get_CurrentPageNum () cil managed 
	{
		// Method begins at RVA 0x26fc0
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] int32
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_CurrentPageNum
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_CurrentPageNum

	.method public hidebysig specialname 
		instance void set_CurrentPageNum (
			int32 'value'
		) cil managed 
	{
		// Method begins at RVA 0x26fd8
		// Header size: 12
		// Code size: 125 (0x7d)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool,
			[2] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_CurrentPageNum
		IL_0007: ldarg.1
		IL_0008: ceq
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_007c

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_CurrentPageNum
		IL_0019: ldarg.0
		IL_001a: ldstr "CurrentPageNum"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: ldarg.0
		IL_0026: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_002b: ldc.i4.0
		IL_002c: ceq
		IL_002e: stloc.1
		IL_002f: ldloc.1
		IL_0030: brfalse.s IL_003d

		IL_0032: nop
		IL_0033: ldarg.0
		IL_0034: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetBindData()
		IL_0039: nop
		IL_003a: nop
		IL_003b: br.s IL_0046

		IL_003d: nop
		IL_003e: ldarg.0
		IL_003f: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetBindSearchData()
		IL_0044: nop
		IL_0045: nop

		IL_0046: ldarg.0
		IL_0047: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_004c: brfalse.s IL_0059

		IL_004e: ldarg.0
		IL_004f: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsMoveItem
		IL_0054: ldc.i4.0
		IL_0055: ceq
		IL_0057: br.s IL_005a

		IL_0059: ldc.i4.0

		IL_005a: stloc.2
		IL_005b: ldloc.2
		IL_005c: brfalse.s IL_007b

		IL_005e: nop
		IL_005f: ldarg.0
		IL_0060: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0065: ldc.i4.0
		IL_0066: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_006b: nop
		IL_006c: ldarg.0
		IL_006d: ldnull
		IL_006e: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0073: ldarg.0
		IL_0074: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_0079: nop
		IL_007a: nop

		IL_007b: nop

		IL_007c: ret
	} // end of method GlobalVariableDialog::set_CurrentPageNum

	.method public hidebysig specialname 
		instance string get_EmptyStatus () cil managed 
	{
		// Method begins at RVA 0x27064
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_EmptyStatus
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_EmptyStatus

	.method public hidebysig specialname 
		instance void set_EmptyStatus (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2707c
		// Header size: 12
		// Code size: 39 (0x27)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_EmptyStatus
		IL_0007: ldarg.1
		IL_0008: call bool [mscorlib]System.String::op_Inequality(string, string)
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0026

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_EmptyStatus
		IL_0019: ldarg.0
		IL_001a: ldstr "EmptyStatus"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: nop

		IL_0026: ret
	} // end of method GlobalVariableDialog::set_EmptyStatus

	.method public hidebysig specialname 
		instance string get_PageNumShowStatus () cil managed 
	{
		// Method begins at RVA 0x270b0
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_PageNumShowStatus
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_PageNumShowStatus

	.method public hidebysig specialname 
		instance void set_PageNumShowStatus (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x270c8
		// Header size: 12
		// Code size: 39 (0x27)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_PageNumShowStatus
		IL_0007: ldarg.1
		IL_0008: call bool [mscorlib]System.String::op_Inequality(string, string)
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0026

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_PageNumShowStatus
		IL_0019: ldarg.0
		IL_001a: ldstr "PageNumShowStatus"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: nop

		IL_0026: ret
	} // end of method GlobalVariableDialog::set_PageNumShowStatus

	.method public hidebysig specialname 
		instance string get_IsShowProcessbar () cil managed 
	{
		// Method begins at RVA 0x270fc
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsShowProcessbar
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_IsShowProcessbar

	.method public hidebysig specialname 
		instance void set_IsShowProcessbar (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x27114
		// Header size: 1
		// Code size: 21 (0x15)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsShowProcessbar
		IL_0008: ldarg.0
		IL_0009: ldstr "IsShowProcessbar"
		IL_000e: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0013: nop
		IL_0014: ret
	} // end of method GlobalVariableDialog::set_IsShowProcessbar

	.method public hidebysig specialname 
		instance string get_ProcessbarMessage () cil managed 
	{
		// Method begins at RVA 0x2712c
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_ProcessbarMessage
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_ProcessbarMessage

	.method public hidebysig specialname 
		instance void set_ProcessbarMessage (
			string 'value'
		) cil managed 
	{
		// Method begins at RVA 0x27144
		// Header size: 1
		// Code size: 21 (0x15)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog::_ProcessbarMessage
		IL_0008: ldarg.0
		IL_0009: ldstr "ProcessbarMessage"
		IL_000e: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0013: nop
		IL_0014: ret
	} // end of method GlobalVariableDialog::set_ProcessbarMessage

	.method public hidebysig specialname 
		instance int32 get_PageCount () cil managed 
	{
		// Method begins at RVA 0x2715c
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] int32
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_pageCount
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_PageCount

	.method public hidebysig specialname 
		instance void set_PageCount (
			int32 'value'
		) cil managed 
	{
		// Method begins at RVA 0x27174
		// Header size: 12
		// Code size: 63 (0x3f)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_pageCount
		IL_0007: ldarg.1
		IL_0008: ceq
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_003e

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_pageCount
		IL_0019: ldarg.0
		IL_001a: ldstr "PageCount"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: ldarg.0
		IL_0026: ldc.i4.0
		IL_0027: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
		IL_002c: nop
		IL_002d: ldarg.0
		IL_002e: ldc.i4.0
		IL_002f: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
		IL_0034: nop
		IL_0035: ldarg.0
		IL_0036: ldc.i4.0
		IL_0037: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
		IL_003c: nop
		IL_003d: nop

		IL_003e: ret
	} // end of method GlobalVariableDialog::set_PageCount

	.method public hidebysig specialname 
		instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> get_Items () cil managed 
	{
		// Method begins at RVA 0x271c0
		// Header size: 12
		// Code size: 22 (0x16)
		.maxstack 2
		.locals init (
			[0] class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldsfld class [WindowsBase]System.Windows.DependencyProperty VMControls.WPF.GlobalUI.GlobalVariableDialog::ItemsProperty
		IL_0007: call instance object [WindowsBase]System.Windows.DependencyObject::GetValue(class [WindowsBase]System.Windows.DependencyProperty)
		IL_000c: castclass class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>
		IL_0011: stloc.0
		IL_0012: br.s IL_0014

		IL_0014: ldloc.0
		IL_0015: ret
	} // end of method GlobalVariableDialog::get_Items

	.method public hidebysig specialname 
		instance void set_Items (
			class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> 'value'
		) cil managed 
	{
		// Method begins at RVA 0x271e2
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldsfld class [WindowsBase]System.Windows.DependencyProperty VMControls.WPF.GlobalUI.GlobalVariableDialog::ItemsProperty
		IL_0007: ldarg.1
		IL_0008: call instance void [WindowsBase]System.Windows.DependencyObject::SetValue(class [WindowsBase]System.Windows.DependencyProperty, object)
		IL_000d: nop
		IL_000e: ret
	} // end of method GlobalVariableDialog::set_Items

	.method public hidebysig specialname 
		instance bool get_IsAllowOperat () cil managed 
	{
		// Method begins at RVA 0x271f4
		// Header size: 12
		// Code size: 12 (0xc)
		.maxstack 1
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsAllowOperat
		IL_0007: stloc.0
		IL_0008: br.s IL_000a

		IL_000a: ldloc.0
		IL_000b: ret
	} // end of method GlobalVariableDialog::get_IsAllowOperat

	.method public hidebysig specialname 
		instance void set_IsAllowOperat (
			bool 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2720c
		// Header size: 12
		// Code size: 39 (0x27)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsAllowOperat
		IL_0007: ldarg.1
		IL_0008: ceq
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0026

		IL_0011: nop
		IL_0012: ldarg.0
		IL_0013: ldarg.1
		IL_0014: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_IsAllowOperat
		IL_0019: ldarg.0
		IL_001a: ldstr "IsAllowOperat"
		IL_001f: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0024: nop
		IL_0025: nop

		IL_0026: ret
	} // end of method GlobalVariableDialog::set_IsAllowOperat

	.method private hidebysig 
		instance void GetAuthority () cil managed 
	{
		// Method begins at RVA 0x27240
		// Header size: 12
		// Code size: 43 (0x2b)
		.maxstack 2
		.locals init (
			[0] valuetype [VM.User]VM.User.AccessUser,
			[1] bool
		)

		IL_0000: nop
		IL_0001: call class [VM.User]VM.User.CodeManager [VM.User]VM.User.CodeManager::GetInstance()
		IL_0006: callvirt instance valuetype [VM.User]VM.User.AccessUser [VM.User]VM.User.CodeManager::GetLoginUser()
		IL_000b: stloc.0
		IL_000c: ldloc.0
		IL_000d: ldc.i4.2
		IL_000e: ceq
		IL_0010: stloc.1
		IL_0011: ldloc.1
		IL_0012: brfalse.s IL_0020

		IL_0014: nop
		IL_0015: ldarg.0
		IL_0016: ldc.i4.0
		IL_0017: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsAllowOperat(bool)
		IL_001c: nop
		IL_001d: nop
		IL_001e: br.s IL_002a

		IL_0020: nop
		IL_0021: ldarg.0
		IL_0022: ldc.i4.1
		IL_0023: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsAllowOperat(bool)
		IL_0028: nop
		IL_0029: nop

		IL_002a: ret
	} // end of method GlobalVariableDialog::GetAuthority

	.method public hidebysig specialname 
		instance object get_SolutionData () cil managed 
	{
		// Method begins at RVA 0x27277
		// Header size: 1
		// Code size: 6 (0x6)
		.maxstack 8

		IL_0000: call class [VM.Core]VM.Core.VmSolution [VM.Core]VM.Core.VmSolution::get_Instance()
		IL_0005: ret
	} // end of method GlobalVariableDialog::get_SolutionData

	.method public hidebysig specialname 
		instance bool get_IsEdit () cil managed 
	{
		// Method begins at RVA 0x27280
		// Header size: 12
		// Code size: 22 (0x16)
		.maxstack 1
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SolutionData()
		IL_0007: isinst [VMControls.Interface]VMControls.Interface.IVMRun
		IL_000c: callvirt instance bool [VMControls.Interface]VMControls.Interface.IVMRun::get_IsRunning()
		IL_0011: stloc.0
		IL_0012: br.s IL_0014

		IL_0014: ldloc.0
		IL_0015: ret
	} // end of method GlobalVariableDialog::get_IsEdit

	.method public hidebysig 
		instance string GetDefaultName (
			class [mscorlib]System.Text.StringBuilder stringBuilder
		) cil managed 
	{
		// Method begins at RVA 0x272a4
		// Header size: 12
		// Code size: 236 (0xec)
		.maxstack 3
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0',
			[1] bool,
			[2] int32,
			[3] int32,
			[4] bool,
			[5] bool,
			[6] string
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.1
		IL_0008: stfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_000d: nop
		IL_000e: ldloc.0
		IL_000f: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_0014: ldnull
		IL_0015: ceq
		IL_0017: stloc.1
		IL_0018: ldloc.1
		IL_0019: brfalse.s IL_004e

		IL_001b: nop
		IL_001c: ldloc.0
		IL_001d: ldc.i4.5
		IL_001e: newobj instance void [mscorlib]System.Text.StringBuilder::.ctor(int32)
		IL_0023: stfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_0028: ldloc.0
		IL_0029: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_002e: ldstr "var"
		IL_0033: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
		IL_0038: pop
		IL_0039: ldloc.0
		IL_003a: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_003f: ldarg.0
		IL_0040: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_variableIndex
		IL_0045: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(int32)
		IL_004a: pop
		IL_004b: nop
		IL_004c: br.s IL_00b3

		IL_004e: nop
		IL_004f: ldarg.0
		IL_0050: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_variableIndex
		IL_0055: stloc.3
		IL_0056: ldc.i4.0
		IL_0057: stloc.2
		IL_0058: br.s IL_0065
		// loop start (head: IL_0065)
			IL_005a: nop
			IL_005b: ldloc.3
			IL_005c: ldc.i4.s 10
			IL_005e: div
			IL_005f: stloc.3
			IL_0060: nop
			IL_0061: ldloc.2
			IL_0062: ldc.i4.1
			IL_0063: add
			IL_0064: stloc.2

			IL_0065: ldloc.3
			IL_0066: ldc.i4.0
			IL_0067: cgt
			IL_0069: stloc.s 4
			IL_006b: ldloc.s 4
			IL_006d: brtrue.s IL_005a
		// end loop

		IL_006f: ldloc.2
		IL_0070: ldc.i4.0
		IL_0071: ceq
		IL_0073: stloc.s 5
		IL_0075: ldloc.s 5
		IL_0077: brfalse.s IL_007b

		IL_0079: ldc.i4.1
		IL_007a: stloc.2

		IL_007b: ldloc.0
		IL_007c: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_0081: ldstr "var"
		IL_0086: call instance int32 [mscorlib]System.String::get_Length()
		IL_008b: ldloc.2
		IL_008c: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Remove(int32, int32)
		IL_0091: pop
		IL_0092: ldarg.0
		IL_0093: ldarg.0
		IL_0094: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_variableIndex
		IL_0099: ldc.i4.1
		IL_009a: add
		IL_009b: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_variableIndex
		IL_00a0: ldloc.0
		IL_00a1: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_00a6: ldarg.0
		IL_00a7: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::_variableIndex
		IL_00ac: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(int32)
		IL_00b1: pop
		IL_00b2: nop

		IL_00b3: ldarg.0
		IL_00b4: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_00b9: ldloc.0
		IL_00ba: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::'<GetDefaultName>b__0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_00c0: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_00c5: call bool [System.Core]System.Linq.Enumerable::Any<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_00ca: brtrue.s IL_00d9

		IL_00cc: ldloc.0
		IL_00cd: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_00d2: callvirt instance string [mscorlib]System.Object::ToString()
		IL_00d7: br.s IL_00e5

		IL_00d9: ldarg.0
		IL_00da: ldloc.0
		IL_00db: ldfld class [mscorlib]System.Text.StringBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass94_0'::stringBuilder
		IL_00e0: call instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::GetDefaultName(class [mscorlib]System.Text.StringBuilder)

		IL_00e5: stloc.s 6
		IL_00e7: br.s IL_00e9

		IL_00e9: ldloc.s 6
		IL_00eb: ret
	} // end of method GlobalVariableDialog::GetDefaultName

	.method private hidebysig 
		instance void AddNewItem (
			class VMControls.WPF.GlobalUI.GlobalVariableItem item,
			[opt] bool isAddBind
		) cil managed 
	{
		.param [2] = bool(false)
		// Method begins at RVA 0x2739c
		// Header size: 12
		// Code size: 299 (0x12b)
		.maxstack 5
		.locals init (
			[0] class [VM.Core]VM.Core.VmIO,
			[1] bool,
			[2] bool,
			[3] class [mscorlib]System.Exception,
			[4] bool
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: ldnull
		IL_0003: ceq
		IL_0005: stloc.1
		IL_0006: ldloc.1
		IL_0007: brfalse.s IL_000f

		IL_0009: nop
		IL_000a: br IL_012a

		IL_000f: ldarg.1
		IL_0010: ldarg.0
		IL_0011: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_0016: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Parent(object)
		IL_001b: nop
		IL_001c: ldarg.2
		IL_001d: stloc.2
		IL_001e: ldloc.2
		IL_001f: brfalse IL_00b7

		IL_0024: nop
		.try
		{
			IL_0025: nop
			IL_0026: ldarg.0
			IL_0027: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_002c: ldarg.1
			IL_002d: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0032: ldarg.1
			IL_0033: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_0038: ldstr ""
			IL_003d: ldarg.0
			IL_003e: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0043: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
			IL_0048: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::AddVar(string, string, string, string)
			IL_004d: nop
			IL_004e: nop
			IL_004f: leave.s IL_009a
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0051: stloc.3
			IL_0052: nop
			IL_0053: ldstr "添加变量异常:"
			IL_0058: ldarg.1
			IL_0059: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_005e: ldstr ",exception:"
			IL_0063: ldloc.3
			IL_0064: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
			IL_0069: callvirt instance string [mscorlib]System.Object::ToString()
			IL_006e: call string [mscorlib]System.String::Concat(string, string, string, string)
			IL_0073: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_0078: nop
			IL_0079: ldstr "Lang_GlobalVariableAdd"
			IL_007e: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0083: ldstr ":Failed"
			IL_0088: call string [mscorlib]System.String::Concat(string, string)
			IL_008d: ldc.i4.3
			IL_008e: ldc.i4.0
			IL_008f: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0094: pop
			IL_0095: leave IL_012a
		} // end handler

		IL_009a: ldarg.0
		IL_009b: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_00a0: ldarg.1
		IL_00a1: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Add(!0)
		IL_00a6: nop
		IL_00a7: ldarg.0
		IL_00a8: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_00ad: ldarg.1
		IL_00ae: callvirt instance void VMControls.WPF.GlobalUI.VarGroupItem::AddItem(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_00b3: nop
		IL_00b4: nop
		IL_00b5: br.s IL_00c6

		IL_00b7: nop
		IL_00b8: ldarg.0
		IL_00b9: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::sortItemsList
		IL_00be: ldarg.1
		IL_00bf: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Add(!0)
		IL_00c4: nop
		IL_00c5: nop

		IL_00c6: ldarg.0
		IL_00c7: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_00cc: ldarg.1
		IL_00cd: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_00d2: ldarg.1
		IL_00d3: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
		IL_00d8: ldarg.1
		IL_00d9: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_00de: callvirt instance class [VM.Core]VM.Core.VmIO [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::CheckVarAddIO(string, string, int32)
		IL_00e3: stloc.0
		IL_00e4: ldloc.0
		IL_00e5: ldnull
		IL_00e6: cgt.un
		IL_00e8: stloc.s 4
		IL_00ea: ldloc.s 4
		IL_00ec: brfalse.s IL_0105

		IL_00ee: nop
		IL_00ef: ldarg.1
		IL_00f0: ldloc.0
		IL_00f1: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_DecorateSource(object)
		IL_00f6: nop
		IL_00f7: ldloc.0
		IL_00f8: ldarg.1
		IL_00f9: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_00fe: callvirt instance void [VM.Core]VM.Core.VmIO::set_Index(int32)
		IL_0103: nop
		IL_0104: nop

		IL_0105: ldarg.1
		IL_0106: ldarg.0
		IL_0107: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_010c: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::SetVarManager(class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager)
		IL_0111: nop
		IL_0112: ldarg.1
		IL_0113: ldstr "PropertyChanged"
		IL_0118: ldarg.0
		IL_0119: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ValueChanged(object, class [System]System.ComponentModel.PropertyChangedEventArgs)
		IL_011f: newobj instance void class [mscorlib]System.EventHandler`1<class [System]System.ComponentModel.PropertyChangedEventArgs>::.ctor(object, native int)
		IL_0124: call void class [WindowsBase]System.Windows.WeakEventManager`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, class [System]System.ComponentModel.PropertyChangedEventArgs>::AddHandler(!0, string, class [mscorlib]System.EventHandler`1<!1>)
		IL_0129: nop

		IL_012a: ret
	} // end of method GlobalVariableDialog::AddNewItem

	.method private hidebysig 
		instance bool ExcudePropertyName (
			string PropertyName
		) cil managed 
	{
		// Method begins at RVA 0x274e4
		// Header size: 12
		// Code size: 58 (0x3a)
		.maxstack 3
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass96_0',
			[1] bool,
			[2] bool
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass96_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.1
		IL_0008: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass96_0'::PropertyName
		IL_000d: nop
		IL_000e: ldarg.0
		IL_000f: ldfld class [mscorlib]System.Collections.Generic.List`1<string> VMControls.WPF.GlobalUI.GlobalVariableDialog::ExcudePropertyNameList
		IL_0014: ldloc.0
		IL_0015: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass96_0'::'<ExcudePropertyName>b__0'(string)
		IL_001b: newobj instance void class [mscorlib]System.Predicate`1<string>::.ctor(object, native int)
		IL_0020: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<string>::FindIndex(class [mscorlib]System.Predicate`1<!0>)
		IL_0025: ldc.i4.0
		IL_0026: clt
		IL_0028: ldc.i4.0
		IL_0029: ceq
		IL_002b: stloc.1
		IL_002c: ldloc.1
		IL_002d: brfalse.s IL_0034

		IL_002f: nop
		IL_0030: ldc.i4.1
		IL_0031: stloc.2
		IL_0032: br.s IL_0038

		IL_0034: ldc.i4.0
		IL_0035: stloc.2
		IL_0036: br.s IL_0038

		IL_0038: ldloc.2
		IL_0039: ret
	} // end of method GlobalVariableDialog::ExcudePropertyName

	.method private hidebysig 
		instance void ValueChanged (
			object sender,
			class [System]System.ComponentModel.PropertyChangedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x2752c
		// Header size: 12
		// Code size: 1146 (0x47a)
		.maxstack 6
		.locals init (
			[0] bool,
			[1] bool,
			[2] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[3] class [mscorlib]System.Exception,
			[4] class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException,
			[5] bool,
			[6] bool,
			[7] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[8] class [mscorlib]System.Exception,
			[9] class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException,
			[10] bool,
			[11] bool,
			[12] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[13] bool,
			[14] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[15] bool,
			[16] bool,
			[17] bool,
			[18] bool,
			[19] uint8[],
			[20] bool,
			[21] class [mscorlib]System.Exception,
			[22] class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException,
			[23] bool,
			[24] bool,
			[25] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[26] class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[],
			[27] bool,
			[28] class VMControls.WPF.GlobalUI.GlobalVariableItem
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.2
		IL_0003: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_0008: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::ExcudePropertyName(string)
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0017

		IL_0011: nop
		IL_0012: br IL_0479

		IL_0017: ldarg.2
		IL_0018: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_001d: ldstr "Type"
		IL_0022: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0027: stloc.1
		IL_0028: ldloc.1
		IL_0029: brfalse IL_00d8

		IL_002e: nop
		IL_002f: ldarg.1
		IL_0030: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_0035: stloc.2
		.try
		{
			IL_0036: nop
			IL_0037: ldarg.0
			IL_0038: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_003d: ldloc.2
			IL_003e: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0043: ldloc.2
			IL_0044: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_0049: ldc.i4.1
			IL_004a: ldloc.2
			IL_004b: callvirt instance class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode> VMControls.WPF.GlobalUI.GlobalVariableItem::get_SelectedRunParams()
			IL_0050: call class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVarExtern::GetLinkModuleInfoByRunParams(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode>)
			IL_0055: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyType(string, string, bool, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[])
			IL_005a: nop
			IL_005b: ldloc.2
			IL_005c: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::ResetBindStr()
			IL_0061: nop
			IL_0062: nop
			IL_0063: leave.s IL_00d2
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0065: stloc.3
			IL_0066: nop
			IL_0067: ldloc.3
			IL_0068: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
			IL_006d: stloc.s 4
			IL_006f: ldloc.s 4
			IL_0071: ldnull
			IL_0072: cgt.un
			IL_0074: stloc.s 5
			IL_0076: ldloc.s 5
			IL_0078: brfalse.s IL_00a0

			IL_007a: nop
			IL_007b: ldstr "modify type ["
			IL_0080: ldloc.2
			IL_0081: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_0086: ldstr "]  is exception:"
			IL_008b: ldloc.s 4
			IL_008d: callvirt instance string [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException::get_errorMessage()
			IL_0092: call string [mscorlib]System.String::Concat(string, string, string, string)
			IL_0097: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_009c: nop
			IL_009d: nop
			IL_009e: br.s IL_00c3

			IL_00a0: nop
			IL_00a1: ldstr "modify type ["
			IL_00a6: ldloc.2
			IL_00a7: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_00ac: ldstr "] is exception:"
			IL_00b1: ldloc.3
			IL_00b2: callvirt instance string [mscorlib]System.Object::ToString()
			IL_00b7: call string [mscorlib]System.String::Concat(string, string, string, string)
			IL_00bc: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_00c1: nop
			IL_00c2: nop

			IL_00c3: ldloc.3
			IL_00c4: callvirt instance string [mscorlib]System.Exception::get_StackTrace()
			IL_00c9: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_00ce: nop
			IL_00cf: nop
			IL_00d0: leave.s IL_00d2
		} // end handler

		IL_00d2: nop
		IL_00d3: br IL_0479

		IL_00d8: ldarg.2
		IL_00d9: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_00de: ldstr "Name"
		IL_00e3: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_00e8: stloc.s 6
		IL_00ea: ldloc.s 6
		IL_00ec: brfalse IL_01e8

		IL_00f1: nop
		IL_00f2: ldarg.1
		IL_00f3: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_00f8: stloc.s 7
		.try
		{
			IL_00fa: nop
			IL_00fb: ldarg.0
			IL_00fc: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_0101: ldloc.s 7
			IL_0103: ldfld string VMControls.WPF.GlobalUI.GlobalVariableItem::strOldName
			IL_0108: ldloc.s 7
			IL_010a: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_010f: ldloc.s 7
			IL_0111: callvirt instance class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode> VMControls.WPF.GlobalUI.GlobalVariableItem::get_SelectedRunParams()
			IL_0116: call class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVarExtern::GetLinkModuleInfoByRunParams(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode>)
			IL_011b: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyVarName(string, string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[])
			IL_0120: nop
			IL_0121: nop
			IL_0122: leave IL_01e2
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0127: stloc.s 8
			IL_0129: nop
			IL_012a: ldloc.s 8
			IL_012c: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
			IL_0131: stloc.s 9
			IL_0133: ldloc.s 9
			IL_0135: ldnull
			IL_0136: cgt.un
			IL_0138: stloc.s 10
			IL_013a: ldloc.s 10
			IL_013c: brfalse.s IL_0189

			IL_013e: nop
			IL_013f: ldc.i4.6
			IL_0140: newarr [mscorlib]System.String
			IL_0145: dup
			IL_0146: ldc.i4.0
			IL_0147: ldstr "modify name ["
			IL_014c: stelem.ref
			IL_014d: dup
			IL_014e: ldc.i4.1
			IL_014f: ldloc.s 7
			IL_0151: ldfld string VMControls.WPF.GlobalUI.GlobalVariableItem::strOldName
			IL_0156: stelem.ref
			IL_0157: dup
			IL_0158: ldc.i4.2
			IL_0159: ldstr "] to ["
			IL_015e: stelem.ref
			IL_015f: dup
			IL_0160: ldc.i4.3
			IL_0161: ldloc.s 7
			IL_0163: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0168: stelem.ref
			IL_0169: dup
			IL_016a: ldc.i4.4
			IL_016b: ldstr "] is exception:"
			IL_0170: stelem.ref
			IL_0171: dup
			IL_0172: ldc.i4.5
			IL_0173: ldloc.s 9
			IL_0175: callvirt instance string [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException::get_errorMessage()
			IL_017a: stelem.ref
			IL_017b: call string [mscorlib]System.String::Concat(string[])
			IL_0180: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_0185: nop
			IL_0186: nop
			IL_0187: br.s IL_01d2

			IL_0189: nop
			IL_018a: ldc.i4.6
			IL_018b: newarr [mscorlib]System.String
			IL_0190: dup
			IL_0191: ldc.i4.0
			IL_0192: ldstr "modify name ["
			IL_0197: stelem.ref
			IL_0198: dup
			IL_0199: ldc.i4.1
			IL_019a: ldloc.s 7
			IL_019c: ldfld string VMControls.WPF.GlobalUI.GlobalVariableItem::strOldName
			IL_01a1: stelem.ref
			IL_01a2: dup
			IL_01a3: ldc.i4.2
			IL_01a4: ldstr "] to ["
			IL_01a9: stelem.ref
			IL_01aa: dup
			IL_01ab: ldc.i4.3
			IL_01ac: ldloc.s 7
			IL_01ae: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_01b3: stelem.ref
			IL_01b4: dup
			IL_01b5: ldc.i4.4
			IL_01b6: ldstr "] is exception:"
			IL_01bb: stelem.ref
			IL_01bc: dup
			IL_01bd: ldc.i4.5
			IL_01be: ldloc.s 8
			IL_01c0: callvirt instance string [mscorlib]System.Object::ToString()
			IL_01c5: stelem.ref
			IL_01c6: call string [mscorlib]System.String::Concat(string[])
			IL_01cb: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_01d0: nop
			IL_01d1: nop

			IL_01d2: ldloc.s 8
			IL_01d4: callvirt instance string [mscorlib]System.Exception::get_StackTrace()
			IL_01d9: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_01de: nop
			IL_01df: nop
			IL_01e0: leave.s IL_01e2
		} // end handler

		IL_01e2: nop
		IL_01e3: br IL_0479

		IL_01e8: ldarg.2
		IL_01e9: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_01ee: ldstr "RemarkInfo"
		IL_01f3: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_01f8: stloc.s 11
		IL_01fa: ldloc.s 11
		IL_01fc: brfalse.s IL_0227

		IL_01fe: nop
		IL_01ff: ldarg.1
		IL_0200: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_0205: stloc.s 12
		IL_0207: ldarg.0
		IL_0208: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_020d: ldloc.s 12
		IL_020f: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0214: ldloc.s 12
		IL_0216: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_RemarkInfo()
		IL_021b: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyRemark(string, string)
		IL_0220: nop
		IL_0221: nop
		IL_0222: br IL_0479

		IL_0227: ldarg.2
		IL_0228: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_022d: ldstr "Value"
		IL_0232: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0237: stloc.s 13
		IL_0239: ldloc.s 13
		IL_023b: brfalse IL_03ec

		IL_0240: nop
		IL_0241: ldarg.0
		IL_0242: ldc.i4.1
		IL_0243: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsChangeVarValue
		IL_0248: ldarg.1
		IL_0249: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_024e: stloc.s 14
		.try
		{
			IL_0250: nop
			IL_0251: ldloc.s 14
			IL_0253: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_0258: ldstr "int"
			IL_025d: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_0262: stloc.s 15
			IL_0264: ldloc.s 15
			IL_0266: brfalse.s IL_029c

			IL_0268: nop
			IL_0269: ldarg.0
			IL_026a: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_026f: ldloc.s 14
			IL_0271: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0276: ldc.i4.1
			IL_0277: newarr [mscorlib]System.Int32
			IL_027c: dup
			IL_027d: ldc.i4.0
			IL_027e: ldloc.s 14
			IL_0280: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Value()
			IL_0285: call int32 [mscorlib]System.Int32::Parse(string)
			IL_028a: stelem.i4
			IL_028b: ldstr ""
			IL_0290: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetVarIntValue(string, int32[], string)
			IL_0295: nop
			IL_0296: nop
			IL_0297: br IL_0376

			IL_029c: ldloc.s 14
			IL_029e: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_02a3: ldstr "float"
			IL_02a8: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_02ad: stloc.s 16
			IL_02af: ldloc.s 16
			IL_02b1: brfalse.s IL_02e7

			IL_02b3: nop
			IL_02b4: ldarg.0
			IL_02b5: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_02ba: ldloc.s 14
			IL_02bc: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_02c1: ldc.i4.1
			IL_02c2: newarr [mscorlib]System.Single
			IL_02c7: dup
			IL_02c8: ldc.i4.0
			IL_02c9: ldloc.s 14
			IL_02cb: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Value()
			IL_02d0: call float32 [mscorlib]System.Single::Parse(string)
			IL_02d5: stelem.r4
			IL_02d6: ldstr ""
			IL_02db: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetVarFloatValue(string, float32[], string)
			IL_02e0: nop
			IL_02e1: nop
			IL_02e2: br IL_0376

			IL_02e7: ldloc.s 14
			IL_02e9: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_02ee: ldstr "string"
			IL_02f3: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_02f8: stloc.s 17
			IL_02fa: ldloc.s 17
			IL_02fc: brfalse.s IL_032a

			IL_02fe: nop
			IL_02ff: ldarg.0
			IL_0300: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_0305: ldloc.s 14
			IL_0307: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_030c: ldc.i4.1
			IL_030d: newarr [mscorlib]System.String
			IL_0312: dup
			IL_0313: ldc.i4.0
			IL_0314: ldloc.s 14
			IL_0316: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Value()
			IL_031b: stelem.ref
			IL_031c: ldstr ""
			IL_0321: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetVarStringValue(string, string[], string)
			IL_0326: nop
			IL_0327: nop
			IL_0328: br.s IL_0376

			IL_032a: ldloc.s 14
			IL_032c: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
			IL_0331: ldstr "byte"
			IL_0336: call bool [mscorlib]System.String::op_Equality(string, string)
			IL_033b: stloc.s 18
			IL_033d: ldloc.s 18
			IL_033f: brfalse.s IL_0376

			IL_0341: nop
			IL_0342: ldnull
			IL_0343: stloc.s 19
			IL_0345: ldloc.s 14
			IL_0347: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Value()
			IL_034c: ldloca.s 19
			IL_034e: call bool [VM.PlatformSDKCS]VM.PlatformSDKCS.StringUtil::CheckByteString(string, uint8[]&)
			IL_0353: stloc.s 20
			IL_0355: ldloc.s 20
			IL_0357: brfalse.s IL_0375

			IL_0359: nop
			IL_035a: ldarg.0
			IL_035b: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_0360: ldloc.s 14
			IL_0362: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_0367: ldloc.s 19
			IL_0369: ldstr ""
			IL_036e: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetVarByteValue(string, uint8[], string)
			IL_0373: nop
			IL_0374: nop

			IL_0375: nop

			IL_0376: nop
			IL_0377: leave.s IL_03df
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0379: stloc.s 21
			IL_037b: nop
			IL_037c: ldloc.s 21
			IL_037e: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
			IL_0383: stloc.s 22
			IL_0385: ldloc.s 22
			IL_0387: ldnull
			IL_0388: cgt.un
			IL_038a: stloc.s 23
			IL_038c: ldloc.s 23
			IL_038e: brfalse.s IL_03b7

			IL_0390: nop
			IL_0391: ldstr "modify value is exception:"
			IL_0396: ldloc.s 14
			IL_0398: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_039d: ldstr ","
			IL_03a2: ldloc.s 22
			IL_03a4: callvirt instance string [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException::get_errorMessage()
			IL_03a9: call string [mscorlib]System.String::Concat(string, string, string, string)
			IL_03ae: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_03b3: nop
			IL_03b4: nop
			IL_03b5: br.s IL_03dc

			IL_03b7: nop
			IL_03b8: ldstr "modify value is exception:"
			IL_03bd: ldloc.s 14
			IL_03bf: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_03c4: ldstr ","
			IL_03c9: ldloc.s 21
			IL_03cb: callvirt instance string [mscorlib]System.Object::ToString()
			IL_03d0: call string [mscorlib]System.String::Concat(string, string, string, string)
			IL_03d5: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_03da: nop
			IL_03db: nop

			IL_03dc: nop
			IL_03dd: leave.s IL_03df
		} // end handler

		IL_03df: ldarg.0
		IL_03e0: ldc.i4.0
		IL_03e1: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsChangeVarValue
		IL_03e6: nop
		IL_03e7: br IL_0479

		IL_03ec: ldarg.2
		IL_03ed: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_03f2: ldstr "SelectedRunParams"
		IL_03f7: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_03fc: stloc.s 24
		IL_03fe: ldloc.s 24
		IL_0400: brfalse.s IL_043f

		IL_0402: nop
		IL_0403: ldarg.1
		IL_0404: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_0409: stloc.s 25
		IL_040b: ldloc.s 25
		IL_040d: callvirt instance class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode> VMControls.WPF.GlobalUI.GlobalVariableItem::get_SelectedRunParams()
		IL_0412: call class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVarExtern::GetLinkModuleInfoByRunParams(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode>)
		IL_0417: stloc.s 26
		IL_0419: ldarg.0
		IL_041a: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_041f: ldloc.s 25
		IL_0421: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0426: ldloc.s 25
		IL_0428: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Type()
		IL_042d: ldloc.s 25
		IL_042f: callvirt instance class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVariableItem::GetOldSelectedRunParams()
		IL_0434: ldloc.s 26
		IL_0436: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyLinkModuleForSub(string, string, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[], class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[])
		IL_043b: nop
		IL_043c: nop
		IL_043d: br.s IL_0479

		IL_043f: ldarg.2
		IL_0440: callvirt instance string [System]System.ComponentModel.PropertyChangedEventArgs::get_PropertyName()
		IL_0445: ldstr "CommInit"
		IL_044a: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_044f: stloc.s 27
		IL_0451: ldloc.s 27
		IL_0453: brfalse.s IL_0479

		IL_0455: nop
		IL_0456: ldarg.1
		IL_0457: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_045c: stloc.s 28
		IL_045e: ldarg.0
		IL_045f: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_0464: ldloc.s 28
		IL_0466: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_046b: ldloc.s 28
		IL_046d: callvirt instance bool VMControls.WPF.GlobalUI.GlobalVariableItem::get_CommInit()
		IL_0472: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyVarCommInit(string, bool)
		IL_0477: nop
		IL_0478: nop

		IL_0479: ret
	} // end of method GlobalVariableDialog::ValueChanged

	.method private hidebysig 
		instance void UpdatePageData (
			[opt] valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType 'type'
		) cil managed 
	{
		.param [1] = int32(2)
		// Method begins at RVA 0x27a00
		// Header size: 12
		// Code size: 369 (0x171)
		.maxstack 3
		.locals init (
			[0] object,
			[1] bool,
			[2] int32,
			[3] bool,
			[4] bool,
			[5] bool,
			[6] bool,
			[7] int32,
			[8] bool,
			[9] bool,
			[10] bool,
			[11] bool,
			[12] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::_lock
		IL_0007: stloc.0
		IL_0008: ldc.i4.0
		IL_0009: stloc.1
		.try
		{
			IL_000a: ldloc.0
			IL_000b: ldloca.s 1
			IL_000d: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
			IL_0012: nop
			IL_0013: nop
			IL_0014: ldarg.0
			IL_0015: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_001a: brtrue.s IL_0035

			IL_001c: ldarg.0
			IL_001d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0022: ldarg.0
			IL_0023: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::'<UpdatePageData>b__98_0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_0029: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_002e: call int32 [System.Core]System.Linq.Enumerable::Count<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_0033: br.s IL_0040

			IL_0035: ldarg.0
			IL_0036: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_003b: call int32 [System.Core]System.Linq.Enumerable::Count<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)

			IL_0040: stloc.2
			IL_0041: ldloc.2
			IL_0042: ldc.i4.0
			IL_0043: cgt
			IL_0045: stloc.3
			IL_0046: ldloc.3
			IL_0047: brfalse.s IL_0076

			IL_0049: nop
			IL_004a: ldarg.0
			IL_004b: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_0050: ldc.i4.0
			IL_0051: ceq
			IL_0053: stloc.s 4
			IL_0055: ldloc.s 4
			IL_0057: brfalse.s IL_0067

			IL_0059: nop
			IL_005a: ldarg.0
			IL_005b: ldstr "Collapsed"
			IL_0060: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_EmptyStatus(string)
			IL_0065: nop
			IL_0066: nop

			IL_0067: ldarg.0
			IL_0068: ldstr "Visible"
			IL_006d: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_PageNumShowStatus(string)
			IL_0072: nop
			IL_0073: nop
			IL_0074: br.s IL_009a

			IL_0076: ldloc.2
			IL_0077: ldc.i4.0
			IL_0078: ceq
			IL_007a: stloc.s 5
			IL_007c: ldloc.s 5
			IL_007e: brfalse.s IL_009a

			IL_0080: nop
			IL_0081: ldarg.0
			IL_0082: ldstr "Visible"
			IL_0087: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_EmptyStatus(string)
			IL_008c: nop
			IL_008d: ldarg.0
			IL_008e: ldstr "Collapsed"
			IL_0093: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_PageNumShowStatus(string)
			IL_0098: nop
			IL_0099: nop

			IL_009a: ldarg.0
			IL_009b: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ItemsTotalCount()
			IL_00a0: ldloc.2
			IL_00a1: ceq
			IL_00a3: ldc.i4.0
			IL_00a4: ceq
			IL_00a6: stloc.s 6
			IL_00a8: ldloc.s 6
			IL_00aa: brfalse IL_0162

			IL_00af: nop
			IL_00b0: ldc.i4.0
			IL_00b1: stloc.s 7
			IL_00b3: ldarg.0
			IL_00b4: ldloc.2
			IL_00b5: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
			IL_00ba: nop
			IL_00bb: ldarg.0
			IL_00bc: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
			IL_00c1: ldc.i4.0
			IL_00c2: cgt
			IL_00c4: stloc.s 8
			IL_00c6: ldloc.s 8
			IL_00c8: brfalse.s IL_0108

			IL_00ca: nop
			IL_00cb: ldarg.0
			IL_00cc: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ItemsTotalCount()
			IL_00d1: ldarg.0
			IL_00d2: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
			IL_00d7: rem
			IL_00d8: ldc.i4.0
			IL_00d9: cgt
			IL_00db: stloc.s 9
			IL_00dd: ldloc.s 9
			IL_00df: brfalse.s IL_00f6

			IL_00e1: nop
			IL_00e2: ldarg.0
			IL_00e3: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ItemsTotalCount()
			IL_00e8: ldarg.0
			IL_00e9: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
			IL_00ee: div
			IL_00ef: ldc.i4.1
			IL_00f0: add
			IL_00f1: stloc.s 7
			IL_00f3: nop
			IL_00f4: br.s IL_0107

			IL_00f6: nop
			IL_00f7: ldarg.0
			IL_00f8: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ItemsTotalCount()
			IL_00fd: ldarg.0
			IL_00fe: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
			IL_0103: div
			IL_0104: stloc.s 7
			IL_0106: nop

			IL_0107: nop

			IL_0108: ldarg.0
			IL_0109: ldloc.s 7
			IL_010b: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::totalPageNum
			IL_0110: ldarg.1
			IL_0111: brtrue.s IL_011a

			IL_0113: ldloc.s 7
			IL_0115: ldc.i4.0
			IL_0116: cgt
			IL_0118: br.s IL_011b

			IL_011a: ldc.i4.0

			IL_011b: stloc.s 10
			IL_011d: ldloc.s 10
			IL_011f: brfalse.s IL_012d

			IL_0121: nop
			IL_0122: ldarg.0
			IL_0123: ldc.i4.1
			IL_0124: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
			IL_0129: nop
			IL_012a: nop
			IL_012b: br.s IL_0161

			IL_012d: ldarg.1
			IL_012e: ldc.i4.1
			IL_012f: ceq
			IL_0131: stloc.s 11
			IL_0133: ldloc.s 11
			IL_0135: brfalse.s IL_0156

			IL_0137: nop
			IL_0138: ldarg.0
			IL_0139: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
			IL_013e: ldloc.s 7
			IL_0140: cgt
			IL_0142: stloc.s 12
			IL_0144: ldloc.s 12
			IL_0146: brfalse.s IL_0153

			IL_0148: nop
			IL_0149: ldarg.0
			IL_014a: ldloc.s 7
			IL_014c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
			IL_0151: nop
			IL_0152: nop

			IL_0153: nop
			IL_0154: br.s IL_0161

			IL_0156: nop
			IL_0157: ldarg.0
			IL_0158: ldloc.s 7
			IL_015a: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
			IL_015f: nop
			IL_0160: nop

			IL_0161: nop

			IL_0162: nop
			IL_0163: leave.s IL_0170
		} // end .try
		finally
		{
			IL_0165: ldloc.1
			IL_0166: brfalse.s IL_016f

			IL_0168: ldloc.0
			IL_0169: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_016e: nop

			IL_016f: endfinally
		} // end handler

		IL_0170: ret
	} // end of method GlobalVariableDialog::UpdatePageData

	.method private hidebysig 
		instance void SeearchItem (
			string searchName
		) cil managed 
	{
		// Method begins at RVA 0x27b9c
		// Header size: 12
		// Code size: 443 (0x1bb)
		.maxstack 3
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0',
			[1] object,
			[2] bool,
			[3] class [mscorlib]System.Collections.Generic.List`1<int32>,
			[4] bool,
			[5] bool,
			[6] bool,
			[7] bool,
			[8] bool
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.1
		IL_0008: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::searchName
		IL_000d: ldloc.0
		IL_000e: ldarg.0
		IL_000f: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::'<>4__this'
		IL_0014: nop
		IL_0015: ldarg.0
		IL_0016: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::_lock
		IL_001b: stloc.1
		IL_001c: ldc.i4.0
		IL_001d: stloc.2
		.try
		{
			IL_001e: ldloc.1
			IL_001f: ldloca.s 2
			IL_0021: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
			IL_0026: nop
			IL_0027: nop
			IL_0028: ldloc.0
			IL_0029: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::searchName
			IL_002e: call bool [mscorlib]System.String::IsNullOrEmpty(string)
			IL_0033: stloc.s 4
			IL_0035: ldloc.s 4
			IL_0037: brfalse.s IL_00a4

			IL_0039: nop
			IL_003a: ldarg.0
			IL_003b: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_0040: stloc.s 5
			IL_0042: ldloc.s 5
			IL_0044: brfalse.s IL_009f

			IL_0046: nop
			IL_0047: ldarg.0
			IL_0048: ldc.i4.0
			IL_0049: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_004e: ldarg.0
			IL_004f: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_0054: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Clear()
			IL_0059: nop
			IL_005a: ldarg.0
			IL_005b: ldc.i4.0
			IL_005c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
			IL_0061: nop
			IL_0062: ldarg.0
			IL_0063: ldc.i4.0
			IL_0064: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
			IL_0069: nop
			IL_006a: ldarg.0
			IL_006b: ldc.i4.0
			IL_006c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
			IL_0071: nop
			IL_0072: ldarg.0
			IL_0073: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0078: ldnull
			IL_0079: cgt.un
			IL_007b: stloc.s 6
			IL_007d: ldloc.s 6
			IL_007f: brfalse.s IL_0090

			IL_0081: nop
			IL_0082: ldarg.0
			IL_0083: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0088: ldc.i4.0
			IL_0089: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
			IL_008e: nop
			IL_008f: nop

			IL_0090: ldarg.0
			IL_0091: ldnull
			IL_0092: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0097: ldarg.0
			IL_0098: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
			IL_009d: nop
			IL_009e: nop

			IL_009f: leave IL_01ba

			IL_00a4: ldarg.0
			IL_00a5: ldc.i4.1
			IL_00a6: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_00ab: ldarg.0
			IL_00ac: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_00b1: ldloc.0
			IL_00b2: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::'<SeearchItem>b__0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_00b8: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_00bd: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_00c2: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__99_1'
			IL_00c7: dup
			IL_00c8: brtrue.s IL_00e1

			IL_00ca: pop
			IL_00cb: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
			IL_00d0: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<SeearchItem>b__99_1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_00d6: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>::.ctor(object, native int)
			IL_00db: dup
			IL_00dc: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__99_1'

			IL_00e1: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
			IL_00e6: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
			IL_00eb: stloc.3
			IL_00ec: ldloc.3
			IL_00ed: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
			IL_00f2: ldc.i4.0
			IL_00f3: cgt
			IL_00f5: stloc.s 7
			IL_00f7: ldloc.s 7
			IL_00f9: brfalse.s IL_012a

			IL_00fb: nop
			IL_00fc: ldarg.0
			IL_00fd: ldc.i4.0
			IL_00fe: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
			IL_0103: nop
			IL_0104: ldarg.0
			IL_0105: ldc.i4.0
			IL_0106: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
			IL_010b: nop
			IL_010c: ldarg.0
			IL_010d: ldloc.3
			IL_010e: stfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_0113: ldarg.0
			IL_0114: ldc.i4.0
			IL_0115: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
			IL_011a: nop
			IL_011b: ldarg.0
			IL_011c: ldstr "Collapsed"
			IL_0121: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_EmptyStatus(string)
			IL_0126: nop
			IL_0127: nop
			IL_0128: br.s IL_0164

			IL_012a: nop
			IL_012b: ldarg.0
			IL_012c: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0131: callvirt instance void VMControls.WPF.GlobalUI.VarGroupItem::Clear()
			IL_0136: nop
			IL_0137: ldarg.0
			IL_0138: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_013d: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Clear()
			IL_0142: nop
			IL_0143: ldarg.0
			IL_0144: ldc.i4.0
			IL_0145: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
			IL_014a: nop
			IL_014b: ldarg.0
			IL_014c: ldstr "Visible"
			IL_0151: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_EmptyStatus(string)
			IL_0156: nop
			IL_0157: ldarg.0
			IL_0158: ldstr "Collapsed"
			IL_015d: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_PageNumShowStatus(string)
			IL_0162: nop
			IL_0163: nop

			IL_0164: ldarg.0
			IL_0165: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_016a: brfalse.s IL_0188

			IL_016c: ldarg.0
			IL_016d: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_0172: ldloc.0
			IL_0173: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass99_0'::'<SeearchItem>b__2'(int32)
			IL_0179: newobj instance void class [mscorlib]System.Predicate`1<int32>::.ctor(object, native int)
			IL_017e: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::FindIndex(class [mscorlib]System.Predicate`1<!0>)
			IL_0183: ldc.i4.0
			IL_0184: clt
			IL_0186: br.s IL_0189

			IL_0188: ldc.i4.0

			IL_0189: stloc.s 8
			IL_018b: ldloc.s 8
			IL_018d: brfalse.s IL_01ac

			IL_018f: nop
			IL_0190: ldarg.0
			IL_0191: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0196: ldc.i4.0
			IL_0197: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
			IL_019c: nop
			IL_019d: ldarg.0
			IL_019e: ldnull
			IL_019f: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_01a4: ldarg.0
			IL_01a5: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
			IL_01aa: nop
			IL_01ab: nop

			IL_01ac: nop
			IL_01ad: leave.s IL_01ba
		} // end .try
		finally
		{
			IL_01af: ldloc.2
			IL_01b0: brfalse.s IL_01b9

			IL_01b2: ldloc.1
			IL_01b3: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_01b8: nop

			IL_01b9: endfinally
		} // end handler

		IL_01ba: ret
	} // end of method GlobalVariableDialog::SeearchItem

	.method private hidebysig 
		instance void SetBindData () cil managed 
	{
		// Method begins at RVA 0x27d80
		// Header size: 12
		// Code size: 152 (0x98)
		.maxstack 4
		.locals init (
			[0] int32,
			[1] class [System.Core]System.Linq.IOrderedEnumerable`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[2] int32,
			[3] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_0007: ldc.i4.0
		IL_0008: ble.s IL_0015

		IL_000a: ldarg.0
		IL_000b: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_0010: ldnull
		IL_0011: ceq
		IL_0013: br.s IL_0016

		IL_0015: ldc.i4.1

		IL_0016: stloc.3
		IL_0017: ldloc.3
		IL_0018: brfalse.s IL_001d

		IL_001a: nop
		IL_001b: br.s IL_0097

		IL_001d: ldarg.0
		IL_001e: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_0023: ldc.i4.1
		IL_0024: sub
		IL_0025: ldarg.0
		IL_0026: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_002b: mul
		IL_002c: stloc.0
		IL_002d: ldarg.0
		IL_002e: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_0033: ldarg.0
		IL_0034: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::'<SetBindData>b__100_0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_003a: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_003f: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_0044: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__100_1'
		IL_0049: dup
		IL_004a: brtrue.s IL_0063

		IL_004c: pop
		IL_004d: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_0052: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<SetBindData>b__100_1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0058: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>::.ctor(object, native int)
		IL_005d: dup
		IL_005e: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__100_1'

		IL_0063: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_0068: stloc.1
		IL_0069: ldarg.0
		IL_006a: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_006f: ldarg.0
		IL_0070: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::totalPageNum
		IL_0075: blt.s IL_007f

		IL_0077: ldloc.1
		IL_0078: call int32 [System.Core]System.Linq.Enumerable::Count<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_007d: br.s IL_0087

		IL_007f: ldloc.0
		IL_0080: ldarg.0
		IL_0081: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_0086: add

		IL_0087: stloc.2
		IL_0088: ldarg.0
		IL_0089: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_008e: ldloc.0
		IL_008f: ldloc.2
		IL_0090: ldloc.1
		IL_0091: callvirt instance void VMControls.WPF.GlobalUI.VarGroupItem::UpdateBindItems(int32, int32, class [mscorlib]System.Collections.Generic.IEnumerable`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>)
		IL_0096: nop

		IL_0097: ret
	} // end of method GlobalVariableDialog::SetBindData

	.method private hidebysig 
		instance void SetBindSearchData () cil managed 
	{
		// Method begins at RVA 0x27e24
		// Header size: 12
		// Code size: 113 (0x71)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] int32,
			[2] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_0007: ldc.i4.0
		IL_0008: ble.s IL_0015

		IL_000a: ldarg.0
		IL_000b: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_0010: ldnull
		IL_0011: ceq
		IL_0013: br.s IL_0016

		IL_0015: ldc.i4.1

		IL_0016: stloc.2
		IL_0017: ldloc.2
		IL_0018: brfalse.s IL_001d

		IL_001a: nop
		IL_001b: br.s IL_0070

		IL_001d: ldarg.0
		IL_001e: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_0023: ldc.i4.1
		IL_0024: sub
		IL_0025: ldarg.0
		IL_0026: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_002b: mul
		IL_002c: stloc.0
		IL_002d: ldarg.0
		IL_002e: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_0033: ldarg.0
		IL_0034: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::totalPageNum
		IL_0039: blt.s IL_0048

		IL_003b: ldarg.0
		IL_003c: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_0041: call int32 [System.Core]System.Linq.Enumerable::Count<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_0046: br.s IL_0050

		IL_0048: ldloc.0
		IL_0049: ldarg.0
		IL_004a: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_004f: add

		IL_0050: stloc.1
		IL_0051: ldarg.0
		IL_0052: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_0057: ldarg.0
		IL_0058: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_005d: ldarg.0
		IL_005e: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_0063: callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<int32>::ToArray()
		IL_0068: ldloc.0
		IL_0069: ldloc.1
		IL_006a: callvirt instance void VMControls.WPF.GlobalUI.VarGroupItem::UpdateBindItems(class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>, int32[], int32, int32)
		IL_006f: nop

		IL_0070: ret
	} // end of method GlobalVariableDialog::SetBindSearchData

	.method public hidebysig 
		instance void GetDataAsync () cil managed 
	{
		// Method begins at RVA 0x27ea1
		// Header size: 1
		// Code size: 20 (0x14)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldftn instance class [mscorlib]System.Threading.Tasks.Task VMControls.WPF.GlobalUI.GlobalVariableDialog::'<GetDataAsync>b__103_0'()
		IL_0008: newobj instance void class [mscorlib]System.Func`1<class [mscorlib]System.Threading.Tasks.Task>::.ctor(object, native int)
		IL_000d: call class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Run(class [mscorlib]System.Func`1<class [mscorlib]System.Threading.Tasks.Task>)
		IL_0012: pop
		IL_0013: ret
	} // end of method GlobalVariableDialog::GetDataAsync

	.method family hidebysig virtual 
		instance void OnClosing (
			class [System]System.ComponentModel.CancelEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x27eb8
		// Header size: 12
		// Code size: 64 (0x40)
		.maxstack 3

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldarg.1
		IL_0003: call instance void [VM.Controls]VM.Controls.BaseWindow::OnClosing(class [System]System.ComponentModel.CancelEventArgs)
		IL_0008: nop
		IL_0009: ldarg.0
		IL_000a: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_000f: ldc.i4.0
		IL_0010: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetReportState(bool)
		IL_0015: nop
		IL_0016: ldarg.0
		IL_0017: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_001c: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ClearCache()
		IL_0021: nop
		IL_0022: ldarg.0
		IL_0023: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
		IL_0028: isinst [VM.PlatformSDKCS]VM.PlatformSDKCS.IVarModule
		IL_002d: ldarg.0
		IL_002e: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::VarValueReportEvent(object, class [mscorlib]System.EventArgs)
		IL_0034: newobj instance void [mscorlib]System.EventHandler::.ctor(object, native int)
		IL_0039: callvirt instance void [VM.PlatformSDKCS]VM.PlatformSDKCS.IVarModule::remove_VarValueReportEvent(class [mscorlib]System.EventHandler)
		IL_003e: nop
		IL_003f: ret
	} // end of method GlobalVariableDialog::OnClosing

	.method public hidebysig specialname 
		instance bool get_IsLoading () cil managed 
	{
		// Method begins at RVA 0x27f04
		// Header size: 12
		// Code size: 16 (0x10)
		.maxstack 1
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: call class [VM.Core]VM.Core.VmSolution [VM.Core]VM.Core.VmSolution::get_Instance()
		IL_0006: callvirt instance bool [VMControls.Interface]VMControls.Interface.IVmSolution::get_IsLoading()
		IL_000b: stloc.0
		IL_000c: br.s IL_000e

		IL_000e: ldloc.0
		IL_000f: ret
	} // end of method GlobalVariableDialog::get_IsLoading

	.method private hidebysig 
		instance void GetData (
			valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[] vARIABLEs
		) cil managed 
	{
		// Method begins at RVA 0x27f20
		// Header size: 12
		// Code size: 941 (0x3ad)
		.maxstack 3
		.locals init (
			[0] object,
			[1] bool,
			[2] class [VM.Core]VM.Core.VmIOCollection,
			[3] class [mscorlib]System.Collections.Generic.List`1<string>,
			[4] class [VM.Core]VM.Core.VmModule,
			[5] class [mscorlib]System.Collections.Generic.List`1<string>,
			[6] bool,
			[7] bool,
			[8] int32,
			[9] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[10] bool,
			[11] bool,
			[12] class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[13] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[14] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>,
			[15] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0',
			[16] bool,
			[17] bool,
			[18] bool,
			[19] bool,
			[20] class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[21] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[22] class [mscorlib]System.Exception,
			[23] string,
			[24] class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException,
			[25] bool
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: ldarg.0
			IL_0003: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::_lock
			IL_0008: stloc.0
			IL_0009: ldc.i4.0
			IL_000a: stloc.1
			.try
			{
				IL_000b: ldloc.0
				IL_000c: ldloca.s 1
				IL_000e: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
				IL_0013: nop
				IL_0014: nop
				IL_0015: ldarg.0
				IL_0016: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_001b: brfalse.s IL_0030

				IL_001d: ldarg.0
				IL_001e: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_0023: isinst [VMControls.Interface]VMControls.Interface.IVmModule
				IL_0028: brfalse.s IL_0030

				IL_002a: ldarg.1
				IL_002b: ldnull
				IL_002c: ceq
				IL_002e: br.s IL_0031

				IL_0030: ldc.i4.1

				IL_0031: stloc.s 6
				IL_0033: ldloc.s 6
				IL_0035: brfalse.s IL_003d

				IL_0037: nop
				IL_0038: leave IL_03ac

				IL_003d: ldarg.0
				IL_003e: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_0043: isinst [VM.Core]VM.Core.VmModule
				IL_0048: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Inputs()
				IL_004d: stloc.2
				IL_004e: ldarg.0
				IL_004f: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::sortItemsList
				IL_0054: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Clear()
				IL_0059: nop
				IL_005a: ldarg.0
				IL_005b: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_0060: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
				IL_0065: ldarg.1
				IL_0066: ldlen
				IL_0067: conv.i4
				IL_0068: cgt
				IL_006a: stloc.s 7
				IL_006c: ldloc.s 7
				IL_006e: brfalse.s IL_007e

				IL_0070: nop
				IL_0071: ldarg.0
				IL_0072: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_0077: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Clear()
				IL_007c: nop
				IL_007d: nop

				IL_007e: newobj instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
				IL_0083: stloc.3
				IL_0084: ldc.i4.0
				IL_0085: stloc.s 8
				IL_0087: br.s IL_00d5
				// loop start (head: IL_00d5)
					IL_0089: nop
					IL_008a: ldarg.0
					IL_008b: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsLoading()
					IL_0090: stloc.s 10
					IL_0092: ldloc.s 10
					IL_0094: brfalse.s IL_00a7

					IL_0096: nop
					IL_0097: ldstr "solution is loading"
					IL_009c: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
					IL_00a1: nop
					IL_00a2: leave IL_03ac

					IL_00a7: ldarg.1
					IL_00a8: ldloc.s 8
					IL_00aa: ldelem [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE
					IL_00af: call class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVarExtern::StructToGlobalVarItem(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE)
					IL_00b4: stloc.s 9
					IL_00b6: ldarg.0
					IL_00b7: ldloc.s 9
					IL_00b9: ldc.i4.0
					IL_00ba: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::AddNewItem(class VMControls.WPF.GlobalUI.GlobalVariableItem, bool)
					IL_00bf: nop
					IL_00c0: ldloc.3
					IL_00c1: ldloc.s 9
					IL_00c3: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
					IL_00c8: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<string>::Add(!0)
					IL_00cd: nop
					IL_00ce: nop
					IL_00cf: ldloc.s 8
					IL_00d1: ldc.i4.1
					IL_00d2: add
					IL_00d3: stloc.s 8

					IL_00d5: ldloc.s 8
					IL_00d7: ldarg.1
					IL_00d8: ldlen
					IL_00d9: conv.i4
					IL_00da: clt
					IL_00dc: stloc.s 11
					IL_00de: ldloc.s 11
					IL_00e0: brtrue.s IL_0089
				// end loop

				IL_00e2: ldarg.0
				IL_00e3: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::sortItemsList
				IL_00e8: ldsfld class [mscorlib]System.Comparison`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__107_0'
				IL_00ed: dup
				IL_00ee: brtrue.s IL_0107

				IL_00f0: pop
				IL_00f1: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
				IL_00f6: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<GetData>b__107_0'(class VMControls.WPF.GlobalUI.GlobalVariableItem, class VMControls.WPF.GlobalUI.GlobalVariableItem)
				IL_00fc: newobj instance void class [mscorlib]System.Comparison`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor(object, native int)
				IL_0101: dup
				IL_0102: stsfld class [mscorlib]System.Comparison`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__107_0'

				IL_0107: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Sort(class [mscorlib]System.Comparison`1<!0>)
				IL_010c: nop
				IL_010d: ldarg.0
				IL_010e: ldarg.0
				IL_010f: ldfld class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::sortItemsList
				IL_0114: newobj instance void class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::.ctor(class [mscorlib]System.Collections.Generic.List`1<!0>)
				IL_0119: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_Items(class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>)
				IL_011e: nop
				IL_011f: nop
				IL_0120: ldarg.0
				IL_0121: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_0126: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::GetEnumerator()
				IL_012b: stloc.s 12
				.try
				{
					IL_012d: br.s IL_0148
					// loop start (head: IL_0148)
						IL_012f: ldloc.s 12
						IL_0131: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Current()
						IL_0136: stloc.s 13
						IL_0138: nop
						IL_0139: ldloc.s 13
						IL_013b: ldarg.0
						IL_013c: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
						IL_0141: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Parent(object)
						IL_0146: nop
						IL_0147: nop

						IL_0148: ldloc.s 12
						IL_014a: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
						IL_014f: brtrue.s IL_012f
					// end loop

					IL_0151: leave.s IL_0160
				} // end .try
				finally
				{
					IL_0153: ldloc.s 12
					IL_0155: brfalse.s IL_015f

					IL_0157: ldloc.s 12
					IL_0159: callvirt instance void [mscorlib]System.IDisposable::Dispose()
					IL_015e: nop

					IL_015f: endfinally
				} // end handler

				IL_0160: ldarg.0
				IL_0161: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_0166: isinst [VM.Core]VM.Core.VmModule
				IL_016b: stloc.s 4
				IL_016d: ldloc.s 4
				IL_016f: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
				IL_0174: ldsfld class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, string> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__107_1'
				IL_0179: dup
				IL_017a: brtrue.s IL_0193

				IL_017c: pop
				IL_017d: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
				IL_0182: ldftn instance string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<GetData>b__107_1'(class [VM.Core]VM.Core.VmIO)
				IL_0188: newobj instance void class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, string>::.ctor(object, native int)
				IL_018d: dup
				IL_018e: stsfld class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, string> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__107_1'

				IL_0193: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class [VM.Core]VM.Core.VmIO, string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
				IL_0198: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
				IL_019d: stloc.s 5
				IL_019f: nop
				IL_01a0: ldloc.s 5
				IL_01a2: callvirt instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
				IL_01a7: stloc.s 14
				.try
				{
					IL_01a9: br.s IL_0219
					// loop start (head: IL_0219)
						IL_01ab: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0'::.ctor()
						IL_01b0: stloc.s 15
						IL_01b2: ldloc.s 15
						IL_01b4: ldloca.s 14
						IL_01b6: call instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
						IL_01bb: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0'::item
						IL_01c0: nop
						IL_01c1: ldsfld string[] [VM.Core]VM.Core.VmModule::fTimeStamp
						IL_01c6: ldloc.s 15
						IL_01c8: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0'::item
						IL_01cd: call bool [System.Core]System.Linq.Enumerable::Contains<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!0)
						IL_01d2: stloc.s 16
						IL_01d4: ldloc.s 16
						IL_01d6: brfalse.s IL_01db

						IL_01d8: nop
						IL_01d9: br.s IL_0219

						IL_01db: ldloc.3
						IL_01dc: ldloc.s 15
						IL_01de: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0'::'<GetData>b__2'(string)
						IL_01e4: newobj instance void class [mscorlib]System.Predicate`1<string>::.ctor(object, native int)
						IL_01e9: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<string>::FindIndex(class [mscorlib]System.Predicate`1<!0>)
						IL_01ee: ldc.i4.0
						IL_01ef: clt
						IL_01f1: stloc.s 17
						IL_01f3: ldloc.s 17
						IL_01f5: brfalse.s IL_0218

						IL_01f7: nop
						IL_01f8: ldloc.s 4
						IL_01fa: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
						IL_01ff: ldloc.s 15
						IL_0201: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass107_0'::item
						IL_0206: callvirt instance class [VM.Core]VM.Core.VmIO [VM.Core]VM.Core.VmIOCollection::GetIO(string)
						IL_020b: dup
						IL_020c: brtrue.s IL_0211

						IL_020e: pop
						IL_020f: br.s IL_0217

						IL_0211: callvirt instance void [VM.Core]VM.Core.VmIO::Dispose()
						IL_0216: nop

						IL_0217: nop

						IL_0218: nop

						IL_0219: ldloca.s 14
						IL_021b: call instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
						IL_0220: brtrue.s IL_01ab
					// end loop

					IL_0222: leave.s IL_0233
				} // end .try
				finally
				{
					IL_0224: ldloca.s 14
					IL_0226: constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
					IL_022c: callvirt instance void [mscorlib]System.IDisposable::Dispose()
					IL_0231: nop
					IL_0232: endfinally
				} // end handler

				IL_0233: ldarg.0
				IL_0234: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::bFirstGetData
				IL_0239: stloc.s 18
				IL_023b: ldloc.s 18
				IL_023d: brfalse IL_0327

				IL_0242: nop
				IL_0243: ldarg.0
				IL_0244: ldc.i4.0
				IL_0245: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::bFirstGetData
				IL_024a: ldarg.0
				IL_024b: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
				IL_0250: brfalse.s IL_0299

				IL_0252: ldarg.0
				IL_0253: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_0258: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__107_3'
				IL_025d: dup
				IL_025e: brtrue.s IL_0277

				IL_0260: pop
				IL_0261: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
				IL_0266: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<GetData>b__107_3'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
				IL_026c: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
				IL_0271: dup
				IL_0272: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__107_3'

				IL_0277: call int32 [System.Core]System.Linq.Enumerable::Count<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
				IL_027c: ldarg.0
				IL_027d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_0282: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
				IL_0287: bne.un.s IL_0299

				IL_0289: ldarg.0
				IL_028a: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_028f: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
				IL_0294: ldc.i4.0
				IL_0295: cgt
				IL_0297: br.s IL_029a

				IL_0299: ldc.i4.0

				IL_029a: stloc.s 19
				IL_029c: ldloc.s 19
				IL_029e: brfalse.s IL_02ff

				IL_02a0: nop
				IL_02a1: nop
				IL_02a2: ldarg.0
				IL_02a3: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
				IL_02a8: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::GetEnumerator()
				IL_02ad: stloc.s 20
				.try
				{
					IL_02af: br.s IL_02cf
					// loop start (head: IL_02cf)
						IL_02b1: ldloc.s 20
						IL_02b3: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Current()
						IL_02b8: stloc.s 21
						IL_02ba: nop
						IL_02bb: ldloc.s 21
						IL_02bd: ldarg.0
						IL_02be: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
						IL_02c3: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
						IL_02c8: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_VarGroupInfo(string)
						IL_02cd: nop
						IL_02ce: nop

						IL_02cf: ldloc.s 20
						IL_02d1: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
						IL_02d6: brtrue.s IL_02b1
					// end loop

					IL_02d8: leave.s IL_02e7
				} // end .try
				finally
				{
					IL_02da: ldloc.s 20
					IL_02dc: brfalse.s IL_02e6

					IL_02de: ldloc.s 20
					IL_02e0: callvirt instance void [mscorlib]System.IDisposable::Dispose()
					IL_02e5: nop

					IL_02e6: endfinally
				} // end handler

				IL_02e7: ldarg.0
				IL_02e8: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
				IL_02ed: ldarg.0
				IL_02ee: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
				IL_02f3: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
				IL_02f8: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::SetDefaultGroupInfo(string)
				IL_02fd: nop
				IL_02fe: nop

				IL_02ff: ldarg.0
				IL_0300: ldc.i4.0
				IL_0301: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
				IL_0306: nop
				IL_0307: ldarg.0
				IL_0308: ldstr "Lang_Import"
				IL_030d: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
				IL_0312: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ProcessbarMessage(string)
				IL_0317: nop
				IL_0318: ldarg.0
				IL_0319: ldstr "Collapsed"
				IL_031e: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
				IL_0323: nop
				IL_0324: nop
				IL_0325: br.s IL_0331

				IL_0327: nop
				IL_0328: ldarg.0
				IL_0329: ldc.i4.1
				IL_032a: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
				IL_032f: nop
				IL_0330: nop

				IL_0331: nop
				IL_0332: leave.s IL_033f
			} // end .try
			finally
			{
				IL_0334: ldloc.1
				IL_0335: brfalse.s IL_033e

				IL_0337: ldloc.0
				IL_0338: call void [mscorlib]System.Threading.Monitor::Exit(object)
				IL_033d: nop

				IL_033e: endfinally
			} // end handler

			IL_033f: nop
			IL_0340: leave.s IL_03ac
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0342: stloc.s 22
			IL_0344: nop
			IL_0345: ldstr "GetData is error:"
			IL_034a: ldloc.s 22
			IL_034c: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0351: call string [mscorlib]System.String::Concat(string, string)
			IL_0356: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_035b: nop
			IL_035c: ldarg.0
			IL_035d: ldstr "Collapsed"
			IL_0362: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_0367: nop
			IL_0368: ldstr "Lang_VarLoadFailed"
			IL_036d: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0372: stloc.s 23
			IL_0374: ldloc.s 22
			IL_0376: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
			IL_037b: stloc.s 24
			IL_037d: ldloc.s 24
			IL_037f: ldnull
			IL_0380: cgt.un
			IL_0382: stloc.s 25
			IL_0384: ldloc.s 25
			IL_0386: brfalse.s IL_039f

			IL_0388: nop
			IL_0389: ldloc.s 23
			IL_038b: ldstr "\r\n"
			IL_0390: ldloc.s 24
			IL_0392: callvirt instance string [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException::get_errorMessage()
			IL_0397: call string [mscorlib]System.String::Concat(string, string, string)
			IL_039c: stloc.s 23
			IL_039e: nop

			IL_039f: ldloc.s 23
			IL_03a1: ldc.i4.3
			IL_03a2: ldc.i4.0
			IL_03a3: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_03a8: pop
			IL_03a9: nop
			IL_03aa: leave.s IL_03ac
		} // end handler

		IL_03ac: ret
	} // end of method GlobalVariableDialog::GetData

	.method private hidebysig 
		instance void InitDynamicIO () cil managed 
	{
		// Method begins at RVA 0x28358
		// Header size: 12
		// Code size: 339 (0x153)
		.maxstack 4
		.locals init (
			[0] valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[],
			[1] bool,
			[2] bool,
			[3] int32,
			[4] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass108_0',
			[5] string,
			[6] class [VM.Core]VM.Core.VmIO,
			[7] bool,
			[8] class [VM.Core]VM.Core.DynamicVmIO,
			[9] bool,
			[10] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: ldarg.0
			IL_0003: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0008: brfalse.s IL_001d

			IL_000a: ldarg.0
			IL_000b: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0010: isinst [VMControls.Interface]VMControls.Interface.IVmModule
			IL_0015: ldnull
			IL_0016: cgt.un
			IL_0018: ldc.i4.0
			IL_0019: ceq
			IL_001b: br.s IL_001e

			IL_001d: ldc.i4.1

			IL_001e: stloc.1
			IL_001f: ldloc.1
			IL_0020: brfalse.s IL_0028

			IL_0022: nop
			IL_0023: leave IL_0152

			IL_0028: ldc.i4.0
			IL_0029: newarr [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE
			IL_002e: stloc.0
			IL_002f: ldarg.0
			IL_0030: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_0035: dup
			IL_0036: brtrue.s IL_003b

			IL_0038: pop
			IL_0039: br.s IL_0043

			IL_003b: ldloca.s 0
			IL_003d: call instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::GetAllVar(valuetype [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE[]&)
			IL_0042: nop

			IL_0043: ldloc.0
			IL_0044: ldlen
			IL_0045: ldc.i4.0
			IL_0046: ceq
			IL_0048: stloc.2
			IL_0049: ldloc.2
			IL_004a: brfalse.s IL_0052

			IL_004c: nop
			IL_004d: leave IL_0152

			IL_0052: ldc.i4.0
			IL_0053: stloc.3
			IL_0054: br IL_0123
			// loop start (head: IL_0123)
				IL_0059: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass108_0'::.ctor()
				IL_005e: stloc.s 4
				IL_0060: nop
				IL_0061: ldloc.s 4
				IL_0063: ldloc.0
				IL_0064: ldloc.3
				IL_0065: ldelema [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE
				IL_006a: ldfld uint8[] [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE::chszValueName
				IL_006f: call string [VM.PlatformSDKCS]VM.PlatformSDKCS.StringUtil::UTF8GetString(uint8[])
				IL_0074: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass108_0'::varname
				IL_0079: ldloc.0
				IL_007a: ldloc.3
				IL_007b: ldelema [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE
				IL_0080: ldfld uint8[] [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager/IMVS_VARIABLE::chszValueType
				IL_0085: call string [VM.PlatformSDKCS]VM.PlatformSDKCS.StringUtil::UTF8GetString(uint8[])
				IL_008a: stloc.s 5
				IL_008c: ldarg.0
				IL_008d: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_0092: isinst [VM.Core]VM.Core.VmModule
				IL_0097: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
				IL_009c: ldloc.s 4
				IL_009e: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass108_0'::'<InitDynamicIO>b__0'(class [VM.Core]VM.Core.VmIO)
				IL_00a4: newobj instance void class [mscorlib]System.Func`2<class [VM.Core]VM.Core.VmIO, bool>::.ctor(object, native int)
				IL_00a9: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class [VM.Core]VM.Core.VmIO>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
				IL_00ae: stloc.s 6
				IL_00b0: ldloc.s 6
				IL_00b2: ldnull
				IL_00b3: ceq
				IL_00b5: stloc.s 7
				IL_00b7: ldloc.s 7
				IL_00b9: brfalse.s IL_011e

				IL_00bb: nop
				IL_00bc: newobj instance void [VM.Core]VM.Core.DynamicVmIO::.ctor()
				IL_00c1: dup
				IL_00c2: ldloc.s 4
				IL_00c4: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass108_0'::varname
				IL_00c9: callvirt instance void [VM.Core]VM.Core.VmFrameTreeBase::set_Name(string)
				IL_00ce: nop
				IL_00cf: dup
				IL_00d0: ldloc.s 5
				IL_00d2: callvirt instance void [VM.Core]VM.Core.VmIO::set_TypeName(string)
				IL_00d7: nop
				IL_00d8: dup
				IL_00d9: ldarg.0
				IL_00da: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_00df: castclass [VM.Core]VM.Core.VmModule
				IL_00e4: callvirt instance void [VM.Core]VM.Core.VmIO::set_Root(class [VM.Core]VM.Core.VmModule)
				IL_00e9: nop
				IL_00ea: dup
				IL_00eb: ldloc.3
				IL_00ec: ldc.i4.1
				IL_00ed: add
				IL_00ee: callvirt instance void [VM.Core]VM.Core.VmIO::set_Index(int32)
				IL_00f3: nop
				IL_00f4: stloc.s 8
				IL_00f6: ldloc.s 8
				IL_00f8: ldloc.s 8
				IL_00fa: callvirt instance class [VM.Core]VM.Core.VmModule [VM.Core]VM.Core.VmIO::get_Root()
				IL_00ff: callvirt instance void [VM.Core]VM.Core.VmFrameTreeBase::set_Parent(object)
				IL_0104: nop
				IL_0105: ldarg.0
				IL_0106: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
				IL_010b: isinst [VM.Core]VM.Core.VmModule
				IL_0110: callvirt instance class [VM.Core]VM.Core.VmIOCollection [VM.Core]VM.Core.VmModule::get_Outputs()
				IL_0115: ldloc.s 8
				IL_0117: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [VM.Core]VM.Core.VmIO>::Add(!0)
				IL_011c: nop
				IL_011d: nop

				IL_011e: nop
				IL_011f: ldloc.3
				IL_0120: ldc.i4.1
				IL_0121: add
				IL_0122: stloc.3

				IL_0123: ldloc.3
				IL_0124: ldloc.0
				IL_0125: ldlen
				IL_0126: conv.i4
				IL_0127: clt
				IL_0129: stloc.s 9
				IL_012b: ldloc.s 9
				IL_012d: brtrue IL_0059
			// end loop

			IL_0132: nop
			IL_0133: leave.s IL_0152
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0135: stloc.s 10
			IL_0137: nop
			IL_0138: ldstr "GetData is error:"
			IL_013d: ldloc.s 10
			IL_013f: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0144: call string [mscorlib]System.String::Concat(string, string)
			IL_0149: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_014e: nop
			IL_014f: nop
			IL_0150: leave.s IL_0152
		} // end handler

		IL_0152: ret
	} // end of method GlobalVariableDialog::InitDynamicIO

	.method private hidebysig 
		instance void Delete_OnHandler (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x284d4
		// Header size: 12
		// Code size: 816 (0x330)
		.maxstack 5
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0',
			[1] class [PresentationFramework]System.Windows.Controls.Button,
			[2] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[3] bool,
			[4] bool,
			[5] class [mscorlib]System.Exception,
			[6] object,
			[7] bool,
			[8] bool,
			[9] class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[10] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[11] bool,
			[12] int32,
			[13] bool,
			[14] bool,
			[15] bool,
			[16] bool,
			[17] bool,
			[18] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1',
			[19] bool,
			[20] class [mscorlib]System.Collections.Generic.IEnumerable`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[21] bool,
			[22] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[23] bool
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::'<>4__this'
		IL_000d: nop
		IL_000e: ldarg.1
		IL_000f: isinst [PresentationFramework]System.Windows.Controls.Button
		IL_0014: stloc.1
		IL_0015: ldloc.1
		IL_0016: ldnull
		IL_0017: ceq
		IL_0019: stloc.3
		IL_001a: ldloc.3
		IL_001b: brfalse.s IL_0022

		IL_001d: br IL_032f

		IL_0022: ldloc.1
		IL_0023: callvirt instance object [PresentationFramework]System.Windows.FrameworkElement::get_DataContext()
		IL_0028: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_002d: stloc.2
		IL_002e: ldloc.2
		IL_002f: ldnull
		IL_0030: ceq
		IL_0032: stloc.s 4
		IL_0034: ldloc.s 4
		IL_0036: brfalse.s IL_003d

		IL_0038: br IL_032f

		IL_003d: ldloc.2
		IL_003e: ldnull
		IL_003f: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Parent(object)
		IL_0044: nop
		IL_0045: ldloc.2
		IL_0046: ldstr "PropertyChanged"
		IL_004b: ldarg.0
		IL_004c: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ValueChanged(object, class [System]System.ComponentModel.PropertyChangedEventArgs)
		IL_0052: newobj instance void class [mscorlib]System.EventHandler`1<class [System]System.ComponentModel.PropertyChangedEventArgs>::.ctor(object, native int)
		IL_0057: call void class [WindowsBase]System.Windows.WeakEventManager`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, class [System]System.ComponentModel.PropertyChangedEventArgs>::RemoveHandler(!0, string, class [mscorlib]System.EventHandler`1<!1>)
		IL_005c: nop
		.try
		{
			IL_005d: nop
			IL_005e: ldarg.0
			IL_005f: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_0064: ldloc.2
			IL_0065: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_006a: ldc.i4.1
			IL_006b: ldloc.2
			IL_006c: callvirt instance class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode> VMControls.WPF.GlobalUI.GlobalVariableItem::get_SelectedRunParams()
			IL_0071: call class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[] VMControls.WPF.GlobalUI.GlobalVarExtern::GetLinkModuleInfoByRunParams(class [mscorlib]System.Collections.Generic.List`1<class [Apps.XmlUI]Apps.XmlUI.ParamTab.RunParamsBaseNode>)
			IL_0076: ldc.i4.1
			IL_0077: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::DeleteVar(string, bool, class [Apps.XmlParser]Apps.XmlParser.Variable.VarRunParamInfo[], bool)
			IL_007c: nop
			IL_007d: nop
			IL_007e: leave.s IL_00e3
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0080: stloc.s 5
			IL_0082: nop
			IL_0083: ldstr "删除变量异常:"
			IL_0088: ldloc.2
			IL_0089: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
			IL_008e: ldstr ",exception:"
			IL_0093: ldloc.s 5
			IL_0095: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException [VM.Core]VM.Core.VmSolution::GetVmException(class [mscorlib]System.Exception)
			IL_009a: callvirt instance string [mscorlib]System.Object::ToString()
			IL_009f: call string [mscorlib]System.String::Concat(string, string, string, string)
			IL_00a4: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_00a9: nop
			IL_00aa: ldstr "Lang_VariableDelete"
			IL_00af: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00b4: ldstr ":Failed"
			IL_00b9: call string [mscorlib]System.String::Concat(string, string)
			IL_00be: ldc.i4.3
			IL_00bf: ldc.i4.0
			IL_00c0: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_00c5: pop
			IL_00c6: ldloc.2
			IL_00c7: ldstr "PropertyChanged"
			IL_00cc: ldarg.0
			IL_00cd: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ValueChanged(object, class [System]System.ComponentModel.PropertyChangedEventArgs)
			IL_00d3: newobj instance void class [mscorlib]System.EventHandler`1<class [System]System.ComponentModel.PropertyChangedEventArgs>::.ctor(object, native int)
			IL_00d8: call void class [WindowsBase]System.Windows.WeakEventManager`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, class [System]System.ComponentModel.PropertyChangedEventArgs>::AddHandler(!0, string, class [mscorlib]System.EventHandler`1<!1>)
			IL_00dd: nop
			IL_00de: leave IL_032f
		} // end handler

		IL_00e3: ldloc.0
		IL_00e4: ldloc.2
		IL_00e5: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_00ea: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::nDelIndex
		IL_00ef: ldarg.0
		IL_00f0: ldfld object VMControls.WPF.GlobalUI.GlobalVariableDialog::_lock
		IL_00f5: stloc.s 6
		IL_00f7: ldc.i4.0
		IL_00f8: stloc.s 7
		.try
		{
			IL_00fa: ldloc.s 6
			IL_00fc: ldloca.s 7
			IL_00fe: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
			IL_0103: nop
			IL_0104: nop
			IL_0105: ldarg.0
			IL_0106: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_010b: brfalse.s IL_0122

			IL_010d: ldarg.0
			IL_010e: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0113: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0118: ldloc.0
			IL_0119: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::nDelIndex
			IL_011e: ceq
			IL_0120: br.s IL_0123

			IL_0122: ldc.i4.0

			IL_0123: stloc.s 8
			IL_0125: ldloc.s 8
			IL_0127: brfalse.s IL_0132

			IL_0129: nop
			IL_012a: ldarg.0
			IL_012b: ldnull
			IL_012c: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0131: nop

			IL_0132: nop
			IL_0133: ldarg.0
			IL_0134: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0139: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::GetEnumerator()
			IL_013e: stloc.s 9
			.try
			{
				IL_0140: br.s IL_0178
				// loop start (head: IL_0178)
					IL_0142: ldloc.s 9
					IL_0144: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Current()
					IL_0149: stloc.s 10
					IL_014b: nop
					IL_014c: ldloc.s 10
					IL_014e: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
					IL_0153: ldloc.0
					IL_0154: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::nDelIndex
					IL_0159: cgt
					IL_015b: stloc.s 11
					IL_015d: ldloc.s 11
					IL_015f: brfalse.s IL_0177

					IL_0161: nop
					IL_0162: ldloc.s 10
					IL_0164: dup
					IL_0165: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
					IL_016a: stloc.s 12
					IL_016c: ldloc.s 12
					IL_016e: ldc.i4.1
					IL_016f: sub
					IL_0170: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
					IL_0175: nop
					IL_0176: nop

					IL_0177: nop

					IL_0178: ldloc.s 9
					IL_017a: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_017f: brtrue.s IL_0142
				// end loop

				IL_0181: leave.s IL_0190
			} // end .try
			finally
			{
				IL_0183: ldloc.s 9
				IL_0185: brfalse.s IL_018f

				IL_0187: ldloc.s 9
				IL_0189: callvirt instance void [mscorlib]System.IDisposable::Dispose()
				IL_018e: nop

				IL_018f: endfinally
			} // end handler

			IL_0190: ldarg.0
			IL_0191: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_0196: stloc.s 13
			IL_0198: ldloc.s 13
			IL_019a: brfalse.s IL_01e1

			IL_019c: nop
			IL_019d: ldarg.0
			IL_019e: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_01a3: ldloc.0
			IL_01a4: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::nDelIndex
			IL_01a9: callvirt instance bool class [mscorlib]System.Collections.Generic.List`1<int32>::Contains(!0)
			IL_01ae: stloc.s 14
			IL_01b0: ldloc.s 14
			IL_01b2: brfalse.s IL_01c8

			IL_01b4: nop
			IL_01b5: ldarg.0
			IL_01b6: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_01bb: ldloc.2
			IL_01bc: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_01c1: callvirt instance bool class [mscorlib]System.Collections.Generic.List`1<int32>::Remove(!0)
			IL_01c6: pop
			IL_01c7: nop

			IL_01c8: ldarg.0
			IL_01c9: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_01ce: ldloc.0
			IL_01cf: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::'<Delete_OnHandler>b__0'(int32)
			IL_01d5: newobj instance void class [mscorlib]System.Action`1<int32>::.ctor(object, native int)
			IL_01da: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::ForEach(class [mscorlib]System.Action`1<!0>)
			IL_01df: nop
			IL_01e0: nop

			IL_01e1: ldarg.0
			IL_01e2: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_01e7: ldloc.2
			IL_01e8: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Contains(!0)
			IL_01ed: stloc.s 15
			IL_01ef: ldloc.s 15
			IL_01f1: brfalse.s IL_0202

			IL_01f3: nop
			IL_01f4: ldarg.0
			IL_01f5: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_01fa: ldloc.2
			IL_01fb: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Remove(!0)
			IL_0200: pop
			IL_0201: nop

			IL_0202: ldarg.0
			IL_0203: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0208: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.VarGroupItem::get_BindItems()
			IL_020d: ldloc.2
			IL_020e: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Contains(!0)
			IL_0213: stloc.s 16
			IL_0215: ldloc.s 16
			IL_0217: brfalse.s IL_022d

			IL_0219: nop
			IL_021a: ldarg.0
			IL_021b: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0220: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.VarGroupItem::get_BindItems()
			IL_0225: ldloc.2
			IL_0226: callvirt instance bool class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Remove(!0)
			IL_022b: pop
			IL_022c: nop

			IL_022d: ldarg.0
			IL_022e: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
			IL_0233: ldarg.0
			IL_0234: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::totalPageNum
			IL_0239: clt
			IL_023b: stloc.s 17
			IL_023d: ldloc.s 17
			IL_023f: brfalse IL_0310

			IL_0244: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::.ctor()
			IL_0249: stloc.s 18
			IL_024b: ldloc.s 18
			IL_024d: ldloc.0
			IL_024e: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::'CS$<>8__locals1'
			IL_0253: nop
			IL_0254: ldloc.s 18
			IL_0256: ldarg.0
			IL_0257: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
			IL_025c: ldarg.0
			IL_025d: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
			IL_0262: mul
			IL_0263: ldc.i4.1
			IL_0264: sub
			IL_0265: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::nAddIndex
			IL_026a: ldarg.0
			IL_026b: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
			IL_0270: ldc.i4.0
			IL_0271: ceq
			IL_0273: stloc.s 19
			IL_0275: ldloc.s 19
			IL_0277: brfalse.s IL_02d3

			IL_0279: nop
			IL_027a: ldarg.0
			IL_027b: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0280: ldloc.s 18
			IL_0282: ldfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::'CS$<>8__locals1'
			IL_0287: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_0'::'<Delete_OnHandler>b__1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_028d: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_0292: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_0297: stloc.s 20
			IL_0299: ldloc.s 20
			IL_029b: call int32 [System.Core]System.Linq.Enumerable::Count<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
			IL_02a0: ldloc.s 18
			IL_02a2: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::nAddIndex
			IL_02a7: cgt
			IL_02a9: stloc.s 21
			IL_02ab: ldloc.s 21
			IL_02ad: brfalse.s IL_02d0

			IL_02af: nop
			IL_02b0: ldarg.0
			IL_02b1: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_02b6: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.VarGroupItem::get_BindItems()
			IL_02bb: ldloc.s 20
			IL_02bd: ldloc.s 18
			IL_02bf: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::nAddIndex
			IL_02c4: call !!0 [System.Core]System.Linq.Enumerable::ElementAt<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, int32)
			IL_02c9: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Add(!0)
			IL_02ce: nop
			IL_02cf: nop

			IL_02d0: nop
			IL_02d1: br.s IL_030f

			IL_02d3: nop
			IL_02d4: ldarg.0
			IL_02d5: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_02da: ldloc.s 18
			IL_02dc: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass109_1'::'<Delete_OnHandler>b__2'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_02e2: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_02e7: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_02ec: stloc.s 22
			IL_02ee: ldloc.s 22
			IL_02f0: ldnull
			IL_02f1: cgt.un
			IL_02f3: stloc.s 23
			IL_02f5: ldloc.s 23
			IL_02f7: brfalse.s IL_030e

			IL_02f9: nop
			IL_02fa: ldarg.0
			IL_02fb: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
			IL_0300: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.VarGroupItem::get_BindItems()
			IL_0305: ldloc.s 22
			IL_0307: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::Add(!0)
			IL_030c: nop
			IL_030d: nop

			IL_030e: nop

			IL_030f: nop

			IL_0310: ldarg.0
			IL_0311: ldc.i4.1
			IL_0312: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
			IL_0317: nop
			IL_0318: ldarg.0
			IL_0319: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
			IL_031e: nop
			IL_031f: nop
			IL_0320: leave.s IL_032f
		} // end .try
		finally
		{
			IL_0322: ldloc.s 7
			IL_0324: brfalse.s IL_032e

			IL_0326: ldloc.s 6
			IL_0328: call void [mscorlib]System.Threading.Monitor::Exit(object)
			IL_032d: nop

			IL_032e: endfinally
		} // end handler

		IL_032f: ret
	} // end of method GlobalVariableDialog::Delete_OnHandler

	.method private hidebysig 
		instance void SearchItem_OnClick (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x2885c
		// Header size: 12
		// Code size: 22 (0x16)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText
		IL_0007: callvirt instance string [PresentationFramework]System.Windows.Controls.TextBox::get_Text()
		IL_000c: stloc.0
		IL_000d: ldarg.0
		IL_000e: ldloc.0
		IL_000f: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SeearchItem(string)
		IL_0014: nop
		IL_0015: ret
	} // end of method GlobalVariableDialog::SearchItem_OnClick

	.method private hidebysig 
		instance void AddItem_OnClick (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x28880
		// Header size: 12
		// Code size: 368 (0x170)
		.maxstack 5
		.locals init (
			[0] int32,
			[1] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[2] bool,
			[3] bool,
			[4] int32,
			[5] bool,
			[6] bool,
			[7] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[8] bool,
			[9] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_0007: ldnull
		IL_0008: ceq
		IL_000a: stloc.2
		IL_000b: ldloc.2
		IL_000c: brfalse.s IL_0014

		IL_000e: nop
		IL_000f: br IL_016f

		IL_0014: ldarg.0
		IL_0015: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_001a: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
		IL_001f: stloc.0
		IL_0020: ldloc.0
		IL_0021: ldc.i4 2048
		IL_0026: clt
		IL_0028: ldc.i4.0
		IL_0029: ceq
		IL_002b: stloc.3
		IL_002c: ldloc.3
		IL_002d: brfalse.s IL_0069

		IL_002f: nop
		IL_0030: ldstr "添加"
		IL_0035: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_003a: ldstr "超限"
		IL_003f: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_0044: ldstr ":"
		IL_0049: ldc.i4 2048
		IL_004e: stloc.s 4
		IL_0050: ldloca.s 4
		IL_0052: call instance string [mscorlib]System.Int32::ToString()
		IL_0057: call string [mscorlib]System.String::Concat(string, string, string, string)
		IL_005c: ldc.i4.0
		IL_005d: ldc.i4.2
		IL_005e: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageButtons, valuetype [VM.Controls]VM.Controls.MessageIcons)
		IL_0063: pop
		IL_0064: br IL_016f

		IL_0069: ldarg.0
		IL_006a: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_006f: stloc.s 5
		IL_0071: ldloc.s 5
		IL_0073: brfalse.s IL_0088

		IL_0075: nop
		IL_0076: ldarg.0
		IL_0077: ldfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText
		IL_007c: ldstr ""
		IL_0081: callvirt instance void [PresentationFramework]System.Windows.Controls.TextBox::set_Text(string)
		IL_0086: nop
		IL_0087: nop

		IL_0088: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableItem::.ctor()
		IL_008d: dup
		IL_008e: ldstr "%"
		IL_0093: ldarg.0
		IL_0094: ldnull
		IL_0095: call instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::GetDefaultName(class [mscorlib]System.Text.StringBuilder)
		IL_009a: ldstr "%"
		IL_009f: call string [mscorlib]System.String::Concat(string, string, string)
		IL_00a4: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Name(string)
		IL_00a9: nop
		IL_00aa: dup
		IL_00ab: ldstr "int"
		IL_00b0: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Type(string)
		IL_00b5: nop
		IL_00b6: dup
		IL_00b7: ldstr "0"
		IL_00bc: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Value(string)
		IL_00c1: nop
		IL_00c2: dup
		IL_00c3: ldloc.0
		IL_00c4: ldc.i4.1
		IL_00c5: add
		IL_00c6: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
		IL_00cb: nop
		IL_00cc: dup
		IL_00cd: ldarg.0
		IL_00ce: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_00d3: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
		IL_00d8: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_VarGroupInfo(string)
		IL_00dd: nop
		IL_00de: stloc.1
		IL_00df: ldarg.0
		IL_00e0: ldloc.1
		IL_00e1: ldc.i4.1
		IL_00e2: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::AddNewItem(class VMControls.WPF.GlobalUI.GlobalVariableItem, bool)
		IL_00e7: nop
		IL_00e8: ldarg.0
		IL_00e9: ldc.i4.2
		IL_00ea: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdatePageData(valuetype VMControls.WPF.GlobalUI.GlobalVariableDialog/PageShowType)
		IL_00ef: nop
		IL_00f0: ldarg.0
		IL_00f1: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_00f6: callvirt instance int32 VMControls.WPF.GlobalUI.VarGroupItem::get_Count()
		IL_00fb: ldc.i4.0
		IL_00fc: cgt
		IL_00fe: stloc.s 6
		IL_0100: ldloc.s 6
		IL_0102: brfalse.s IL_016f

		IL_0104: nop
		IL_0105: ldarg.0
		IL_0106: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_010b: callvirt instance class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.VarGroupItem::GetItem()
		IL_0110: stloc.s 7
		IL_0112: ldloc.s 7
		IL_0114: ldnull
		IL_0115: cgt.un
		IL_0117: stloc.s 8
		IL_0119: ldloc.s 8
		IL_011b: brfalse.s IL_016e

		IL_011d: nop
		IL_011e: ldloc.s 7
		IL_0120: ldc.i4.1
		IL_0121: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_0126: nop
		IL_0127: ldarg.0
		IL_0128: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_012d: brfalse.s IL_0148

		IL_012f: ldarg.0
		IL_0130: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0135: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_013a: ldloc.s 7
		IL_013c: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0141: call bool [mscorlib]System.String::op_Inequality(string, string)
		IL_0146: br.s IL_0149

		IL_0148: ldc.i4.0

		IL_0149: stloc.s 9
		IL_014b: ldloc.s 9
		IL_014d: brfalse.s IL_015e

		IL_014f: nop
		IL_0150: ldarg.0
		IL_0151: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0156: ldc.i4.0
		IL_0157: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_015c: nop
		IL_015d: nop

		IL_015e: ldarg.0
		IL_015f: ldloc.s 7
		IL_0161: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0166: ldarg.0
		IL_0167: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_016c: nop
		IL_016d: nop

		IL_016e: nop

		IL_016f: ret
	} // end of method GlobalVariableDialog::AddItem_OnClick

	.method private hidebysig 
		instance void LightSave_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x289fc
		// Header size: 12
		// Code size: 202 (0xca)
		.maxstack 4
		.locals init (
			[0] bool,
			[1] bool,
			[2] class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ItemsTotalCount()
		IL_0007: ldc.i4.0
		IL_0008: cgt
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: stloc.0
		IL_000e: ldloc.0
		IL_000f: brfalse.s IL_0017

		IL_0011: nop
		IL_0012: br IL_00c9

		IL_0017: call class [VM.Core]VM.Core.VmSolution [VM.Core]VM.Core.VmSolution::get_Instance()
		IL_001c: callvirt instance string [VM.Core]VM.Core.VmSolution::get_SolutionPath()
		IL_0021: call bool [mscorlib]System.String::IsNullOrEmpty(string)
		IL_0026: stloc.1
		IL_0027: ldloc.1
		IL_0028: brfalse.s IL_0055

		IL_002a: nop
		IL_002b: ldarg.0
		IL_002c: call instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DisplayName()
		IL_0031: ldstr " "
		IL_0036: ldstr "Lang_SaveSuccess"
		IL_003b: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_0040: call string [mscorlib]System.String::Concat(string, string, string)
		IL_0045: ldc.i4.4
		IL_0046: ldc.i4.4
		IL_0047: ldc.i4 1000
		IL_004c: call void [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, valuetype [VM.Controls]VM.Controls.MessageButtons, uint32)
		IL_0051: nop
		IL_0052: nop
		IL_0053: br.s IL_00c9

		IL_0055: nop
		.try
		{
			IL_0056: nop
			IL_0057: call class [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKManager [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKManager::get_serverSDKManager()
			IL_005c: ldfld class [VM.PlatformSDKCS]VM.PlatformSDKCS.VmInterfaceManager [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKManager::mVmInterfaceManager
			IL_0061: callvirt instance void [VM.PlatformSDKCS]VM.PlatformSDKCS.VmInterfaceManager::LightSaveSolutionEx()
			IL_0066: nop
			IL_0067: ldarg.0
			IL_0068: call instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DisplayName()
			IL_006d: ldstr " "
			IL_0072: ldstr "Lang_SaveSuccess"
			IL_0077: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_007c: call string [mscorlib]System.String::Concat(string, string, string)
			IL_0081: ldc.i4.4
			IL_0082: ldc.i4.4
			IL_0083: ldc.i4 1000
			IL_0088: call void [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, valuetype [VM.Controls]VM.Controls.MessageButtons, uint32)
			IL_008d: nop
			IL_008e: nop
			IL_008f: leave.s IL_00c8
		} // end .try
		catch [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException
		{
			IL_0091: stloc.2
			IL_0092: nop
			IL_0093: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_0098: ldstr "全局变量保存文件数据失败，错误码为:"
			IL_009d: ldloc.2
			IL_009e: callvirt instance int32 [VM.PlatformSDKCS]VM.PlatformSDKCS.VmException::get_errorCode()
			IL_00a3: box [mscorlib]System.Int32
			IL_00a8: call string [mscorlib]System.String::Concat(object, object)
			IL_00ad: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Info(object)
			IL_00b2: nop
			IL_00b3: ldstr "保存文件数据失败"
			IL_00b8: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00bd: ldc.i4.3
			IL_00be: ldc.i4.0
			IL_00bf: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_00c4: pop
			IL_00c5: nop
			IL_00c6: leave.s IL_00c8
		} // end handler

		IL_00c8: nop

		IL_00c9: ret
	} // end of method GlobalVariableDialog::LightSave_Click

	.method private hidebysig 
		instance void SearchText_TextChanged (
			object sender,
			class [PresentationFramework]System.Windows.Controls.TextChangedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x28ae4
		// Header size: 12
		// Code size: 22 (0x16)
		.maxstack 2
		.locals init (
			[0] string
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText
		IL_0007: callvirt instance string [PresentationFramework]System.Windows.Controls.TextBox::get_Text()
		IL_000c: stloc.0
		IL_000d: ldarg.0
		IL_000e: ldloc.0
		IL_000f: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SeearchItem(string)
		IL_0014: nop
		IL_0015: ret
	} // end of method GlobalVariableDialog::SearchText_TextChanged

	.method private hidebysig 
		instance void GotFocus_OnHandler (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x28b08
		// Header size: 12
		// Code size: 111 (0x6f)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[1] bool,
			[2] bool
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: castclass [PresentationFramework]System.Windows.Controls.TextBox
		IL_0007: callvirt instance object [PresentationFramework]System.Windows.FrameworkElement::get_DataContext()
		IL_000c: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ldnull
		IL_0014: cgt.un
		IL_0016: stloc.1
		IL_0017: ldloc.1
		IL_0018: brfalse.s IL_006e

		IL_001a: nop
		IL_001b: ldloc.0
		IL_001c: ldc.i4.1
		IL_001d: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Capture(bool)
		IL_0022: nop
		IL_0023: ldloc.0
		IL_0024: ldc.i4.1
		IL_0025: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_002a: nop
		IL_002b: ldarg.0
		IL_002c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0031: brfalse.s IL_004b

		IL_0033: ldarg.0
		IL_0034: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0039: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_003e: ldloc.0
		IL_003f: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0044: call bool [mscorlib]System.String::op_Inequality(string, string)
		IL_0049: br.s IL_004c

		IL_004b: ldc.i4.0

		IL_004c: stloc.2
		IL_004d: ldloc.2
		IL_004e: brfalse.s IL_005f

		IL_0050: nop
		IL_0051: ldarg.0
		IL_0052: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0057: ldc.i4.0
		IL_0058: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_005d: nop
		IL_005e: nop

		IL_005f: ldarg.0
		IL_0060: ldloc.0
		IL_0061: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0066: ldarg.0
		IL_0067: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_006c: nop
		IL_006d: nop

		IL_006e: ret
	} // end of method GlobalVariableDialog::GotFocus_OnHandler

	.method private hidebysig 
		instance void LostFocus_OnHandler (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x28b84
		// Header size: 12
		// Code size: 35 (0x23)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[1] bool
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: castclass [PresentationFramework]System.Windows.Controls.TextBox
		IL_0007: callvirt instance object [PresentationFramework]System.Windows.FrameworkElement::get_DataContext()
		IL_000c: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ldnull
		IL_0014: cgt.un
		IL_0016: stloc.1
		IL_0017: ldloc.1
		IL_0018: brfalse.s IL_0022

		IL_001a: ldloc.0
		IL_001b: ldc.i4.0
		IL_001c: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Capture(bool)
		IL_0021: nop

		IL_0022: ret
	} // end of method GlobalVariableDialog::LostFocus_OnHandler

	.method private hidebysig 
		instance void TextBox_PreviewKeyDown (
			object sender,
			class [PresentationCore]System.Windows.Input.KeyEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x28bb4
		// Header size: 12
		// Code size: 93 (0x5d)
		.maxstack 2
		.locals init (
			[0] class [PresentationFramework]System.Windows.Controls.TextBox,
			[1] bool,
			[2] bool,
			[3] bool
		)

		IL_0000: nop
		IL_0001: ldarg.2
		IL_0002: callvirt instance valuetype [WindowsBase]System.Windows.Input.Key [PresentationCore]System.Windows.Input.KeyEventArgs::get_Key()
		IL_0007: ldc.i4.s 18
		IL_0009: bne.un.s IL_001b

		IL_000b: ldarg.1
		IL_000c: brfalse.s IL_001b

		IL_000e: ldarg.1
		IL_000f: isinst [PresentationFramework]System.Windows.Controls.TextBox
		IL_0014: dup
		IL_0015: stloc.0
		IL_0016: ldnull
		IL_0017: cgt.un
		IL_0019: br.s IL_001c

		IL_001b: ldc.i4.0

		IL_001c: stloc.1
		IL_001d: ldloc.1
		IL_001e: brfalse.s IL_0041

		IL_0020: nop
		IL_0021: ldloc.0
		IL_0022: callvirt instance string [PresentationFramework]System.Windows.FrameworkElement::get_Name()
		IL_0027: ldstr "itemNameTxt"
		IL_002c: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0031: stloc.2
		IL_0032: ldloc.2
		IL_0033: brfalse.s IL_0040

		IL_0035: nop
		IL_0036: ldarg.2
		IL_0037: ldc.i4.1
		IL_0038: callvirt instance void [PresentationCore]System.Windows.RoutedEventArgs::set_Handled(bool)
		IL_003d: nop
		IL_003e: br.s IL_005c

		IL_0040: nop

		IL_0041: ldarg.2
		IL_0042: callvirt instance valuetype [WindowsBase]System.Windows.Input.Key [PresentationCore]System.Windows.Input.KeyEventArgs::get_Key()
		IL_0047: ldc.i4.6
		IL_0048: ceq
		IL_004a: stloc.3
		IL_004b: ldloc.3
		IL_004c: brfalse.s IL_005c

		IL_004e: nop
		IL_004f: ldarg.0
		IL_0050: ldfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::txtFocus
		IL_0055: callvirt instance bool [PresentationCore]System.Windows.UIElement::Focus()
		IL_005a: pop
		IL_005b: nop

		IL_005c: ret
	} // end of method GlobalVariableDialog::TextBox_PreviewKeyDown

	.method private hidebysig 
		instance void SetHeadIndex (
			int32 currentIndex
		) cil managed 
	{
		// Method begins at RVA 0x28c20
		// Header size: 12
		// Code size: 641 (0x281)
		.maxstack 7
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0',
			[1] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[2] class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[3] int32,
			[4] int32,
			[5] int32,
			[6] int32,
			[7] bool,
			[8] int32,
			[9] bool,
			[10] bool,
			[11] bool,
			[12] int32,
			[13] bool,
			[14] bool,
			[15] bool,
			[16] bool
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.1
		IL_0008: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::currentIndex
		IL_000d: ldloc.0
		IL_000e: ldarg.0
		IL_000f: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::'<>4__this'
		IL_0014: nop
		IL_0015: ldarg.0
		IL_0016: ldc.i4.1
		IL_0017: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsMoveItem
		IL_001c: ldarg.0
		IL_001d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_0022: ldloc.0
		IL_0023: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::'<SetHeadIndex>b__0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0029: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_002e: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_0033: stloc.1
		IL_0034: ldloc.1
		IL_0035: ldnull
		IL_0036: ceq
		IL_0038: stloc.s 7
		IL_003a: ldloc.s 7
		IL_003c: brfalse.s IL_0044

		IL_003e: nop
		IL_003f: br IL_0280

		IL_0044: ldarg.0
		IL_0045: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_004a: ldloc.0
		IL_004b: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::'<SetHeadIndex>b__1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0051: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_0056: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_005b: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__117_2'
		IL_0060: dup
		IL_0061: brtrue.s IL_007a

		IL_0063: pop
		IL_0064: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_0069: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<SetHeadIndex>b__117_2'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_006f: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>::.ctor(object, native int)
		IL_0074: dup
		IL_0075: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__117_2'

		IL_007a: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_007f: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_0084: stloc.2
		IL_0085: ldloc.2
		IL_0086: ldc.i4.0
		IL_0087: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
		IL_008c: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_0091: stloc.3
		IL_0092: ldc.i4.0
		IL_0093: stloc.s 8
		IL_0095: br.s IL_0100
		// loop start (head: IL_0100)
			IL_0097: nop
			IL_0098: ldloc.3
			IL_0099: ldarg.0
			IL_009a: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_009f: ldloc.s 8
			IL_00a1: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_00a6: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_00ab: bgt.s IL_00c9

			IL_00ad: ldarg.0
			IL_00ae: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_00b3: ldloc.s 8
			IL_00b5: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_00ba: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_00bf: ldloc.0
			IL_00c0: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::currentIndex
			IL_00c5: clt
			IL_00c7: br.s IL_00ca

			IL_00c9: ldc.i4.0

			IL_00ca: stloc.s 9
			IL_00cc: ldloc.s 9
			IL_00ce: brfalse.s IL_00f9

			IL_00d0: nop
			IL_00d1: ldarg.0
			IL_00d2: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_00d7: ldloc.s 8
			IL_00d9: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_00de: ldarg.0
			IL_00df: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_00e4: ldloc.s 8
			IL_00e6: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_00eb: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_00f0: ldc.i4.1
			IL_00f1: add
			IL_00f2: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
			IL_00f7: nop
			IL_00f8: nop

			IL_00f9: nop
			IL_00fa: ldloc.s 8
			IL_00fc: ldc.i4.1
			IL_00fd: add
			IL_00fe: stloc.s 8

			IL_0100: ldloc.s 8
			IL_0102: ldarg.0
			IL_0103: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0108: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_010d: clt
			IL_010f: stloc.s 10
			IL_0111: ldloc.s 10
			IL_0113: brtrue.s IL_0097
		// end loop

		IL_0115: ldloc.1
		IL_0116: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_011b: stloc.s 4
		IL_011d: ldloc.1
		IL_011e: ldloc.3
		IL_011f: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
		IL_0124: nop
		IL_0125: ldarg.0
		IL_0126: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_012b: ldloc.1
		IL_012c: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0131: ldloc.1
		IL_0132: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_0137: ldc.i4.1
		IL_0138: ldloc.3
		IL_0139: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyIndex(string, int32, bool, int32)
		IL_013e: nop
		IL_013f: ldarg.0
		IL_0140: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_0145: stloc.s 5
		IL_0147: ldc.i4.m1
		IL_0148: stloc.s 6
		IL_014a: ldarg.0
		IL_014b: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_0150: stloc.s 11
		IL_0152: ldloc.s 11
		IL_0154: brfalse IL_01fb

		IL_0159: nop
		IL_015a: ldarg.0
		IL_015b: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_0160: ldloc.0
		IL_0161: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::'<SetHeadIndex>b__3'(int32)
		IL_0167: newobj instance void class [mscorlib]System.Predicate`1<int32>::.ctor(object, native int)
		IL_016c: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::FindIndex(class [mscorlib]System.Predicate`1<!0>)
		IL_0171: stloc.s 6
		IL_0173: ldc.i4.0
		IL_0174: stloc.s 12
		IL_0176: br.s IL_019d
		// loop start (head: IL_019d)
			IL_0178: nop
			IL_0179: ldarg.0
			IL_017a: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_017f: ldloc.s 12
			IL_0181: ldarg.0
			IL_0182: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
			IL_0187: ldloc.s 12
			IL_0189: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Item(int32)
			IL_018e: ldc.i4.1
			IL_018f: add
			IL_0190: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::set_Item(int32, !0)
			IL_0195: nop
			IL_0196: nop
			IL_0197: ldloc.s 12
			IL_0199: ldc.i4.1
			IL_019a: add
			IL_019b: stloc.s 12

			IL_019d: ldloc.s 12
			IL_019f: ldloc.s 6
			IL_01a1: clt
			IL_01a3: stloc.s 13
			IL_01a5: ldloc.s 13
			IL_01a7: brtrue.s IL_0178
		// end loop

		IL_01a9: ldloc.s 6
		IL_01ab: ldc.i4.m1
		IL_01ac: cgt
		IL_01ae: stloc.s 14
		IL_01b0: ldloc.s 14
		IL_01b2: brfalse.s IL_01c5

		IL_01b4: nop
		IL_01b5: ldarg.0
		IL_01b6: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_01bb: ldloc.s 6
		IL_01bd: ldc.i4.1
		IL_01be: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::set_Item(int32, !0)
		IL_01c3: nop
		IL_01c4: nop

		IL_01c5: ldarg.0
		IL_01c6: ldarg.0
		IL_01c7: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_01cc: ldsfld class [mscorlib]System.Func`2<int32, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__117_4'
		IL_01d1: dup
		IL_01d2: brtrue.s IL_01eb

		IL_01d4: pop
		IL_01d5: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_01da: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<SetHeadIndex>b__117_4'(int32)
		IL_01e0: newobj instance void class [mscorlib]System.Func`2<int32, int32>::.ctor(object, native int)
		IL_01e5: dup
		IL_01e6: stsfld class [mscorlib]System.Func`2<int32, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__117_4'

		IL_01eb: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<int32, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_01f0: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_01f5: stfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_01fa: nop

		IL_01fb: ldarg.0
		IL_01fc: ldc.i4.1
		IL_01fd: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
		IL_0202: nop
		IL_0203: ldloc.s 5
		IL_0205: ldarg.0
		IL_0206: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_020b: ceq
		IL_020d: stloc.s 15
		IL_020f: ldloc.s 15
		IL_0211: brfalse.s IL_0238

		IL_0213: nop
		IL_0214: ldarg.0
		IL_0215: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_021a: ldc.i4.0
		IL_021b: ceq
		IL_021d: stloc.s 16
		IL_021f: ldloc.s 16
		IL_0221: brfalse.s IL_022e

		IL_0223: nop
		IL_0224: ldarg.0
		IL_0225: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetBindData()
		IL_022a: nop
		IL_022b: nop
		IL_022c: br.s IL_0237

		IL_022e: nop
		IL_022f: ldarg.0
		IL_0230: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetBindSearchData()
		IL_0235: nop
		IL_0236: nop

		IL_0237: nop

		IL_0238: ldstr "SetHeadIndex varName:{0} sourceindex:{1},index:{2}"
		IL_023d: ldloc.1
		IL_023e: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0243: ldloc.s 4
		IL_0245: box [mscorlib]System.Int32
		IL_024a: ldloc.3
		IL_024b: box [mscorlib]System.Int32
		IL_0250: call string [mscorlib]System.String::Format(string, object, object, object)
		IL_0255: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
		IL_025a: nop
		IL_025b: ldarg.0
		IL_025c: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_0261: ldloc.0
		IL_0262: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass117_0'::currentIndex
		IL_0267: ldloc.3
		IL_0268: ldc.i4.0
		IL_0269: ldc.i4.0
		IL_026a: ldc.i4.1
		IL_026b: ldc.i4.0
		IL_026c: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ChangeToSendReceiveData(int32, int32, bool, bool, bool, bool)
		IL_0271: nop
		IL_0272: ldarg.0
		IL_0273: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_0278: nop
		IL_0279: ldarg.0
		IL_027a: ldc.i4.0
		IL_027b: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsMoveItem

		IL_0280: ret
	} // end of method GlobalVariableDialog::SetHeadIndex

	.method private hidebysig 
		instance void SwapIndex (
			int32 souceIndex,
			int32 destIndex,
			int32 destGroupIndex
		) cil managed 
	{
		// Method begins at RVA 0x28eb0
		// Header size: 12
		// Code size: 682 (0x2aa)
		.maxstack 7
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0',
			[1] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[2] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[3] int32,
			[4] int32,
			[5] int32,
			[6] int32,
			[7] bool,
			[8] bool,
			[9] bool,
			[10] bool,
			[11] int32,
			[12] bool,
			[13] bool,
			[14] bool,
			[15] bool
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.1
		IL_0008: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_000d: ldloc.0
		IL_000e: ldarg.2
		IL_000f: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_0014: ldloc.0
		IL_0015: ldarg.0
		IL_0016: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<>4__this'
		IL_001b: nop
		IL_001c: ldloc.0
		IL_001d: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_0022: ldc.i4.0
		IL_0023: blt.s IL_0030

		IL_0025: ldloc.0
		IL_0026: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_002b: ldc.i4.0
		IL_002c: clt
		IL_002e: br.s IL_0031

		IL_0030: ldc.i4.1

		IL_0031: stloc.s 7
		IL_0033: ldloc.s 7
		IL_0035: brfalse.s IL_003d

		IL_0037: nop
		IL_0038: br IL_02a9

		IL_003d: ldarg.0
		IL_003e: ldc.i4.1
		IL_003f: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsMoveItem
		IL_0044: ldarg.0
		IL_0045: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_004a: ldloc.0
		IL_004b: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<SwapIndex>b__0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0051: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_0056: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_005b: stloc.1
		IL_005c: ldloc.1
		IL_005d: ldnull
		IL_005e: ceq
		IL_0060: stloc.s 8
		IL_0062: ldloc.s 8
		IL_0064: brfalse.s IL_006c

		IL_0066: nop
		IL_0067: br IL_02a9

		IL_006c: ldarg.0
		IL_006d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_0072: ldloc.0
		IL_0073: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<SwapIndex>b__1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0079: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_007e: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_0083: stloc.2
		IL_0084: ldloc.2
		IL_0085: ldnull
		IL_0086: ceq
		IL_0088: stloc.s 9
		IL_008a: ldloc.s 9
		IL_008c: brfalse.s IL_0094

		IL_008e: nop
		IL_008f: br IL_02a9

		IL_0094: ldarg.0
		IL_0095: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_009a: ldloc.1
		IL_009b: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_00a0: ldloc.0
		IL_00a1: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_00a6: ldc.i4.0
		IL_00a7: ldc.i4.0
		IL_00a8: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyIndex(string, int32, bool, int32)
		IL_00ad: nop
		IL_00ae: ldarg.0
		IL_00af: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_00b4: ldloc.2
		IL_00b5: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_00ba: ldloc.0
		IL_00bb: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_00c0: ldc.i4.0
		IL_00c1: ldc.i4.0
		IL_00c2: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ModifyIndex(string, int32, bool, int32)
		IL_00c7: nop
		IL_00c8: ldloc.2
		IL_00c9: ldloc.0
		IL_00ca: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_00cf: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
		IL_00d4: nop
		IL_00d5: ldloc.1
		IL_00d6: ldloc.0
		IL_00d7: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_00dc: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_Index(int32)
		IL_00e1: nop
		IL_00e2: ldarg.0
		IL_00e3: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_00e8: stloc.3
		IL_00e9: ldarg.0
		IL_00ea: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_00ef: brtrue.s IL_010a

		IL_00f1: ldarg.0
		IL_00f2: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_00f7: ldloc.0
		IL_00f8: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<SwapIndex>b__2'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_00fe: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_0103: call int32 [System.Core]System.Linq.Enumerable::Count<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_0108: br.s IL_0115

		IL_010a: ldarg.0
		IL_010b: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_0110: call int32 [System.Core]System.Linq.Enumerable::Count<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)

		IL_0115: stloc.s 4
		IL_0117: ldloc.s 4
		IL_0119: ldarg.0
		IL_011a: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_011f: div
		IL_0120: ldc.i4.1
		IL_0121: add
		IL_0122: stloc.s 5
		IL_0124: ldc.i4.m1
		IL_0125: stloc.s 6
		IL_0127: ldarg.0
		IL_0128: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_012d: stloc.s 10
		IL_012f: ldloc.s 10
		IL_0131: brfalse IL_01e1

		IL_0136: nop
		IL_0137: ldarg.0
		IL_0138: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_013d: ldloc.0
		IL_013e: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<SwapIndex>b__3'(int32)
		IL_0144: newobj instance void class [mscorlib]System.Predicate`1<int32>::.ctor(object, native int)
		IL_0149: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::FindIndex(class [mscorlib]System.Predicate`1<!0>)
		IL_014e: stloc.s 6
		IL_0150: ldarg.0
		IL_0151: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_0156: ldloc.0
		IL_0157: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::'<SwapIndex>b__4'(int32)
		IL_015d: newobj instance void class [mscorlib]System.Predicate`1<int32>::.ctor(object, native int)
		IL_0162: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::FindIndex(class [mscorlib]System.Predicate`1<!0>)
		IL_0167: stloc.s 11
		IL_0169: ldloc.s 6
		IL_016b: ldc.i4.m1
		IL_016c: cgt
		IL_016e: stloc.s 12
		IL_0170: ldloc.s 12
		IL_0172: brfalse.s IL_018a

		IL_0174: nop
		IL_0175: ldarg.0
		IL_0176: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_017b: ldloc.s 6
		IL_017d: ldloc.0
		IL_017e: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_0183: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::set_Item(int32, !0)
		IL_0188: nop
		IL_0189: nop

		IL_018a: ldloc.s 11
		IL_018c: ldc.i4.m1
		IL_018d: cgt
		IL_018f: stloc.s 13
		IL_0191: ldloc.s 13
		IL_0193: brfalse.s IL_01ab

		IL_0195: nop
		IL_0196: ldarg.0
		IL_0197: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_019c: ldloc.s 11
		IL_019e: ldloc.0
		IL_019f: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_01a4: callvirt instance void class [mscorlib]System.Collections.Generic.List`1<int32>::set_Item(int32, !0)
		IL_01a9: nop
		IL_01aa: nop

		IL_01ab: ldarg.0
		IL_01ac: ldarg.0
		IL_01ad: ldfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_01b2: ldsfld class [mscorlib]System.Func`2<int32, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__118_5'
		IL_01b7: dup
		IL_01b8: brtrue.s IL_01d1

		IL_01ba: pop
		IL_01bb: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_01c0: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<SwapIndex>b__118_5'(int32)
		IL_01c6: newobj instance void class [mscorlib]System.Func`2<int32, int32>::.ctor(object, native int)
		IL_01cb: dup
		IL_01cc: stsfld class [mscorlib]System.Func`2<int32, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__118_5'

		IL_01d1: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<int32, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_01d6: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_01db: stfld class [mscorlib]System.Collections.Generic.List`1<int32> VMControls.WPF.GlobalUI.GlobalVariableDialog::iSearchCount
		IL_01e0: nop

		IL_01e1: ldarg.0
		IL_01e2: ldarg.0
		IL_01e3: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_01e8: brtrue.s IL_01f6

		IL_01ea: ldarg.3
		IL_01eb: ldarg.0
		IL_01ec: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_01f1: div
		IL_01f2: ldc.i4.1
		IL_01f3: add
		IL_01f4: br.s IL_0201

		IL_01f6: ldloc.s 6
		IL_01f8: ldarg.0
		IL_01f9: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		IL_01fe: div
		IL_01ff: ldc.i4.1
		IL_0200: add

		IL_0201: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
		IL_0206: nop
		IL_0207: ldloc.3
		IL_0208: ldarg.0
		IL_0209: call instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		IL_020e: ceq
		IL_0210: stloc.s 14
		IL_0212: ldloc.s 14
		IL_0214: brfalse.s IL_023b

		IL_0216: nop
		IL_0217: ldarg.0
		IL_0218: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsSearchState
		IL_021d: ldc.i4.0
		IL_021e: ceq
		IL_0220: stloc.s 15
		IL_0222: ldloc.s 15
		IL_0224: brfalse.s IL_0231

		IL_0226: nop
		IL_0227: ldarg.0
		IL_0228: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetBindData()
		IL_022d: nop
		IL_022e: nop
		IL_022f: br.s IL_023a

		IL_0231: nop
		IL_0232: ldarg.0
		IL_0233: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetBindSearchData()
		IL_0238: nop
		IL_0239: nop

		IL_023a: nop

		IL_023b: ldstr "SwapIndex srcVarName:{0} sourceindex:{1},dstVarName:{2} dstindex:{3}"
		IL_0240: ldc.i4.4
		IL_0241: newarr [mscorlib]System.Object
		IL_0246: dup
		IL_0247: ldc.i4.0
		IL_0248: ldloc.1
		IL_0249: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_024e: stelem.ref
		IL_024f: dup
		IL_0250: ldc.i4.1
		IL_0251: ldloc.0
		IL_0252: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_0257: box [mscorlib]System.Int32
		IL_025c: stelem.ref
		IL_025d: dup
		IL_025e: ldc.i4.2
		IL_025f: ldloc.2
		IL_0260: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0265: stelem.ref
		IL_0266: dup
		IL_0267: ldc.i4.3
		IL_0268: ldloc.0
		IL_0269: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_026e: box [mscorlib]System.Int32
		IL_0273: stelem.ref
		IL_0274: call string [mscorlib]System.String::Format(string, object[])
		IL_0279: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
		IL_027e: nop
		IL_027f: ldarg.0
		IL_0280: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_0285: ldloc.0
		IL_0286: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::souceIndex
		IL_028b: ldloc.0
		IL_028c: ldfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass118_0'::destIndex
		IL_0291: ldc.i4.0
		IL_0292: ldc.i4.0
		IL_0293: ldc.i4.0
		IL_0294: ldc.i4.0
		IL_0295: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ChangeToSendReceiveData(int32, int32, bool, bool, bool, bool)
		IL_029a: nop
		IL_029b: ldarg.0
		IL_029c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_02a1: nop
		IL_02a2: ldarg.0
		IL_02a3: ldc.i4.0
		IL_02a4: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::IsMoveItem

		IL_02a9: ret
	} // end of method GlobalVariableDialog::SwapIndex

	.method private hidebysig 
		instance void GridGlob_MouseDown (
			object sender,
			class [PresentationCore]System.Windows.Input.MouseButtonEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29168
		// Header size: 12
		// Code size: 103 (0x67)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[1] bool,
			[2] bool
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: isinst [PresentationFramework]System.Windows.FrameworkElement
		IL_0007: callvirt instance object [PresentationFramework]System.Windows.FrameworkElement::get_DataContext()
		IL_000c: isinst VMControls.WPF.GlobalUI.GlobalVariableItem
		IL_0011: stloc.0
		IL_0012: ldloc.0
		IL_0013: ldnull
		IL_0014: cgt.un
		IL_0016: stloc.1
		IL_0017: ldloc.1
		IL_0018: brfalse.s IL_0066

		IL_001a: nop
		IL_001b: ldloc.0
		IL_001c: ldc.i4.1
		IL_001d: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_0022: nop
		IL_0023: ldarg.0
		IL_0024: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0029: brfalse.s IL_0043

		IL_002b: ldarg.0
		IL_002c: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0031: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_0036: ldloc.0
		IL_0037: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_Name()
		IL_003c: call bool [mscorlib]System.String::op_Inequality(string, string)
		IL_0041: br.s IL_0044

		IL_0043: ldc.i4.0

		IL_0044: stloc.2
		IL_0045: ldloc.2
		IL_0046: brfalse.s IL_0057

		IL_0048: nop
		IL_0049: ldarg.0
		IL_004a: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_004f: ldc.i4.0
		IL_0050: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_IsSelected(bool)
		IL_0055: nop
		IL_0056: nop

		IL_0057: ldarg.0
		IL_0058: ldloc.0
		IL_0059: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_005e: ldarg.0
		IL_005f: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::UpdateBtnEnable()
		IL_0064: nop
		IL_0065: nop

		IL_0066: ret
	} // end of method GlobalVariableDialog::GridGlob_MouseDown

	.method private hidebysig 
		instance void UpdateBtnEnable () cil managed 
	{
		// Method begins at RVA 0x291dc
		// Header size: 12
		// Code size: 301 (0x12d)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[1] int32,
			[2] bool,
			[3] bool,
			[4] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_0007: ldarg.0
		IL_0008: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::'<UpdateBtnEnable>b__120_0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_000e: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_0013: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_0018: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__120_1'
		IL_001d: dup
		IL_001e: brtrue.s IL_0037

		IL_0020: pop
		IL_0021: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_0026: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<UpdateBtnEnable>b__120_1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_002c: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>::.ctor(object, native int)
		IL_0031: dup
		IL_0032: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__120_1'

		IL_0037: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_003c: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_0041: stloc.0
		IL_0042: ldloc.0
		IL_0043: ldarg.0
		IL_0044: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0049: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::IndexOf(!0)
		IL_004e: stloc.1
		IL_004f: ldarg.0
		IL_0050: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0055: brfalse.s IL_0062

		IL_0057: ldloc.0
		IL_0058: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
		IL_005d: ldc.i4.1
		IL_005e: ceq
		IL_0060: br.s IL_0063

		IL_0062: ldc.i4.1

		IL_0063: stloc.2
		IL_0064: ldloc.2
		IL_0065: brfalse.s IL_0094

		IL_0067: nop
		IL_0068: ldarg.0
		IL_0069: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnTop
		IL_006e: ldc.i4.0
		IL_006f: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_0074: nop
		IL_0075: ldarg.0
		IL_0076: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnUp
		IL_007b: ldc.i4.0
		IL_007c: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_0081: nop
		IL_0082: ldarg.0
		IL_0083: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnDown
		IL_0088: ldc.i4.0
		IL_0089: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_008e: nop
		IL_008f: br IL_012c

		IL_0094: ldloc.1
		IL_0095: ldc.i4.0
		IL_0096: ceq
		IL_0098: stloc.3
		IL_0099: ldloc.3
		IL_009a: brfalse.s IL_00c7

		IL_009c: nop
		IL_009d: ldarg.0
		IL_009e: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnTop
		IL_00a3: ldc.i4.0
		IL_00a4: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_00a9: nop
		IL_00aa: ldarg.0
		IL_00ab: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnUp
		IL_00b0: ldc.i4.0
		IL_00b1: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_00b6: nop
		IL_00b7: ldarg.0
		IL_00b8: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnDown
		IL_00bd: ldc.i4.1
		IL_00be: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_00c3: nop
		IL_00c4: nop
		IL_00c5: br.s IL_012c

		IL_00c7: ldloc.1
		IL_00c8: ldloc.0
		IL_00c9: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
		IL_00ce: ldc.i4.1
		IL_00cf: sub
		IL_00d0: ceq
		IL_00d2: stloc.s 4
		IL_00d4: ldloc.s 4
		IL_00d6: brfalse.s IL_0103

		IL_00d8: nop
		IL_00d9: ldarg.0
		IL_00da: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnDown
		IL_00df: ldc.i4.0
		IL_00e0: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_00e5: nop
		IL_00e6: ldarg.0
		IL_00e7: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnTop
		IL_00ec: ldc.i4.1
		IL_00ed: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_00f2: nop
		IL_00f3: ldarg.0
		IL_00f4: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnUp
		IL_00f9: ldc.i4.1
		IL_00fa: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_00ff: nop
		IL_0100: nop
		IL_0101: br.s IL_012c

		IL_0103: nop
		IL_0104: ldarg.0
		IL_0105: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnTop
		IL_010a: ldc.i4.1
		IL_010b: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_0110: nop
		IL_0111: ldarg.0
		IL_0112: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnUp
		IL_0117: ldc.i4.1
		IL_0118: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_011d: nop
		IL_011e: ldarg.0
		IL_011f: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnDown
		IL_0124: ldc.i4.1
		IL_0125: callvirt instance void [PresentationCore]System.Windows.UIElement::set_IsEnabled(bool)
		IL_012a: nop
		IL_012b: nop

		IL_012c: ret
	} // end of method GlobalVariableDialog::UpdateBtnEnable

	.method private hidebysig 
		instance void BtnUp_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29318
		// Header size: 12
		// Code size: 218 (0xda)
		.maxstack 4
		.locals init (
			[0] bool,
			[1] int32,
			[2] int32,
			[3] class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[4] int32,
			[5] bool,
			[6] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0007: ldnull
		IL_0008: cgt.un
		IL_000a: stloc.0
		IL_000b: ldloc.0
		IL_000c: brfalse IL_00d9

		IL_0011: nop
		IL_0012: ldc.i4.0
		IL_0013: stloc.1
		IL_0014: ldc.i4.0
		IL_0015: stloc.2
		IL_0016: ldarg.0
		IL_0017: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_001c: ldarg.0
		IL_001d: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::'<BtnUp_Click>b__122_0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0023: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_0028: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_002d: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__122_1'
		IL_0032: dup
		IL_0033: brtrue.s IL_004c

		IL_0035: pop
		IL_0036: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_003b: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<BtnUp_Click>b__122_1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0041: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>::.ctor(object, native int)
		IL_0046: dup
		IL_0047: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__122_1'

		IL_004c: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_0051: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_0056: stloc.3
		IL_0057: ldc.i4.0
		IL_0058: stloc.s 4
		IL_005a: br.s IL_009c
		// loop start (head: IL_009c)
			IL_005c: nop
			IL_005d: ldloc.3
			IL_005e: ldloc.s 4
			IL_0060: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_0065: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_006a: ldarg.0
			IL_006b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0070: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0075: ceq
			IL_0077: stloc.s 5
			IL_0079: ldloc.s 5
			IL_007b: brfalse.s IL_0095

			IL_007d: nop
			IL_007e: ldloc.3
			IL_007f: ldloc.s 4
			IL_0081: ldc.i4.1
			IL_0082: sub
			IL_0083: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_0088: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_008d: stloc.1
			IL_008e: ldloc.s 4
			IL_0090: ldc.i4.1
			IL_0091: sub
			IL_0092: stloc.2
			IL_0093: br.s IL_00ac

			IL_0095: nop
			IL_0096: ldloc.s 4
			IL_0098: ldc.i4.1
			IL_0099: add
			IL_009a: stloc.s 4

			IL_009c: ldloc.s 4
			IL_009e: ldloc.3
			IL_009f: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_00a4: clt
			IL_00a6: stloc.s 6
			IL_00a8: ldloc.s 6
			IL_00aa: brtrue.s IL_005c
		// end loop

		IL_00ac: ldarg.0
		IL_00ad: ldarg.0
		IL_00ae: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_00b3: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_00b8: ldloc.1
		IL_00b9: ldloc.2
		IL_00ba: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SwapIndex(int32, int32, int32)
		IL_00bf: nop
		IL_00c0: ldarg.0
		IL_00c1: ldarg.0
		IL_00c2: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_00c7: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_00cc: ldarg.0
		IL_00cd: ldstr "SelectedVarItem"
		IL_00d2: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_00d7: nop
		IL_00d8: nop

		IL_00d9: ret
	} // end of method GlobalVariableDialog::BtnUp_Click

	.method private hidebysig 
		instance void BtnDown_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29400
		// Header size: 12
		// Code size: 218 (0xda)
		.maxstack 4
		.locals init (
			[0] bool,
			[1] int32,
			[2] int32,
			[3] class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[4] int32,
			[5] bool,
			[6] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0007: ldnull
		IL_0008: cgt.un
		IL_000a: stloc.0
		IL_000b: ldloc.0
		IL_000c: brfalse IL_00d9

		IL_0011: nop
		IL_0012: ldc.i4.0
		IL_0013: stloc.1
		IL_0014: ldc.i4.0
		IL_0015: stloc.2
		IL_0016: ldarg.0
		IL_0017: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		IL_001c: ldarg.0
		IL_001d: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::'<BtnDown_Click>b__123_0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0023: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
		IL_0028: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_002d: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__123_1'
		IL_0032: dup
		IL_0033: brtrue.s IL_004c

		IL_0035: pop
		IL_0036: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_003b: ldftn instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<BtnDown_Click>b__123_1'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
		IL_0041: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>::.ctor(object, native int)
		IL_0046: dup
		IL_0047: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__123_1'

		IL_004c: call class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class VMControls.WPF.GlobalUI.GlobalVariableItem, int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_0051: call class [mscorlib]System.Collections.Generic.List`1<!!0> [System.Core]System.Linq.Enumerable::ToList<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_0056: stloc.3
		IL_0057: ldc.i4.0
		IL_0058: stloc.s 4
		IL_005a: br.s IL_009c
		// loop start (head: IL_009c)
			IL_005c: nop
			IL_005d: ldloc.3
			IL_005e: ldloc.s 4
			IL_0060: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_0065: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_006a: ldarg.0
			IL_006b: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
			IL_0070: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_0075: ceq
			IL_0077: stloc.s 5
			IL_0079: ldloc.s 5
			IL_007b: brfalse.s IL_0095

			IL_007d: nop
			IL_007e: ldloc.3
			IL_007f: ldloc.s 4
			IL_0081: ldc.i4.1
			IL_0082: add
			IL_0083: callvirt instance !0 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Item(int32)
			IL_0088: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
			IL_008d: stloc.1
			IL_008e: ldloc.s 4
			IL_0090: ldc.i4.1
			IL_0091: add
			IL_0092: stloc.2
			IL_0093: br.s IL_00ac

			IL_0095: nop
			IL_0096: ldloc.s 4
			IL_0098: ldc.i4.1
			IL_0099: add
			IL_009a: stloc.s 4

			IL_009c: ldloc.s 4
			IL_009e: ldloc.3
			IL_009f: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_00a4: clt
			IL_00a6: stloc.s 6
			IL_00a8: ldloc.s 6
			IL_00aa: brtrue.s IL_005c
		// end loop

		IL_00ac: ldarg.0
		IL_00ad: ldarg.0
		IL_00ae: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_00b3: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_00b8: ldloc.1
		IL_00b9: ldloc.2
		IL_00ba: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SwapIndex(int32, int32, int32)
		IL_00bf: nop
		IL_00c0: ldarg.0
		IL_00c1: ldarg.0
		IL_00c2: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_00c7: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_00cc: ldarg.0
		IL_00cd: ldstr "SelectedVarItem"
		IL_00d2: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_00d7: nop
		IL_00d8: nop

		IL_00d9: ret
	} // end of method GlobalVariableDialog::BtnDown_Click

	.method private hidebysig 
		instance void BtnTop_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x294e8
		// Header size: 12
		// Code size: 59 (0x3b)
		.maxstack 2
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0007: ldnull
		IL_0008: cgt.un
		IL_000a: stloc.0
		IL_000b: ldloc.0
		IL_000c: brfalse.s IL_003a

		IL_000e: nop
		IL_000f: ldarg.0
		IL_0010: ldarg.0
		IL_0011: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0016: callvirt instance int32 VMControls.WPF.GlobalUI.GlobalVariableItem::get_Index()
		IL_001b: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SetHeadIndex(int32)
		IL_0020: nop
		IL_0021: ldarg.0
		IL_0022: ldarg.0
		IL_0023: ldfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::oldSelectedItem
		IL_0028: stfld class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::_selectedVarItem
		IL_002d: ldarg.0
		IL_002e: ldstr "SelectedVarItem"
		IL_0033: call instance void [VM.Controls]VM.Controls.BaseWindow::OnPropertyChanged(string)
		IL_0038: nop
		IL_0039: nop

		IL_003a: ret
	} // end of method GlobalVariableDialog::BtnTop_Click

	.method private hidebysig 
		instance void BtnImport_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29530
		// Header size: 12
		// Code size: 291 (0x123)
		.maxstack 6
		.locals init (
			[0] bool,
			[1] bool,
			[2] class [PresentationFramework]Microsoft.Win32.OpenFileDialog,
			[3] valuetype [mscorlib]System.Nullable`1<bool>,
			[4] string,
			[5] bool,
			[6] int64,
			[7] bool,
			[8] bool,
			[9] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: ldarg.0
			IL_0003: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0008: ldnull
			IL_0009: ceq
			IL_000b: stloc.0
			IL_000c: ldloc.0
			IL_000d: brfalse.s IL_0015

			IL_000f: nop
			IL_0010: leave IL_0122

			IL_0015: call bool [VM.Utility]VM.Utility.SingleInstance.RemoteOperate::get_IsRemoteMode()
			IL_001a: ldc.i4.0
			IL_001b: ceq
			IL_001d: stloc.1
			IL_001e: ldloc.1
			IL_001f: brfalse IL_00de

			IL_0024: nop
			IL_0025: newobj instance void [PresentationFramework]Microsoft.Win32.OpenFileDialog::.ctor()
			IL_002a: stloc.2
			IL_002b: ldloc.2
			IL_002c: ldc.i4.1
			IL_002d: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_CheckFileExists(bool)
			IL_0032: nop
			IL_0033: ldloc.2
			IL_0034: ldc.i4.1
			IL_0035: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_CheckPathExists(bool)
			IL_003a: nop
			IL_003b: ldloc.2
			IL_003c: ldstr "*.gvar|*.gvar"
			IL_0041: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_Filter(string)
			IL_0046: nop
			IL_0047: ldloc.2
			IL_0048: callvirt instance valuetype [mscorlib]System.Nullable`1<bool> [PresentationFramework]Microsoft.Win32.CommonDialog::ShowDialog()
			IL_004d: stloc.3
			IL_004e: ldloc.2
			IL_004f: callvirt instance string [PresentationFramework]Microsoft.Win32.FileDialog::get_FileName()
			IL_0054: stloc.s 4
			IL_0056: ldloca.s 3
			IL_0058: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::get_Value()
			IL_005d: stloc.s 5
			IL_005f: ldloc.s 5
			IL_0061: brfalse.s IL_00db

			IL_0063: nop
			IL_0064: ldarg.0
			IL_0065: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_006a: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_006f: ldc.i4.0
			IL_0070: cgt
			IL_0072: stloc.s 7
			IL_0074: ldloc.s 7
			IL_0076: brfalse.s IL_00a6

			IL_0078: nop
			IL_0079: ldstr "Lang_ImportSure"
			IL_007e: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0083: ldstr "Lang_ImportEmpty"
			IL_0088: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_008d: ldc.i4.1
			IL_008e: ldc.i4.1
			IL_008f: ldc.i4.0
			IL_0090: ldc.i4.0
			IL_0091: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, string, valuetype [VM.Controls]VM.Controls.MessageButtons, valuetype [VM.Controls]VM.Controls.MessageIcons, bool, bool)
			IL_0096: ldc.i4.1
			IL_0097: ceq
			IL_0099: ldc.i4.0
			IL_009a: ceq
			IL_009c: stloc.s 8
			IL_009e: ldloc.s 8
			IL_00a0: brfalse.s IL_00a5

			IL_00a2: nop
			IL_00a3: leave.s IL_0122

			IL_00a5: nop

			IL_00a6: ldloc.s 4
			IL_00a8: newobj instance void [mscorlib]System.IO.FileInfo::.ctor(string)
			IL_00ad: call instance int64 [mscorlib]System.IO.FileInfo::get_Length()
			IL_00b2: stloc.s 6
			IL_00b4: ldarg.0
			IL_00b5: ldstr "Lang_Import"
			IL_00ba: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00bf: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ProcessbarMessage(string)
			IL_00c4: nop
			IL_00c5: ldarg.0
			IL_00c6: ldstr "Visible"
			IL_00cb: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
			IL_00d0: nop
			IL_00d1: ldarg.0
			IL_00d2: ldloc.s 4
			IL_00d4: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ImportGlobalVar(string)
			IL_00d9: nop
			IL_00da: nop

			IL_00db: nop
			IL_00dc: br.s IL_00ed

			IL_00de: nop
			IL_00df: ldarg.0
			IL_00e0: ldfld class [PresentationFramework]System.Windows.Controls.Primitives.Popup VMControls.WPF.GlobalUI.GlobalVariableDialog::PopImport
			IL_00e5: ldc.i4.1
			IL_00e6: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.Popup::set_IsOpen(bool)
			IL_00eb: nop
			IL_00ec: nop

			IL_00ed: nop
			IL_00ee: leave.s IL_0122
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_00f0: stloc.s 9
			IL_00f2: nop
			IL_00f3: ldstr "Lang_LoadFail"
			IL_00f8: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00fd: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_0102: pop
			IL_0103: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_0108: ldstr "全局变量导入失败，ex:"
			IL_010d: ldloc.s 9
			IL_010f: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0114: call string [mscorlib]System.String::Concat(string, string)
			IL_0119: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_011e: nop
			IL_011f: nop
			IL_0120: leave.s IL_0122
		} // end handler

		IL_0122: ret
	} // end of method GlobalVariableDialog::BtnImport_Click

	.method private hidebysig 
		instance void ImportGlobalVar (
			string filePath
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = (
			01 00 44 56 4d 43 6f 6e 74 72 6f 6c 73 2e 57 50
			46 2e 47 6c 6f 62 61 6c 55 49 2e 47 6c 6f 62 61
			6c 56 61 72 69 61 62 6c 65 44 69 61 6c 6f 67 2b
			3c 49 6d 70 6f 72 74 47 6c 6f 62 61 6c 56 61 72
			3e 64 5f 5f 31 32 36 00 00
		)
		.custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x29670
		// Header size: 12
		// Code size: 55 (0x37)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126',
			[1] valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>4__this'
		IL_000d: ldloc.0
		IL_000e: ldarg.1
		IL_000f: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::filePath
		IL_0014: ldloc.0
		IL_0015: call valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
		IL_001a: stfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>t__builder'
		IL_001f: ldloc.0
		IL_0020: ldc.i4.m1
		IL_0021: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>1__state'
		IL_0026: ldloc.0
		IL_0027: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'::'<>t__builder'
		IL_002c: stloc.1
		IL_002d: ldloca.s 1
		IL_002f: ldloca.s 0
		IL_0031: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ImportGlobalVar>d__126'>(!!0&)
		IL_0036: ret
	} // end of method GlobalVariableDialog::ImportGlobalVar

	.method private hidebysig 
		instance void BtnExport_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x296b4
		// Header size: 12
		// Code size: 213 (0xd5)
		.maxstack 3
		.locals init (
			[0] bool,
			[1] bool,
			[2] class [PresentationFramework]Microsoft.Win32.SaveFileDialog,
			[3] bool,
			[4] valuetype [mscorlib]System.Nullable`1<bool>,
			[5] bool,
			[6] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: ldarg.0
			IL_0003: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0008: brfalse.s IL_001d

			IL_000a: ldarg.0
			IL_000b: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0010: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_0015: ldc.i4.0
			IL_0016: cgt
			IL_0018: ldc.i4.0
			IL_0019: ceq
			IL_001b: br.s IL_001e

			IL_001d: ldc.i4.1

			IL_001e: stloc.0
			IL_001f: ldloc.0
			IL_0020: brfalse.s IL_0028

			IL_0022: nop
			IL_0023: leave IL_00d4

			IL_0028: call bool [VM.Utility]VM.Utility.SingleInstance.RemoteOperate::get_IsRemoteMode()
			IL_002d: ldc.i4.0
			IL_002e: ceq
			IL_0030: stloc.1
			IL_0031: ldloc.1
			IL_0032: brfalse.s IL_0090

			IL_0034: nop
			IL_0035: newobj instance void [PresentationFramework]Microsoft.Win32.SaveFileDialog::.ctor()
			IL_003a: stloc.2
			IL_003b: ldloc.2
			IL_003c: ldstr "*.gvar|*.gvar"
			IL_0041: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_Filter(string)
			IL_0046: nop
			IL_0047: ldloc.2
			IL_0048: ldc.i4.1
			IL_0049: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_FilterIndex(int32)
			IL_004e: nop
			IL_004f: ldloc.2
			IL_0050: ldc.i4.1
			IL_0051: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_RestoreDirectory(bool)
			IL_0056: nop
			IL_0057: ldloc.2
			IL_0058: callvirt instance valuetype [mscorlib]System.Nullable`1<bool> [PresentationFramework]Microsoft.Win32.CommonDialog::ShowDialog()
			IL_005d: stloc.s 4
			IL_005f: ldc.i4.1
			IL_0060: stloc.s 5
			IL_0062: ldloca.s 4
			IL_0064: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
			IL_0069: ldloc.s 5
			IL_006b: ceq
			IL_006d: ldloca.s 4
			IL_006f: call instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
			IL_0074: and
			IL_0075: stloc.3
			IL_0076: ldloc.3
			IL_0077: brfalse.s IL_008d

			IL_0079: nop
			IL_007a: ldarg.0
			IL_007b: ldloc.2
			IL_007c: callvirt instance string [PresentationFramework]Microsoft.Win32.FileDialog::get_FileName()
			IL_0081: ldstr ""
			IL_0086: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ExportGlobalVar(string, string)
			IL_008b: nop
			IL_008c: nop

			IL_008d: nop
			IL_008e: br.s IL_009f

			IL_0090: nop
			IL_0091: ldarg.0
			IL_0092: ldfld class [PresentationFramework]System.Windows.Controls.Primitives.Popup VMControls.WPF.GlobalUI.GlobalVariableDialog::PopExport
			IL_0097: ldc.i4.1
			IL_0098: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.Popup::set_IsOpen(bool)
			IL_009d: nop
			IL_009e: nop

			IL_009f: nop
			IL_00a0: leave.s IL_00d4
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_00a2: stloc.s 6
			IL_00a4: nop
			IL_00a5: ldstr "Lang_ExportFailed"
			IL_00aa: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00af: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_00b4: pop
			IL_00b5: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_00ba: ldstr "全局变量导出失败，ex:"
			IL_00bf: ldloc.s 6
			IL_00c1: callvirt instance string [mscorlib]System.Object::ToString()
			IL_00c6: call string [mscorlib]System.String::Concat(string, string)
			IL_00cb: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_00d0: nop
			IL_00d1: nop
			IL_00d2: leave.s IL_00d4
		} // end handler

		IL_00d4: ret
	} // end of method GlobalVariableDialog::BtnExport_Click

	.method private hidebysig 
		instance void ExportGlobalVar (
			string filePath,
			[opt] string downLoadPath
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = (
			01 00 44 56 4d 43 6f 6e 74 72 6f 6c 73 2e 57 50
			46 2e 47 6c 6f 62 61 6c 55 49 2e 47 6c 6f 62 61
			6c 56 61 72 69 61 62 6c 65 44 69 61 6c 6f 67 2b
			3c 45 78 70 6f 72 74 47 6c 6f 62 61 6c 56 61 72
			3e 64 5f 5f 31 32 38 00 00
		)
		.custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (
			01 00 00 00
		)
		.param [2] = ""
		// Method begins at RVA 0x297a8
		// Header size: 12
		// Code size: 62 (0x3e)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128',
			[1] valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>4__this'
		IL_000d: ldloc.0
		IL_000e: ldarg.1
		IL_000f: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::filePath
		IL_0014: ldloc.0
		IL_0015: ldarg.2
		IL_0016: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::downLoadPath
		IL_001b: ldloc.0
		IL_001c: call valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
		IL_0021: stfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>t__builder'
		IL_0026: ldloc.0
		IL_0027: ldc.i4.m1
		IL_0028: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>1__state'
		IL_002d: ldloc.0
		IL_002e: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'::'<>t__builder'
		IL_0033: stloc.1
		IL_0034: ldloca.s 1
		IL_0036: ldloca.s 0
		IL_0038: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<ExportGlobalVar>d__128'>(!!0&)
		IL_003d: ret
	} // end of method GlobalVariableDialog::ExportGlobalVar

	.method private hidebysig 
		instance void Sv_ScrollChanged (
			object sender,
			class [PresentationFramework]System.Windows.Controls.ScrollChangedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x297f4
		// Header size: 12
		// Code size: 37 (0x25)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::CanScroll
		IL_0007: ldc.i4.0
		IL_0008: ceq
		IL_000a: stloc.0
		IL_000b: ldloc.0
		IL_000c: brfalse.s IL_0018

		IL_000e: nop
		IL_000f: ldarg.0
		IL_0010: ldc.i4.1
		IL_0011: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::CanScroll
		IL_0016: br.s IL_0024

		IL_0018: ldarg.0
		IL_0019: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::CanScroll
		IL_001e: stloc.1
		IL_001f: ldloc.1
		IL_0020: brfalse.s IL_0024

		IL_0022: nop
		IL_0023: nop

		IL_0024: ret
	} // end of method GlobalVariableDialog::Sv_ScrollChanged

	.method private hidebysig 
		instance string GetUniqueName (
			string startName
		) cil managed 
	{
		// Method begins at RVA 0x29828
		// Header size: 12
		// Code size: 97 (0x61)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.Collections.Generic.IEnumerable`1<string>,
			[1] string,
			[2] int32,
			[3] bool,
			[4] string,
			[5] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		IL_0007: ldsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.VarGroupItem, string> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__131_0'
		IL_000c: dup
		IL_000d: brtrue.s IL_0026

		IL_000f: pop
		IL_0010: ldsfld class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c' VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9'
		IL_0015: ldftn instance string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<GetUniqueName>b__131_0'(class VMControls.WPF.GlobalUI.VarGroupItem)
		IL_001b: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.VarGroupItem, string>::.ctor(object, native int)
		IL_0020: dup
		IL_0021: stsfld class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.VarGroupItem, string> VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c'::'<>9__131_0'

		IL_0026: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class VMControls.WPF.GlobalUI.VarGroupItem, string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, !!1>)
		IL_002b: stloc.0
		IL_002c: ldarg.1
		IL_002d: stloc.1
		IL_002e: ldc.i4.0
		IL_002f: stloc.2
		IL_0030: br.s IL_0059
		// loop start (head: IL_0059)
			IL_0032: nop
			IL_0033: ldarg.1
			IL_0034: ldloc.2
			IL_0035: box [mscorlib]System.Int32
			IL_003a: call string [mscorlib]System.String::Concat(object, object)
			IL_003f: stloc.1
			IL_0040: ldloc.0
			IL_0041: ldloc.1
			IL_0042: call bool [System.Core]System.Linq.Enumerable::Contains<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, !!0)
			IL_0047: ldc.i4.0
			IL_0048: ceq
			IL_004a: stloc.3
			IL_004b: ldloc.3
			IL_004c: brfalse.s IL_0054

			IL_004e: nop
			IL_004f: ldloc.1
			IL_0050: stloc.s 4
			IL_0052: br.s IL_005e

			IL_0054: nop
			IL_0055: ldloc.2
			IL_0056: ldc.i4.1
			IL_0057: add
			IL_0058: stloc.2

			IL_0059: ldc.i4.1
			IL_005a: stloc.s 5
			IL_005c: br.s IL_0032
		// end loop

		IL_005e: ldloc.s 4
		IL_0060: ret
	} // end of method GlobalVariableDialog::GetUniqueName

	.method private hidebysig 
		instance void AddNewVarGroup () cil managed 
	{
		// Method begins at RVA 0x29898
		// Header size: 12
		// Code size: 155 (0x9b)
		.maxstack 4
		.locals init (
			[0] string,
			[1] bool,
			[2] int32
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		IL_0007: call int32 [System.Core]System.Linq.Enumerable::Count<class VMControls.WPF.GlobalUI.VarGroupItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
		IL_000c: ldc.i4.s 10
		IL_000e: clt
		IL_0010: ldc.i4.0
		IL_0011: ceq
		IL_0013: stloc.1
		IL_0014: ldloc.1
		IL_0015: brfalse.s IL_004a

		IL_0017: nop
		IL_0018: ldstr "添加"
		IL_001d: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_0022: ldstr "超限"
		IL_0027: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_002c: ldstr ":"
		IL_0031: ldc.i4.s 10
		IL_0033: stloc.2
		IL_0034: ldloca.s 2
		IL_0036: call instance string [mscorlib]System.Int32::ToString()
		IL_003b: call string [mscorlib]System.String::Concat(string, string, string, string)
		IL_0040: ldc.i4.0
		IL_0041: ldc.i4.2
		IL_0042: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageButtons, valuetype [VM.Controls]VM.Controls.MessageIcons)
		IL_0047: pop
		IL_0048: br.s IL_009a

		IL_004a: ldarg.0
		IL_004b: ldstr "Lang_VarGroup"
		IL_0050: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_0055: call instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::GetUniqueName(string)
		IL_005a: stloc.0
		IL_005b: ldarg.0
		IL_005c: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
		IL_0061: ldloc.0
		IL_0062: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::AddGroup(string)
		IL_0067: nop
		IL_0068: ldarg.0
		IL_0069: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		IL_006e: ldloc.0
		IL_006f: ldarg.0
		IL_0070: newobj instance void VMControls.WPF.GlobalUI.VarGroupItem::.ctor(string, class VMControls.WPF.GlobalUI.IVarGroup)
		IL_0075: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::Add(!0)
		IL_007a: nop
		IL_007b: ldarg.0
		IL_007c: ldarg.0
		IL_007d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		IL_0082: ldarg.0
		IL_0083: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		IL_0088: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Count()
		IL_008d: ldc.i4.1
		IL_008e: sub
		IL_008f: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Item(int32)
		IL_0094: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectGroupItem(class VMControls.WPF.GlobalUI.VarGroupItem)
		IL_0099: nop

		IL_009a: ret
	} // end of method GlobalVariableDialog::AddNewVarGroup

	.method private hidebysig 
		instance void AddVarGroup_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29940
		// Header size: 12
		// Code size: 72 (0x48)
		.maxstack 2
		.locals init (
			[0] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: ldarg.0
			IL_0003: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::AddNewVarGroup()
			IL_0008: nop
			IL_0009: nop
			IL_000a: leave.s IL_0047
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_000c: stloc.0
			IL_000d: nop
			IL_000e: ldstr "AddNewVarGroup is exception:"
			IL_0013: ldarg.2
			IL_0014: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0019: call string [mscorlib]System.String::Concat(string, string)
			IL_001e: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_0023: nop
			IL_0024: ldstr "Lang_AddGroup"
			IL_0029: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_002e: ldstr "失败"
			IL_0033: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0038: call string [mscorlib]System.String::Concat(string, string)
			IL_003d: ldc.i4.0
			IL_003e: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, valuetype [VM.Controls]VM.Controls.MessageButtons)
			IL_0043: pop
			IL_0044: nop
			IL_0045: leave.s IL_0047
		} // end handler

		IL_0047: ret
	} // end of method GlobalVariableDialog::AddVarGroup_Click

	.method private hidebysig 
		instance void DelVarGroup_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x299a4
		// Header size: 12
		// Code size: 105 (0x69)
		.maxstack 6
		.locals init (
			[0] object,
			[1] class VMControls.WPF.GlobalUI.VarGroupItem,
			[2] bool,
			[3] bool
		)

		IL_0000: nop
		IL_0001: ldstr "Lang_ConfirmDelete"
		IL_0006: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_000b: ldstr ""
		IL_0010: ldc.i4.1
		IL_0011: ldc.i4.1
		IL_0012: ldc.i4.0
		IL_0013: ldc.i4.0
		IL_0014: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, string, valuetype [VM.Controls]VM.Controls.MessageButtons, valuetype [VM.Controls]VM.Controls.MessageIcons, bool, bool)
		IL_0019: ldc.i4.1
		IL_001a: ceq
		IL_001c: ldc.i4.0
		IL_001d: ceq
		IL_001f: stloc.2
		IL_0020: ldloc.2
		IL_0021: brfalse.s IL_0026

		IL_0023: nop
		IL_0024: br.s IL_0068

		IL_0026: ldarg.1
		IL_0027: isinst [PresentationFramework]System.Windows.Controls.MenuItem
		IL_002c: callvirt instance object [PresentationFramework]System.Windows.FrameworkElement::get_DataContext()
		IL_0031: stloc.0
		IL_0032: ldloc.0
		IL_0033: isinst VMControls.WPF.GlobalUI.VarGroupItem
		IL_0038: dup
		IL_0039: stloc.1
		IL_003a: ldnull
		IL_003b: cgt.un
		IL_003d: stloc.3
		IL_003e: ldloc.3
		IL_003f: brfalse.s IL_0068

		IL_0041: nop
		IL_0042: ldarg.0
		IL_0043: ldstr "Lang_DeleteGroup"
		IL_0048: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
		IL_004d: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ProcessbarMessage(string)
		IL_0052: nop
		IL_0053: ldarg.0
		IL_0054: ldstr "Visible"
		IL_0059: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
		IL_005e: nop
		IL_005f: ldarg.0
		IL_0060: ldloc.1
		IL_0061: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::DelVarGroup(class VMControls.WPF.GlobalUI.VarGroupItem)
		IL_0066: nop
		IL_0067: nop

		IL_0068: ret
	} // end of method GlobalVariableDialog::DelVarGroup_Click

	.method private hidebysig 
		instance void DelVarGroup (
			class VMControls.WPF.GlobalUI.VarGroupItem varGroup
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = (
			01 00 40 56 4d 43 6f 6e 74 72 6f 6c 73 2e 57 50
			46 2e 47 6c 6f 62 61 6c 55 49 2e 47 6c 6f 62 61
			6c 56 61 72 69 61 62 6c 65 44 69 61 6c 6f 67 2b
			3c 44 65 6c 56 61 72 47 72 6f 75 70 3e 64 5f 5f
			31 33 35 00 00
		)
		.custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x29a1c
		// Header size: 12
		// Code size: 55 (0x37)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135',
			[1] valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>4__this'
		IL_000d: ldloc.0
		IL_000e: ldarg.1
		IL_000f: stfld class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::varGroup
		IL_0014: ldloc.0
		IL_0015: call valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
		IL_001a: stfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>t__builder'
		IL_001f: ldloc.0
		IL_0020: ldc.i4.m1
		IL_0021: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>1__state'
		IL_0026: ldloc.0
		IL_0027: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'::'<>t__builder'
		IL_002c: stloc.1
		IL_002d: ldloca.s 1
		IL_002f: ldloca.s 0
		IL_0031: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<DelVarGroup>d__135'>(!!0&)
		IL_0036: ret
	} // end of method GlobalVariableDialog::DelVarGroup

	.method public hidebysig 
		instance bool SetVarGroupMsg () cil managed 
	{
		// Method begins at RVA 0x29a60
		// Header size: 12
		// Code size: 166 (0xa6)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.Text.StringBuilder,
			[1] class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.VarGroupItem>,
			[2] class VMControls.WPF.GlobalUI.VarGroupItem,
			[3] bool,
			[4] class [mscorlib]System.Exception,
			[5] bool
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: newobj instance void [mscorlib]System.Text.StringBuilder::.ctor()
			IL_0007: stloc.0
			IL_0008: nop
			IL_0009: ldarg.0
			IL_000a: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
			IL_000f: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::GetEnumerator()
			IL_0014: stloc.1
			.try
			{
				IL_0015: br.s IL_0037
				// loop start (head: IL_0037)
					IL_0017: ldloc.1
					IL_0018: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Current()
					IL_001d: stloc.2
					IL_001e: nop
					IL_001f: ldloc.0
					IL_0020: ldloc.2
					IL_0021: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
					IL_0026: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
					IL_002b: ldstr "\r"
					IL_0030: callvirt instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
					IL_0035: pop
					IL_0036: nop

					IL_0037: ldloc.1
					IL_0038: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_003d: brtrue.s IL_0017
				// end loop

				IL_003f: leave.s IL_004c
			} // end .try
			finally
			{
				IL_0041: ldloc.1
				IL_0042: brfalse.s IL_004b

				IL_0044: ldloc.1
				IL_0045: callvirt instance void [mscorlib]System.IDisposable::Dispose()
				IL_004a: nop

				IL_004b: endfinally
			} // end handler

			IL_004c: ldloc.0
			IL_004d: callvirt instance int32 [mscorlib]System.Text.StringBuilder::get_Length()
			IL_0052: ldc.i4.0
			IL_0053: cgt
			IL_0055: stloc.3
			IL_0056: ldloc.3
			IL_0057: brfalse.s IL_007c

			IL_0059: nop
			IL_005a: ldarg.0
			IL_005b: call instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
			IL_0060: isinst [VM.Core]VM.Core.VmModule
			IL_0065: callvirt instance class [VM.Core]VM.Core.ModuleParam [VM.Core]VM.Core.VmModule::get_Params()
			IL_006a: ldstr "VarGroupInfo"
			IL_006f: ldloc.0
			IL_0070: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0075: callvirt instance void [VM.Core]VM.Core.ModuleParam::set_Item(string, string)
			IL_007a: nop
			IL_007b: nop

			IL_007c: nop
			IL_007d: leave.s IL_009e
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_007f: stloc.s 4
			IL_0081: nop
			IL_0082: ldstr "SetVarGroupMsg is error:"
			IL_0087: ldloc.s 4
			IL_0089: callvirt instance string [mscorlib]System.Object::ToString()
			IL_008e: call string [mscorlib]System.String::Concat(string, string)
			IL_0093: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_0098: nop
			IL_0099: ldc.i4.0
			IL_009a: stloc.s 5
			IL_009c: leave.s IL_00a3
		} // end handler

		IL_009e: ldc.i4.1
		IL_009f: stloc.s 5
		IL_00a1: br.s IL_00a3

		IL_00a3: ldloc.s 5
		IL_00a5: ret
	} // end of method GlobalVariableDialog::SetVarGroupMsg

	.method public hidebysig 
		instance bool GetVarGroupMsg () cil managed 
	{
		// Method begins at RVA 0x29b30
		// Header size: 12
		// Code size: 127 (0x7f)
		.maxstack 3
		.locals init (
			[0] class [mscorlib]System.Collections.Generic.List`1<string>,
			[1] bool,
			[2] class [mscorlib]System.Exception,
			[3] bool
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: newobj instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
			IL_0007: stloc.0
			IL_0008: ldarg.0
			IL_0009: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_000e: ldloc.0
			IL_000f: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::GetGroupInfo(class [mscorlib]System.Collections.Generic.List`1<string>)
			IL_0014: nop
			IL_0015: ldloc.0
			IL_0016: callvirt instance int32 class [mscorlib]System.Collections.Generic.List`1<string>::get_Count()
			IL_001b: ldc.i4.0
			IL_001c: cgt
			IL_001e: stloc.1
			IL_001f: ldloc.1
			IL_0020: brfalse.s IL_0051

			IL_0022: nop
			IL_0023: ldloc.0
			IL_0024: callvirt instance !0[] class [mscorlib]System.Collections.Generic.List`1<string>::ToArray()
			IL_0029: ldarg.0
			IL_002a: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::'<GetVarGroupMsg>b__137_0'(string)
			IL_0030: newobj instance void class [mscorlib]System.Action`1<string>::.ctor(object, native int)
			IL_0035: call void [mscorlib]System.Array::ForEach<string>(!!0[], class [mscorlib]System.Action`1<!!0>)
			IL_003a: nop
			IL_003b: ldarg.0
			IL_003c: ldarg.0
			IL_003d: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
			IL_0042: ldc.i4.0
			IL_0043: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::get_Item(int32)
			IL_0048: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectGroupItem(class VMControls.WPF.GlobalUI.VarGroupItem)
			IL_004d: nop
			IL_004e: nop
			IL_004f: br.s IL_005a

			IL_0051: nop
			IL_0052: ldarg.0
			IL_0053: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::AddNewVarGroup()
			IL_0058: nop
			IL_0059: nop

			IL_005a: nop
			IL_005b: leave.s IL_0079
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_005d: stloc.2
			IL_005e: nop
			IL_005f: ldstr "GetVarGroupMsg is error:"
			IL_0064: ldloc.2
			IL_0065: callvirt instance string [mscorlib]System.Object::ToString()
			IL_006a: call string [mscorlib]System.String::Concat(string, string)
			IL_006f: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_0074: nop
			IL_0075: ldc.i4.0
			IL_0076: stloc.3
			IL_0077: leave.s IL_007d
		} // end handler

		IL_0079: ldc.i4.1
		IL_007a: stloc.3
		IL_007b: br.s IL_007d

		IL_007d: ldloc.3
		IL_007e: ret
	} // end of method GlobalVariableDialog::GetVarGroupMsg

	.method public final hidebysig newslot virtual 
		instance bool ReName (
			string oldName,
			string newName
		) cil managed 
	{
		// Method begins at RVA 0x29bcc
		// Header size: 12
		// Code size: 268 (0x10c)
		.maxstack 4
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0',
			[1] class [mscorlib]System.Collections.Generic.IEnumerable`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[2] bool,
			[3] bool,
			[4] class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>,
			[5] class VMControls.WPF.GlobalUI.GlobalVariableItem,
			[6] class [mscorlib]System.Exception
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.2
		IL_0008: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::newName
		IL_000d: ldloc.0
		IL_000e: ldarg.1
		IL_000f: stfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::oldName
		IL_0014: nop
		.try
		{
			IL_0015: nop
			IL_0016: ldarg.0
			IL_0017: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
			IL_001c: ldloc.0
			IL_001d: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::'<ReName>b__1'(class VMControls.WPF.GlobalUI.VarGroupItem)
			IL_0023: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.VarGroupItem, bool>::.ctor(object, native int)
			IL_0028: call !!0 [System.Core]System.Linq.Enumerable::FirstOrDefault<class VMControls.WPF.GlobalUI.VarGroupItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_002d: ldnull
			IL_002e: cgt.un
			IL_0030: stloc.2
			IL_0031: ldloc.2
			IL_0032: brfalse.s IL_004e

			IL_0034: nop
			IL_0035: ldstr "Lang_ParamNameErr"
			IL_003a: ldstr "Lang_PmNameExited"
			IL_003f: ldc.i4.3
			IL_0040: ldc.i4.1
			IL_0041: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_0046: pop
			IL_0047: ldc.i4.0
			IL_0048: stloc.3
			IL_0049: leave IL_010a

			IL_004e: ldarg.0
			IL_004f: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_0054: ldloc.0
			IL_0055: ldftn instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::'<ReName>b__0'(class VMControls.WPF.GlobalUI.GlobalVariableItem)
			IL_005b: newobj instance void class [mscorlib]System.Func`2<class VMControls.WPF.GlobalUI.GlobalVariableItem, bool>::.ctor(object, native int)
			IL_0060: call class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class VMControls.WPF.GlobalUI.GlobalVariableItem>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
			IL_0065: stloc.1
			IL_0066: nop
			IL_0067: ldloc.1
			IL_0068: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::GetEnumerator()
			IL_006d: stloc.s 4
			.try
			{
				IL_006f: br.s IL_008a
				// loop start (head: IL_008a)
					IL_0071: ldloc.s 4
					IL_0073: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Current()
					IL_0078: stloc.s 5
					IL_007a: nop
					IL_007b: ldloc.s 5
					IL_007d: ldloc.0
					IL_007e: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::newName
					IL_0083: callvirt instance void VMControls.WPF.GlobalUI.GlobalVariableItem::set_VarGroupInfo(string)
					IL_0088: nop
					IL_0089: nop

					IL_008a: ldloc.s 4
					IL_008c: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_0091: brtrue.s IL_0071
				// end loop

				IL_0093: leave.s IL_00a2
			} // end .try
			finally
			{
				IL_0095: ldloc.s 4
				IL_0097: brfalse.s IL_00a1

				IL_0099: ldloc.s 4
				IL_009b: callvirt instance void [mscorlib]System.IDisposable::Dispose()
				IL_00a0: nop

				IL_00a1: endfinally
			} // end handler

			IL_00a2: ldarg.0
			IL_00a3: ldfld class [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager VMControls.WPF.GlobalUI.GlobalVariableDialog::varManager
			IL_00a8: ldloc.0
			IL_00a9: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::oldName
			IL_00ae: ldloc.0
			IL_00af: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::newName
			IL_00b4: callvirt instance void [Apps.XmlParser]Apps.XmlParser.Variable.VariableManager::ReNameGroup(string, string)
			IL_00b9: nop
			IL_00ba: ldc.i4.1
			IL_00bb: stloc.3
			IL_00bc: leave.s IL_010a
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_00be: stloc.s 6
			IL_00c0: nop
			IL_00c1: ldc.i4.6
			IL_00c2: newarr [mscorlib]System.String
			IL_00c7: dup
			IL_00c8: ldc.i4.0
			IL_00c9: ldstr "ReName is ecxeption:"
			IL_00ce: stelem.ref
			IL_00cf: dup
			IL_00d0: ldc.i4.1
			IL_00d1: ldloc.0
			IL_00d2: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::oldName
			IL_00d7: stelem.ref
			IL_00d8: dup
			IL_00d9: ldc.i4.2
			IL_00da: ldstr ","
			IL_00df: stelem.ref
			IL_00e0: dup
			IL_00e1: ldc.i4.3
			IL_00e2: ldloc.0
			IL_00e3: ldfld string VMControls.WPF.GlobalUI.GlobalVariableDialog/'<>c__DisplayClass138_0'::newName
			IL_00e8: stelem.ref
			IL_00e9: dup
			IL_00ea: ldc.i4.4
			IL_00eb: ldstr ","
			IL_00f0: stelem.ref
			IL_00f1: dup
			IL_00f2: ldc.i4.5
			IL_00f3: ldloc.s 6
			IL_00f5: callvirt instance string [mscorlib]System.Object::ToString()
			IL_00fa: stelem.ref
			IL_00fb: call string [mscorlib]System.String::Concat(string[])
			IL_0100: call void [Apps.Log]Apps.Log.LogHelper::Error(object)
			IL_0105: nop
			IL_0106: ldc.i4.0
			IL_0107: stloc.3
			IL_0108: leave.s IL_010a
		} // end handler

		IL_010a: ldloc.3
		IL_010b: ret
	} // end of method GlobalVariableDialog::ReName

	.method private hidebysig 
		instance void LocalImport_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29d00
		// Header size: 12
		// Code size: 295 (0x127)
		.maxstack 6
		.locals init (
			[0] class [PresentationFramework]Microsoft.Win32.OpenFileDialog,
			[1] valuetype [mscorlib]System.Nullable`1<bool>,
			[2] string,
			[3] bool,
			[4] bool,
			[5] bool,
			[6] bool,
			[7] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: newobj instance void [PresentationFramework]Microsoft.Win32.OpenFileDialog::.ctor()
			IL_0007: stloc.0
			IL_0008: ldloc.0
			IL_0009: ldc.i4.1
			IL_000a: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_CheckFileExists(bool)
			IL_000f: nop
			IL_0010: ldloc.0
			IL_0011: ldc.i4.1
			IL_0012: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_CheckPathExists(bool)
			IL_0017: nop
			IL_0018: ldloc.0
			IL_0019: ldstr "*.gvar|*.gvar"
			IL_001e: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_Filter(string)
			IL_0023: nop
			IL_0024: ldloc.0
			IL_0025: callvirt instance valuetype [mscorlib]System.Nullable`1<bool> [PresentationFramework]Microsoft.Win32.CommonDialog::ShowDialog()
			IL_002a: stloc.1
			IL_002b: ldloca.s 1
			IL_002d: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::get_Value()
			IL_0032: brtrue.s IL_003b

			IL_0034: ldstr ""
			IL_0039: br.s IL_0040

			IL_003b: ldstr "/root/vmtempfiles/ftp/ModelFiles/temp.gvar"

			IL_0040: stloc.2
			IL_0041: ldloca.s 1
			IL_0043: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::get_Value()
			IL_0048: stloc.3
			IL_0049: ldloc.3
			IL_004a: brfalse IL_00f1

			IL_004f: nop
			IL_0050: ldsfld native int [VM.PlatformSDKCS]VM.PlatformSDKCS.CVmObjectBase::mhandle
			IL_0055: ldloc.0
			IL_0056: callvirt instance string [PresentationFramework]Microsoft.Win32.FileDialog::get_FileName()
			IL_005b: ldstr "/root/vmtempfiles/ftp/ModelFiles/temp.gvar"
			IL_0060: ldc.i4.1
			IL_0061: ldc.i4.1
			IL_0062: call bool [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKAPI::FtpUploadFile(native int, string, string, bool, bool)
			IL_0067: ldc.i4.0
			IL_0068: ceq
			IL_006a: stloc.s 4
			IL_006c: ldloc.s 4
			IL_006e: brfalse.s IL_00a6

			IL_0070: nop
			IL_0071: ldstr "Lang_LoadFail"
			IL_0076: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_007b: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_0080: pop
			IL_0081: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_0086: ldstr "Upload local file "
			IL_008b: ldloc.0
			IL_008c: callvirt instance string [PresentationFramework]Microsoft.Win32.FileDialog::get_FileName()
			IL_0091: ldstr " to remote path /root/vmtempfiles/ftp/ModelFiles/temp.gvar fail"
			IL_0096: call string [mscorlib]System.String::Concat(string, string, string)
			IL_009b: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_00a0: nop
			IL_00a1: leave IL_0126

			IL_00a6: ldarg.0
			IL_00a7: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_00ac: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_00b1: ldc.i4.0
			IL_00b2: cgt
			IL_00b4: stloc.s 5
			IL_00b6: ldloc.s 5
			IL_00b8: brfalse.s IL_00e8

			IL_00ba: nop
			IL_00bb: ldstr "Lang_ImportSure"
			IL_00c0: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00c5: ldstr "Lang_ImportEmpty"
			IL_00ca: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00cf: ldc.i4.1
			IL_00d0: ldc.i4.1
			IL_00d1: ldc.i4.0
			IL_00d2: ldc.i4.0
			IL_00d3: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, string, valuetype [VM.Controls]VM.Controls.MessageButtons, valuetype [VM.Controls]VM.Controls.MessageIcons, bool, bool)
			IL_00d8: ldc.i4.1
			IL_00d9: ceq
			IL_00db: ldc.i4.0
			IL_00dc: ceq
			IL_00de: stloc.s 6
			IL_00e0: ldloc.s 6
			IL_00e2: brfalse.s IL_00e7

			IL_00e4: nop
			IL_00e5: leave.s IL_0126

			IL_00e7: nop

			IL_00e8: ldarg.0
			IL_00e9: ldloc.2
			IL_00ea: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ImportGlobalVar(string)
			IL_00ef: nop
			IL_00f0: nop

			IL_00f1: nop
			IL_00f2: leave.s IL_0126
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_00f4: stloc.s 7
			IL_00f6: nop
			IL_00f7: ldstr "Lang_LoadFail"
			IL_00fc: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0101: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_0106: pop
			IL_0107: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_010c: ldstr "全局变量导入失败，ex:"
			IL_0111: ldloc.s 7
			IL_0113: callvirt instance string [mscorlib]System.Object::ToString()
			IL_0118: call string [mscorlib]System.String::Concat(string, string)
			IL_011d: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_0122: nop
			IL_0123: nop
			IL_0124: leave.s IL_0126
		} // end handler

		IL_0126: ret
	} // end of method GlobalVariableDialog::LocalImport_Click

	.method private hidebysig 
		instance void RemoteImport_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29e44
		// Header size: 12
		// Code size: 201 (0xc9)
		.maxstack 6
		.locals init (
			[0] class [DirectoryBrowser]DirectoryBrowser.OpenFileDialog,
			[1] valuetype [mscorlib]System.Nullable`1<bool>,
			[2] string,
			[3] bool,
			[4] bool,
			[5] bool,
			[6] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: newobj instance void [DirectoryBrowser]DirectoryBrowser.OpenFileDialog::.ctor()
			IL_0007: stloc.0
			IL_0008: ldloc.0
			IL_0009: ldstr "*.gvar|*.gvar"
			IL_000e: callvirt instance void [DirectoryBrowser]DirectoryBrowser.OpenFileDialog::set_Filter(string)
			IL_0013: nop
			IL_0014: ldloc.0
			IL_0015: ldc.i4.1
			IL_0016: callvirt instance void [DirectoryBrowser]DirectoryBrowser.OpenFileDialog::set_CheckFileExists(bool)
			IL_001b: nop
			IL_001c: ldloc.0
			IL_001d: callvirt instance valuetype [mscorlib]System.Nullable`1<bool> [VM.Controls]VM.Controls.BaseWindow::ShowDialog()
			IL_0022: stloc.1
			IL_0023: ldloca.s 1
			IL_0025: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::get_Value()
			IL_002a: brtrue.s IL_0033

			IL_002c: ldstr ""
			IL_0031: br.s IL_003b

			IL_0033: ldloc.0
			IL_0034: callvirt instance string[] [DirectoryBrowser]DirectoryBrowser.OpenFileDialog::get_FileNames()
			IL_0039: ldc.i4.0
			IL_003a: ldelem.ref

			IL_003b: stloc.2
			IL_003c: ldloca.s 1
			IL_003e: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::get_Value()
			IL_0043: stloc.3
			IL_0044: ldloc.3
			IL_0045: brfalse.s IL_0093

			IL_0047: nop
			IL_0048: ldarg.0
			IL_0049: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
			IL_004e: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>::get_Count()
			IL_0053: ldc.i4.0
			IL_0054: cgt
			IL_0056: stloc.s 4
			IL_0058: ldloc.s 4
			IL_005a: brfalse.s IL_008a

			IL_005c: nop
			IL_005d: ldstr "Lang_ImportSure"
			IL_0062: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0067: ldstr "Lang_ImportEmpty"
			IL_006c: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0071: ldc.i4.1
			IL_0072: ldc.i4.1
			IL_0073: ldc.i4.0
			IL_0074: ldc.i4.0
			IL_0075: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, string, valuetype [VM.Controls]VM.Controls.MessageButtons, valuetype [VM.Controls]VM.Controls.MessageIcons, bool, bool)
			IL_007a: ldc.i4.1
			IL_007b: ceq
			IL_007d: ldc.i4.0
			IL_007e: ceq
			IL_0080: stloc.s 5
			IL_0082: ldloc.s 5
			IL_0084: brfalse.s IL_0089

			IL_0086: nop
			IL_0087: leave.s IL_00c8

			IL_0089: nop

			IL_008a: ldarg.0
			IL_008b: ldloc.2
			IL_008c: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ImportGlobalVar(string)
			IL_0091: nop
			IL_0092: nop

			IL_0093: nop
			IL_0094: leave.s IL_00c8
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0096: stloc.s 6
			IL_0098: nop
			IL_0099: ldstr "Lang_LoadFail"
			IL_009e: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_00a3: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_00a8: pop
			IL_00a9: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_00ae: ldstr "全局变量导入失败，ex:"
			IL_00b3: ldloc.s 6
			IL_00b5: callvirt instance string [mscorlib]System.Object::ToString()
			IL_00ba: call string [mscorlib]System.String::Concat(string, string)
			IL_00bf: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_00c4: nop
			IL_00c5: nop
			IL_00c6: leave.s IL_00c8
		} // end handler

		IL_00c8: ret
	} // end of method GlobalVariableDialog::RemoteImport_Click

	.method private hidebysig 
		instance void LocalExport_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29f2c
		// Header size: 12
		// Code size: 157 (0x9d)
		.maxstack 3
		.locals init (
			[0] class [PresentationFramework]Microsoft.Win32.SaveFileDialog,
			[1] bool,
			[2] valuetype [mscorlib]System.Nullable`1<bool>,
			[3] bool,
			[4] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: newobj instance void [PresentationFramework]Microsoft.Win32.SaveFileDialog::.ctor()
			IL_0007: stloc.0
			IL_0008: ldloc.0
			IL_0009: ldstr "*.gvar|*.gvar"
			IL_000e: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_Filter(string)
			IL_0013: nop
			IL_0014: ldloc.0
			IL_0015: ldc.i4.1
			IL_0016: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_FilterIndex(int32)
			IL_001b: nop
			IL_001c: ldloc.0
			IL_001d: ldc.i4.1
			IL_001e: callvirt instance void [PresentationFramework]Microsoft.Win32.FileDialog::set_RestoreDirectory(bool)
			IL_0023: nop
			IL_0024: ldloc.0
			IL_0025: callvirt instance valuetype [mscorlib]System.Nullable`1<bool> [PresentationFramework]Microsoft.Win32.CommonDialog::ShowDialog()
			IL_002a: stloc.2
			IL_002b: ldc.i4.1
			IL_002c: stloc.3
			IL_002d: ldloca.s 2
			IL_002f: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
			IL_0034: ldloc.3
			IL_0035: ceq
			IL_0037: ldloca.s 2
			IL_0039: call instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
			IL_003e: and
			IL_003f: stloc.1
			IL_0040: ldloc.1
			IL_0041: brfalse.s IL_0067

			IL_0043: nop
			IL_0044: ldsfld native int [VM.PlatformSDKCS]VM.PlatformSDKCS.CVmObjectBase::mhandle
			IL_0049: ldstr "/root/vmtempfiles/ftp/ModelFiles/"
			IL_004e: call bool [VM.PlatformSDKCS]VM.PlatformSDKCS.ServerSDKAPI::FtpCreatDir(native int, string)
			IL_0053: pop
			IL_0054: ldarg.0
			IL_0055: ldstr "/root/vmtempfiles/ftp/ModelFiles/temp.gvar"
			IL_005a: ldloc.0
			IL_005b: callvirt instance string [PresentationFramework]Microsoft.Win32.FileDialog::get_FileName()
			IL_0060: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ExportGlobalVar(string, string)
			IL_0065: nop
			IL_0066: nop

			IL_0067: nop
			IL_0068: leave.s IL_009c
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_006a: stloc.s 4
			IL_006c: nop
			IL_006d: ldstr "导出失败"
			IL_0072: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0077: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_007c: pop
			IL_007d: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_0082: ldstr "全局变量导出失败，ex:"
			IL_0087: ldloc.s 4
			IL_0089: callvirt instance string [mscorlib]System.Object::ToString()
			IL_008e: call string [mscorlib]System.String::Concat(string, string)
			IL_0093: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_0098: nop
			IL_0099: nop
			IL_009a: leave.s IL_009c
		} // end handler

		IL_009c: ret
	} // end of method GlobalVariableDialog::LocalExport_Click

	.method private hidebysig 
		instance void RemoteExport_Click (
			object sender,
			class [PresentationCore]System.Windows.RoutedEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x29fe8
		// Header size: 12
		// Code size: 125 (0x7d)
		.maxstack 3
		.locals init (
			[0] class [DirectoryBrowser]DirectoryBrowser.SaveFileDialog,
			[1] bool,
			[2] valuetype [mscorlib]System.Nullable`1<bool>,
			[3] bool,
			[4] class [mscorlib]System.Exception
		)

		IL_0000: nop
		.try
		{
			IL_0001: nop
			IL_0002: newobj instance void [DirectoryBrowser]DirectoryBrowser.SaveFileDialog::.ctor()
			IL_0007: stloc.0
			IL_0008: ldloc.0
			IL_0009: ldstr "*.gvar|*.gvar"
			IL_000e: callvirt instance void [DirectoryBrowser]DirectoryBrowser.SaveFileDialog::set_Filter(string)
			IL_0013: nop
			IL_0014: ldloc.0
			IL_0015: callvirt instance valuetype [mscorlib]System.Nullable`1<bool> [VM.Controls]VM.Controls.BaseWindow::ShowDialog()
			IL_001a: stloc.2
			IL_001b: ldc.i4.1
			IL_001c: stloc.3
			IL_001d: ldloca.s 2
			IL_001f: call instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
			IL_0024: ldloc.3
			IL_0025: ceq
			IL_0027: ldloca.s 2
			IL_0029: call instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
			IL_002e: and
			IL_002f: stloc.1
			IL_0030: ldloc.1
			IL_0031: brfalse.s IL_0047

			IL_0033: nop
			IL_0034: ldarg.0
			IL_0035: ldloc.0
			IL_0036: ldfld string [DirectoryBrowser]DirectoryBrowser.SaveFileDialog::FilePath
			IL_003b: ldstr ""
			IL_0040: call instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::ExportGlobalVar(string, string)
			IL_0045: nop
			IL_0046: nop

			IL_0047: nop
			IL_0048: leave.s IL_007c
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_004a: stloc.s 4
			IL_004c: nop
			IL_004d: ldstr "导出失败"
			IL_0052: call string [Apps.Localization]Apps.Localization.AppLocalizationService::GetLang(string)
			IL_0057: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string)
			IL_005c: pop
			IL_005d: ldsfld class [Apps.BaseLog]Apps.BaseLog.ILog [Apps.Log]Apps.Log.LogHelper::objLog
			IL_0062: ldstr "全局变量导出失败，ex:"
			IL_0067: ldloc.s 4
			IL_0069: callvirt instance string [mscorlib]System.Object::ToString()
			IL_006e: call string [mscorlib]System.String::Concat(string, string)
			IL_0073: callvirt instance void [Apps.BaseLog]Apps.BaseLog.ILog::Error(object)
			IL_0078: nop
			IL_0079: nop
			IL_007a: leave.s IL_007c
		} // end handler

		IL_007c: ret
	} // end of method GlobalVariableDialog::RemoteExport_Click

	.method private hidebysig 
		instance void varDataGrid_AutoGeneratedColumns (
			object sender,
			class [mscorlib]System.EventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x2a084
		// Header size: 12
		// Code size: 147 (0x93)
		.maxstack 2
		.locals init (
			[0] class [PresentationFramework]System.Windows.Controls.DataGrid,
			[1] bool,
			[2] int32,
			[3] bool,
			[4] bool,
			[5] bool
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: brfalse.s IL_0011

		IL_0004: ldarg.1
		IL_0005: isinst [PresentationFramework]System.Windows.Controls.DataGrid
		IL_000a: dup
		IL_000b: stloc.0
		IL_000c: ldnull
		IL_000d: cgt.un
		IL_000f: br.s IL_0012

		IL_0011: ldc.i4.0

		IL_0012: stloc.1
		IL_0013: ldloc.1
		IL_0014: brfalse.s IL_0092

		IL_0016: nop
		IL_0017: ldc.i4.0
		IL_0018: stloc.2
		IL_0019: br.s IL_007d
		// loop start (head: IL_007d)
			IL_001b: nop
			IL_001c: ldloc.2
			IL_001d: ldc.i4.7
			IL_001e: ceq
			IL_0020: stloc.3
			IL_0021: ldloc.3
			IL_0022: brfalse.s IL_0044

			IL_0024: nop
			IL_0025: ldloc.0
			IL_0026: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn> [PresentationFramework]System.Windows.Controls.DataGrid::get_Columns()
			IL_002b: ldloc.2
			IL_002c: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn>::get_Item(int32)
			IL_0031: ldarg.0
			IL_0032: call instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsVisibleComInit()
			IL_0037: brtrue.s IL_003c

			IL_0039: ldc.i4.2
			IL_003a: br.s IL_003d

			IL_003c: ldc.i4.0

			IL_003d: callvirt instance void [PresentationFramework]System.Windows.Controls.DataGridColumn::set_Visibility(valuetype [PresentationCore]System.Windows.Visibility)
			IL_0042: nop
			IL_0043: nop

			IL_0044: ldloc.0
			IL_0045: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn> [PresentationFramework]System.Windows.Controls.DataGrid::get_Columns()
			IL_004a: ldloc.2
			IL_004b: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn>::get_Item(int32)
			IL_0050: callvirt instance object [PresentationFramework]System.Windows.Controls.DataGridColumn::get_Header()
			IL_0055: isinst [mscorlib]System.String
			IL_005a: ldnull
			IL_005b: cgt.un
			IL_005d: stloc.s 4
			IL_005f: ldloc.s 4
			IL_0061: brfalse.s IL_0078

			IL_0063: nop
			IL_0064: ldloc.0
			IL_0065: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn> [PresentationFramework]System.Windows.Controls.DataGrid::get_Columns()
			IL_006a: ldloc.2
			IL_006b: callvirt instance !0 class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn>::get_Item(int32)
			IL_0070: ldc.i4.2
			IL_0071: callvirt instance void [PresentationFramework]System.Windows.Controls.DataGridColumn::set_Visibility(valuetype [PresentationCore]System.Windows.Visibility)
			IL_0076: nop
			IL_0077: nop

			IL_0078: nop
			IL_0079: ldloc.2
			IL_007a: ldc.i4.1
			IL_007b: add
			IL_007c: stloc.2

			IL_007d: ldloc.2
			IL_007e: ldloc.0
			IL_007f: callvirt instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn> [PresentationFramework]System.Windows.Controls.DataGrid::get_Columns()
			IL_0084: callvirt instance int32 class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.Controls.DataGridColumn>::get_Count()
			IL_0089: clt
			IL_008b: stloc.s 5
			IL_008d: ldloc.s 5
			IL_008f: brtrue.s IL_001b
		// end loop

		IL_0091: nop

		IL_0092: ret
	} // end of method GlobalVariableDialog::varDataGrid_AutoGeneratedColumns

	.method public final hidebysig newslot virtual 
		instance void InitializeComponent () cil managed 
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string, string) = (
			01 00 16 50 72 65 73 65 6e 74 61 74 69 6f 6e 42
			75 69 6c 64 54 61 73 6b 73 07 34 2e 30 2e 30 2e
			30 00 00
		)
		// Method begins at RVA 0x2a124
		// Header size: 12
		// Code size: 42 (0x2a)
		.maxstack 2
		.locals init (
			[0] class [System]System.Uri,
			[1] bool
		)

		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: ldfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_contentLoaded
		IL_0007: stloc.1
		IL_0008: ldloc.1
		IL_0009: brfalse.s IL_000e

		IL_000b: nop
		IL_000c: br.s IL_0029

		IL_000e: ldarg.0
		IL_000f: ldc.i4.1
		IL_0010: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_contentLoaded
		IL_0015: ldstr "/VMControls.WPF;component/globalui/globalvariable/globalvariabledialog.xaml"
		IL_001a: ldc.i4.2
		IL_001b: newobj instance void [System]System.Uri::.ctor(string, valuetype [System]System.UriKind)
		IL_0020: stloc.0
		IL_0021: ldarg.0
		IL_0022: ldloc.0
		IL_0023: call void [PresentationFramework]System.Windows.Application::LoadComponent(object, class [System]System.Uri)
		IL_0028: nop

		IL_0029: ret
	} // end of method GlobalVariableDialog::InitializeComponent

	.method private final hidebysig newslot virtual 
		instance void System.Windows.Markup.IComponentConnector.Connect (
			int32 connectionId,
			object target
		) cil managed 
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string, string) = (
			01 00 16 50 72 65 73 65 6e 74 61 74 69 6f 6e 42
			75 69 6c 64 54 61 73 6b 73 07 34 2e 30 2e 30 2e
			30 00 00
		)
		.custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = (
			01 00 01 00 00 00 00 00
		)
		.override method instance void [System.Xaml]System.Windows.Markup.IComponentConnector::Connect(int32, object)
		// Method begins at RVA 0x2a15c
		// Header size: 12
		// Code size: 759 (0x2f7)
		.maxstack 3
		.locals init (
			[0] int32
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: stloc.0
		IL_0003: ldloc.0
		IL_0004: ldc.i4.1
		IL_0005: sub
		IL_0006: switch (IL_0080, IL_0091, IL_02ef, IL_02ef, IL_02ef, IL_00a2, IL_00bf, IL_02ef, IL_00dc, IL_00ed, IL_010a, IL_0133, IL_0144, IL_0161, IL_017e, IL_01a7, IL_01b8, IL_01d5, IL_01f2, IL_021b, IL_0238, IL_0261, IL_0287, IL_02ad, IL_02d3, IL_02ef, IL_02ef, IL_02e1)

		IL_007b: br IL_02ef

		IL_0080: ldarg.0
		IL_0081: ldarg.2
		IL_0082: castclass VMControls.WPF.GlobalUI.GlobalVariableDialog
		IL_0087: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog::source
		IL_008c: br IL_02f6

		IL_0091: ldarg.0
		IL_0092: ldarg.2
		IL_0093: castclass [PresentationFramework]System.Windows.Controls.Image
		IL_0098: stfld class [PresentationFramework]System.Windows.Controls.Image VMControls.WPF.GlobalUI.GlobalVariableDialog::img
		IL_009d: br IL_02f6

		IL_00a2: ldarg.2
		IL_00a3: castclass [PresentationFramework]System.Windows.Controls.MenuItem
		IL_00a8: ldarg.0
		IL_00a9: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::DelVarGroup_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_00af: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_00b4: callvirt instance void [PresentationFramework]System.Windows.Controls.MenuItem::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_00b9: nop
		IL_00ba: br IL_02f6

		IL_00bf: ldarg.2
		IL_00c0: castclass [Apps.PcUI]Apps.PcUI.Common.SuperImageButton
		IL_00c5: ldarg.0
		IL_00c6: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::AddVarGroup_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_00cc: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_00d1: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_00d6: nop
		IL_00d7: br IL_02f6

		IL_00dc: ldarg.0
		IL_00dd: ldarg.2
		IL_00de: castclass [PresentationFramework]System.Windows.Controls.TextBox
		IL_00e3: stfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::txtFocus
		IL_00e8: br IL_02f6

		IL_00ed: ldarg.2
		IL_00ee: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_00f3: ldarg.0
		IL_00f4: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::AddItem_OnClick(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_00fa: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_00ff: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_0104: nop
		IL_0105: br IL_02f6

		IL_010a: ldarg.0
		IL_010b: ldarg.2
		IL_010c: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_0111: stfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnImport
		IL_0116: ldarg.0
		IL_0117: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnImport
		IL_011c: ldarg.0
		IL_011d: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::BtnImport_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0123: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_0128: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_012d: nop
		IL_012e: br IL_02f6

		IL_0133: ldarg.0
		IL_0134: ldarg.2
		IL_0135: castclass [PresentationFramework]System.Windows.Controls.Primitives.Popup
		IL_013a: stfld class [PresentationFramework]System.Windows.Controls.Primitives.Popup VMControls.WPF.GlobalUI.GlobalVariableDialog::PopImport
		IL_013f: br IL_02f6

		IL_0144: ldarg.2
		IL_0145: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_014a: ldarg.0
		IL_014b: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::LocalImport_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0151: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_0156: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_015b: nop
		IL_015c: br IL_02f6

		IL_0161: ldarg.2
		IL_0162: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_0167: ldarg.0
		IL_0168: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::RemoteImport_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_016e: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_0173: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_0178: nop
		IL_0179: br IL_02f6

		IL_017e: ldarg.0
		IL_017f: ldarg.2
		IL_0180: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_0185: stfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnExport
		IL_018a: ldarg.0
		IL_018b: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnExport
		IL_0190: ldarg.0
		IL_0191: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::BtnExport_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0197: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_019c: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_01a1: nop
		IL_01a2: br IL_02f6

		IL_01a7: ldarg.0
		IL_01a8: ldarg.2
		IL_01a9: castclass [PresentationFramework]System.Windows.Controls.Primitives.Popup
		IL_01ae: stfld class [PresentationFramework]System.Windows.Controls.Primitives.Popup VMControls.WPF.GlobalUI.GlobalVariableDialog::PopExport
		IL_01b3: br IL_02f6

		IL_01b8: ldarg.2
		IL_01b9: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_01be: ldarg.0
		IL_01bf: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::LocalExport_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_01c5: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_01ca: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_01cf: nop
		IL_01d0: br IL_02f6

		IL_01d5: ldarg.2
		IL_01d6: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_01db: ldarg.0
		IL_01dc: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::RemoteExport_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_01e2: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_01e7: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_01ec: nop
		IL_01ed: br IL_02f6

		IL_01f2: ldarg.0
		IL_01f3: ldarg.2
		IL_01f4: castclass [PresentationFramework]System.Windows.Controls.TextBox
		IL_01f9: stfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText
		IL_01fe: ldarg.0
		IL_01ff: ldfld class [PresentationFramework]System.Windows.Controls.TextBox VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText
		IL_0204: ldarg.0
		IL_0205: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchText_TextChanged(object, class [PresentationFramework]System.Windows.Controls.TextChangedEventArgs)
		IL_020b: newobj instance void [PresentationFramework]System.Windows.Controls.TextChangedEventHandler::.ctor(object, native int)
		IL_0210: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.TextBoxBase::add_TextChanged(class [PresentationFramework]System.Windows.Controls.TextChangedEventHandler)
		IL_0215: nop
		IL_0216: br IL_02f6

		IL_021b: ldarg.2
		IL_021c: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_0221: ldarg.0
		IL_0222: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::SearchItem_OnClick(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0228: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_022d: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_0232: nop
		IL_0233: br IL_02f6

		IL_0238: ldarg.0
		IL_0239: ldarg.2
		IL_023a: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_023f: stfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnTop
		IL_0244: ldarg.0
		IL_0245: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnTop
		IL_024a: ldarg.0
		IL_024b: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::BtnTop_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0251: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_0256: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_025b: nop
		IL_025c: br IL_02f6

		IL_0261: ldarg.0
		IL_0262: ldarg.2
		IL_0263: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_0268: stfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnUp
		IL_026d: ldarg.0
		IL_026e: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnUp
		IL_0273: ldarg.0
		IL_0274: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::BtnUp_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_027a: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_027f: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_0284: nop
		IL_0285: br.s IL_02f6

		IL_0287: ldarg.0
		IL_0288: ldarg.2
		IL_0289: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_028e: stfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnDown
		IL_0293: ldarg.0
		IL_0294: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::btnDown
		IL_0299: ldarg.0
		IL_029a: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::BtnDown_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_02a0: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_02a5: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_02aa: nop
		IL_02ab: br.s IL_02f6

		IL_02ad: ldarg.0
		IL_02ae: ldarg.2
		IL_02af: castclass [PresentationFramework]System.Windows.Controls.Button
		IL_02b4: stfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::LightSave
		IL_02b9: ldarg.0
		IL_02ba: ldfld class [PresentationFramework]System.Windows.Controls.Button VMControls.WPF.GlobalUI.GlobalVariableDialog::LightSave
		IL_02bf: ldarg.0
		IL_02c0: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::LightSave_Click(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_02c6: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_02cb: callvirt instance void [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::add_Click(class [PresentationCore]System.Windows.RoutedEventHandler)
		IL_02d0: nop
		IL_02d1: br.s IL_02f6

		IL_02d3: ldarg.0
		IL_02d4: ldarg.2
		IL_02d5: castclass [PresentationFramework]System.Windows.Controls.TabControl
		IL_02da: stfld class [PresentationFramework]System.Windows.Controls.TabControl VMControls.WPF.GlobalUI.GlobalVariableDialog::tabControl
		IL_02df: br.s IL_02f6

		IL_02e1: ldarg.0
		IL_02e2: ldarg.2
		IL_02e3: castclass [Apps.PcUI]Apps.PcUI.Controls.ProgressMask
		IL_02e8: stfld class [Apps.PcUI]Apps.PcUI.Controls.ProgressMask VMControls.WPF.GlobalUI.GlobalVariableDialog::windowShadow
		IL_02ed: br.s IL_02f6

		IL_02ef: ldarg.0
		IL_02f0: ldc.i4.1
		IL_02f1: stfld bool VMControls.WPF.GlobalUI.GlobalVariableDialog::_contentLoaded

		IL_02f6: ret
	} // end of method GlobalVariableDialog::System.Windows.Markup.IComponentConnector.Connect

	.method private final hidebysig newslot virtual 
		instance void System.Windows.Markup.IStyleConnector.Connect (
			int32 connectionId,
			object target
		) cil managed 
	{
		.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [System]System.CodeDom.Compiler.GeneratedCodeAttribute::.ctor(string, string) = (
			01 00 16 50 72 65 73 65 6e 74 61 74 69 6f 6e 42
			75 69 6c 64 54 61 73 6b 73 07 34 2e 30 2e 30 2e
			30 00 00
		)
		.custom instance void [System]System.ComponentModel.EditorBrowsableAttribute::.ctor(valuetype [System]System.ComponentModel.EditorBrowsableState) = (
			01 00 01 00 00 00 00 00
		)
		.override method instance void [PresentationFramework]System.Windows.Markup.IStyleConnector::Connect(int32, object)
		// Method begins at RVA 0x2a460
		// Header size: 12
		// Code size: 487 (0x1e7)
		.maxstack 3
		.locals init (
			[0] class [PresentationFramework]System.Windows.EventSetter,
			[1] int32
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: stloc.1
		IL_0003: ldloc.1
		IL_0004: ldc.i4.3
		IL_0005: sub
		IL_0006: switch (IL_003c, IL_0078, IL_0095, IL_01e6, IL_01e6, IL_013f)

		IL_0023: br.s IL_0025

		IL_0025: ldloc.1
		IL_0026: ldc.i4.s 26
		IL_0028: beq IL_015c

		IL_002d: br.s IL_002f

		IL_002f: ldloc.1
		IL_0030: ldc.i4.s 27
		IL_0032: beq IL_0176

		IL_0037: br IL_01e6

		IL_003c: newobj instance void [PresentationFramework]System.Windows.EventSetter::.ctor()
		IL_0041: stloc.0
		IL_0042: ldloc.0
		IL_0043: ldsfld class [PresentationCore]System.Windows.RoutedEvent [PresentationFramework]System.Windows.Controls.Primitives.ButtonBase::ClickEvent
		IL_0048: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Event(class [PresentationCore]System.Windows.RoutedEvent)
		IL_004d: nop
		IL_004e: ldloc.0
		IL_004f: ldarg.0
		IL_0050: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::Delete_OnHandler(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0056: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_005b: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Handler(class [mscorlib]System.Delegate)
		IL_0060: nop
		IL_0061: ldarg.2
		IL_0062: castclass [PresentationFramework]System.Windows.Style
		IL_0067: callvirt instance class [PresentationFramework]System.Windows.SetterBaseCollection [PresentationFramework]System.Windows.Style::get_Setters()
		IL_006c: ldloc.0
		IL_006d: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.SetterBase>::Add(!0)
		IL_0072: nop
		IL_0073: br IL_01e6

		IL_0078: ldarg.2
		IL_0079: castclass [PresentationFramework]System.Windows.Controls.Grid
		IL_007e: ldarg.0
		IL_007f: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::GridGlob_MouseDown(object, class [PresentationCore]System.Windows.Input.MouseButtonEventArgs)
		IL_0085: newobj instance void [PresentationCore]System.Windows.Input.MouseButtonEventHandler::.ctor(object, native int)
		IL_008a: callvirt instance void [PresentationCore]System.Windows.UIElement::add_MouseDown(class [PresentationCore]System.Windows.Input.MouseButtonEventHandler)
		IL_008f: nop
		IL_0090: br IL_01e6

		IL_0095: newobj instance void [PresentationFramework]System.Windows.EventSetter::.ctor()
		IL_009a: stloc.0
		IL_009b: ldloc.0
		IL_009c: ldsfld class [PresentationCore]System.Windows.RoutedEvent [PresentationCore]System.Windows.UIElement::GotFocusEvent
		IL_00a1: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Event(class [PresentationCore]System.Windows.RoutedEvent)
		IL_00a6: nop
		IL_00a7: ldloc.0
		IL_00a8: ldarg.0
		IL_00a9: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::GotFocus_OnHandler(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_00af: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_00b4: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Handler(class [mscorlib]System.Delegate)
		IL_00b9: nop
		IL_00ba: ldarg.2
		IL_00bb: castclass [PresentationFramework]System.Windows.Style
		IL_00c0: callvirt instance class [PresentationFramework]System.Windows.SetterBaseCollection [PresentationFramework]System.Windows.Style::get_Setters()
		IL_00c5: ldloc.0
		IL_00c6: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.SetterBase>::Add(!0)
		IL_00cb: nop
		IL_00cc: newobj instance void [PresentationFramework]System.Windows.EventSetter::.ctor()
		IL_00d1: stloc.0
		IL_00d2: ldloc.0
		IL_00d3: ldsfld class [PresentationCore]System.Windows.RoutedEvent [PresentationCore]System.Windows.UIElement::LostFocusEvent
		IL_00d8: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Event(class [PresentationCore]System.Windows.RoutedEvent)
		IL_00dd: nop
		IL_00de: ldloc.0
		IL_00df: ldarg.0
		IL_00e0: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::LostFocus_OnHandler(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_00e6: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_00eb: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Handler(class [mscorlib]System.Delegate)
		IL_00f0: nop
		IL_00f1: ldarg.2
		IL_00f2: castclass [PresentationFramework]System.Windows.Style
		IL_00f7: callvirt instance class [PresentationFramework]System.Windows.SetterBaseCollection [PresentationFramework]System.Windows.Style::get_Setters()
		IL_00fc: ldloc.0
		IL_00fd: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.SetterBase>::Add(!0)
		IL_0102: nop
		IL_0103: newobj instance void [PresentationFramework]System.Windows.EventSetter::.ctor()
		IL_0108: stloc.0
		IL_0109: ldloc.0
		IL_010a: ldsfld class [PresentationCore]System.Windows.RoutedEvent [PresentationCore]System.Windows.UIElement::PreviewKeyDownEvent
		IL_010f: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Event(class [PresentationCore]System.Windows.RoutedEvent)
		IL_0114: nop
		IL_0115: ldloc.0
		IL_0116: ldarg.0
		IL_0117: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::TextBox_PreviewKeyDown(object, class [PresentationCore]System.Windows.Input.KeyEventArgs)
		IL_011d: newobj instance void [PresentationCore]System.Windows.Input.KeyEventHandler::.ctor(object, native int)
		IL_0122: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Handler(class [mscorlib]System.Delegate)
		IL_0127: nop
		IL_0128: ldarg.2
		IL_0129: castclass [PresentationFramework]System.Windows.Style
		IL_012e: callvirt instance class [PresentationFramework]System.Windows.SetterBaseCollection [PresentationFramework]System.Windows.Style::get_Setters()
		IL_0133: ldloc.0
		IL_0134: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.SetterBase>::Add(!0)
		IL_0139: nop
		IL_013a: br IL_01e6

		IL_013f: ldarg.2
		IL_0140: castclass [PresentationFramework]System.Windows.Controls.ScrollViewer
		IL_0145: ldarg.0
		IL_0146: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::Sv_ScrollChanged(object, class [PresentationFramework]System.Windows.Controls.ScrollChangedEventArgs)
		IL_014c: newobj instance void [PresentationFramework]System.Windows.Controls.ScrollChangedEventHandler::.ctor(object, native int)
		IL_0151: callvirt instance void [PresentationFramework]System.Windows.Controls.ScrollViewer::add_ScrollChanged(class [PresentationFramework]System.Windows.Controls.ScrollChangedEventHandler)
		IL_0156: nop
		IL_0157: br IL_01e6

		IL_015c: ldarg.2
		IL_015d: castclass [PresentationFramework]System.Windows.Controls.DataGrid
		IL_0162: ldarg.0
		IL_0163: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::varDataGrid_AutoGeneratedColumns(object, class [mscorlib]System.EventArgs)
		IL_0169: newobj instance void [mscorlib]System.EventHandler::.ctor(object, native int)
		IL_016e: callvirt instance void [PresentationFramework]System.Windows.Controls.DataGrid::add_AutoGeneratedColumns(class [mscorlib]System.EventHandler)
		IL_0173: nop
		IL_0174: br.s IL_01e6

		IL_0176: newobj instance void [PresentationFramework]System.Windows.EventSetter::.ctor()
		IL_017b: stloc.0
		IL_017c: ldloc.0
		IL_017d: ldsfld class [PresentationCore]System.Windows.RoutedEvent [PresentationCore]System.Windows.UIElement::LostFocusEvent
		IL_0182: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Event(class [PresentationCore]System.Windows.RoutedEvent)
		IL_0187: nop
		IL_0188: ldloc.0
		IL_0189: ldarg.0
		IL_018a: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::LostFocus_OnHandler(object, class [PresentationCore]System.Windows.RoutedEventArgs)
		IL_0190: newobj instance void [PresentationCore]System.Windows.RoutedEventHandler::.ctor(object, native int)
		IL_0195: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Handler(class [mscorlib]System.Delegate)
		IL_019a: nop
		IL_019b: ldarg.2
		IL_019c: castclass [PresentationFramework]System.Windows.Style
		IL_01a1: callvirt instance class [PresentationFramework]System.Windows.SetterBaseCollection [PresentationFramework]System.Windows.Style::get_Setters()
		IL_01a6: ldloc.0
		IL_01a7: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.SetterBase>::Add(!0)
		IL_01ac: nop
		IL_01ad: newobj instance void [PresentationFramework]System.Windows.EventSetter::.ctor()
		IL_01b2: stloc.0
		IL_01b3: ldloc.0
		IL_01b4: ldsfld class [PresentationCore]System.Windows.RoutedEvent [PresentationCore]System.Windows.UIElement::PreviewKeyDownEvent
		IL_01b9: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Event(class [PresentationCore]System.Windows.RoutedEvent)
		IL_01be: nop
		IL_01bf: ldloc.0
		IL_01c0: ldarg.0
		IL_01c1: ldftn instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::TextBox_PreviewKeyDown(object, class [PresentationCore]System.Windows.Input.KeyEventArgs)
		IL_01c7: newobj instance void [PresentationCore]System.Windows.Input.KeyEventHandler::.ctor(object, native int)
		IL_01cc: callvirt instance void [PresentationFramework]System.Windows.EventSetter::set_Handler(class [mscorlib]System.Delegate)
		IL_01d1: nop
		IL_01d2: ldarg.2
		IL_01d3: castclass [PresentationFramework]System.Windows.Style
		IL_01d8: callvirt instance class [PresentationFramework]System.Windows.SetterBaseCollection [PresentationFramework]System.Windows.Style::get_Setters()
		IL_01dd: ldloc.0
		IL_01de: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class [PresentationFramework]System.Windows.SetterBase>::Add(!0)
		IL_01e3: nop
		IL_01e4: br.s IL_01e6

		IL_01e6: ret
	} // end of method GlobalVariableDialog::System.Windows.Markup.IStyleConnector.Connect

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x2a654
		// Header size: 12
		// Code size: 88 (0x58)
		.maxstack 4

		IL_0000: ldstr "ItemsTotalCount"
		IL_0005: ldtoken [mscorlib]System.Int32
		IL_000a: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_000f: ldtoken VMControls.WPF.GlobalUI.GlobalVariableDialog
		IL_0014: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_0019: ldc.i4.0
		IL_001a: box [mscorlib]System.Int32
		IL_001f: newobj instance void [WindowsBase]System.Windows.PropertyMetadata::.ctor(object)
		IL_0024: call class [WindowsBase]System.Windows.DependencyProperty [WindowsBase]System.Windows.DependencyProperty::Register(string, class [mscorlib]System.Type, class [mscorlib]System.Type, class [WindowsBase]System.Windows.PropertyMetadata)
		IL_0029: stsfld class [WindowsBase]System.Windows.DependencyProperty VMControls.WPF.GlobalUI.GlobalVariableDialog::ItemsTotalCountProperty
		IL_002e: ldstr "Items"
		IL_0033: ldtoken class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>
		IL_0038: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_003d: ldtoken VMControls.WPF.GlobalUI.GlobalVariableDialog
		IL_0042: call class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
		IL_0047: ldnull
		IL_0048: newobj instance void [WindowsBase]System.Windows.PropertyMetadata::.ctor(object)
		IL_004d: call class [WindowsBase]System.Windows.DependencyProperty [WindowsBase]System.Windows.DependencyProperty::Register(string, class [mscorlib]System.Type, class [mscorlib]System.Type, class [WindowsBase]System.Windows.PropertyMetadata)
		IL_0052: stsfld class [WindowsBase]System.Windows.DependencyProperty VMControls.WPF.GlobalUI.GlobalVariableDialog::ItemsProperty
		IL_0057: ret
	} // end of method GlobalVariableDialog::.cctor

	.method private hidebysig 
		instance bool '<UpdatePageData>b__98_0' (
			class VMControls.WPF.GlobalUI.GlobalVariableItem x
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a6b8
		// Header size: 1
		// Code size: 23 (0x17)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
		IL_0006: ldarg.0
		IL_0007: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_000c: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
		IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0016: ret
	} // end of method GlobalVariableDialog::'<UpdatePageData>b__98_0'

	.method private hidebysig 
		instance bool '<SetBindData>b__100_0' (
			class VMControls.WPF.GlobalUI.GlobalVariableItem x
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a6d0
		// Header size: 1
		// Code size: 23 (0x17)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
		IL_0006: ldarg.0
		IL_0007: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_000c: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
		IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0016: ret
	} // end of method GlobalVariableDialog::'<SetBindData>b__100_0'

	.method private hidebysig 
		instance class [mscorlib]System.Threading.Tasks.Task '<GetDataAsync>b__103_0' () cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = (
			01 00 46 56 4d 43 6f 6e 74 72 6f 6c 73 2e 57 50
			46 2e 47 6c 6f 62 61 6c 55 49 2e 47 6c 6f 62 61
			6c 56 61 72 69 61 62 6c 65 44 69 61 6c 6f 67 2b
			3c 3c 47 65 74 44 61 74 61 41 73 79 6e 63 3e 62
			5f 5f 31 30 33 5f 30 3e 64 00 00
		)
		.custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a6e8
		// Header size: 12
		// Code size: 59 (0x3b)
		.maxstack 2
		.locals init (
			[0] class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d',
			[1] valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder
		)

		IL_0000: newobj instance void VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.0
		IL_0008: stfld class VMControls.WPF.GlobalUI.GlobalVariableDialog VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>4__this'
		IL_000d: ldloc.0
		IL_000e: call valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
		IL_0013: stfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>t__builder'
		IL_0018: ldloc.0
		IL_0019: ldc.i4.m1
		IL_001a: stfld int32 VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>1__state'
		IL_001f: ldloc.0
		IL_0020: ldfld valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>t__builder'
		IL_0025: stloc.1
		IL_0026: ldloca.s 1
		IL_0028: ldloca.s 0
		IL_002a: call instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'>(!!0&)
		IL_002f: ldloc.0
		IL_0030: ldflda valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder VMControls.WPF.GlobalUI.GlobalVariableDialog/'<<GetDataAsync>b__103_0>d'::'<>t__builder'
		IL_0035: call instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
		IL_003a: ret
	} // end of method GlobalVariableDialog::'<GetDataAsync>b__103_0'

	.method private hidebysig 
		instance bool '<UpdateBtnEnable>b__120_0' (
			class VMControls.WPF.GlobalUI.GlobalVariableItem x
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a72f
		// Header size: 1
		// Code size: 23 (0x17)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
		IL_0006: ldarg.0
		IL_0007: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_000c: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
		IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0016: ret
	} // end of method GlobalVariableDialog::'<UpdateBtnEnable>b__120_0'

	.method private hidebysig 
		instance bool '<BtnUp_Click>b__122_0' (
			class VMControls.WPF.GlobalUI.GlobalVariableItem x
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a747
		// Header size: 1
		// Code size: 23 (0x17)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
		IL_0006: ldarg.0
		IL_0007: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_000c: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
		IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0016: ret
	} // end of method GlobalVariableDialog::'<BtnUp_Click>b__122_0'

	.method private hidebysig 
		instance bool '<BtnDown_Click>b__123_0' (
			class VMControls.WPF.GlobalUI.GlobalVariableItem x
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a75f
		// Header size: 1
		// Code size: 23 (0x17)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: callvirt instance string VMControls.WPF.GlobalUI.GlobalVariableItem::get_VarGroupInfo()
		IL_0006: ldarg.0
		IL_0007: call instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		IL_000c: callvirt instance string VMControls.WPF.GlobalUI.VarGroupItem::get_Name()
		IL_0011: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_0016: ret
	} // end of method GlobalVariableDialog::'<BtnDown_Click>b__123_0'

	.method private hidebysig 
		instance void '<GetVarGroupMsg>b__137_0' (
			string x
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2a778
		// Header size: 12
		// Code size: 34 (0x22)
		.maxstack 3
		.locals init (
			[0] bool
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: call bool [mscorlib]System.String::IsNullOrEmpty(string)
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: brfalse.s IL_000e

		IL_000b: nop
		IL_000c: br.s IL_0021

		IL_000e: ldarg.0
		IL_000f: call instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		IL_0014: ldarg.1
		IL_0015: ldarg.0
		IL_0016: newobj instance void VMControls.WPF.GlobalUI.VarGroupItem::.ctor(string, class VMControls.WPF.GlobalUI.IVarGroup)
		IL_001b: callvirt instance void class [mscorlib]System.Collections.ObjectModel.Collection`1<class VMControls.WPF.GlobalUI.VarGroupItem>::Add(!0)
		IL_0020: nop

		IL_0021: ret
	} // end of method GlobalVariableDialog::'<GetVarGroupMsg>b__137_0'

	// Properties
	.property instance class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot ParamsConfig()
	{
		.get instance class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ParamsConfig()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ParamsConfig(class [Apps.XmlParser]Apps.XmlParser.ParamTab.AlgorithmTabRoot)
	}
	.property instance object DecorateSource()
	{
		.get instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DecorateSource()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_DecorateSource(object)
	}
	.property instance string DisplayName()
	{
		.get instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_DisplayName()
	}
	.property instance float64 ConfigWidth()
	{
		.get instance float64 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ConfigWidth()
	}
	.property instance float64 ComInitWidth()
	{
		.get instance float64 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ComInitWidth()
	}
	.property instance bool IsVisibleComInit()
	{
		.get instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsVisibleComInit()
	}
	.property instance class VMControls.WPF.GlobalUI.GlobalVariableItem SelectedVarItem()
	{
		.get instance class VMControls.WPF.GlobalUI.GlobalVariableItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectedVarItem()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectedVarItem(class VMControls.WPF.GlobalUI.GlobalVariableItem)
	}
	.property instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VarItemGroups()
	{
		.get instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_VarItemGroups()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_VarItemGroups(class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.VarGroupItem>)
	}
	.property instance class VMControls.WPF.GlobalUI.VarGroupItem SelectGroupItem()
	{
		.get instance class VMControls.WPF.GlobalUI.VarGroupItem VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SelectGroupItem()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_SelectGroupItem(class VMControls.WPF.GlobalUI.VarGroupItem)
	}
	.property instance int32 ItemsTotalCount()
	{
		.get instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ItemsTotalCount()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ItemsTotalCount(int32)
	}
	.property instance int32 CurrentPageNum()
	{
		.get instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_CurrentPageNum()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_CurrentPageNum(int32)
	}
	.property instance string EmptyStatus()
	{
		.get instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_EmptyStatus()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_EmptyStatus(string)
	}
	.property instance string PageNumShowStatus()
	{
		.get instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageNumShowStatus()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_PageNumShowStatus(string)
	}
	.property instance string IsShowProcessbar()
	{
		.get instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsShowProcessbar()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsShowProcessbar(string)
	}
	.property instance string ProcessbarMessage()
	{
		.get instance string VMControls.WPF.GlobalUI.GlobalVariableDialog::get_ProcessbarMessage()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_ProcessbarMessage(string)
	}
	.property instance int32 PageCount()
	{
		.get instance int32 VMControls.WPF.GlobalUI.GlobalVariableDialog::get_PageCount()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_PageCount(int32)
	}
	.property instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> Items()
	{
		.get instance class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem> VMControls.WPF.GlobalUI.GlobalVariableDialog::get_Items()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_Items(class [System]System.Collections.ObjectModel.ObservableCollection`1<class VMControls.WPF.GlobalUI.GlobalVariableItem>)
	}
	.property instance bool IsAllowOperat()
	{
		.get instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsAllowOperat()
		.set instance void VMControls.WPF.GlobalUI.GlobalVariableDialog::set_IsAllowOperat(bool)
	}
	.property instance object SolutionData()
	{
		.get instance object VMControls.WPF.GlobalUI.GlobalVariableDialog::get_SolutionData()
	}
	.property instance bool IsEdit()
	{
		.get instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsEdit()
	}
	.property instance bool IsLoading()
	{
		.get instance bool VMControls.WPF.GlobalUI.GlobalVariableDialog::get_IsLoading()
	}

} // end of class VMControls.WPF.GlobalUI.GlobalVariableDialog
