
CAbstractUserModule * FUN_18001bf40(CAbstractUserModule *param_1)

{
  longlong lVar1;
  CAbstractUserModule *pCVar2;
  
  FUN_18004d530(param_1);
  FUN_180014380((undefined8 *)(param_1 + 0x50));
  *(undefined ***)param_1 = CVariableModuBase::vftable;
  *(undefined ***)(param_1 + 0x50) = CVariableModuBase::vftable;
  basic_string<>((_String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> *)
                 (param_1 + 0xd8));
  FUN_180035900((longlong *)(param_1 + 0xf8));
  FUN_18000d160((LPCRITICAL_SECTION)(param_1 + 0x558));
  FUN_180050228((longlong)(param_1 + 0x580),0x20,0x800,basic_string<>);
  FUN_18000d160((LPCRITICAL_SECTION)(param_1 + 0x10580));
  basic_string<>((_String_alloc<0,std::_String_base_types<char,std::allocator<char>_>_> *)
                 (param_1 + 0x105a8));
  vector<>((undefined8 *)(param_1 + 0x105c8));
  FUN_180035d90((longlong *)(param_1 + 0x105e8));
  FUN_18000d160((LPCRITICAL_SECTION)(param_1 + 0x105f8));
  FUN_180035f90((longlong *)(param_1 + 0x10620));
  FUN_180025f00();
  pCVar2 = param_1 + 0x148;
  for (lVar1 = 0x400; lVar1 != 0; lVar1 = lVar1 + -1) {
    *pCVar2 = (CAbstractUserModule)0x0;
    pCVar2 = pCVar2 + 1;
  }
  pCVar2 = param_1 + 0x108;
  for (lVar1 = 0x40; lVar1 != 0; lVar1 = lVar1 + -1) {
    *pCVar2 = (CAbstractUserModule)0x0;
    pCVar2 = pCVar2 + 1;
  }
  FUN_180003c40((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                (param_1 + 0x105a8),"");
  FUN_180003c40((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)(param_1 + 0xd8),
                "");
  *(undefined8 *)(param_1 + 0x548) = 0;
  *(undefined8 *)(param_1 + 0xd0) = 0;
  *(undefined4 *)(param_1 + 0x550) = 0;
  *(undefined8 *)(param_1 + 0x10) = 0;
  *(undefined4 *)(param_1 + 0x18) = 0;
  *(undefined8 *)(param_1 + 0x20) = 0;
  *(undefined4 *)(param_1 + 0x28) = 0;
  param_1[0x105e0] = (CAbstractUserModule)0x0;
  FUN_180035f10((longlong *)(param_1 + 0x105e8));
  return param_1;
}

