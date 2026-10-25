
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */

int FUN_1800295b0(longlong param_1,char *param_2,longlong param_3,int param_4,undefined1 param_5,
                 char param_6)

{
  char cVar1;
  shared_ptr<class___ExceptionPtr> *psVar2;
  longlong lVar3;
  _String_val<std::_Simple_types<char>_> *p_Var4;
  undefined8 uVar5;
  undefined1 auStack_118 [32];
  char *local_f8;
  undefined4 local_f0;
  char *local_e8;
  undefined4 local_e0;
  longlong local_d8;
  int local_c8;
  ulonglong local_c0;
  longlong local_b8;
  shared_ptr<__ExceptionPtr> local_b0 [16];
  int local_a0;
  int local_9c;
  undefined8 local_98;
  longlong local_90;
  longlong local_88;
  longlong local_80;
  longlong local_78;
  ulonglong local_70;
  longlong local_68;
  undefined8 local_60;
  _String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> local_58 [32];
  _String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> local_38 [32];
  ulonglong local_18;
  
  local_60 = 0xfffffffffffffffe;
  local_18 = DAT_180067000 ^ (ulonglong)auStack_118;
  if ((param_3 == 0) || (param_2 == (char *)0x0)) {
    local_c8 = -0x1fffffff;
  }
  else {
    local_c8 = 0;
    local_f8 = (char *)CONCAT71(local_f8._1_7_,param_5);
    local_c8 = VM_M_BatchSetGlobalVarInt(*(undefined8 *)(param_1 + 8),param_2,param_3,param_4);
    if (((local_c8 == 0) && (0 < param_4)) && (param_6 == '\0')) {
      basic_string<>(local_38,param_2);
      psVar2 = (shared_ptr<class___ExceptionPtr> *)
               FUN_180035930((longlong *)(param_1 + 0xf8),
                             (_String_val<std::_Simple_types<char>_> *)local_38);
      std::shared_ptr<__ExceptionPtr>::shared_ptr<__ExceptionPtr>(local_b0,psVar2);
      ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)local_38);
      local_c0 = 0;
      while (lVar3 = FUN_180009510((undefined8 *)local_b0),
            local_c0 < (ulonglong)(longlong)*(int *)(lVar3 + 0x43c)) {
        lVar3 = FUN_180009510((undefined8 *)local_b0);
        local_88 = *(longlong *)(lVar3 + 0x440) + 0x14 + local_c0 * 0x94;
        local_b8 = -1;
        do {
          local_b8 = local_b8 + 1;
        } while (*(char *)(local_88 + local_b8) != '\0');
        if (local_b8 == 0) {
          cVar1 = HSlog_ShouldLog(4);
          if (cVar1 != '\0') {
            p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
            uVar5 = FUN_180003ec0(p_Var4);
            local_f0 = 0x7ad;
            local_f8 = "CVariableModuBase::SetVarIntValue";
            HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] link module Run ParamName is null",uVar5,
                           *(undefined4 *)(param_1 + 0x48));
          }
          local_9c = local_c8;
          FUN_180016480((longlong)local_b0);
          return local_9c;
        }
        FUN_18001b450(local_58);
        local_70 = FUN_180003ee0((longlong)local_58);
        local_98 = FUN_180003ec0((_String_val<std::_Simple_types<char>_> *)local_58);
        lVar3 = FUN_180009510((undefined8 *)local_b0);
        local_78 = *(longlong *)(lVar3 + 0x440) + 0x14 + local_c0 * 0x94;
        lVar3 = FUN_180009510((undefined8 *)local_b0);
        local_a0 = VM_M_SetBinayData(*(undefined4 *)(*(longlong *)(lVar3 + 0x440) + local_c0 * 0x94)
                                     ,local_78,local_98,local_70 & 0xffffffff);
        if ((local_a0 != 0) && (cVar1 = HSlog_ShouldLog(4), cVar1 != '\0')) {
          lVar3 = FUN_180009510((undefined8 *)local_b0);
          local_68 = *(longlong *)(lVar3 + 0x440) + 0x14 + local_c0 * 0x94;
          lVar3 = FUN_180009510((undefined8 *)local_b0);
          local_90 = local_c0 * 0x94;
          local_80 = *(longlong *)(lVar3 + 0x440);
          p_Var4 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
          uVar5 = FUN_180003ec0(p_Var4);
          local_d8 = local_68;
          local_e0 = *(undefined4 *)(local_80 + local_90);
          local_f0 = 0x7b4;
          local_f8 = "CVariableModuBase::SetVarIntValue";
          local_e8 = param_2;
          HSlog_LogPrint(4,
                         "[%s] [ModuId:%d] [%s@%d] SetVarIntValue is error,varName:[%s],id:[%d],paramName:[%s]"
                         ,uVar5,*(undefined4 *)(param_1 + 0x48));
        }
        ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)local_58)
        ;
        local_c0 = local_c0 + 1;
      }
      FUN_180016480((longlong)local_b0);
    }
  }
  return local_c8;
}

