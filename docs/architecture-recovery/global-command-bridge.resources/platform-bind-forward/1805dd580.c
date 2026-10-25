
undefined8 * FUN_1805dd580(longlong param_1,undefined8 *param_2,int param_3)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  undefined8 *puVar4;
  undefined8 *puVar5;
  longlong *plVar6;
  char cVar7;
  undefined8 **ppuVar8;
  undefined8 uVar9;
  undefined8 *puVar10;
  undefined8 *puVar11;
  undefined8 *puStackX_8;
  undefined8 *puStackX_20;
  undefined4 uVar12;
  undefined8 uVar13;
  longlong lVar14;
  undefined8 uStack_30;
  longlong *plStack_28;
  
  uVar13 = 0xfffffffffffffffe;
  uVar12 = 0;
  uStack_30 = 0;
  plStack_28 = (longlong *)0x0;
  lVar3 = param_1 + 0x7d3c0;
  lVar14 = lVar3;
  if (lVar3 != 0) {
    EnterCriticalSection(lVar3);
  }
  puVar4 = *(undefined8 **)(param_1 + 0x7d3e8);
  cVar7 = *(char *)((longlong)puVar4[1] + 0x19);
  puVar5 = puVar4;
  puVar11 = (undefined8 *)puVar4[1];
  while (cVar7 == '\0') {
    if (*(int *)(puVar11 + 4) < param_3) {
      puVar10 = (undefined8 *)puVar11[2];
      puVar11 = puVar5;
    }
    else {
      puVar10 = (undefined8 *)*puVar11;
    }
    puVar5 = puVar11;
    puVar11 = puVar10;
    cVar7 = *(char *)((longlong)puVar10 + 0x19);
  }
  if ((puVar5 == puVar4) || (param_3 < *(int *)(puVar5 + 4))) {
    puStackX_20 = puVar4;
    ppuVar8 = &puStackX_20;
  }
  else {
    puStackX_8 = puVar5;
    ppuVar8 = &puStackX_8;
  }
  puVar4 = *ppuVar8;
  if (puVar4 == *(undefined8 **)(param_1 + 0x7d3e8)) {
    cVar7 = FUN_18057a5b0(4);
    if (cVar7 != '\0') {
      uVar9 = FUN_18057a310();
      FUN_18057a4b0(uVar9,4,&UNK_1807d73d8,&UNK_1807d7400,0xd88,param_3,uVar12,uVar13,lVar14);
    }
    *param_2 = 0;
    param_2[1] = 0;
    if (param_2 != &uStack_30) {
      param_2[1] = 0;
      plStack_28 = (longlong *)0x0;
      *param_2 = 0;
    }
    plVar1 = plStack_28;
    if (lVar3 != 0) {
      LeaveCriticalSection(lVar3);
    }
    plVar6 = plStack_28;
    if (plVar1 != (longlong *)0x0) {
      LOCK();
      plVar1 = plVar1 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_28)(plStack_28);
        LOCK();
        piVar2 = (int *)((longlong)plVar6 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_28 + 8))();
        }
      }
    }
  }
  else {
    *param_2 = 0;
    param_2[1] = 0;
    uVar13 = puVar4[5];
    if (puVar4[6] != 0) {
      LOCK();
      piVar2 = (int *)(puVar4[6] + 8);
      *piVar2 = *piVar2 + 1;
      UNLOCK();
    }
    FUN_180583d90(param_2,uVar13);
    if (lVar3 != 0) {
      LeaveCriticalSection(lVar3);
    }
  }
  return param_2;
}

