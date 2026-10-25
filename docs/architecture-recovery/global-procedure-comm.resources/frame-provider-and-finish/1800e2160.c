
void FUN_1800e2160(longlong param_1)

{
  longlong *plVar1;
  int *piVar2;
  char cVar3;
  char cVar4;
  char *pcVar5;
  undefined8 uVar6;
  undefined8 *puVar7;
  undefined *puVar8;
  undefined *puVar9;
  undefined *puVar10;
  undefined1 auStack_20 [8];
  longlong *plStack_18;
  
  if ((*(char *)(param_1 + 0x1a9) != '\0') && (pcVar5 = (char *)FUN_18008eae0(), *pcVar5 != '\0')) {
    uVar6 = FUN_18008eae0();
    cVar3 = FUN_1800ec610(uVar6,*(undefined4 *)(param_1 + 0x50));
    if (cVar3 != '\0') {
      if ((DAT_1802b129e == '\0') && (*(longlong *)(param_1 + 0xf0) != 0)) {
        FUN_180079770(*(longlong *)(param_1 + 0xf0),*(undefined4 *)(param_1 + 0xe0));
      }
      puVar7 = (undefined8 *)FUN_1800d33b0(auStack_20);
      FUN_1800d34a0(*puVar7,*(undefined4 *)(param_1 + 0x50),*(undefined4 *)(param_1 + 0x50),
                    *(undefined8 *)(param_1 + 0x198));
      if (plStack_18 == (longlong *)0x0) {
        return;
      }
      LOCK();
      plVar1 = plStack_18 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 != 0) {
        return;
      }
      (**(code **)*plStack_18)(plStack_18);
      LOCK();
      piVar2 = (int *)((longlong)plStack_18 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 != 0) {
        return;
      }
      (**(code **)(*plStack_18 + 8))(plStack_18);
      return;
    }
  }
  cVar3 = FUN_18001dbc0(1);
  if (cVar3 != '\0') {
    pcVar5 = (char *)FUN_18008eae0();
    cVar3 = *pcVar5;
    uVar6 = FUN_18008eae0();
    cVar4 = FUN_1800ec610(uVar6,*(undefined4 *)(param_1 + 0x50));
    puVar9 = &UNK_18021cf28;
    puVar10 = &UNK_18021cf28;
    if (cVar4 != '\0') {
      puVar10 = &UNK_18021cf30;
    }
    puVar8 = &UNK_18021cf28;
    if (cVar3 != '\0') {
      puVar8 = &UNK_18021cf30;
    }
    if (*(char *)(param_1 + 0x1a9) != '\0') {
      puVar9 = &UNK_18021cf30;
    }
    uVar6 = FUN_18001db50();
    FUN_180015b40(uVar6,1,&UNK_18022d4e0,&UNK_18022d478,0x541,*(undefined4 *)(param_1 + 0x50),puVar9
                  ,puVar8,puVar10);
  }
  return;
}

