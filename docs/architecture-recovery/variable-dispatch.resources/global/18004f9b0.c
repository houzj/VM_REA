
/* WARNING: Removing unreachable block (ram,0x00018004fb7e) */
/* WARNING: Removing unreachable block (ram,0x00018004fb90) */
/* WARNING: Removing unreachable block (ram,0x00018004fbd3) */
/* WARNING: Removing unreachable block (ram,0x00018004fc13) */
/* WARNING: Removing unreachable block (ram,0x00018004fbf5) */
/* WARNING: Removing unreachable block (ram,0x00018004fc1f) */

CAbstractUserModule * CreateModule(undefined8 param_1)

{
  char cVar1;
  int iVar2;
  _String_val<std::_Simple_types<char>_> *p_Var3;
  undefined8 uVar4;
  undefined4 uStack_74;
  CAbstractUserModule *pCStack_70;
  CAbstractUserModule *pCStack_68;
  CAbstractUserModule *pCStack_60;
  CAbstractUserModule *pCStack_50;
  CAbstractUserModule *pCStack_48;
  CAbstractUserModule *pCStack_38;
  CAbstractUserModule *pCStack_28;
  CAbstractUserModule *pCStack_18;
  
                    /* 0x4f9b0  1  CreateModule */
  pCStack_48 = (CAbstractUserModule *)FUN_1800502a4(0x10630);
  if (pCStack_48 == (CAbstractUserModule *)0x0) {
    pCStack_50 = (CAbstractUserModule *)0x0;
  }
  else {
    pCStack_50 = FUN_18004fc30(pCStack_48);
  }
  pCStack_28 = pCStack_50;
  pCStack_70 = pCStack_50;
  if (pCStack_50 == (CAbstractUserModule *)0x0) {
    pCStack_70 = (CAbstractUserModule *)0x0;
  }
  else {
    iVar2 = (**(code **)(*(longlong *)pCStack_50 + 0x50))(pCStack_50);
    if (iVar2 == 0) {
      uStack_74 = 0xffffffff;
      iVar2 = VM_M_GetModuleId(param_1,&uStack_74);
      if (iVar2 == 0) {
        *(undefined4 *)(pCStack_70 + 0x48) = uStack_74;
      }
      else {
        cVar1 = HSlog_ShouldLog(4);
        if (cVar1 != '\0') {
          p_Var3 = (_String_val<std::_Simple_types<char>_> *)_Immortalize<>();
          uVar4 = FUN_180003ec0(p_Var3);
          HSlog_LogPrint(4,"[%s] [ModuId:%d] [%s@%d] VM_M_GetModuleId return[%x].",uVar4,uStack_74,
                         "CreateModule",0x2a,iVar2);
        }
        pCStack_38 = pCStack_70;
        pCStack_68 = pCStack_70;
        if (pCStack_70 != (CAbstractUserModule *)0x0) {
          (*(code *)**(undefined8 **)pCStack_70)(pCStack_70,1);
        }
        pCStack_70 = (CAbstractUserModule *)0x0;
      }
    }
    else {
      pCStack_18 = pCStack_70;
      pCStack_60 = pCStack_70;
      if (pCStack_70 != (CAbstractUserModule *)0x0) {
        (*(code *)**(undefined8 **)pCStack_70)(pCStack_70,1);
      }
      pCStack_70 = (CAbstractUserModule *)0x0;
    }
  }
  return pCStack_70;
}

