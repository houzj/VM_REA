
bool FUN_180110680(undefined8 param_1,undefined4 param_2)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  longlong *plVar4;
  longlong lStack_18;
  longlong *plStack_10;
  
  FUN_1801111d0(param_1,&lStack_18,param_2);
  plVar4 = plStack_10;
  if (plStack_10 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_10 + 1;
    lVar3 = *plVar1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)lVar3 == 1) {
      (**(code **)*plStack_10)(plStack_10);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_10 + 8))();
      }
    }
    return lStack_18 != 0;
  }
  return lStack_18 != 0;
}

