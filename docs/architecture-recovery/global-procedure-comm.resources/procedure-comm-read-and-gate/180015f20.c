
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 FUN_180015f20(longlong param_1,undefined8 *param_2)

{
  longlong *plVar1;
  int *piVar2;
  undefined4 *puVar3;
  longlong *plVar4;
  longlong lVar5;
  char cVar6;
  undefined8 *puVar7;
  undefined *puVar8;
  undefined4 uVar9;
  undefined4 uVar10;
  
  piVar2 = (int *)*param_2;
  if (piVar2[1] == 0) {
    cVar6 = HSlog_ShouldLog(4);
    if (cVar6 == '\0') goto LAB_180015fcf;
    puVar3 = (undefined4 *)*param_2;
    if ((_DAT_180025eb0 & 1) == 0) {
      _DAT_180025eb0 = _DAT_180025eb0 | 1;
      FUN_1800013f0();
      FUN_180018df8(FUN_18001a860);
    }
    uVar10 = *puVar3;
    uVar9 = 0x39a;
    puVar8 = &UNK_18001d4f0;
LAB_180015f9c:
    puVar7 = (undefined8 *)&DAT_180025e90;
    if (0xf < _DAT_180025ea8) {
      puVar7 = _DAT_180025e90;
    }
  }
  else {
    if (*piVar2 == -1) {
      cVar6 = HSlog_ShouldLog(4);
      if (cVar6 == '\0') goto LAB_180015fcf;
      puVar3 = (undefined4 *)*param_2;
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      uVar10 = *puVar3;
      uVar9 = 0x39f;
      puVar8 = &UNK_18001d528;
      goto LAB_180015f9c;
    }
    if (piVar2[4] != -1) {
      if (((piVar2[3] == 0x20) || (piVar2[3] == 0x40)) && (piVar2[5] == -1)) {
        cVar6 = HSlog_ShouldLog(4);
        if (cVar6 == '\0') goto LAB_1800161c5;
        puVar3 = (undefined4 *)*param_2;
        puVar7 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar7[3]) {
          puVar7 = (undefined8 *)*puVar7;
        }
        uVar10 = *puVar3;
        uVar9 = 0x3ab;
        puVar8 = &UNK_18001d598;
      }
      else {
        if ((piVar2[3] != 0x80) && (piVar2[3] != 0x10)) {
LAB_180016164:
          FUN_18000ac70(param_2);
          return 1;
        }
        if ((piVar2[8] == -1) || (piVar2[7] == -1)) {
          cVar6 = HSlog_ShouldLog(4);
          if (cVar6 == '\0') goto LAB_1800161c5;
          puVar3 = (undefined4 *)*param_2;
          puVar7 = (undefined8 *)FUN_1800013b0();
          if (0xf < (ulonglong)puVar7[3]) {
            puVar7 = (undefined8 *)*puVar7;
          }
          uVar10 = *puVar3;
          uVar9 = 0x3b4;
          puVar8 = &UNK_18001d5d8;
        }
        else {
          if ((piVar2[1] != 1) || (piVar2[9] != -1)) goto LAB_180016164;
          cVar6 = HSlog_ShouldLog(4);
          if (cVar6 == '\0') goto LAB_1800161c5;
          puVar3 = (undefined4 *)*param_2;
          puVar7 = (undefined8 *)FUN_1800013b0();
          if (0xf < (ulonglong)puVar7[3]) {
            puVar7 = (undefined8 *)*puVar7;
          }
          uVar10 = *puVar3;
          uVar9 = 0x3ba;
          puVar8 = &UNK_18001d620;
        }
      }
      HSlog_LogPrint(4,puVar8,puVar7,*(undefined4 *)(param_1 + 0x48),&UNK_18001d4c8,uVar9,uVar10);
LAB_1800161c5:
      FUN_18000ac70(param_2);
      return 0;
    }
    cVar6 = HSlog_ShouldLog(4);
    if (cVar6 == '\0') goto LAB_180015fcf;
    puVar3 = (undefined4 *)*param_2;
    puVar7 = (undefined8 *)FUN_1800013b0();
    if (0xf < (ulonglong)puVar7[3]) {
      puVar7 = (undefined8 *)*puVar7;
    }
    uVar10 = *puVar3;
    uVar9 = 0x3a4;
    puVar8 = &UNK_18001d558;
  }
  HSlog_LogPrint(4,puVar8,puVar7,*(undefined4 *)(param_1 + 0x48),&UNK_18001d4c8,uVar9,uVar10);
LAB_180015fcf:
  plVar4 = (longlong *)param_2[1];
  if (plVar4 != (longlong *)0x0) {
    LOCK();
    plVar1 = plVar4 + 1;
    lVar5 = *plVar1;
    *(int *)plVar1 = (int)*plVar1 + -1;
    UNLOCK();
    if ((int)lVar5 == 1) {
      (**(code **)*plVar4)(plVar4);
      LOCK();
      piVar2 = (int *)((longlong)plVar4 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plVar4 + 8))(plVar4);
      }
    }
  }
  return 0;
}

