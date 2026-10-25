
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 * FUN_1800d33b0(undefined8 *param_1)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  longlong lVar4;
  undefined8 uVar5;
  undefined4 uVar6;
  undefined8 uStack_18;
  longlong *plStack_10;
  
  if (_DAT_1802b11e0 == 0) {
    lVar4 = __2_YAPEAX_K_Z(0xe8);
    uVar5 = 0;
    if (lVar4 != 0) {
      uVar5 = FUN_1800d30a0(lVar4);
    }
    uStack_18 = 0;
    plStack_10 = (longlong *)0x0;
    uVar6 = FUN_1800d6d40(&uStack_18,uVar5);
    FUN_1800d63d0(uVar6,&uStack_18);
    plVar3 = plStack_10;
    if (plStack_10 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_10 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_10)(plStack_10);
        LOCK();
        piVar2 = (int *)((longlong)plVar3 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_10 + 8))();
        }
      }
    }
  }
  *param_1 = 0;
  param_1[1] = 0;
  lVar4 = _DAT_1802b11e0;
  if (_DAT_1802b11e8 != 0) {
    LOCK();
    *(int *)(_DAT_1802b11e8 + 8) = *(int *)(_DAT_1802b11e8 + 8) + 1;
    UNLOCK();
  }
  FUN_180015c40(param_1,lVar4);
  return param_1;
}

