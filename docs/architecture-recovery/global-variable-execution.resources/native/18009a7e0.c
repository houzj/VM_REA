
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4
FUN_18009a7e0(undefined8 param_1,undefined4 param_2,undefined8 param_3,undefined4 param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined4 uVar4;
  longlong lStack_20;
  longlong *plStack_18;
  
  FUN_1801e5430(0x1802b1208);
  if ((_DAT_1802b2b48 & 1) == 0) {
    _DAT_1802b2b48 = _DAT_1802b2b48 | 1;
    FUN_1800c8f10(0x1802b2b50);
    FUN_1801e8930(&UNK_180202a80);
  }
  FUN_1800cae00(0x1802b2b50,&lStack_20,param_2);
  plVar3 = plStack_18;
  if (lStack_20 == 0) {
    if (plStack_18 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_18 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_18)(plStack_18);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_18 + 8))();
        }
      }
    }
    uVar4 = 0xe0000003;
  }
  else {
    uVar4 = FUN_1800f9970(lStack_20,param_3,param_4);
    plVar3 = plStack_18;
    if (plStack_18 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_18 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_18)(plStack_18);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_18 + 8))();
        }
      }
    }
  }
  FUN_1801e5390(0x1802b1208);
  return uVar4;
}

