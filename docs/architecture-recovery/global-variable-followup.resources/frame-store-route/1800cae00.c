
undefined8 * FUN_1800cae00(longlong param_1,undefined8 *param_2,undefined4 param_3)

{
  int *piVar1;
  longlong lVar2;
  undefined8 uVar3;
  longlong lStackX_8;
  undefined4 auStackX_18 [4];
  
  auStackX_18[0] = param_3;
  FUN_18009ab20((longlong *)(param_1 + 8),&lStackX_8,auStackX_18);
  lVar2 = *(longlong *)(param_1 + 8);
  *param_2 = 0;
  param_2[1] = 0;
  if (lStackX_8 != lVar2) {
    uVar3 = *(undefined8 *)(lStackX_8 + 0x18);
    if (*(longlong *)(lStackX_8 + 0x20) != 0) {
      LOCK();
      piVar1 = (int *)(*(longlong *)(lStackX_8 + 0x20) + 8);
      *piVar1 = *piVar1 + 1;
      UNLOCK();
    }
    FUN_180015c40(param_2,uVar3);
  }
  return param_2;
}

