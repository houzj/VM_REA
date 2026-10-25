
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */

int FUN_18002a7b0(longlong param_1,char *param_2,longlong param_3,int param_4,undefined1 param_5,
                 char param_6)

{
  undefined1 auVar1 [16];
  char cVar2;
  __uint64 _Var3;
  _String_val<std::_Simple_types<char>_> *p_Var4;
  undefined8 uVar5;
  shared_ptr<class___ExceptionPtr> *psVar6;
  longlong lVar7;
  undefined1 auStack_118 [32];
  char *local_f8;
  undefined4 local_f0;
  char *local_e8;
  undefined4 local_e0;
  longlong local_d8;
  int local_c8;
  int local_c4;
  ulonglong local_c0;
  void *local_b8;
  longlong local_b0;
  shared_ptr<__ExceptionPtr> local_a8 [16];
  int local_98;
  int local_94;
  void *local_90;
  longlong local_88;
  ulonglong local_80;
  longlong local_78;
  longlong local_70;
  longlong local_68;
  longlong local_60;
  longlong local_58;
  void *local_50;
  longlong local_48;
  undefined8 local_40;
  _String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> local_38 [32];
  ulonglong local_18;
  
  local_40 = 0xfffffffffffffffe;
  local_18 = DAT_180067000 ^ (ulonglong)auStack_118;
  if ((param_3 == 0) || (param_2 == (char *)0x0)) {
    local_c4 = -0x1fffffff;
  }
  else {
    local_c4 = 0;
    local_80 = (ulonglong)param_4;
    auVar1._8_8_ = 0;
    auVar1._0_8_ = local_80;
    _Var3 = SUB168(ZEXT816(0x10) * auVar1,0);
    if (SUB168(ZEXT816(0x10) * auVar1,8) != 0) {
      _Var3 = 0xffffffffffffffff;
    }
    local_b8 = (void *)thunk_FUN_1800502a4(_Var3);
    local_50 = local_b8;
    if (local_b8 == (void *)0x0) {
      cVar2 = HSlog_ShouldLog(4);
      if (cVar2 != '\0') {
        p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
        uVar5 = FUN_180003ec0(p_Var4);
        local_f0 = 0x827;
        local_f8 = "CVariableModuBase::SetVarStringValue";
        HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] STRING_UNIT new memory is error",uVar5,
                       *(undefined4 *)(param_1 + 0x48));
      }
      local_c4 = -0x1ffffffe;
    }
    else {
      for (local_c8 = 0; local_c8 < param_4; local_c8 = local_c8 + 1) {
        *(undefined8 *)((longlong)local_b8 + (longlong)local_c8 * 0x10) =
             *(undefined8 *)(param_3 + (longlong)local_c8 * 0x60);
        *(undefined4 *)((longlong)local_b8 + (longlong)local_c8 * 0x10 + 8) =
             *(undefined4 *)(param_3 + 8 + (longlong)local_c8 * 0x60);
        *(undefined4 *)((longlong)local_b8 + (longlong)local_c8 * 0x10 + 0xc) =
             *(undefined4 *)(param_3 + 0xc + (longlong)local_c8 * 0x60);
      }
      local_f8 = (char *)CONCAT71(local_f8._1_7_,param_5);
      local_c4 = VM_M_BatchSetGlobalVarString(*(undefined8 *)(param_1 + 8),param_2,local_b8,param_4)
      ;
      local_90 = local_b8;
      operator_delete__(local_b8);
      if (((local_c4 == 0) && (0 < param_4)) && (param_6 == '\0')) {
        basic_string<>(local_38,param_2);
        psVar6 = (shared_ptr<class___ExceptionPtr> *)
                 FUN_180035930((longlong *)(param_1 + 0xf8),
                               (_String_val<std::_Simple_types<char>_> *)local_38);
        std::shared_ptr<__ExceptionPtr>::shared_ptr<__ExceptionPtr>(local_a8,psVar6);
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)local_38)
        ;
        local_c0 = 0;
        while (lVar7 = FUN_180009510((undefined8 *)local_a8),
              local_c0 < (ulonglong)(longlong)*(int *)(lVar7 + 0x43c)) {
          lVar7 = FUN_180009510((undefined8 *)local_a8);
          local_58 = *(longlong *)(lVar7 + 0x440) + 0x14 + local_c0 * 0x94;
          local_b0 = -1;
          do {
            local_b0 = local_b0 + 1;
          } while (*(char *)(local_58 + local_b0) != '\0');
          if (local_b0 == 0) {
            cVar2 = HSlog_ShouldLog(4);
            if (cVar2 != '\0') {
              p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
              uVar5 = FUN_180003ec0(p_Var4);
              local_f0 = 0x83c;
              local_f8 = "CVariableModuBase::SetVarStringValue";
              HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] link module Run ParamName is null",uVar5,
                             *(undefined4 *)(param_1 + 0x48));
            }
            local_94 = local_c4;
            FUN_180016480((longlong)local_a8);
            return local_94;
          }
          local_70 = 0;
          local_48 = 0;
          lVar7 = FUN_180009510((undefined8 *)local_a8);
          local_60 = *(longlong *)(lVar7 + 0x440) + 0x14 + local_c0 * 0x94;
          lVar7 = FUN_180009510((undefined8 *)local_a8);
          local_98 = VM_M_SetBinayData(*(undefined4 *)
                                        (*(longlong *)(lVar7 + 0x440) + local_c0 * 0x94),local_60,
                                       *(undefined8 *)(param_3 + local_48),
                                       *(undefined4 *)(param_3 + 8 + local_70));
          if ((local_98 != 0) && (cVar2 = HSlog_ShouldLog(4), cVar2 != '\0')) {
            lVar7 = FUN_180009510((undefined8 *)local_a8);
            local_88 = *(longlong *)(lVar7 + 0x440) + 0x14 + local_c0 * 0x94;
            lVar7 = FUN_180009510((undefined8 *)local_a8);
            local_68 = local_c0 * 0x94;
            local_78 = *(longlong *)(lVar7 + 0x440);
            p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
            uVar5 = FUN_180003ec0(p_Var4);
            local_d8 = local_88;
            local_e0 = *(undefined4 *)(local_78 + local_68);
            local_f0 = 0x842;
            local_f8 = "CVariableModuBase::SetVarStringValue";
            local_e8 = param_2;
            HSlog_LogPrint(4,
                           "[%s] [ModuId:%d] [%s@%d] SetVarStringValue is error,varName:[%s],id:[%d],paramName:[%s]"
                           ,uVar5,*(undefined4 *)(param_1 + 0x48));
          }
          local_c0 = local_c0 + 1;
        }
        FUN_180016480((longlong)local_a8);
      }
    }
  }
  return local_c4;
}

