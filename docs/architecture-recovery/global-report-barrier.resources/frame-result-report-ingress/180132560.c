
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_ReportData(void)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined4 in_R9D;
  undefined1 auStack_88 [32];
  undefined4 uStack_68;
  undefined8 *puStack_60;
  undefined8 auStack_58 [3];
  ulonglong uStack_40;
  ulonglong uStack_38;
  
                    /* 0x132560  168  VM_M_ReportData */
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_88;
  cVar1 = FUN_18001dbc0(0);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    puStack_60 = (undefined8 *)CONCAT44(puStack_60._4_4_,in_R9D);
    uStack_68 = 0x285;
    FUN_180015b40(uVar3,0,&UNK_180234420,&UNK_180234410);
  }
  if ((_DAT_1802b2b28 & 1) == 0) {
    _DAT_1802b2b28 = _DAT_1802b2b28 | 1;
    FUN_1801e8930(&UNK_1802029f0);
  }
  uStack_68 = in_R9D;
  uVar2 = FUN_1801241c0();
  cVar1 = FUN_18001dbc0(0);
  if (cVar1 != '\0') {
    puVar4 = (undefined8 *)FUN_18008f750(auStack_58,uVar2);
    if (0xf < (ulonglong)puVar4[3]) {
      puVar4 = (undefined8 *)*puVar4;
    }
    uVar3 = FUN_18001db50();
    uStack_68 = 0x289;
    puStack_60 = puVar4;
    FUN_180015b40(uVar3,0,&UNK_180234258,&UNK_180234410);
    if (0xf < uStack_40) {
      __3_YAXPEAX_Z(auStack_58[0]);
    }
  }
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_88);
  return;
}

