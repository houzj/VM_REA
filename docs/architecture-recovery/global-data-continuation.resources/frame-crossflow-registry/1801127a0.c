
undefined8 FUN_1801127a0(longlong param_1,int param_2,undefined8 param_3)

{
  int *piVar1;
  char cVar2;
  undefined8 *puVar3;
  longlong *plVar4;
  longlong lVar5;
  longlong *plVar6;
  undefined8 *puVar7;
  longlong *plVar8;
  undefined8 uVar9;
  undefined8 **ppuVar10;
  undefined8 *puVar11;
  longlong *plVar12;
  undefined8 *puVar13;
  int aiStackX_8 [2];
  undefined8 *puStackX_20;
  undefined8 *puStack_68;
  undefined8 *puStack_60;
  undefined8 *puStack_58;
  undefined8 *puStack_50;
  undefined8 *puStack_48;
  undefined8 uStack_40;
  longlong lStack_38;
  longlong *plStack_30;
  undefined1 auStack_28 [16];
  
  uStack_40 = 0xfffffffffffffffe;
  puVar3 = *(undefined8 **)(param_1 + 0xb0);
  cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
  puVar7 = puVar3;
  puVar13 = (undefined8 *)puVar3[1];
  while (cVar2 == '\0') {
    if (*(int *)(puVar13 + 4) < param_2) {
      puVar11 = (undefined8 *)puVar13[2];
      puVar13 = puVar7;
    }
    else {
      puVar11 = (undefined8 *)*puVar13;
    }
    puVar7 = puVar13;
    puVar13 = puVar11;
    cVar2 = *(char *)((longlong)puVar11 + 0x19);
  }
  if ((puVar7 == puVar3) || (param_2 < *(int *)(puVar7 + 4))) {
    puStack_68 = puVar3;
    ppuVar10 = &puStack_68;
  }
  else {
    ppuVar10 = &puStackX_20;
    puStackX_20 = puVar7;
  }
  puVar3 = *ppuVar10;
  if (puVar3 == *(undefined8 **)(param_1 + 0xb0)) {
    uVar9 = 0xe000031a;
  }
  else {
    plVar4 = (longlong *)puVar3[6];
    lVar5 = puVar3[5];
    if (plVar4 != (longlong *)0x0) {
      LOCK();
      *(int *)(plVar4 + 1) = (int)plVar4[1] + 1;
      UNLOCK();
    }
    plStack_30 = plVar4;
    lStack_38 = lVar5;
    plVar12 = (longlong *)**(longlong **)(lVar5 + 0x20);
    if (plVar12 != *(longlong **)(lVar5 + 0x20)) {
      do {
        aiStackX_8[0] = *(int *)((longlong)plVar12 + 0x1c);
        puVar3 = *(undefined8 **)(param_1 + 0x40);
        cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
        puVar7 = puVar3;
        puVar13 = (undefined8 *)puVar3[1];
        while (cVar2 == '\0') {
          if (*(int *)(puVar13 + 4) < aiStackX_8[0]) {
            puVar11 = (undefined8 *)puVar13[2];
            puVar13 = puVar7;
          }
          else {
            puVar11 = (undefined8 *)*puVar13;
          }
          puVar7 = puVar13;
          puVar13 = puVar11;
          cVar2 = *(char *)((longlong)puVar11 + 0x19);
        }
        if ((puVar7 == puVar3) || (aiStackX_8[0] < *(int *)(puVar7 + 4))) {
          puStack_58 = puVar3;
          ppuVar10 = &puStack_58;
        }
        else {
          puStack_60 = puVar7;
          ppuVar10 = &puStack_60;
        }
        if (*ppuVar10 == puVar3) {
          puVar3 = *(undefined8 **)(param_1 + 0xa0);
          cVar2 = *(char *)((longlong)puVar3[1] + 0x19);
          puVar7 = puVar3;
          puVar13 = (undefined8 *)puVar3[1];
          while (cVar2 == '\0') {
            if (*(int *)(puVar13 + 4) < aiStackX_8[0]) {
              puVar11 = (undefined8 *)puVar13[2];
              puVar13 = puVar7;
            }
            else {
              puVar11 = (undefined8 *)*puVar13;
            }
            puVar7 = puVar13;
            puVar13 = puVar11;
            cVar2 = *(char *)((longlong)puVar11 + 0x19);
          }
          if ((puVar7 == puVar3) || (aiStackX_8[0] < *(int *)(puVar7 + 4))) {
            puStack_48 = puVar3;
            ppuVar10 = &puStack_48;
          }
          else {
            puStack_50 = puVar7;
            ppuVar10 = &puStack_50;
          }
          if (*ppuVar10 == puVar3) {
            uVar9 = 0xe0000018;
            goto LAB_1801129bb;
          }
          FUN_1801127a0(param_1,aiStackX_8[0],param_3);
        }
        else {
          FUN_180071a90(param_3,auStack_28,0,aiStackX_8,DAT_1802b1205);
        }
        if (*(char *)((longlong)plVar12 + 0x19) == '\0') {
          plVar6 = (longlong *)plVar12[2];
          if (*(char *)((longlong)plVar6 + 0x19) == '\0') {
            cVar2 = *(char *)(*plVar6 + 0x19);
            plVar12 = plVar6;
            plVar6 = (longlong *)*plVar6;
            while (cVar2 == '\0') {
              cVar2 = *(char *)(*plVar6 + 0x19);
              plVar12 = plVar6;
              plVar6 = (longlong *)*plVar6;
            }
          }
          else {
            cVar2 = *(char *)(plVar12[1] + 0x19);
            plVar8 = (longlong *)plVar12[1];
            plVar6 = plVar12;
            while ((plVar12 = plVar8, cVar2 == '\0' && (plVar6 == (longlong *)plVar12[2]))) {
              cVar2 = *(char *)(plVar12[1] + 0x19);
              plVar8 = (longlong *)plVar12[1];
              plVar6 = plVar12;
            }
          }
        }
      } while (plVar12 != *(longlong **)(lVar5 + 0x20));
    }
    uVar9 = 0;
LAB_1801129bb:
    if (plVar4 != (longlong *)0x0) {
      LOCK();
      plVar12 = plVar4 + 1;
      *(int *)plVar12 = (int)*plVar12 + -1;
      UNLOCK();
      if ((int)*plVar12 == 0) {
        (**(code **)*plVar4)(plVar4);
        LOCK();
        piVar1 = (int *)((longlong)plVar4 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plVar4 + 8))(plVar4);
        }
      }
    }
  }
  return uVar9;
}

