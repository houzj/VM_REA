
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800e3460(longlong *param_1)

{
  undefined8 ***pppuVar1;
  undefined1 auStack_58 [32];
  undefined8 uStack_38;
  undefined8 **appuStack_30 [2];
  undefined4 uStack_20;
  ulonglong uStack_18;
  ulonglong uStack_10;
  
  uStack_38 = 0xfffffffffffffffe;
  uStack_10 = _DAT_1802a5230 ^ (ulonglong)auStack_58;
  if (*param_1 != 0) {
    FUN_1800e3590(appuStack_30);
    pppuVar1 = appuStack_30;
    if (0xf < uStack_18) {
      pppuVar1 = (undefined8 ***)appuStack_30[0];
    }
    (**(code **)(*(longlong *)*param_1 + 0x18))
              ((longlong *)*param_1,&UNK_18022d710,pppuVar1,uStack_20);
    if (0xf < uStack_18) {
      __3_YAXPEAX_Z(appuStack_30[0]);
    }
  }
  FUN_1801e8810(uStack_10 ^ (ulonglong)auStack_58);
  return;
}

