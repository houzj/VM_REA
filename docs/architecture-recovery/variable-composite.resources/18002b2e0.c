
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */

undefined4
FUN_18002b2e0(longlong param_1,char *param_2,void *param_3,int param_4,undefined4 param_5)

{
  char cVar1;
  int iVar2;
  undefined4 uVar3;
  _String_val<std::_Simple_types<char>_> *p_Var4;
  undefined8 uVar5;
  longlong lVar6;
  int *piVar7;
  void **ppvVar8;
  undefined1 auStackY_e8 [32];
  int local_b4;
  longlong local_b0;
  void *local_a8;
  int local_a0 [34];
  ulonglong local_18;
  
  local_18 = DAT_180067000 ^ (ulonglong)auStackY_e8;
  cVar1 = HSlog_ShouldLog(4);
  if (cVar1 != '\0') {
    p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
    uVar5 = FUN_180003ec0(p_Var4);
    HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] set image start",uVar5,
                   *(undefined4 *)(param_1 + 0x48));
  }
  if ((param_3 == (void *)0x0) || (param_2 == (char *)0x0)) {
    uVar3 = 0xe0000001;
  }
  else {
    local_a8 = (void *)0x0;
    piVar7 = local_a0;
    for (lVar6 = 0x88; lVar6 != 0; lVar6 = lVar6 + -1) {
      *(undefined1 *)piVar7 = 0;
      piVar7 = (int *)((longlong)piVar7 + 1);
    }
    ppvVar8 = &local_a8;
    for (lVar6 = 0x90; lVar6 != 0; lVar6 = lVar6 + -1) {
      *(undefined1 *)ppvVar8 = 0;
      ppvVar8 = (void **)((longlong)ppvVar8 + 1);
    }
    local_b0 = 0;
    local_b4 = 0;
    FUN_180033e00(param_1,param_2,&local_b4);
    iVar2 = FUN_1800145c0(param_1 + 0x50,*(undefined4 *)(param_1 + 0x48),(longlong)param_4,
                          (longlong *)&local_a8,&local_b0,local_b4);
    if (((iVar2 == 0) && (local_a8 != (void *)0x0)) && (local_b0 != 0)) {
      memcpy_s(local_a8,(longlong)param_4,param_3,(longlong)param_4);
      local_a0[0] = param_4;
      uVar3 = VM_M_SetGlobalVarImageEx(*(undefined8 *)(param_1 + 8),param_2,param_5,&local_a8);
      cVar1 = HSlog_ShouldLog(4);
      if (cVar1 != '\0') {
        p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
        uVar5 = FUN_180003ec0(p_Var4);
        HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] set image end",uVar5,
                       *(undefined4 *)(param_1 + 0x48));
      }
    }
    else {
      uVar3 = 0xe0000002;
    }
  }
  return uVar3;
}

