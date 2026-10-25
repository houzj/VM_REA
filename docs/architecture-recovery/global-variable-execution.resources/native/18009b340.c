
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8
FUN_18009b340(undefined8 param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  int iVar1;
  undefined8 uVar2;
  
  uVar2 = 0xfffffffffffffffe;
  iVar1 = _Mtx_init(0x1802b2ed0,2,param_3,param_4,0xfffffffffffffffe);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  _DAT_1802b2ed8 = 0;
  _DAT_1802b2ee0 = 0;
  _DAT_1802b2ed8 = FUN_18003c7f0();
  _DAT_1802b2ee8 = 0;
  _DAT_1802b2ef0 = 0;
  _DAT_1802b2ee8 = FUN_180079000(&DAT_1802b2ee8);
  iVar1 = _Mtx_init(0x1802b2ef8,2,param_3,param_4,uVar2);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  _DAT_1802b2f00 = 0;
  _DAT_1802b2f08 = 0;
  _DAT_1802b2f00 = FUN_18003c7f0();
  _DAT_1802b2f10 = 0;
  _DAT_1802b2f18 = 0;
  _DAT_1802b2f10 = FUN_18003c7f0();
  return 0x1802b2ed0;
}

