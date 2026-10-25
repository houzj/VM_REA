
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 *
FUN_1800c8f10(undefined8 *param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  int iVar1;
  undefined8 uVar2;
  undefined8 uVar3;
  
  uVar3 = 0xfffffffffffffffe;
  *param_1 = &UNK_18022bb70;
  FUN_1800ceed0(param_1 + 1);
  param_1[9] = 0;
  param_1[10] = 0;
  uVar2 = FUN_18003c7f0();
  param_1[9] = uVar2;
  iVar1 = _Mtx_init(param_1 + 0xb,2,param_3,param_4,uVar3);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  param_1[0xc] = 0;
  param_1[0xd] = 0;
  uVar2 = FUN_180020380();
  param_1[0xc] = uVar2;
  param_1[0xe] = 0;
  param_1[0xf] = 0;
  param_1[0x10] = 0;
  param_1[0x11] = 0;
  param_1[0x12] = 0;
  uVar2 = FUN_18003c7f0();
  param_1[0x11] = uVar2;
  param_1[0x13] = 0;
  param_1[0x14] = 0;
  uVar2 = FUN_1800d0af0();
  param_1[0x13] = uVar2;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180112e60(&DAT_1802b1890,param_1);
  *(undefined4 *)(param_1 + 0x15) = 0;
  return param_1;
}

