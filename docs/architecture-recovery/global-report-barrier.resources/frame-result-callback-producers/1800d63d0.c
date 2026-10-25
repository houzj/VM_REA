
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined * FUN_1800d63d0(undefined8 param_1,undefined8 *param_2)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined8 uVar4;
  undefined8 uStack_18;
  longlong *plStack_10;
  
  uStack_18 = 0;
  if (&uStack_18 == param_2) {
    uVar4 = 0;
  }
  else {
    uVar4 = param_2[1];
    param_2[1] = 0;
    uStack_18 = *param_2;
    *param_2 = 0;
  }
  plVar3 = _DAT_1802b11e8;
  _DAT_1802b11e0 = uStack_18;
  _DAT_1802b11e8 = (longlong *)uVar4;
  plStack_10 = plVar3;
  if (plVar3 != (longlong *)0x0) {
    LOCK();
    plVar1 = plVar3 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plVar3)(plVar3);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_10 + 8))();
      }
    }
    return &DAT_1802b11e0;
  }
  return &DAT_1802b11e0;
}

