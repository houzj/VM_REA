
int FUN_180670170(longlong param_1,int param_2)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  longlong lVar4;
  
  cVar1 = FUN_1805a2010(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180ac82a0,&UNK_180ac82c8,0x1508,param_2);
  }
  if (param_2 < 1) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_1805a1d70();
      FUN_1805a1f10(uVar3,4,&UNK_180ac8130,&UNK_180ac82c8,0x150a);
    }
    return -0x1fffffff;
  }
  func_0x0001809e87c4(*(undefined8 *)(param_1 + 0x108),0,0xc800);
  sprintf_s(*(undefined8 *)(param_1 + 0x108),0xc800,&UNK_180ac54ec,0x407,param_2);
  lVar4 = -1;
  do {
    lVar4 = lVar4 + 1;
  } while (*(char *)(*(longlong *)(param_1 + 0x108) + lVar4) != '\0');
  iVar2 = FUN_1806c00d0(param_1);
  if (iVar2 != 0) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_1805a1d70();
      FUN_1805a1f10(uVar3,4,&UNK_180ac5400,&UNK_180ac82c8,0x1514,iVar2);
    }
    return iVar2;
  }
  cVar1 = FUN_1805a2010(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180ac8288,&UNK_180ac82c8,0x1518);
  }
  return 0;
}

