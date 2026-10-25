// IL code: VM.User
.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi sealed VM.User.AccessUser
	extends [mscorlib]System.Enum
{
	// Fields
	.field public specialname rtspecialname int32 value__
	.field public static literal valuetype VM.User.AccessUser None = int32(0)
	.field public static literal valuetype VM.User.AccessUser Admin = int32(1)
	.field public static literal valuetype VM.User.AccessUser Tech = int32(2)
	.field public static literal valuetype VM.User.AccessUser Operator = int32(3)

} // end of class VM.User.AccessUser

.class public sequential ansi sealed beforefieldinit VM.User.AccessUserInfo
	extends [mscorlib]System.ValueType
{
	.pack 1
	.size 0

	// Fields
	.field public valuetype VM.User.AccessUser curLoginUser
	.field public bool adminEnable
	.field public marshal(fixed sysstring[32]) string adminCode
	.field public bool techEnable
	.field public marshal(fixed sysstring[32]) string techCode
	.field public bool operatorEnable
	.field public marshal(fixed sysstring[32]) string operatorCode
	.field public marshal(fixed sysstring[64]) string IDInfo

} // end of class VM.User.AccessUserInfo

.class public auto ansi beforefieldinit VM.User.CodeManager
	extends [Apps.Data]Apps.Data.NotifyPropertyChangedBase
{
	// Nested Types
	.class nested public auto ansi sealed AccessEnableCallBack
		extends [mscorlib]System.MulticastDelegate
	{
		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor (
				object 'object',
				native int 'method'
			) runtime managed 
		{
		} // end of method AccessEnableCallBack::.ctor

		.method public hidebysig newslot virtual 
			instance bool Invoke (
				bool bEnable,
				valuetype VM.User.AccessUser curLoginUser
			) runtime managed 
		{
		} // end of method AccessEnableCallBack::Invoke

		.method public hidebysig newslot virtual 
			instance class [mscorlib]System.IAsyncResult BeginInvoke (
				bool bEnable,
				valuetype VM.User.AccessUser curLoginUser,
				class [mscorlib]System.AsyncCallback callback,
				object 'object'
			) runtime managed 
		{
		} // end of method AccessEnableCallBack::BeginInvoke

		.method public hidebysig newslot virtual 
			instance bool EndInvoke (
				class [mscorlib]System.IAsyncResult result
			) runtime managed 
		{
		} // end of method AccessEnableCallBack::EndInvoke

	} // end of class AccessEnableCallBack

	.class nested public auto ansi sealed IsAllAccessCallBack
		extends [mscorlib]System.MulticastDelegate
	{
		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor (
				object 'object',
				native int 'method'
			) runtime managed 
		{
		} // end of method IsAllAccessCallBack::.ctor

		.method public hidebysig newslot virtual 
			instance bool Invoke (
				bool bEnable
			) runtime managed 
		{
		} // end of method IsAllAccessCallBack::Invoke

		.method public hidebysig newslot virtual 
			instance class [mscorlib]System.IAsyncResult BeginInvoke (
				bool bEnable,
				class [mscorlib]System.AsyncCallback callback,
				object 'object'
			) runtime managed 
		{
		} // end of method IsAllAccessCallBack::BeginInvoke

		.method public hidebysig newslot virtual 
			instance bool EndInvoke (
				class [mscorlib]System.IAsyncResult result
			) runtime managed 
		{
		} // end of method IsAllAccessCallBack::EndInvoke

	} // end of class IsAllAccessCallBack

	.class nested private auto ansi sealed beforefieldinit '<>c__DisplayClass37_0'
		extends [mscorlib]System.Object
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Fields
		.field public string nodename

		// Methods
		.method public hidebysig specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x26d0
			// Header size: 1
			// Code size: 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: call instance void [mscorlib]System.Object::.ctor()
			IL_0006: ret
		} // end of method '<>c__DisplayClass37_0'::.ctor

		.method assembly hidebysig 
			instance bool '<GetElement>b__0' (
				class [System.Xml.Linq]System.Xml.Linq.XElement p
			) cil managed 
		{
			// Method begins at RVA 0x26d8
			// Header size: 1
			// Code size: 23 (0x17)
			.maxstack 8

			IL_0000: ldarg.1
			IL_0001: callvirt instance class [System.Xml.Linq]System.Xml.Linq.XName [System.Xml.Linq]System.Xml.Linq.XElement::get_Name()
			IL_0006: ldarg.0
			IL_0007: ldfld string VM.User.CodeManager/'<>c__DisplayClass37_0'::nodename
			IL_000c: call class [System.Xml.Linq]System.Xml.Linq.XName [System.Xml.Linq]System.Xml.Linq.XName::op_Implicit(string)
			IL_0011: call bool [System.Xml.Linq]System.Xml.Linq.XName::op_Equality(class [System.Xml.Linq]System.Xml.Linq.XName, class [System.Xml.Linq]System.Xml.Linq.XName)
			IL_0016: ret
		} // end of method '<>c__DisplayClass37_0'::'<GetElement>b__0'

	} // end of class <>c__DisplayClass37_0


	// Fields
	.field public class VM.User.CodeManager/AccessEnableCallBack OnAccessEnableCallBack
	.field public class VM.User.CodeManager/IsAllAccessCallBack OnAllAccessCallBack
	.field public valuetype VM.User.AccessUserInfo _accessCurUserInfo
	.field public string accessInfoNodeName
	.field private string SoftEncryptNodeName
	.field private static object syncObj
	.field public static class VM.User.CodeManager 'instance'
	.field private bool _isAdmin

	// Methods
	.method private hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 36 (0x24)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldstr "AccessInfo"
		IL_0006: stfld string VM.User.CodeManager::accessInfoNodeName
		IL_000b: ldarg.0
		IL_000c: ldstr "IsSoftEncrypt"
		IL_0011: stfld string VM.User.CodeManager::SoftEncryptNodeName
		IL_0016: ldarg.0
		IL_0017: ldc.i4.1
		IL_0018: stfld bool VM.User.CodeManager::_isAdmin
		IL_001d: ldarg.0
		IL_001e: call instance void [Apps.Data]Apps.Data.NotifyPropertyChangedBase::.ctor()
		IL_0023: ret
	} // end of method CodeManager::.ctor

	.method family hidebysig virtual 
		instance void Finalize () cil managed 
	{
		.override method instance void [mscorlib]System.Object::Finalize()
		// Method begins at RVA 0x2078
		// Header size: 12
		// Code size: 10 (0xa)
		.maxstack 1

		.try
		{
			IL_0000: leave.s IL_0009
		} // end .try
		finally
		{
			IL_0002: ldarg.0
			IL_0003: call instance void [mscorlib]System.Object::Finalize()
			IL_0008: endfinally
		} // end handler

		IL_0009: ret
	} // end of method CodeManager::Finalize

	.method public hidebysig static 
		class VM.User.CodeManager GetInstance () cil managed 
	{
		// Method begins at RVA 0x20a0
		// Header size: 12
		// Code size: 53 (0x35)
		.maxstack 2
		.locals init (
			[0] object,
			[1] bool,
			[2] class VM.User.CodeManager
		)

		IL_0000: ldsfld object VM.User.CodeManager::syncObj
		IL_0005: stloc.0
		IL_0006: ldc.i4.0
		IL_0007: stloc.1
		.try
		{
			IL_0008: ldloc.0
			IL_0009: ldloca.s 1
			IL_000b: call void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
			IL_0010: ldsfld class VM.User.CodeManager VM.User.CodeManager::'instance'
			IL_0015: brtrue.s IL_0021

			IL_0017: newobj instance void VM.User.CodeManager::.ctor()
			IL_001c: stsfld class VM.User.CodeManager VM.User.CodeManager::'instance'

			IL_0021: ldsfld class VM.User.CodeManager VM.User.CodeManager::'instance'
			IL_0026: stloc.2
			IL_0027: leave.s IL_0033
		} // end .try
		finally
		{
			IL_0029: ldloc.1
			IL_002a: brfalse.s IL_0032

			IL_002c: ldloc.0
			IL_002d: call void [mscorlib]System.Threading.Monitor::Exit(object)

			IL_0032: endfinally
		} // end handler

		IL_0033: ldloc.2
		IL_0034: ret
	} // end of method CodeManager::GetInstance

	.method public hidebysig specialname static 
		class VM.User.CodeManager get_Instance () cil managed 
	{
		// Method begins at RVA 0x20f4
		// Header size: 1
		// Code size: 6 (0x6)
		.maxstack 8

		IL_0000: call class VM.User.CodeManager VM.User.CodeManager::GetInstance()
		IL_0005: ret
	} // end of method CodeManager::get_Instance

	.method public hidebysig 
		instance bool IsUseSoftEncrypt () cil managed 
	{
		// Method begins at RVA 0x20fc
		// Header size: 12
		// Code size: 45 (0x2d)
		.maxstack 2
		.locals init (
			[0] bool,
			[1] class [mscorlib]System.Exception
		)

		.try
		{
			IL_0000: ldarg.0
			IL_0001: ldfld string VM.User.CodeManager::SoftEncryptNodeName
			IL_0006: call string [VM.Utility]VM.Utility.ConfigurationHelper::GetConfig(string)
			IL_000b: call bool [mscorlib]System.Convert::ToBoolean(string)
			IL_0010: stloc.0
			IL_0011: leave.s IL_002b
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_0013: stloc.1
			IL_0014: ldstr "读取软加密信息失败:"
			IL_0019: ldloc.1
			IL_001a: callvirt instance string [mscorlib]System.Exception::get_Message()
			IL_001f: call string [mscorlib]System.String::Concat(string, string)
			IL_0024: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
			IL_0029: ldloc.1
			IL_002a: throw
		} // end handler

		IL_002b: ldloc.0
		IL_002c: ret
	} // end of method CodeManager::IsUseSoftEncrypt

	.method public hidebysig 
		instance bool IsAllAccessUser () cil managed 
	{
		// Method begins at RVA 0x2148
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_000b: ldc.i4.1
		IL_000c: beq.s IL_001b

		IL_000e: ldarg.0
		IL_000f: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0014: ldfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_0019: brtrue.s IL_001d

		IL_001b: ldc.i4.1
		IL_001c: ret

		IL_001d: ldc.i4.0
		IL_001e: ret
	} // end of method CodeManager::IsAllAccessUser

	.method public hidebysig specialname 
		instance bool get_IsAccessEnable () cil managed 
	{
		// Method begins at RVA 0x2168
		// Header size: 1
		// Code size: 15 (0xf)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_000b: ldc.i4.0
		IL_000c: cgt.un
		IL_000e: ret
	} // end of method CodeManager::get_IsAccessEnable

	.method public hidebysig specialname 
		instance bool get_IsAdmin () cil managed 
	{
		// Method begins at RVA 0x2178
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld bool VM.User.CodeManager::_isAdmin
		IL_0006: ret
	} // end of method CodeManager::get_IsAdmin

	.method public hidebysig specialname 
		instance void set_IsAdmin (
			bool 'value'
		) cil managed 
	{
		// Method begins at RVA 0x2180
		// Header size: 1
		// Code size: 28 (0x1c)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld bool VM.User.CodeManager::_isAdmin
		IL_0006: ldarg.1
		IL_0007: beq.s IL_001b

		IL_0009: ldarg.0
		IL_000a: ldarg.1
		IL_000b: stfld bool VM.User.CodeManager::_isAdmin
		IL_0010: ldarg.0
		IL_0011: ldstr "IsAdmin"
		IL_0016: callvirt instance void [Apps.Data]Apps.Data.NotifyPropertyChangedBase::OnPropertyChanged(string)

		IL_001b: ret
	} // end of method CodeManager::set_IsAdmin

	.method public hidebysig specialname 
		instance string get_UserName () cil managed 
	{
		// Method begins at RVA 0x21a0
		// Header size: 12
		// Code size: 74 (0x4a)
		.maxstack 1
		.locals init (
			[0] string,
			[1] valuetype VM.User.AccessUser
		)

		IL_0000: ldsfld string [mscorlib]System.String::Empty
		IL_0005: stloc.0
		IL_0006: ldarg.0
		IL_0007: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_000c: ldfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_0011: stloc.1
		IL_0012: ldloc.1
		IL_0013: switch (IL_002a, IL_0032, IL_003a, IL_0042)

		IL_0028: br.s IL_0048

		IL_002a: ldsfld string [mscorlib]System.String::Empty
		IL_002f: stloc.0
		IL_0030: br.s IL_0048

		IL_0032: ldstr "Lang_Role_Admin"
		IL_0037: stloc.0
		IL_0038: br.s IL_0048

		IL_003a: ldstr "Lang_Role_Tech"
		IL_003f: stloc.0
		IL_0040: br.s IL_0048

		IL_0042: ldstr "Lang_Role_Operator"
		IL_0047: stloc.0

		IL_0048: ldloc.0
		IL_0049: ret
	} // end of method CodeManager::get_UserName

	.method public hidebysig 
		instance bool SaveAccessInfo () cil managed 
	{
		// Method begins at RVA 0x21f8
		// Header size: 12
		// Code size: 87 (0x57)
		.maxstack 4
		.locals init (
			[0] string,
			[1] class [mscorlib]System.Exception,
			[2] bool
		)

		.try
		{
			IL_0000: ldarg.0
			IL_0001: ldarg.0
			IL_0002: ldfld valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
			IL_0007: box VM.User.AccessUserInfo
			IL_000c: call instance uint8[] VM.User.CodeManager::StructTOBytes(object)
			IL_0011: call string [mscorlib]System.Convert::ToBase64String(uint8[])
			IL_0016: stloc.0
			IL_0017: ldarg.0
			IL_0018: ldfld string VM.User.CodeManager::accessInfoNodeName
			IL_001d: ldloc.0
			IL_001e: call void [VM.Utility]VM.Utility.ConfigurationHelper::SaveConfig(string, string)
			IL_0023: ldarg.0
			IL_0024: ldstr "IsAccessEnable"
			IL_0029: callvirt instance void [Apps.Data]Apps.Data.NotifyPropertyChangedBase::OnPropertyChanged(string)
			IL_002e: ldarg.0
			IL_002f: ldstr "IsAdmin"
			IL_0034: callvirt instance void [Apps.Data]Apps.Data.NotifyPropertyChangedBase::OnPropertyChanged(string)
			IL_0039: leave.s IL_0053
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_003b: stloc.1
			IL_003c: ldstr "Lang_CfgSaveErr"
			IL_0041: ldloc.1
			IL_0042: callvirt instance string [mscorlib]System.Exception::get_Message()
			IL_0047: ldc.i4.2
			IL_0048: ldc.i4.0
			IL_0049: call valuetype [VM.Controls]VM.Controls.DialogResult [VM.Controls]VM.Controls.HikMessageBox::Show(string, string, valuetype [VM.Controls]VM.Controls.MessageIcons, bool)
			IL_004e: pop
			IL_004f: ldc.i4.0
			IL_0050: stloc.2
			IL_0051: leave.s IL_0055
		} // end handler

		IL_0053: ldc.i4.1
		IL_0054: ret

		IL_0055: ldloc.2
		IL_0056: ret
	} // end of method CodeManager::SaveAccessInfo

	.method public hidebysig 
		instance valuetype VM.User.AccessUser GetLoginUser () cil managed 
	{
		// Method begins at RVA 0x226c
		// Header size: 1
		// Code size: 12 (0xc)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_000b: ret
	} // end of method CodeManager::GetLoginUser

	.method public hidebysig static 
		bool CheckCodeFormat (
			string tempCode
		) cil managed 
	{
		// Method begins at RVA 0x2279
		// Header size: 1
		// Code size: 23 (0x17)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_0006: ldc.i4.3
		IL_0007: blt.s IL_0013

		IL_0009: ldarg.0
		IL_000a: callvirt instance int32 [mscorlib]System.String::get_Length()
		IL_000f: ldc.i4.s 15
		IL_0011: ble.s IL_0015

		IL_0013: ldc.i4.0
		IL_0014: ret

		IL_0015: ldc.i4.1
		IL_0016: ret
	} // end of method CodeManager::CheckCodeFormat

	.method public hidebysig 
		instance bool ResetAccess () cil managed 
	{
		// Method begins at RVA 0x2294
		// Header size: 12
		// Code size: 153 (0x99)
		.maxstack 3

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldc.i4.0
		IL_0007: stfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_000c: ldarg.0
		IL_000d: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0012: ldc.i4.0
		IL_0013: stfld bool VM.User.AccessUserInfo::adminEnable
		IL_0018: ldarg.0
		IL_0019: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_001e: ldnull
		IL_001f: stfld string VM.User.AccessUserInfo::adminCode
		IL_0024: ldarg.0
		IL_0025: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_002a: ldc.i4.0
		IL_002b: stfld bool VM.User.AccessUserInfo::techEnable
		IL_0030: ldarg.0
		IL_0031: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0036: ldnull
		IL_0037: stfld string VM.User.AccessUserInfo::techCode
		IL_003c: ldarg.0
		IL_003d: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0042: ldc.i4.0
		IL_0043: stfld bool VM.User.AccessUserInfo::operatorEnable
		IL_0048: ldarg.0
		IL_0049: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_004e: ldnull
		IL_004f: stfld string VM.User.AccessUserInfo::operatorCode
		IL_0054: ldarg.0
		IL_0055: ldstr "LoginUser"
		IL_005a: callvirt instance void [Apps.Data]Apps.Data.NotifyPropertyChangedBase::OnPropertyChanged(string)
		IL_005f: ldarg.0
		IL_0060: ldc.i4.1
		IL_0061: call instance void VM.User.CodeManager::set_IsAdmin(bool)
		IL_0066: ldarg.0
		IL_0067: ldfld class VM.User.CodeManager/IsAllAccessCallBack VM.User.CodeManager::OnAllAccessCallBack
		IL_006c: dup
		IL_006d: brtrue.s IL_0072

		IL_006f: pop
		IL_0070: br.s IL_007e

		IL_0072: ldarg.0
		IL_0073: call instance bool VM.User.CodeManager::get_IsAdmin()
		IL_0078: callvirt instance bool VM.User.CodeManager/IsAllAccessCallBack::Invoke(bool)
		IL_007d: pop

		IL_007e: ldarg.0
		IL_007f: ldfld class VM.User.CodeManager/AccessEnableCallBack VM.User.CodeManager::OnAccessEnableCallBack
		IL_0084: dup
		IL_0085: brtrue.s IL_008a

		IL_0087: pop
		IL_0088: br.s IL_0092

		IL_008a: ldc.i4.0
		IL_008b: ldc.i4.0
		IL_008c: callvirt instance bool VM.User.CodeManager/AccessEnableCallBack::Invoke(bool, valuetype VM.User.AccessUser)
		IL_0091: pop

		IL_0092: ldarg.0
		IL_0093: call instance bool VM.User.CodeManager::SaveAccessInfo()
		IL_0098: ret
	} // end of method CodeManager::ResetAccess

	.method public hidebysig 
		instance bool SetAdminEnable (
			bool bEnable,
			string adminCode
		) cil managed 
	{
		// Method begins at RVA 0x233c
		// Header size: 12
		// Code size: 87 (0x57)
		.maxstack 2

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldarg.1
		IL_0007: stfld bool VM.User.AccessUserInfo::adminEnable
		IL_000c: ldarg.0
		IL_000d: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0012: ldarg.2
		IL_0013: stfld string VM.User.AccessUserInfo::adminCode
		IL_0018: ldstr "AccessCheckDisk"
		IL_001d: call string [VM.Utility]VM.Utility.ConfigurationHelper::GetConfig(string)
		IL_0022: ldstr "1"
		IL_0027: call bool [mscorlib]System.String::op_Equality(string, string)
		IL_002c: brfalse.s IL_0040

		IL_002e: ldarg.0
		IL_002f: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0034: call string VM.User.CodeManager::GetDiskID()
		IL_0039: stfld string VM.User.AccessUserInfo::IDInfo
		IL_003e: br.s IL_0050

		IL_0040: ldarg.0
		IL_0041: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0046: ldstr ""
		IL_004b: stfld string VM.User.AccessUserInfo::IDInfo

		IL_0050: ldarg.0
		IL_0051: call instance bool VM.User.CodeManager::SaveAccessInfo()
		IL_0056: ret
	} // end of method CodeManager::SetAdminEnable

	.method public hidebysig 
		instance bool SetTechEnable (
			bool bEnable,
			string techCode
		) cil managed 
	{
		// Method begins at RVA 0x239f
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldarg.1
		IL_0007: stfld bool VM.User.AccessUserInfo::techEnable
		IL_000c: ldarg.0
		IL_000d: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0012: ldarg.2
		IL_0013: stfld string VM.User.AccessUserInfo::techCode
		IL_0018: ldarg.0
		IL_0019: call instance bool VM.User.CodeManager::SaveAccessInfo()
		IL_001e: ret
	} // end of method CodeManager::SetTechEnable

	.method public hidebysig 
		instance bool SetOperatorEnable (
			bool bEnable,
			string operatorCode
		) cil managed 
	{
		// Method begins at RVA 0x23bf
		// Header size: 1
		// Code size: 31 (0x1f)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0006: ldarg.1
		IL_0007: stfld bool VM.User.AccessUserInfo::operatorEnable
		IL_000c: ldarg.0
		IL_000d: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0012: ldarg.2
		IL_0013: stfld string VM.User.AccessUserInfo::operatorCode
		IL_0018: ldarg.0
		IL_0019: call instance bool VM.User.CodeManager::SaveAccessInfo()
		IL_001e: ret
	} // end of method CodeManager::SetOperatorEnable

	.method public hidebysig 
		instance bool GetAdminCode (
			[out] bool& bEnable,
			[out] string& adminCode
		) cil managed 
	{
		// Method begins at RVA 0x23df
		// Header size: 1
		// Code size: 28 (0x1c)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: ldarg.0
		IL_0002: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0007: ldfld bool VM.User.AccessUserInfo::adminEnable
		IL_000c: stind.i1
		IL_000d: ldarg.2
		IL_000e: ldarg.0
		IL_000f: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0014: ldfld string VM.User.AccessUserInfo::adminCode
		IL_0019: stind.ref
		IL_001a: ldc.i4.1
		IL_001b: ret
	} // end of method CodeManager::GetAdminCode

	.method public hidebysig 
		instance bool GetTechCode (
			[out] bool& bEnable,
			[out] string& techCode
		) cil managed 
	{
		// Method begins at RVA 0x23fc
		// Header size: 1
		// Code size: 28 (0x1c)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: ldarg.0
		IL_0002: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0007: ldfld bool VM.User.AccessUserInfo::techEnable
		IL_000c: stind.i1
		IL_000d: ldarg.2
		IL_000e: ldarg.0
		IL_000f: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0014: ldfld string VM.User.AccessUserInfo::techCode
		IL_0019: stind.ref
		IL_001a: ldc.i4.1
		IL_001b: ret
	} // end of method CodeManager::GetTechCode

	.method public hidebysig 
		instance bool GetOperatorCode (
			[out] bool& bEnable,
			[out] string& operatorCode
		) cil managed 
	{
		// Method begins at RVA 0x2419
		// Header size: 1
		// Code size: 28 (0x1c)
		.maxstack 8

		IL_0000: ldarg.1
		IL_0001: ldarg.0
		IL_0002: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0007: ldfld bool VM.User.AccessUserInfo::operatorEnable
		IL_000c: stind.i1
		IL_000d: ldarg.2
		IL_000e: ldarg.0
		IL_000f: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_0014: ldfld string VM.User.AccessUserInfo::operatorCode
		IL_0019: stind.ref
		IL_001a: ldc.i4.1
		IL_001b: ret
	} // end of method CodeManager::GetOperatorCode

	.method public hidebysig 
		instance void RefreshTitleShowUserName () cil managed 
	{
		// Method begins at RVA 0x2438
		// Header size: 12
		// Code size: 43 (0x2b)
		.maxstack 3
		.locals init (
			[0] valuetype VM.User.AccessUser
		)

		IL_0000: call class VM.User.CodeManager VM.User.CodeManager::GetInstance()
		IL_0005: ldflda valuetype VM.User.AccessUserInfo VM.User.CodeManager::_accessCurUserInfo
		IL_000a: ldfld valuetype VM.User.AccessUser VM.User.AccessUserInfo::curLoginUser
		IL_000f: stloc.0
		IL_0010: ldloc.0
		IL_0011: brfalse.s IL_002a

		IL_0013: call class VM.User.CodeManager VM.User.CodeManager::GetInstance()
		IL_0018: ldfld class VM.User.CodeManager/AccessEnableCallBack VM.User.CodeManager::OnAccessEnableCallBack
		IL_001d: dup
		IL_001e: brtrue.s IL_0022

		IL_0020: pop
		IL_0021: ret

		IL_0022: ldc.i4.1
		IL_0023: ldloc.0
		IL_0024: callvirt instance bool VM.User.CodeManager/AccessEnableCallBack::Invoke(bool, valuetype VM.User.AccessUser)
		IL_0029: pop

		IL_002a: ret
	} // end of method CodeManager::RefreshTitleShowUserName

	.method public hidebysig static 
		string GetDiskID () cil managed 
	{
		// Method begins at RVA 0x2470
		// Header size: 12
		// Code size: 268 (0x10c)
		.maxstack 3
		.locals init (
			[0] string,
			[1] string,
			[2] class [System.Management]System.Management.ManagementObjectCollection/ManagementObjectEnumerator,
			[3] string,
			[4] string,
			[5] string,
			[6] class [mscorlib]System.Exception
		)

		IL_0000: ldstr ""
		IL_0005: stloc.0
		.try
		{
			IL_0006: call class [mscorlib]System.AppDomain [mscorlib]System.AppDomain::get_CurrentDomain()
			IL_000b: callvirt instance class [mscorlib]System.AppDomainSetup [mscorlib]System.AppDomain::get_SetupInformation()
			IL_0010: callvirt instance string [mscorlib]System.AppDomainSetup::get_ApplicationBase()
			IL_0015: ldc.i4.0
			IL_0016: ldc.i4.1
			IL_0017: callvirt instance string [mscorlib]System.String::Substring(int32, int32)
			IL_001c: stloc.1
			IL_001d: ldstr "Win32_Volume"
			IL_0022: newobj instance void [System.Management]System.Management.SelectQuery::.ctor(string)
			IL_0027: newobj instance void [System.Management]System.Management.ManagementObjectSearcher::.ctor(class [System.Management]System.Management.ObjectQuery)
			IL_002c: callvirt instance class [System.Management]System.Management.ManagementObjectCollection [System.Management]System.Management.ManagementObjectSearcher::Get()
			IL_0031: callvirt instance class [System.Management]System.Management.ManagementObjectCollection/ManagementObjectEnumerator [System.Management]System.Management.ManagementObjectCollection::GetEnumerator()
			IL_0036: stloc.2
			.try
			{
				IL_0037: br.s IL_009b
				// loop start (head: IL_009b)
					IL_0039: ldloc.2
					IL_003a: callvirt instance class [System.Management]System.Management.ManagementBaseObject [System.Management]System.Management.ManagementObjectCollection/ManagementObjectEnumerator::get_Current()
					IL_003f: castclass [System.Management]System.Management.ManagementObject
					IL_0044: dup
					IL_0045: callvirt instance class [System.Management]System.Management.PropertyDataCollection [System.Management]System.Management.ManagementBaseObject::get_Properties()
					IL_004a: ldstr "Caption"
					IL_004f: callvirt instance class [System.Management]System.Management.PropertyData [System.Management]System.Management.PropertyDataCollection::get_Item(string)
					IL_0054: callvirt instance object [System.Management]System.Management.PropertyData::get_Value()
					IL_0059: callvirt instance string [mscorlib]System.Object::ToString()
					IL_005e: stloc.3
					IL_005f: callvirt instance class [System.Management]System.Management.PropertyDataCollection [System.Management]System.Management.ManagementBaseObject::get_Properties()
					IL_0064: ldstr "DeviceID"
					IL_0069: callvirt instance class [System.Management]System.Management.PropertyData [System.Management]System.Management.PropertyDataCollection::get_Item(string)
					IL_006e: callvirt instance object [System.Management]System.Management.PropertyData::get_Value()
					IL_0073: callvirt instance string [mscorlib]System.Object::ToString()
					IL_0078: stloc.s 4
					IL_007a: ldloc.3
					IL_007b: ldc.i4.0
					IL_007c: ldc.i4.1
					IL_007d: callvirt instance string [mscorlib]System.String::Substring(int32, int32)
					IL_0082: ldloc.1
					IL_0083: call bool [mscorlib]System.String::op_Equality(string, string)
					IL_0088: brfalse.s IL_009b

					IL_008a: ldloc.0
					IL_008b: ldloc.3
					IL_008c: call string [mscorlib]System.String::Concat(string, string)
					IL_0091: stloc.0
					IL_0092: ldloc.0
					IL_0093: ldloc.s 4
					IL_0095: call string [mscorlib]System.String::Concat(string, string)
					IL_009a: stloc.0

					IL_009b: ldloc.2
					IL_009c: callvirt instance bool [System.Management]System.Management.ManagementObjectCollection/ManagementObjectEnumerator::MoveNext()
					IL_00a1: brtrue.s IL_0039
				// end loop

				IL_00a3: leave.s IL_00af
			} // end .try
			finally
			{
				IL_00a5: ldloc.2
				IL_00a6: brfalse.s IL_00ae

				IL_00a8: ldloc.2
				IL_00a9: callvirt instance void [mscorlib]System.IDisposable::Dispose()

				IL_00ae: endfinally
			} // end handler

			IL_00af: ldstr "获取本机硬盘标识:"
			IL_00b4: ldloc.0
			IL_00b5: call string [mscorlib]System.String::Concat(string, string)
			IL_00ba: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
			IL_00bf: ldloc.0
			IL_00c0: callvirt instance int32 [mscorlib]System.String::get_Length()
			IL_00c5: ldc.i4.s 63
			IL_00c7: ble.s IL_00d3

			IL_00c9: ldloc.0
			IL_00ca: ldc.i4.0
			IL_00cb: ldc.i4.s 63
			IL_00cd: callvirt instance string [mscorlib]System.String::Substring(int32, int32)
			IL_00d2: stloc.0

			IL_00d3: ldstr "截断本机硬盘标识:"
			IL_00d8: ldloc.0
			IL_00d9: call string [mscorlib]System.String::Concat(string, string)
			IL_00de: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
			IL_00e3: ldloc.0
			IL_00e4: stloc.s 5
			IL_00e6: leave.s IL_0109
		} // end .try
		catch [mscorlib]System.Exception
		{
			IL_00e8: stloc.s 6
			IL_00ea: ldstr "获取本机硬盘标识失败:"
			IL_00ef: ldloc.s 6
			IL_00f1: callvirt instance string [mscorlib]System.Exception::get_Message()
			IL_00f6: call string [mscorlib]System.String::Concat(string, string)
			IL_00fb: call void [Apps.Log]Apps.Log.LogHelper::Info(object)
			IL_0100: ldstr "unknow"
			IL_0105: stloc.s 5
			IL_0107: leave.s IL_0109
		} // end handler

		IL_0109: ldloc.s 5
		IL_010b: ret
	} // end of method CodeManager::GetDiskID

	.method private hidebysig static 
		string GetUserMd5 (
			string originCode
		) cil managed 
	{
		// Method begins at RVA 0x25a4
		// Header size: 12
		// Code size: 70 (0x46)
		.maxstack 3
		.locals init (
			[0] string,
			[1] string,
			[2] uint8[],
			[3] int32
		)

		IL_0000: ldarg.0
		IL_0001: stloc.0
		IL_0002: ldstr ""
		IL_0007: stloc.1
		IL_0008: call class [mscorlib]System.Security.Cryptography.MD5 [mscorlib]System.Security.Cryptography.MD5::Create()
		IL_000d: call class [mscorlib]System.Text.Encoding [mscorlib]System.Text.Encoding::get_UTF8()
		IL_0012: ldloc.0
		IL_0013: callvirt instance uint8[] [mscorlib]System.Text.Encoding::GetBytes(string)
		IL_0018: callvirt instance uint8[] [mscorlib]System.Security.Cryptography.HashAlgorithm::ComputeHash(uint8[])
		IL_001d: stloc.2
		IL_001e: ldc.i4.0
		IL_001f: stloc.3
		IL_0020: br.s IL_003e
		// loop start (head: IL_003e)
			IL_0022: ldloc.1
			IL_0023: ldloc.2
			IL_0024: ldloc.3
			IL_0025: ldelema [mscorlib]System.Byte
			IL_002a: ldstr "X"
			IL_002f: call instance string [mscorlib]System.Byte::ToString(string)
			IL_0034: call string [mscorlib]System.String::Concat(string, string)
			IL_0039: stloc.1
			IL_003a: ldloc.3
			IL_003b: ldc.i4.1
			IL_003c: add
			IL_003d: stloc.3

			IL_003e: ldloc.3
			IL_003f: ldloc.2
			IL_0040: ldlen
			IL_0041: conv.i4
			IL_0042: blt.s IL_0022
		// end loop

		IL_0044: ldloc.1
		IL_0045: ret
	} // end of method CodeManager::GetUserMd5

	.method private hidebysig 
		instance class [System.Xml.Linq]System.Xml.Linq.XElement GetElement (
			class [System.Xml.Linq]System.Xml.Linq.XDocument xDoc,
			string nodename
		) cil managed 
	{
		// Method begins at RVA 0x25f8
		// Header size: 12
		// Code size: 52 (0x34)
		.maxstack 3
		.locals init (
			[0] class VM.User.CodeManager/'<>c__DisplayClass37_0'
		)

		IL_0000: newobj instance void VM.User.CodeManager/'<>c__DisplayClass37_0'::.ctor()
		IL_0005: stloc.0
		IL_0006: ldloc.0
		IL_0007: ldarg.2
		IL_0008: stfld string VM.User.CodeManager/'<>c__DisplayClass37_0'::nodename
		IL_000d: ldarg.1
		IL_000e: callvirt instance class [System.Xml.Linq]System.Xml.Linq.XElement [System.Xml.Linq]System.Xml.Linq.XDocument::get_Root()
		IL_0013: brtrue.s IL_0017

		IL_0015: ldnull
		IL_0016: ret

		IL_0017: ldarg.1
		IL_0018: callvirt instance class [System.Xml.Linq]System.Xml.Linq.XElement [System.Xml.Linq]System.Xml.Linq.XDocument::get_Root()
		IL_001d: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Xml.Linq]System.Xml.Linq.XElement> [System.Xml.Linq]System.Xml.Linq.XContainer::Elements()
		IL_0022: ldloc.0
		IL_0023: ldftn instance bool VM.User.CodeManager/'<>c__DisplayClass37_0'::'<GetElement>b__0'(class [System.Xml.Linq]System.Xml.Linq.XElement)
		IL_0029: newobj instance void class [mscorlib]System.Func`2<class [System.Xml.Linq]System.Xml.Linq.XElement, bool>::.ctor(object, native int)
		IL_002e: call !!0 [System.Core]System.Linq.Enumerable::SingleOrDefault<class [System.Xml.Linq]System.Xml.Linq.XElement>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib]System.Func`2<!!0, bool>)
		IL_0033: ret
	} // end of method CodeManager::GetElement

	.method public hidebysig 
		instance object BytesToStruct (
			uint8[] bytes,
			class [mscorlib]System.Type 'type'
		) cil managed 
	{
		// Method begins at RVA 0x2638
		// Header size: 12
		// Code size: 62 (0x3e)
		.maxstack 4
		.locals init (
			[0] int32,
			[1] native int
		)

		IL_0000: ldarg.2
		IL_0001: call int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(class [mscorlib]System.Type)
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: conv.r8
		IL_0009: ldc.r8 1
		IL_0012: call float64 [mscorlib]System.Math::Log(float64, float64)
		IL_0017: pop
		IL_0018: ldloc.0
		IL_0019: ldarg.1
		IL_001a: ldlen
		IL_001b: conv.i4
		IL_001c: ble.s IL_0020

		IL_001e: ldnull
		IL_001f: ret

		IL_0020: ldloc.0
		IL_0021: call native int [mscorlib]System.Runtime.InteropServices.Marshal::AllocHGlobal(int32)
		IL_0026: stloc.1
		IL_0027: ldarg.1
		IL_0028: ldc.i4.0
		IL_0029: ldloc.1
		IL_002a: ldloc.0
		IL_002b: call void [mscorlib]System.Runtime.InteropServices.Marshal::Copy(uint8[], int32, native int, int32)
		IL_0030: ldloc.1
		IL_0031: ldarg.2
		IL_0032: call object [mscorlib]System.Runtime.InteropServices.Marshal::PtrToStructure(native int, class [mscorlib]System.Type)
		IL_0037: ldloc.1
		IL_0038: call void [mscorlib]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		IL_003d: ret
	} // end of method CodeManager::BytesToStruct

	.method public hidebysig 
		instance uint8[] StructTOBytes (
			object obj
		) cil managed 
	{
		// Method begins at RVA 0x2684
		// Header size: 12
		// Code size: 46 (0x2e)
		.maxstack 4
		.locals init (
			[0] int32,
			[1] uint8[],
			[2] native int
		)

		IL_0000: ldarg.1
		IL_0001: call int32 [mscorlib]System.Runtime.InteropServices.Marshal::SizeOf(object)
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: newarr [mscorlib]System.Byte
		IL_000d: stloc.1
		IL_000e: ldloc.0
		IL_000f: call native int [mscorlib]System.Runtime.InteropServices.Marshal::AllocHGlobal(int32)
		IL_0014: stloc.2
		IL_0015: ldarg.1
		IL_0016: ldloc.2
		IL_0017: ldc.i4.0
		IL_0018: call void [mscorlib]System.Runtime.InteropServices.Marshal::StructureToPtr(object, native int, bool)
		IL_001d: ldloc.2
		IL_001e: ldloc.1
		IL_001f: ldc.i4.0
		IL_0020: ldloc.0
		IL_0021: call void [mscorlib]System.Runtime.InteropServices.Marshal::Copy(native int, uint8[], int32, int32)
		IL_0026: ldloc.2
		IL_0027: call void [mscorlib]System.Runtime.InteropServices.Marshal::FreeHGlobal(native int)
		IL_002c: ldloc.1
		IL_002d: ret
	} // end of method CodeManager::StructTOBytes

	.method private hidebysig specialname rtspecialname static 
		void .cctor () cil managed 
	{
		// Method begins at RVA 0x26be
		// Header size: 1
		// Code size: 17 (0x11)
		.maxstack 8

		IL_0000: newobj instance void [mscorlib]System.Object::.ctor()
		IL_0005: stsfld object VM.User.CodeManager::syncObj
		IL_000a: ldnull
		IL_000b: stsfld class VM.User.CodeManager VM.User.CodeManager::'instance'
		IL_0010: ret
	} // end of method CodeManager::.cctor

	// Properties
	.property class VM.User.CodeManager Instance()
	{
		.get class VM.User.CodeManager VM.User.CodeManager::get_Instance()
	}
	.property instance bool IsAccessEnable()
	{
		.get instance bool VM.User.CodeManager::get_IsAccessEnable()
	}
	.property instance bool IsAdmin()
	{
		.get instance bool VM.User.CodeManager::get_IsAdmin()
		.set instance void VM.User.CodeManager::set_IsAdmin(bool)
	}
	.property instance string UserName()
	{
		.get instance string VM.User.CodeManager::get_UserName()
	}

} // end of class VM.User.CodeManager

