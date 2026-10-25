
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18008e2b0(undefined8 param_1)

{
  longlong lVar1;
  undefined8 ***pppuVar2;
  undefined1 auStack_58 [32];
  undefined8 **appuStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
  uStack_18 = _DAT_180192230 ^ (ulonglong)auStack_58;
  FUN_18008e1c0(appuStack_38,param_1);
  pppuVar2 = appuStack_38;
  if (7 < uStack_20) {
    pppuVar2 = (undefined8 ***)appuStack_38[0];
  }
  lVar1 = GetModuleHandleW(pppuVar2);
  if (lVar1 == 0) {
    pppuVar2 = appuStack_38;
    if (7 < uStack_20) {
      pppuVar2 = (undefined8 ***)appuStack_38[0];
    }
    LoadLibraryW(pppuVar2);
  }
  if (7 < uStack_20) {
    __3_YAXPEAX_Z(appuStack_38[0]);
  }
  FUN_18011da90(uStack_18 ^ (ulonglong)auStack_58);
  return;
}

