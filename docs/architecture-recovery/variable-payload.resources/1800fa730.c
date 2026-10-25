
void FUN_1800fa730(longlong param_1)

{
  longlong lVar1;
  
  lVar1 = *(longlong *)(param_1 + 0x10);
  if (lVar1 != 0) {
    FUN_1800f7410(lVar1);
                    /* WARNING: Could not recover jumptable at 0x0001800fa74f. Too many branches */
                    /* WARNING: Treating indirect jump as call */
    __3_YAXPEAX_Z(lVar1);
    return;
  }
  return;
}

