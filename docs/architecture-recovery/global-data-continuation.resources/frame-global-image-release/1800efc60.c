
void FUN_1800efc60(longlong param_1,undefined4 param_2)

{
  int iVar1;
  longlong lStackX_8;
  undefined4 auStackX_10 [2];
  
  auStackX_10[0] = param_2;
  FUN_1800ef740();
  FUN_1800ef800(param_1,param_2);
  iVar1 = _Mtx_lock(param_1 + 400);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  FUN_18009ab20(param_1 + 0x108,&lStackX_8,auStackX_10);
  if (lStackX_8 != *(longlong *)(param_1 + 0x108)) {
    FUN_1800f0a10(param_1 + 0x108,auStackX_10);
  }
  iVar1 = _Mtx_unlock(param_1 + 400);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  return;
}

