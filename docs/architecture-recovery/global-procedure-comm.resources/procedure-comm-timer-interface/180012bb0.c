
undefined8 *
FUN_180012bb0(undefined8 *param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  int iVar1;
  undefined8 uVar2;
  
  uVar2 = 0xfffffffffffffffe;
  __0CAbstractUserModule__QEAA_XZ();
  param_1[6] = 0;
  param_1[7] = 0;
  param_1[8] = 0;
  param_1[2] = 0;
  *(undefined4 *)(param_1 + 3) = 0;
  param_1[4] = 0;
  *(undefined4 *)(param_1 + 5) = 0;
  *(undefined4 *)(param_1 + 9) = 0xffffffff;
  *param_1 = &UNK_18001d810;
  param_1[10] = 0;
  param_1[0xb] = 0;
  param_1[0xc] = 0;
  param_1[0xd] = 0;
  uVar2 = FUN_1800186a0(0,0,0,param_4,uVar2);
  param_1[0xc] = uVar2;
  iVar1 = _Mtx_init(param_1 + 0xe,2);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  return param_1;
}

