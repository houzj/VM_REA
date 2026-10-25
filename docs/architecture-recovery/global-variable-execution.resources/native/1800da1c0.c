
ulonglong FUN_1800da1c0(undefined8 param_1,ulonglong param_2)

{
  ulonglong uVar1;
  int iVar2;
  ulonglong uVar3;
  
  if ((param_2 == 0xffffffff) || ((param_2 & 0xffffffff00000000) == 0)) {
    uVar3 = WaitForSingleObject(param_1);
  }
  else {
    uVar3 = WaitForSingleObject(param_1,param_2 % 0xfffffffe);
    iVar2 = (int)uVar3;
    uVar1 = param_2 / 0xfffffffe;
    while ((iVar2 == 0x102 && (uVar1 != 0))) {
      uVar3 = WaitForSingleObject(param_1,0xfffffffe);
      iVar2 = (int)uVar3;
      uVar1 = uVar1 - 1;
    }
  }
  if ((uVar3 & 0xffffff7f) != 0) {
    if ((int)uVar3 == 0x102) {
      return uVar3;
    }
                    /* WARNING: Could not recover jumptable at 0x0001800da25e. Too many branches */
                    /* WARNING: Treating indirect jump as call */
    uVar3 = GetLastError();
    return uVar3;
  }
  return 0;
}

