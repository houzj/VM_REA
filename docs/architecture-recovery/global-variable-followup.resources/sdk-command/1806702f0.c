
int FUN_1806702f0(longlong param_1,int param_2,undefined4 param_3)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  longlong lVar4;
  
  cVar1 = FUN_1805a2010(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180ac8368,&UNK_180ac83a0,0x151f,param_2,param_3);
  }
  if (param_2 < 1) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_1805a1d70();
      FUN_1805a1f10(uVar3,4,&UNK_180ac8130,&UNK_180ac83a0,0x1521);
    }
    return -0x1fffffff;
  }
  func_0x0001809e87c4(*(undefined8 *)(param_1 + 0x108),0,0xc800);
  sprintf_s(*(undefined8 *)(param_1 + 0x108),0xc800,&UNK_180ac5830,0x408,param_2,param_3);
  lVar4 = -1;
  do {
    lVar4 = lVar4 + 1;
  } while (*(char *)(*(longlong *)(param_1 + 0x108) + lVar4) != '\0');
  iVar2 = FUN_1806c00d0(param_1);
  if (iVar2 != 0) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_1805a1d70();
      FUN_1805a1f10(uVar3,4,&UNK_180ac5400,&UNK_180ac83a0,0x152a,iVar2);
    }
    return iVar2;
  }
  cVar1 = FUN_1805a2010(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180abc4c0,&UNK_180ac83a0,0x152e);
  }
  return 0;
}

