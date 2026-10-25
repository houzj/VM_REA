
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180695890(undefined8 param_1,longlong param_2)

{
  char cVar1;
  int iVar2;
  uint uVar3;
  undefined8 uVar4;
  ulonglong uVar5;
  ulonglong uVar6;
  longlong lVar7;
  undefined1 auStack_a8 [32];
  undefined4 uStack_88;
  uint uStack_80;
  undefined1 *puStack_78;
  int iStack_68;
  undefined4 uStack_64;
  int aiStack_60 [2];
  ulonglong uStack_58;
  undefined4 uStack_50;
  int aiStack_4c [3];
  undefined1 uStack_40;
  undefined7 uStack_3f;
  undefined8 uStack_30;
  ulonglong uStack_28;
  ulonglong uStack_20;
  
  aiStack_4c[1] = -2;
  aiStack_4c[2] = -1;
  uStack_20 = _DAT_180c35580 ^ (ulonglong)auStack_a8;
  cVar1 = FUN_1805a2010(2);
  if (cVar1 != '\0') {
    uVar4 = FUN_1805a1d70();
    uStack_88 = 0x320e;
    FUN_1805a1f10(uVar4,2,&UNK_180abc948,&UNK_180accac8);
  }
  iStack_68 = 0xe00000ff;
  uStack_58 = 0;
  if (param_2 == 0) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar4 = FUN_1805a1d70();
      uStack_88 = 0x3215;
      FUN_1805a1f10(uVar4,4,&UNK_180aac378,&UNK_180accac8);
    }
    iStack_68 = 0xe000000a;
    uStack_50 = 0xe000000a;
    func_0x0001809e87d0(&uStack_50,&UNK_180c23c00);
  }
  uStack_28 = 0xf;
  uStack_30 = 0;
  uStack_40 = 0;
  FUN_1805911d0(&uStack_40,&UNK_180aab591,0);
  iVar2 = FUN_180680b40();
  iStack_68 = iVar2;
  if (iVar2 != 0) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar4 = FUN_1805a1d70();
      uStack_88 = 0x321e;
      FUN_1805a1f10(uVar4,4,&UNK_180acca70,&UNK_180accac8);
    }
    aiStack_4c[0] = iVar2;
    func_0x0001809e87d0(aiStack_4c,&UNK_180c23c00);
  }
  iVar2 = (int)uStack_30;
  lVar7 = (longlong)((int)uStack_30 + 0x10);
  uVar5 = func_0x0001809e81e4(lVar7,0x180c9ea09);
  uVar6 = uVar5;
  uStack_58 = uVar5;
  if (uVar5 == 0) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar3 = GetLastError();
      uVar6 = (ulonglong)uVar3;
      uVar4 = FUN_1805a1d70();
      uStack_88 = 0x3226;
      uStack_80 = uVar3;
      FUN_1805a1f10(uVar4,4,&UNK_180ac5580,&UNK_180accac8);
    }
    iStack_68 = -0x1ffffffe;
    uStack_64 = 0xe0000002;
    uVar5 = func_0x0001809e87d0(&uStack_64,&UNK_180c23c00);
  }
  func_0x0001809e87c4(uVar5,0,lVar7);
  puStack_78 = &uStack_40;
  if (0xf < uStack_28) {
    puStack_78 = (undefined1 *)CONCAT71(uStack_3f,uStack_40);
  }
  uStack_80 = 0xc09;
  uStack_88 = 0x417;
  _snprintf_s(uVar6,lVar7,(longlong)(iVar2 + 0xf),&UNK_180ac5570);
  lVar7 = -1;
  do {
    lVar7 = lVar7 + 1;
  } while (*(char *)(uVar6 + lVar7) != '\0');
  iVar2 = FUN_1806c00d0(param_1,uVar6);
  iStack_68 = iVar2;
  if (iVar2 != 0) {
    cVar1 = FUN_1805a2010(4);
    if (cVar1 != '\0') {
      uVar4 = FUN_1805a1d70();
      uStack_88 = 0x3233;
      uStack_80 = iVar2;
      FUN_1805a1f10(uVar4,4,&UNK_180ac5400,&UNK_180accac8);
    }
    aiStack_60[0] = iVar2;
    func_0x0001809e87d0(aiStack_60,&UNK_180c23c00);
  }
  ___V_YAXPEAX_Z(uVar6);
  uStack_58 = 0;
  if (0xf < uStack_28) {
    __3_YAXPEAX_Z(CONCAT71(uStack_3f,uStack_40));
  }
  uStack_28 = 0xf;
  uStack_30 = 0;
  uStack_40 = 0;
  cVar1 = FUN_1805a2010(2);
  if (cVar1 != '\0') {
    uVar4 = FUN_1805a1d70();
    uStack_88 = 0x3248;
    FUN_1805a1f10(uVar4,2,&UNK_180ab52d8,&UNK_180accac8);
  }
  FUN_1809e7e30(uStack_20 ^ (ulonglong)auStack_a8);
  return;
}

