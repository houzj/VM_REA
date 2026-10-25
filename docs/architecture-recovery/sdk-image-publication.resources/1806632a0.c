
undefined8 FUN_1806632a0(longlong param_1,longlong param_2,undefined8 param_3)

{
  longlong lVar1;
  char cVar2;
  undefined8 uVar3;
  
  cVar2 = FUN_1805a2010(2);
  if (cVar2 != '\0') {
    uVar3 = FUN_1805a1d70();
    FUN_1805a1f10(uVar3,2,&UNK_180abc4d0,&UNK_180ac4f70,0x48c);
  }
  if (*(longlong *)(param_1 + 0x388) == 0) {
    if ((*(longlong *)(param_1 + 0x58) == 0) || (param_2 == 0)) {
      lVar1 = *(longlong *)(param_1 + 0x13a80);
      *(longlong *)(param_1 + 0x58) = param_2;
      *(undefined8 *)(param_1 + 0x60) = param_3;
      *(longlong *)(lVar1 + 0x50) = param_2;
      *(undefined8 *)(lVar1 + 0x58) = param_3;
      *(bool *)(param_1 + 0xd238) = param_2 != 0;
      cVar2 = FUN_1805a2010(2);
      if (cVar2 != '\0') {
        uVar3 = FUN_1805a1d70();
        FUN_1805a1f10(uVar3,2,&UNK_180abc4c0,&UNK_180ac4f70,0x4a9);
      }
      return 0;
    }
    cVar2 = FUN_1805a2010(4);
    if (cVar2 != '\0') {
      uVar3 = FUN_1805a1d70();
      FUN_1805a1f10(uVar3,4,&UNK_180ac50b0,&UNK_180ac4f70,0x496);
      return 0xe0000007;
    }
  }
  else {
    cVar2 = FUN_1805a2010(4);
    if (cVar2 != '\0') {
      uVar3 = FUN_1805a1d70();
      FUN_1805a1f10(uVar3,4,&UNK_180ac5110,&UNK_180ac4f70,0x490);
    }
  }
  return 0xe0000007;
}

