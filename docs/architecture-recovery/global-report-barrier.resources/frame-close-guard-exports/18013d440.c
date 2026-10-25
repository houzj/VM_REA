
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_CloseSolution(void)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined1 auStack_68 [32];
  undefined4 uStack_48;
  undefined8 *puStack_40;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x13d440  57  VM_M_CloseSolution */
  uStack_18 = _DAT_1802a5230 ^ (ulonglong)auStack_68;
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    uStack_48 = 0x484;
    FUN_180015b40(uVar3,2,&UNK_1802374e0,&UNK_1802374c8);
  }
  if ((_DAT_1802b12c0 & 1) == 0) {
    _DAT_1802b12c0 = _DAT_1802b12c0 | 1;
    FUN_18003fde0();
    FUN_1801e8930(&UNK_180202970);
  }
  uVar2 = FUN_180047570(&DAT_1802b12d0);
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    puStack_40 = (undefined8 *)FUN_18008f750(auStack_38,uVar2);
    if (0xf < (ulonglong)puStack_40[3]) {
      puStack_40 = (undefined8 *)*puStack_40;
    }
    uVar3 = FUN_18001db50();
    uStack_48 = 0x48a;
    FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_1802374c8);
    if (0xf < uStack_20) {
      __3_YAXPEAX_Z(auStack_38[0]);
    }
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_68);
  return;
}

