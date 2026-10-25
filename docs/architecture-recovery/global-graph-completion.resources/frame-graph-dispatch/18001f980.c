
undefined8 *
FUN_18001f980(undefined8 *param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined8 uVar4;
  undefined8 uStack_30;
  longlong *plStack_28;
  undefined8 uStack_20;
  longlong *plStack_18;
  
  *param_1 = &UNK_18021d368;
  param_1[1] = 0;
  param_1[2] = 0;
  param_1[3] = 0;
  param_1[4] = 0;
  param_1[5] = 0;
  param_1[6] = 0;
  uVar4 = FUN_18001f150(param_1,0,0,param_4,0xfffffffffffffffe);
  param_1[5] = uVar4;
  uStack_30 = 0;
  plStack_28 = (longlong *)0x0;
  FUN_18001d870(param_1 + 1,&uStack_30);
  plVar3 = plStack_28;
  if (plStack_28 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_28 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_28)(plStack_28);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_28 + 8))();
      }
    }
  }
  uStack_20 = 0;
  plStack_18 = (longlong *)0x0;
  FUN_18001d870(param_1 + 3,&uStack_20);
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
  return param_1;
}

