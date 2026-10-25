
undefined8 FUN_1800dd030(longlong param_1,char param_2)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  
  if (*(char *)(param_1 + 0x1a8) != '\0') {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      FUN_180015b40(uVar3,4,&UNK_18022c980,&UNK_18022c9b0,0x110,*(undefined4 *)(param_1 + 0x50));
    }
    return 0xe0000300;
  }
  cVar1 = FUN_1800dd200();
  if (cVar1 == '\0') {
    cVar1 = FUN_18001dbc0(2);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      FUN_180015b40(uVar3,2,&UNK_18022ca10,&UNK_18022c9b0,0x116,*(undefined4 *)(param_1 + 0x50));
    }
    FUN_1800de180(param_1,3,0,*(undefined4 *)(param_1 + 300));
    return 0;
  }
  iVar2 = SetEvent(*(undefined8 *)(param_1 + 0x90));
  if (iVar2 == 0) {
    GetLastError();
  }
  iVar2 = SetEvent(*(undefined8 *)(param_1 + 0x80));
  if (iVar2 == 0) {
    GetLastError();
  }
  *(undefined1 *)(param_1 + 0x7c) = 0;
  uVar3 = 3000;
  if (param_2 != '\0') {
    uVar3 = 0xffffffff;
  }
  iVar2 = FUN_1800da1c0(*(undefined8 *)(param_1 + 0x88),uVar3);
  if (iVar2 == 0x102) {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18001db50();
      FUN_180015b40(uVar3,4,&UNK_18022c9c8,&UNK_18022c9b0,0x120,*(undefined4 *)(param_1 + 0x50));
    }
    uVar3 = FUN_18008ea20();
    FUN_1800d8d30(uVar3,*(undefined4 *)(param_1 + 0x50));
    return 0xe0000300;
  }
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    FUN_180015b40(uVar3,2,&UNK_18022ca70,&UNK_18022c9b0,0x125,*(undefined4 *)(param_1 + 0x50));
  }
  return 0;
}

