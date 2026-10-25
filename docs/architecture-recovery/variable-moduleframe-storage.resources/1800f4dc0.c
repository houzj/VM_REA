
longlong FUN_1800f4dc0(undefined8 param_1,undefined8 param_2,longlong param_3,int param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  longlong lVar4;
  longlong *plStack_20;
  longlong *plStack_18;
  
  FUN_1800f7650(param_1,&plStack_20,param_2,param_4,0xfffffffffffffffe);
  if (plStack_20 == (longlong *)0x0) {
    lVar4 = 0xe0000003;
  }
  else {
    (**(code **)(*plStack_20 + 8))();
    lVar4 = 0;
    lVar3 = lVar4;
    if (0 < (longlong)param_4) {
      do {
        FUN_18000fd30(plStack_20,*(undefined4 *)(param_3 + lVar3 * 4));
        lVar3 = lVar3 + 1;
      } while (lVar3 < param_4);
    }
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
  return lVar4;
}

