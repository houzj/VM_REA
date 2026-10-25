
void FUN_18009a730(longlong param_1,int param_2)

{
  int iVar1;
  int aiStackX_10 [2];
  longlong alStackX_18 [2];
  
  if (((param_2 - 24000U < 2000) || (param_2 - 13000U < 1000)) || (param_2 - 31000U < 3000)) {
    aiStackX_10[0] = param_2;
    iVar1 = _Mtx_lock(param_1 + 8);
    if (iVar1 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar1);
    }
    FUN_18009ab20(param_1 + 0x18,alStackX_18,aiStackX_10);
    if (alStackX_18[0] != *(longlong *)(param_1 + 0x18)) {
      FUN_18009aa40(param_1 + 0x18,aiStackX_10);
    }
    iVar1 = _Mtx_unlock(param_1 + 8);
    if (iVar1 != 0) {
                    /* WARNING: Could not recover jumptable at 0x00018009a7c4. Too many branches */
                    /* WARNING: Treating indirect jump as call */
      __Throw_C_error_std__YAXH_Z(iVar1);
      return;
    }
  }
  return;
}

