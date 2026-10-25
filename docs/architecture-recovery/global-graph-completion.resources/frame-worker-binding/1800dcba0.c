
void FUN_1800dcba0(longlong param_1,longlong *param_2)

{
  int *piVar1;
  longlong *plVar2;
  longlong lVar3;
  longlong *plVar4;
  undefined8 uStack_18;
  longlong *plStack_10;
  
  lVar3 = *param_2;
  uStack_18 = 0;
  plStack_10 = (longlong *)0x0;
  if (param_2[1] != 0) {
    LOCK();
    piVar1 = (int *)(param_2[1] + 8);
    *piVar1 = *piVar1 + 1;
    UNLOCK();
  }
  FUN_180015c40(&uStack_18,lVar3);
  FUN_18007dc50(param_1 + 200,&uStack_18);
  plVar4 = plStack_10;
  if (plStack_10 != (longlong *)0x0) {
    LOCK();
    plVar2 = plStack_10 + 1;
    *(int *)plVar2 = (int)*plVar2 + -1;
    UNLOCK();
    if ((int)*plVar2 == 0) {
      (**(code **)*plStack_10)(plStack_10);
      LOCK();
      piVar1 = (int *)((longlong)plVar4 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_10 + 8))();
      }
    }
  }
  *(bool *)(param_1 + 0x130) = *param_2 != 0;
  FUN_18007dc50(param_1 + 0x180,param_2);
  plVar4 = (longlong *)param_2[1];
  if (plVar4 != (longlong *)0x0) {
    LOCK();
    plVar2 = plVar4 + 1;
    *(int *)plVar2 = (int)*plVar2 + -1;
    UNLOCK();
    if ((int)*plVar2 == 0) {
      (**(code **)*plVar4)(plVar4);
      LOCK();
      piVar1 = (int *)((longlong)plVar4 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plVar4 + 8))(plVar4);
      }
    }
  }
  return;
}

