
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800f01e0(undefined8 *param_1)

{
  char cVar1;
  int iVar2;
  int iVar3;
  undefined8 uVar4;
  undefined8 uVar5;
  undefined8 *puVar6;
  undefined8 in_stack_ffffffffffffffd8;
  undefined4 uVar7;
  
  uVar7 = (undefined4)((ulonglong)in_stack_ffffffffffffffd8 >> 0x20);
  if ((param_1[6] != 0) && (param_1[5] != 0)) {
    iVar3 = 0;
    iVar2 = UnmapViewOfFile();
    if (iVar2 == 0) {
      iVar3 = GetLastError();
    }
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      puVar6 = param_1;
      if (0xf < (ulonglong)param_1[3]) {
        puVar6 = (undefined8 *)*param_1;
      }
      uVar4 = FUN_18001db50();
      uVar5 = param_1[5];
      FUN_180015b40(uVar4,0,&UNK_18022e340,&UNK_18022e2d0,0x161,puVar6,uVar5,iVar3);
      uVar7 = (undefined4)((ulonglong)uVar5 >> 0x20);
    }
    if ((iVar3 != 0) && (cVar1 = FUN_18001dbc0(4), cVar1 != '\0')) {
      puVar6 = param_1;
      if (0xf < (ulonglong)param_1[3]) {
        puVar6 = (undefined8 *)*param_1;
      }
      uVar5 = FUN_18001db50();
      FUN_180015b40(uVar5,4,&UNK_18022e320,&UNK_18022e2d0,0x164,puVar6);
    }
  }
  if (param_1[4] != -1) {
    iVar3 = 0;
    iVar2 = CloseHandle();
    if (iVar2 == 0) {
      iVar3 = GetLastError();
    }
    cVar1 = FUN_18001dbc0(0);
    if (cVar1 != '\0') {
      puVar6 = param_1;
      if (0xf < (ulonglong)param_1[3]) {
        puVar6 = (undefined8 *)*param_1;
      }
      uVar5 = FUN_18001db50();
      uVar4 = CONCAT44(uVar7,iVar3);
      FUN_180015b40(uVar5,0,&UNK_18022e3a0,&UNK_18022e2d0,0x16b,puVar6,uVar4);
      uVar7 = (undefined4)((ulonglong)uVar4 >> 0x20);
    }
    if ((iVar3 != 0) && (cVar1 = FUN_18001dbc0(4), cVar1 != '\0')) {
      puVar6 = param_1;
      if (0xf < (ulonglong)param_1[3]) {
        puVar6 = (undefined8 *)*param_1;
      }
      uVar5 = FUN_18001db50();
      FUN_180015b40(uVar5,4,&UNK_18022e378,&UNK_18022e2d0,0x16e,puVar6);
    }
  }
  LOCK();
  _DAT_1802b11f4 = _DAT_1802b11f4 + -1;
  UNLOCK();
  cVar1 = FUN_18001dbc0(1);
  if (cVar1 != '\0') {
    puVar6 = param_1;
    if (0xf < (ulonglong)param_1[3]) {
      puVar6 = (undefined8 *)*param_1;
    }
    uVar5 = FUN_18001db50();
    FUN_180015b40(uVar5,1,&UNK_18022e3d0,&UNK_18022e2d0,0x172,puVar6,CONCAT44(uVar7,_DAT_1802b11f4))
    ;
  }
  if (0xf < (ulonglong)param_1[3]) {
    __3_YAXPEAX_Z(*param_1);
  }
  param_1[3] = 0xf;
  param_1[2] = 0;
  *(undefined1 *)param_1 = 0;
  return;
}

