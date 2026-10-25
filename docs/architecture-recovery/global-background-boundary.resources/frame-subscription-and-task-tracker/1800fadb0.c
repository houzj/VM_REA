
undefined4 FUN_1800fadb0(longlong param_1,int param_2,undefined8 param_3,undefined8 param_4)

{
  int *piVar1;
  int iVar2;
  char cVar3;
  longlong *plVar4;
  undefined8 *puVar5;
  longlong lVar6;
  undefined4 uVar7;
  undefined8 **ppuVar8;
  undefined8 *puVar9;
  undefined8 uVar10;
  longlong *plVar11;
  undefined8 *puVar12;
  undefined8 *puVar13;
  longlong *plVar14;
  longlong *plVar15;
  int aiStackX_10 [2];
  undefined8 *puStack_78;
  undefined8 *puStack_70;
  undefined8 uStack_68;
  undefined8 *puStack_60;
  longlong *plStack_58;
  undefined8 *puStack_50;
  longlong *plStack_48;
  longlong *plStack_38;
  
  uStack_68 = 0xfffffffffffffffe;
  aiStackX_10[0] = param_2;
  FUN_1801e5430();
  puVar9 = *(undefined8 **)(param_1 + 8);
  cVar3 = *(char *)((longlong)puVar9[1] + 0x19);
  puVar5 = puVar9;
  puVar13 = (undefined8 *)puVar9[1];
  while (cVar3 == '\0') {
    if (*(int *)(puVar13 + 4) < aiStackX_10[0]) {
      puVar12 = (undefined8 *)puVar13[2];
      puVar13 = puVar5;
    }
    else {
      puVar12 = (undefined8 *)*puVar13;
    }
    puVar5 = puVar13;
    puVar13 = puVar12;
    cVar3 = *(char *)((longlong)puVar12 + 0x19);
  }
  if ((puVar5 == puVar9) || (aiStackX_10[0] < *(int *)(puVar5 + 4))) {
    puStack_70 = puVar9;
    ppuVar8 = &puStack_70;
  }
  else {
    puStack_78 = puVar5;
    ppuVar8 = &puStack_78;
  }
  puVar9 = *ppuVar8;
  puStack_50 = (undefined8 *)0x0;
  plStack_48 = (longlong *)0x0;
  if (puVar9 == *(undefined8 **)(param_1 + 8)) {
    puVar9 = (undefined8 *)__2_YAPEAX_K_Z(0x10);
    if (puVar9 == (undefined8 *)0x0) {
      puVar9 = (undefined8 *)0x0;
    }
    else {
      *puVar9 = 0;
      puVar9[1] = 0;
      puStack_60 = puVar9;
      uVar10 = FUN_1800703a0();
      *puVar9 = uVar10;
    }
    puStack_60 = (undefined8 *)0x0;
    plStack_58 = (longlong *)0x0;
    FUN_1800fb3d0(&puStack_60,puVar9);
    plVar14 = plStack_58;
    puVar9 = puStack_60;
    plStack_48 = plStack_58;
    puStack_50 = puStack_60;
    plVar11 = (longlong *)FUN_1800853b0(param_1 + 8,aiStackX_10);
    plVar15 = plVar14;
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      *(int *)(plVar14 + 1) = (int)plVar14[1] + 1;
      UNLOCK();
      plVar14 = plStack_48;
      puVar9 = puStack_60;
      plVar15 = plStack_58;
    }
    plVar4 = (longlong *)plVar11[1];
    plVar11[1] = (longlong)plVar15;
    *plVar11 = (longlong)puVar9;
    plStack_38 = plVar4;
    if (plVar4 != (longlong *)0x0) {
      LOCK();
      plVar15 = plVar4 + 1;
      *(int *)plVar15 = (int)*plVar15 + -1;
      UNLOCK();
      plVar14 = plStack_48;
      if ((int)*plVar15 == 0) {
        (**(code **)*plVar4)(plVar4);
        LOCK();
        piVar1 = (int *)((longlong)plVar4 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        plVar14 = plStack_48;
        if (*piVar1 == 0) {
          (**(code **)(*plStack_38 + 8))();
          plVar14 = plStack_48;
        }
      }
    }
  }
  else {
    plStack_48 = (longlong *)puVar9[6];
    puStack_50 = (undefined8 *)puVar9[5];
    plVar14 = plStack_48;
    if (plStack_48 != (longlong *)0x0) {
      LOCK();
      *(int *)(plStack_48 + 1) = (int)plStack_48[1] + 1;
      UNLOCK();
    }
  }
  plVar15 = plStack_48;
  if (puStack_50 == (undefined8 *)0x0) {
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      plVar14 = plVar14 + 1;
      lVar6 = *plVar14;
      *(int *)plVar14 = (int)*plVar14 + -1;
      UNLOCK();
      if ((int)lVar6 == 1) {
        (**(code **)*plStack_48)(plStack_48);
        LOCK();
        piVar1 = (int *)((longlong)plVar15 + 0xc);
        iVar2 = *piVar1;
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (iVar2 == 1) {
          (**(code **)(*plStack_48 + 8))();
        }
      }
    }
    FUN_1801e5390(param_1 + 0x20);
    uVar7 = 0xe0000018;
  }
  else {
    uVar7 = FUN_1800d9620(puStack_50,param_3,param_4);
    plVar15 = plStack_48;
    if (plVar14 != (longlong *)0x0) {
      LOCK();
      plVar14 = plVar14 + 1;
      lVar6 = *plVar14;
      *(int *)plVar14 = (int)*plVar14 + -1;
      UNLOCK();
      if ((int)lVar6 == 1) {
        (**(code **)*plStack_48)(plStack_48);
        LOCK();
        piVar1 = (int *)((longlong)plVar15 + 0xc);
        iVar2 = *piVar1;
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (iVar2 == 1) {
          (**(code **)(*plStack_48 + 8))();
        }
      }
    }
    FUN_1801e5390(param_1 + 0x20);
  }
  return uVar7;
}

