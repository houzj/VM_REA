
void FUN_1800a53a0(longlong param_1)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined1 auStack_98 [32];
  longlong lStack_78;
  undefined4 *puStack_70;
  undefined8 *puStack_68;
  undefined4 auStack_58 [2];
  undefined8 uStack_50;
  undefined4 uStack_48;
  undefined1 uStack_44;
  undefined2 uStack_43;
  undefined1 uStack_41;
  undefined8 uStack_40;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
  uStack_50 = 0xfffffffffffffffe;
  uStack_18 = DAT_1802a5230 ^ (ulonglong)auStack_98;
  uStack_48 = *(undefined4 *)(param_1 + 0x2c);
  uStack_44 = 0;
  uStack_43 = 0;
  uStack_41 = 0;
  uStack_40 = *(undefined8 *)(param_1 + 0xf8);
  auStack_58[0] = 0;
  if ((DAT_1802b2b28 & 1) == 0) {
    DAT_1802b2b28 = DAT_1802b2b28 | 1;
    FUN_1801e8930(&DAT_1802029f0);
  }
  lStack_78 = param_1 + 0x21c;
  puStack_70 = auStack_58;
  iVar2 = FUN_18011e580(&DAT_1802b2b2c,&uStack_48,&DAT_180227688,0);
  if (iVar2 == 0) {
    if ((DAT_1802b2b28 & 1) == 0) {
      DAT_1802b2b28 = DAT_1802b2b28 | 1;
      FUN_1801e8930(&DAT_1802029f0);
    }
    lStack_78 = param_1 + 0x220;
    puStack_70 = auStack_58;
    iVar2 = FUN_18011e580(&DAT_1802b2b2c,&uStack_48,&DAT_180227748,0);
    if (iVar2 == 0) goto LAB_1800a557a;
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 == '\0') goto LAB_1800a557a;
    puStack_68 = (undefined8 *)FUN_18008f750(auStack_38,iVar2);
    if (0xf < (ulonglong)puStack_68[3]) {
      puStack_68 = (undefined8 *)*puStack_68;
    }
    uVar3 = FUN_18001db50();
    puStack_70 = (undefined4 *)CONCAT44(puStack_70._4_4_,*(undefined4 *)(param_1 + 0x2c));
    lStack_78 = CONCAT44(lStack_78._4_4_,0x3b7);
    FUN_180015b40(uVar3,4,&DAT_180227da8,&DAT_180227d68);
  }
  else {
    cVar1 = FUN_18001dbc0(4);
    if (cVar1 == '\0') goto LAB_1800a557a;
    puStack_68 = (undefined8 *)FUN_18008f750(auStack_38,iVar2);
    if (0xf < (ulonglong)puStack_68[3]) {
      puStack_68 = (undefined8 *)*puStack_68;
    }
    uVar3 = FUN_18001db50();
    puStack_70 = (undefined4 *)CONCAT44(puStack_70._4_4_,*(undefined4 *)(param_1 + 0x2c));
    lStack_78 = CONCAT44(lStack_78._4_4_,0x3b0);
    FUN_180015b40(uVar3,4,&DAT_180227d28,&DAT_180227d68);
  }
  if (0xf < uStack_20) {
    __3_YAXPEAX_Z(auStack_38[0]);
  }
LAB_1800a557a:
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_98);
  return;
}

