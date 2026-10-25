
void FUN_18000ffe0(longlong *param_1,int param_2,undefined8 *param_3)

{
  int iVar1;
  undefined8 uVar2;
  undefined1 auStack_68 [48];
  undefined4 uStack_38;
  undefined8 uStack_30;
  undefined8 *puStack_28;
  ulonglong uStack_20;
  
  uStack_30 = 0xfffffffffffffffe;
  uStack_20 = DAT_1802a5230 ^ (ulonglong)auStack_68;
  uStack_38 = 0;
  puStack_28 = param_3;
  if (param_2 < 0) {
    if (0xf < (ulonglong)param_3[3]) {
      __3_YAXPEAX_Z(*param_3);
    }
    param_3[3] = 0xf;
    param_3[2] = 0;
    *(undefined1 *)param_3 = 0;
  }
  else {
    FUN_18000fb90();
    iVar1 = (**(code **)(*param_1 + 0x18))(param_1);
    if (param_2 < iVar1) {
      FUN_180006470(*(undefined8 *)(*(longlong *)(param_1[2] + 0x38) + 8 + (longlong)param_2 * 8),
                    param_3,0,0xffffffffffffffff);
    }
    else {
      uVar2 = FUN_18000de60(param_1[2] + 0x28);
      FUN_180006470(uVar2,param_3,0,0xffffffffffffffff);
    }
    if (0xf < (ulonglong)param_3[3]) {
      __3_YAXPEAX_Z(*param_3);
    }
    param_3[3] = 0xf;
    param_3[2] = 0;
    *(undefined1 *)param_3 = 0;
  }
  FUN_1801e8810(uStack_20 ^ (ulonglong)auStack_68);
  return;
}

