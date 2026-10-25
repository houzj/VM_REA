
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8
FUN_1800bcd20(undefined8 param_1,undefined4 param_2,undefined8 param_3,undefined8 param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  longlong *plVar4;
  undefined8 uVar5;
  longlong lStack_20;
  longlong *plStack_18;
  
  uVar5 = 0xfffffffffffffffe;
  if ((_DAT_1802b1888 & 1) == 0) {
    _DAT_1802b1888 = _DAT_1802b1888 | 1;
    FUN_180107fc0();
    FUN_1801e8930(&UNK_180202980);
  }
  FUN_18010a720(&DAT_1802b1890,&lStack_20,param_2,param_4,uVar5);
  plVar4 = plStack_18;
  if (lStack_20 == 0) {
    uVar5 = 0;
  }
  else if ((*(int *)(lStack_20 + 0x28) == 10999) || (*(int *)(lStack_20 + 0x28) == 100000)) {
    uVar5 = 1;
  }
  else {
    uVar5 = 0;
  }
  if (plStack_18 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_18 + 1;
    lVar3 = *plVar1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)lVar3 == 1) {
      (**(code **)*plStack_18)(plStack_18);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_18 + 8))();
      }
    }
  }
  return uVar5;
}

