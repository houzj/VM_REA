
undefined4
FUN_1800c99f0(undefined8 param_1,undefined4 param_2,undefined8 param_3,undefined4 param_4,
             longlong param_5,undefined8 param_6)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined4 uVar4;
  longlong lStack_20;
  longlong *plStack_18;
  
  if (param_5 == 0) {
    uVar4 = 0xe0000001;
  }
  else {
    FUN_1800cae00(param_1,&lStack_20,param_2);
    if (lStack_20 == 0) {
      uVar4 = 0xe0000003;
    }
    else {
      uVar4 = FUN_1800f49a0(lStack_20,param_3,param_4,param_5,param_6);
    }
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
  return uVar4;
}

