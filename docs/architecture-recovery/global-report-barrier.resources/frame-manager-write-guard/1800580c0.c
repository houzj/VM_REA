
int FUN_1800580c0(longlong param_1)

{
  undefined1 uVar1;
  undefined1 uVar2;
  undefined1 uVar3;
  undefined1 uVar4;
  char cVar5;
  undefined8 uVar6;
  int iVar7;
  
  iVar7 = 0;
  if ((*(char *)(param_1 + 0x20) != '\0') || (*(char *)(param_1 + 0x58) != '\0')) {
    iVar7 = -0x1ffffff3;
  }
  if (*(char *)(param_1 + 0x22) != '\0') {
    iVar7 = -0x1fffffe3;
  }
  if (*(char *)(param_1 + 0x23) == '\0') {
    if (iVar7 == 0) {
      cVar5 = FUN_18001dbc0(0);
      if (cVar5 == '\0') {
        return 0;
      }
      uVar1 = *(undefined1 *)(param_1 + 0x23);
      uVar2 = *(undefined1 *)(param_1 + 0x58);
      uVar3 = *(undefined1 *)(param_1 + 0x22);
      uVar4 = *(undefined1 *)(param_1 + 0x20);
      uVar6 = FUN_18001db50();
      FUN_180015b40(uVar6,0,&UNK_180222350,&UNK_180222320,0x11fd,uVar4,uVar3,uVar2,uVar1);
      return 0;
    }
  }
  else {
    iVar7 = -0x1fffffe2;
  }
  cVar5 = FUN_18001dbc0(3);
  if (cVar5 != '\0') {
    uVar1 = *(undefined1 *)(param_1 + 0x23);
    uVar2 = *(undefined1 *)(param_1 + 0x58);
    uVar3 = *(undefined1 *)(param_1 + 0x22);
    uVar4 = *(undefined1 *)(param_1 + 0x20);
    uVar6 = FUN_18001db50();
    FUN_180015b40(uVar6,3,&UNK_180222350,&UNK_180222320,0x11f8,uVar4,uVar3,uVar2,uVar1);
  }
  return iVar7;
}

