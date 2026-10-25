
undefined8 FUN_1800d9620(undefined8 param_1,undefined8 param_2,undefined4 *param_3)

{
  longlong *plVar1;
  int *piVar2;
  int iVar3;
  longlong *plVar4;
  undefined4 uVar5;
  undefined4 uVar6;
  undefined4 uVar7;
  undefined8 *puVar8;
  undefined8 uVar9;
  longlong lVar10;
  undefined8 *puVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  undefined4 *puVar14;
  undefined4 *puStack_30;
  longlong *plStack_28;
  longlong *plStack_18;
  
  uVar9 = __2_YAPEAX_K_Z(0x110);
  puStack_30 = (undefined4 *)0x0;
  plStack_28 = (longlong *)0x0;
  FUN_18007e960(&puStack_30,uVar9);
  puVar14 = puStack_30;
  *puStack_30 = *param_3;
  lVar10 = 2;
  puVar11 = (undefined8 *)(puStack_30 + 1);
  puVar8 = (undefined8 *)(param_3 + 1);
  do {
    puVar13 = puVar8;
    puVar12 = puVar11;
    uVar9 = puVar13[1];
    *puVar12 = *puVar13;
    puVar12[1] = uVar9;
    uVar9 = puVar13[3];
    puVar12[2] = puVar13[2];
    puVar12[3] = uVar9;
    uVar9 = puVar13[5];
    puVar12[4] = puVar13[4];
    puVar12[5] = uVar9;
    uVar9 = puVar13[7];
    puVar12[6] = puVar13[6];
    puVar12[7] = uVar9;
    uVar9 = puVar13[9];
    puVar12[8] = puVar13[8];
    puVar12[9] = uVar9;
    uVar9 = puVar13[0xb];
    puVar12[10] = puVar13[10];
    puVar12[0xb] = uVar9;
    uVar5 = *(undefined4 *)((longlong)puVar13 + 100);
    uVar6 = *(undefined4 *)(puVar13 + 0xd);
    uVar7 = *(undefined4 *)((longlong)puVar13 + 0x6c);
    *(undefined4 *)(puVar12 + 0xc) = *(undefined4 *)(puVar13 + 0xc);
    *(undefined4 *)((longlong)puVar12 + 100) = uVar5;
    *(undefined4 *)(puVar12 + 0xd) = uVar6;
    *(undefined4 *)((longlong)puVar12 + 0x6c) = uVar7;
    uVar5 = *(undefined4 *)((longlong)puVar13 + 0x74);
    uVar6 = *(undefined4 *)(puVar13 + 0xf);
    uVar7 = *(undefined4 *)((longlong)puVar13 + 0x7c);
    *(undefined4 *)(puVar12 + 0xe) = *(undefined4 *)(puVar13 + 0xe);
    *(undefined4 *)((longlong)puVar12 + 0x74) = uVar5;
    *(undefined4 *)(puVar12 + 0xf) = uVar6;
    *(undefined4 *)((longlong)puVar12 + 0x7c) = uVar7;
    lVar10 = lVar10 + -1;
    puVar11 = puVar12 + 0x10;
    puVar8 = puVar13 + 0x10;
  } while (lVar10 != 0);
  puVar12[0x10] = puVar13[0x10];
  *(undefined4 *)(puVar12 + 0x11) = *(undefined4 *)(puVar13 + 0x11);
  puVar11 = (undefined8 *)FUN_1800d9920(param_1,param_2);
  if (plStack_28 != (longlong *)0x0) {
    LOCK();
    *(int *)(plStack_28 + 1) = (int)plStack_28[1] + 1;
    UNLOCK();
    puVar14 = puStack_30;
  }
  plVar4 = (longlong *)puVar11[1];
  puVar11[1] = plStack_28;
  *puVar11 = puVar14;
  plStack_18 = plVar4;
  if (plVar4 != (longlong *)0x0) {
    LOCK();
    plVar1 = plVar4 + 1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)*plVar1 == 0) {
      (**(code **)*plVar4)(plVar4);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_18 + 8))();
      }
    }
  }
  plVar4 = plStack_28;
  if (plStack_28 != (longlong *)0x0) {
    LOCK();
    plVar1 = plStack_28 + 1;
    lVar10 = *plVar1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)lVar10 == 1) {
      (**(code **)*plStack_28)(plStack_28);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      iVar3 = *piVar2;
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (iVar3 == 1) {
        (**(code **)(*plStack_28 + 8))();
      }
    }
  }
  return 0;
}

