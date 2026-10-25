
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1805edd00(undefined8 param_1,undefined4 param_2,ulonglong param_3,int param_4)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_98 [32];
  undefined1 *puStack_78;
  ulonglong uStack_70;
  int iStack_68;
  undefined8 uStack_58;
  undefined1 uStack_50;
  undefined7 uStack_4f;
  undefined8 uStack_40;
  ulonglong uStack_38;
  ulonglong uStack_30;
  
  uStack_58 = 0xfffffffffffffffe;
  uStack_30 = _DAT_1808559e0 ^ (ulonglong)auStack_98;
  cVar1 = FUN_18057a5b0(1);
  if (cVar1 != '\0') {
    uVar3 = FUN_18057a310();
    puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x1da7);
    FUN_18057a4b0(uVar3,1,&UNK_1807daa68,&UNK_1807da920);
  }
  if ((param_3 == 0) || (param_4 < 1)) {
    cVar1 = FUN_18057a5b0(4);
    if (cVar1 != '\0') {
      uVar3 = FUN_18057a310();
      puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x1daa);
      FUN_18057a4b0(uVar3,4,&UNK_1807daa40,&UNK_1807da920);
    }
  }
  else {
    uStack_38 = 0xf;
    uStack_40 = 0;
    uStack_50 = 0;
    uStack_70 = uStack_70 & 0xffffffffffffff00;
    puStack_78 = &uStack_50;
    iVar2 = FUN_1805de710(param_1,param_3,(longlong)param_4,param_2);
    if ((iVar2 != 0) && (cVar1 = FUN_18057a5b0(4), cVar1 != '\0')) {
      uVar3 = FUN_18057a310();
      puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x1db1);
      uStack_70 = param_3;
      FUN_18057a4b0(uVar3,4,&UNK_1807daa10,&UNK_1807da920);
    }
    cVar1 = FUN_18057a5b0(2);
    if (cVar1 != '\0') {
      uVar3 = FUN_18057a310();
      uStack_70 = CONCAT44(uStack_70._4_4_,param_2);
      puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x1db4);
      iStack_68 = iVar2;
      FUN_18057a4b0(uVar3,2,&UNK_1807da9d8,&UNK_1807da920);
    }
    if (0xf < uStack_38) {
      __3_YAXPEAX_Z(CONCAT71(uStack_4f,uStack_50));
    }
  }
  FUN_180745b60(uStack_30 ^ (ulonglong)auStack_98);
  return;
}

