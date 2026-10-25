
undefined8 FUN_180080990(undefined8 param_1,longlong param_2,undefined8 param_3,undefined8 param_4)

{
  longlong *plVar1;
  code *pcVar2;
  char cVar3;
  int iVar4;
  undefined8 uVar5;
  
  cVar3 = FUN_180080a30(param_1,*(undefined8 *)(param_2 + 8),1,param_4,0xfffffffffffffffe);
  if (cVar3 == '\0') {
    if (*(longlong **)(param_2 + 0x28) == (longlong *)0x0) {
      __Xbad_function_call_std__YAXXZ();
      pcVar2 = (code *)swi(3);
      uVar5 = (*pcVar2)();
      return uVar5;
    }
    (**(code **)(**(longlong **)(param_2 + 0x28) + 0x10))();
    iVar4 = SetEvent(*(undefined8 *)(param_2 + 8));
    if (iVar4 == 0) {
      GetLastError();
    }
  }
  plVar1 = *(longlong **)(param_2 + 0x28);
  if (plVar1 != (longlong *)0x0) {
    (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != (longlong *)(param_2 + 0x10));
    *(undefined8 *)(param_2 + 0x28) = 0;
  }
  plVar1 = *(longlong **)(param_2 + 0x28);
  if (plVar1 != (longlong *)0x0) {
    (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != (longlong *)(param_2 + 0x10));
    *(undefined8 *)(param_2 + 0x28) = 0;
  }
  return 0;
}

