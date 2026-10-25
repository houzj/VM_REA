
undefined8 FUN_1800dd200(longlong param_1)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  char cVar4;
  undefined8 *puVar5;
  undefined8 uVar6;
  undefined *puVar7;
  undefined4 uVar8;
  undefined4 uVar9;
  undefined8 uVar10;
  undefined1 auStack_20 [8];
  longlong *plStack_18;
  
  uVar10 = 0xfffffffffffffffe;
  if ((*(char *)(param_1 + 0xe9) == '\0') && (*(int *)(param_1 + 0xd8) != 2)) {
    puVar5 = (undefined8 *)FUN_1800d33b0(auStack_20);
    cVar4 = FUN_1800d52d0(*puVar5,*(undefined4 *)(param_1 + 0x50),*(undefined8 *)(param_1 + 0x198));
    if (plStack_18 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_18 + 1;
      lVar3 = *plVar1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)lVar3 == 1) {
        (**(code **)*plStack_18)(plStack_18);
        LOCK();
        piVar2 = (int *)((longlong)plStack_18 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_18 + 8))(plStack_18);
        }
      }
    }
    if (cVar4 != '\0') {
      return 0;
    }
    if (DAT_1802b129e != '\0') {
      return 1;
    }
    cVar4 = FUN_18001dbc0(2);
    if (cVar4 == '\0') {
      return 1;
    }
    uVar6 = FUN_18001db50();
    uVar9 = *(undefined4 *)(param_1 + 0x50);
    uVar8 = 0x144;
    puVar7 = &UNK_18022caa8;
  }
  else {
    if (DAT_1802b129e != '\0') {
      return 1;
    }
    cVar4 = FUN_18001dbc0(2);
    if (cVar4 == '\0') {
      return 1;
    }
    uVar6 = FUN_18001db50();
    uVar9 = *(undefined4 *)(param_1 + 0x50);
    uVar8 = 0x13b;
    puVar7 = &UNK_18022cae0;
  }
  FUN_180015b40(uVar6,2,puVar7,&UNK_18022ca58,uVar8,uVar9,uVar10);
  return 1;
}

