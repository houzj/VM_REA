
void VM_M_SetImageBufferNodeNum(undefined4 param_1,undefined4 param_2)

{
  char cVar1;
  undefined4 uVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  undefined1 auStack_78 [32];
  undefined4 uStack_58;
  undefined8 *puStack_50;
  undefined4 uStack_48;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x147230  202  VM_M_SetImageBufferNodeNum */
  uStack_18 = DAT_1802a5230 ^ (ulonglong)auStack_78;
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    uVar3 = FUN_18001db50();
    puStack_50 = (undefined8 *)CONCAT44(puStack_50._4_4_,param_1);
    uStack_58 = 0xb00;
    uStack_48 = param_2;
    FUN_180015b40(uVar3,2,&DAT_180239358,&DAT_180239288);
  }
  if ((DAT_1802b3358 & 1) == 0) {
    DAT_1802b3358 = DAT_1802b3358 | 1;
    FUN_18008f080();
    FUN_1801e8930(&DAT_180202f60);
  }
  FUN_1800efd00(&DAT_1802b3360,param_1,param_2);
  if ((DAT_1802b12c0 & 1) == 0) {
    DAT_1802b12c0 = DAT_1802b12c0 | 1;
    FUN_18003fde0();
    FUN_1801e8930(&DAT_180202970);
  }
  uVar2 = FUN_18006cb50();
  cVar1 = FUN_18001dbc0(2);
  if (cVar1 != '\0') {
    puVar4 = (undefined8 *)FUN_18008f750(auStack_38,uVar2);
    if (0xf < (ulonglong)puVar4[3]) {
      puVar4 = (undefined8 *)*puVar4;
    }
    uVar3 = FUN_18001db50();
    uStack_58 = 0xb05;
    puStack_50 = puVar4;
    FUN_180015b40(uVar3,2,&DAT_180234258,&DAT_180239288);
    if (0xf < uStack_20) {
      __3_YAXPEAX_Z(auStack_38[0]);
    }
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_78);
  return;
}

