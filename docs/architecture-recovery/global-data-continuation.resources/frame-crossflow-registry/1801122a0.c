
undefined8 FUN_1801122a0(longlong param_1,ulonglong param_2,longlong *param_3,undefined8 param_4)

{
  undefined8 *puVar1;
  longlong *plVar2;
  longlong *plVar3;
  char cVar4;
  undefined8 uVar5;
  undefined8 *puVar6;
  undefined8 *puVar7;
  longlong *plVar8;
  uint auStackX_10 [2];
  undefined8 *puStackX_18;
  longlong *plStack_40;
  undefined8 uStack_38;
  undefined1 auStack_30 [24];
  
  auStackX_10[0] = (uint)param_2;
  puVar1 = (undefined8 *)*param_3;
  cVar4 = *(char *)((longlong)puVar1[1] + 0x19);
  puStackX_18 = puVar1;
  puVar7 = (undefined8 *)puVar1[1];
  while (cVar4 == '\0') {
    if (*(int *)((longlong)puVar7 + 0x1c) < (int)auStackX_10[0]) {
      puVar6 = (undefined8 *)puVar7[2];
      puVar7 = puStackX_18;
    }
    else {
      puVar6 = (undefined8 *)*puVar7;
    }
    puStackX_18 = puVar7;
    puVar7 = puVar6;
    cVar4 = *(char *)((longlong)puVar6 + 0x19);
  }
  if ((puStackX_18 == puVar1) || ((int)auStackX_10[0] < *(int *)((longlong)puStackX_18 + 0x1c))) {
    puStackX_18 = puVar1;
  }
  if ((puStackX_18 == puVar1) && (puVar7 = (undefined8 *)*puVar1, puVar7 != puVar1)) {
    while (cVar4 = FUN_180116b70(param_1,param_2,*(undefined4 *)((longlong)puVar7 + 0x1c)),
          cVar4 == '\0') {
      if (*(char *)((longlong)puVar7 + 0x19) == '\0') {
        puVar1 = (undefined8 *)puVar7[2];
        if (*(char *)((longlong)puVar1 + 0x19) == '\0') {
          cVar4 = *(char *)((longlong)*puVar1 + 0x19);
          puVar7 = puVar1;
          puVar1 = (undefined8 *)*puVar1;
          while (cVar4 == '\0') {
            cVar4 = *(char *)((longlong)*puVar1 + 0x19);
            puVar7 = puVar1;
            puVar1 = (undefined8 *)*puVar1;
          }
        }
        else {
          cVar4 = *(char *)((longlong)puVar7[1] + 0x19);
          puVar6 = (undefined8 *)puVar7[1];
          puVar1 = puVar7;
          while ((puVar7 = puVar6, cVar4 == '\0' && (puVar1 == (undefined8 *)puVar7[2]))) {
            cVar4 = *(char *)((longlong)puVar7[1] + 0x19);
            puVar6 = (undefined8 *)puVar7[1];
            puVar1 = puVar7;
          }
        }
      }
      if (puVar7 == (undefined8 *)*param_3) {
        return 0;
      }
      param_2 = (ulonglong)auStackX_10[0];
    }
    puVar1 = *(undefined8 **)(param_1 + 0x40);
    cVar4 = *(char *)((longlong)puVar1[1] + 0x19);
    puStackX_18 = puVar1;
    puVar7 = (undefined8 *)puVar1[1];
    while (cVar4 == '\0') {
      if (*(int *)(puVar7 + 4) < (int)auStackX_10[0]) {
        puVar6 = (undefined8 *)puVar7[2];
        puVar7 = puStackX_18;
      }
      else {
        puVar6 = (undefined8 *)*puVar7;
      }
      puStackX_18 = puVar7;
      puVar7 = puVar6;
      cVar4 = *(char *)((longlong)puVar6 + 0x19);
    }
    if ((puStackX_18 == puVar1) || ((int)auStackX_10[0] < *(int *)(puStackX_18 + 4))) {
      puStackX_18 = puVar1;
    }
    if (puStackX_18 != puVar1) {
      FUN_180071a90(param_4,auStack_30,0,auStackX_10,DAT_1802b1205);
      uVar5 = FUN_18009e260(param_1 + 0x50,auStackX_10);
      FUN_180076af0(&plStack_40,uVar5);
      plVar8 = (longlong *)*plStack_40;
      if (plVar8 != plStack_40) {
        do {
          FUN_1801122a0(param_1,*(undefined4 *)((longlong)plVar8 + 0x1c),param_3,param_4);
          if (*(char *)((longlong)plVar8 + 0x19) == '\0') {
            plVar2 = (longlong *)plVar8[2];
            if (*(char *)((longlong)plVar2 + 0x19) == '\0') {
              cVar4 = *(char *)(*plVar2 + 0x19);
              plVar8 = plVar2;
              plVar2 = (longlong *)*plVar2;
              while (cVar4 == '\0') {
                cVar4 = *(char *)(*plVar2 + 0x19);
                plVar8 = plVar2;
                plVar2 = (longlong *)*plVar2;
              }
            }
            else {
              cVar4 = *(char *)(plVar8[1] + 0x19);
              plVar3 = (longlong *)plVar8[1];
              plVar2 = plVar8;
              while ((plVar8 = plVar3, cVar4 == '\0' && (plVar2 == (longlong *)plVar8[2]))) {
                cVar4 = *(char *)(plVar8[1] + 0x19);
                plVar3 = (longlong *)plVar8[1];
                plVar2 = plVar8;
              }
            }
          }
        } while (plVar8 != plStack_40);
      }
      cVar4 = *(char *)(plStack_40[1] + 0x19);
      plVar8 = (longlong *)plStack_40[1];
      while (cVar4 == '\0') {
        FUN_18003c6d0(&plStack_40,plVar8[2]);
        plVar2 = (longlong *)*plVar8;
        __3_YAXPEAX_Z(plVar8);
        plVar8 = plVar2;
        cVar4 = *(char *)((longlong)plVar2 + 0x19);
      }
      plStack_40[1] = (longlong)plStack_40;
      *plStack_40 = (longlong)plStack_40;
      plStack_40[2] = (longlong)plStack_40;
      uStack_38 = 0;
      __3_YAXPEAX_Z(plStack_40);
    }
  }
  return 0;
}

