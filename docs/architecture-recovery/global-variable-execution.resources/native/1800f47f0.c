
undefined4
FUN_1800f47f0(undefined8 param_1,undefined8 param_2,undefined4 param_3,undefined4 param_4)

{
  longlong *plVar1;
  int *piVar2;
  undefined4 uVar3;
  longlong lStack_20;
  longlong *plStack_18;
  
  FUN_1800f74b0(param_1,&lStack_20,param_2,param_4,0xfffffffffffffffe);
  if (lStack_20 == 0) {
    uVar3 = 0xe0000003;
  }
  else {
    uVar3 = FUN_1800f3ee0(lStack_20,param_3,param_4);
  }
  if (plStack_18 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_18 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plStack_18)(plStack_18);
      LOCK();
      piVar2 = (int *)((longlong)plStack_18 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_18 + 8))(plStack_18);
      }
    }
  }
  return uVar3;
}

