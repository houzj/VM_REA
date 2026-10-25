
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_180012dc0(longlong param_1,undefined4 param_2)

{
  int *piVar1;
  longlong *plVar2;
  char cVar3;
  longlong *plVar4;
  undefined8 uVar5;
  undefined *puVar6;
  longlong lStack_30;
  longlong *plStack_28;
  undefined8 uStack_20;
  undefined8 uStack_18;
  
  FUN_180015b50(param_1,&lStack_30,param_2);
  plVar4 = plStack_28;
  if (lStack_30 == 0) {
    cVar3 = HSlog_ShouldLog(4);
    if (cVar3 != '\0') {
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar6 = &DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar6 = _DAT_180025e90;
      }
      HSlog_LogPrint(4,&UNK_18001cc88,puVar6,*(undefined4 *)(param_1 + 0x48),&UNK_18001cc68,0x32,
                     param_2);
    }
    uVar5 = 0xe0000001;
    plVar4 = plStack_28;
  }
  else {
    uStack_20 = 0;
    uStack_18 = 0;
    if (plStack_28 != (longlong *)0x0) {
      LOCK();
      *(int *)(plStack_28 + 1) = (int)plStack_28[1] + 1;
      UNLOCK();
    }
    FUN_180018b20(&uStack_20,lStack_30,plStack_28);
    FUN_180012f00(param_1,&uStack_20);
    uVar5 = 0;
  }
  plVar2 = plStack_28;
  if (plVar4 != (longlong *)0x0) {
    LOCK();
    plVar4 = plVar4 + 1;
    *(int *)plVar4 = (int)*plVar4 + -1;
    UNLOCK();
    if ((int)*plVar4 == 0) {
      (**(code **)*plStack_28)(plStack_28);
      LOCK();
      piVar1 = (int *)((longlong)plVar2 + 0xc);
      *piVar1 = *piVar1 + -1;
      UNLOCK();
      if (*piVar1 == 0) {
        (**(code **)(*plStack_28 + 8))();
      }
    }
  }
  return uVar5;
}

