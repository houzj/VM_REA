
int FUN_180028110(longlong param_1,undefined8 *param_2)

{
  char cVar1;
  longlong lVar2;
  _String_val<std::_Simple_types<char>_> *p_Var3;
  undefined8 uVar4;
  int local_28;
  ulonglong local_20;
  
  local_28 = 0;
  lVar2 = FUN_180009510(param_2);
  if (*(char *)(lVar2 + 0x448) == '\0') {
    lVar2 = FUN_180009510(param_2);
    local_28 = VM_M_DelGlobalVar(*(undefined8 *)(param_1 + 8),lVar2 + 0x29);
  }
  else {
    for (local_20 = 0; lVar2 = FUN_180009510(param_2), local_20 < *(uint *)(lVar2 + 0x450);
        local_20 = local_20 + 1) {
      lVar2 = FUN_180009510(param_2);
      local_28 = VM_M_DelGlobalVar(*(undefined8 *)(param_1 + 8),
                                   *(longlong *)(lVar2 + 0x470) + local_20 * 0x130);
    }
  }
  if ((local_28 != 0) && (cVar1 = HSlog_ShouldLog(4), cVar1 != '\0')) {
    p_Var3 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
    uVar4 = FUN_180003ec0(p_Var3);
    HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] DeleteVarValue is error:[%x]",uVar4,
                   *(undefined4 *)(param_1 + 0x48),"CVariableModuBase::DeleteVarValue",0x6a8,
                   local_28);
  }
  FUN_180016480((longlong)param_2);
  return local_28;
}

