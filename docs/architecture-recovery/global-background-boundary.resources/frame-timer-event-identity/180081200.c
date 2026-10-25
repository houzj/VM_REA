
void FUN_180081200(longlong param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  int iVar1;
  undefined8 uStackX_10;
  undefined8 uVar2;
  
  uVar2 = 0xfffffffffffffffe;
  uStackX_10 = param_2;
  iVar1 = _Mtx_lock(param_1 + 8);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  FUN_1800818c0(param_1 + 0x48,&uStackX_10,param_3,param_4,uVar2);
  iVar1 = _Mtx_unlock(param_1 + 8);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  return;
}

