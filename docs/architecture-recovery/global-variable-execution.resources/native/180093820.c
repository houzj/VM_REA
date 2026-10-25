
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined *
FUN_180093820(undefined8 param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  int iVar1;
  
  _DAT_1802b2f80 = &UNK_180226d90;
  iVar1 = _Mtx_init(0x1802b2f88,2,param_3,param_4,0xfffffffffffffffe);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  FUN_18009a900(0x1802b2f98);
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_180112e60(&DAT_1802b1890,&DAT_1802b2f80);
  DAT_1802b2f90 = 0;
  return &DAT_1802b2f80;
}

