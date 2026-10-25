
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */

longlong FUN_180009260(longlong param_1)

{
  undefined1 auStack_98 [32];
  uint local_78;
  uint local_70;
  uint local_68;
  uint local_60;
  uint local_58;
  uint local_50;
  uint local_48;
  uint local_40;
  uint local_38;
  uint local_30;
  GUID local_28;
  ulonglong local_18;
  
  local_18 = DAT_180019000 ^ (ulonglong)auStack_98;
  CoCreateGuid(&local_28);
  local_30 = (uint)local_28.Data4[7];
  local_38 = (uint)local_28.Data4[6];
  local_40 = (uint)local_28.Data4[5];
  local_48 = (uint)local_28.Data4[4];
  local_50 = (uint)local_28.Data4[3];
  local_58 = (uint)local_28.Data4[2];
  local_60 = (uint)local_28.Data4[1];
  local_68 = (uint)local_28.Data4[0];
  local_70 = (uint)local_28.Data3;
  local_78 = (uint)local_28.Data2;
  sprintf_s((char *)(param_1 + 0xc),0x40,"{%08X-%04X-%04X-%02X%02X-%02X%02X%02X%02X%02X%02X}");
  return param_1 + 0xc;
}

