
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 FUN_180012f00(longlong param_1,longlong *param_2)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined4 *puVar4;
  char cVar5;
  undefined4 uVar6;
  undefined8 uVar7;
  undefined8 *puVar8;
  undefined *puVar9;
  undefined4 uVar10;
  undefined8 uStack_50;
  undefined8 uStack_48;
  undefined1 auStack_40 [16];
  undefined1 auStack_30 [16];
  undefined1 auStack_20 [24];
  
  if (*param_2 == 0) {
    plVar3 = (longlong *)param_2[1];
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
          (**(code **)(*plVar3 + 8))(plVar3);
        }
      }
    }
    return 0xe0000001;
  }
  uStack_50 = 0;
  uStack_48 = 0;
  if (param_2[1] != 0) {
    LOCK();
    piVar2 = (int *)(param_2[1] + 8);
    *piVar2 = *piVar2 + 1;
    UNLOCK();
  }
  FUN_180018b20(&uStack_50);
  cVar5 = FUN_180015f20(param_1,&uStack_50);
  if (cVar5 == '\0') {
    cVar5 = HSlog_ShouldLog(4);
    if (cVar5 != '\0') {
      puVar4 = (undefined4 *)*param_2;
      if ((_DAT_180025eb0 & 1) == 0) {
        _DAT_180025eb0 = _DAT_180025eb0 | 1;
        FUN_1800013f0();
        FUN_180018df8(FUN_18001a860);
      }
      puVar8 = (undefined8 *)&DAT_180025e90;
      if (0xf < _DAT_180025ea8) {
        puVar8 = _DAT_180025e90;
      }
      uVar6 = *puVar4;
      uVar10 = 0x43;
      puVar9 = &UNK_18001ccc0;
LAB_1800130de:
      HSlog_LogPrint(4,puVar9,puVar8,*(undefined4 *)(param_1 + 0x48),&UNK_18001cc68,uVar10,uVar6);
    }
LAB_1800130fa:
    plVar3 = (longlong *)param_2[1];
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
          (**(code **)(*plVar3 + 8))(plVar3);
        }
      }
    }
    uVar6 = 0xe000001b;
  }
  else {
    switch(*(undefined4 *)(*param_2 + 0xc)) {
    case 1:
    case 2:
    case 4:
    case 8:
      uVar7 = FUN_180016bc0(auStack_40,param_2);
      uVar6 = FUN_1800131d0(param_1,uVar7);
      FUN_18000ac70(param_2);
      break;
    default:
      cVar5 = HSlog_ShouldLog(4);
      if (cVar5 != '\0') {
        puVar4 = (undefined4 *)*param_2;
        puVar8 = (undefined8 *)FUN_1800013b0();
        if (0xf < (ulonglong)puVar8[3]) {
          puVar8 = (undefined8 *)*puVar8;
        }
        uVar6 = *puVar4;
        uVar10 = 0x59;
        puVar9 = &UNK_18001ccf0;
        goto LAB_1800130de;
      }
      goto LAB_1800130fa;
    case 0x10:
    case 0x80:
      uVar7 = FUN_180016bc0(auStack_30,param_2);
      uVar6 = FUN_180013bc0(param_1,uVar7);
      FUN_18000ac70(param_2);
      break;
    case 0x20:
    case 0x40:
      uVar7 = FUN_180016bc0(auStack_20,param_2);
      uVar6 = FUN_180013670(param_1,uVar7);
      FUN_18000ac70(param_2);
    }
  }
  return uVar6;
}

