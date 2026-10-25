
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1805e3640(longlong param_1,undefined4 param_2)

{
  undefined4 uVar1;
  char cVar2;
  int iVar3;
  undefined8 uVar4;
  undefined1 *puVar5;
  undefined1 auStack_98 [32];
  undefined1 *puStack_78;
  undefined *puStack_70;
  undefined8 uStack_58;
  undefined1 uStack_50;
  undefined7 uStack_4f;
  undefined8 uStack_40;
  ulonglong uStack_38;
  undefined1 uStack_30;
  undefined7 uStack_2f;
  undefined8 uStack_20;
  ulonglong uStack_18;
  ulonglong uStack_10;
  
  uStack_58 = 0xfffffffffffffffe;
  uStack_10 = _DAT_1808559e0 ^ (ulonglong)auStack_98;
  cVar2 = FUN_18057a5b0(2);
  if (cVar2 != '\0') {
    uVar4 = FUN_18057a310();
    puStack_70 = (undefined *)CONCAT44(puStack_70._4_4_,param_2);
    puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x13b9);
    FUN_18057a4b0(uVar4,2,&UNK_1807d85b8,&UNK_1807d85f0);
  }
  LOCK();
  *(int *)(param_1 + 0x7d420) = *(int *)(param_1 + 0x7d420) + 1;
  UNLOCK();
  uVar1 = *(undefined4 *)(param_1 + 0x7d420);
  uStack_38 = 0xf;
  uStack_40 = 0;
  uStack_50 = 0;
  FUN_180577de0(&uStack_50,&UNK_1807c90ad,0);
  FUN_180670200();
  uStack_18 = 0xf;
  uStack_20 = 0;
  uStack_30 = 0;
  FUN_180577de0(&uStack_30,&UNK_1807c90ad,0);
  puVar5 = &uStack_50;
  if (0xf < uStack_38) {
    puVar5 = (undefined1 *)CONCAT71(uStack_4f,uStack_50);
  }
  puStack_70 = (undefined *)((ulonglong)puStack_70 & 0xffffffffffffff00);
  puStack_78 = &uStack_30;
  iVar3 = FUN_1805de710(param_1,puVar5,uStack_40,uVar1);
  if (iVar3 == 0) {
    cVar2 = FUN_18057a5b0(2);
    if (cVar2 != '\0') {
      uVar4 = FUN_18057a310();
      puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x13c2);
      FUN_18057a4b0(uVar4,2,&UNK_1807d8708,&UNK_1807d85f0);
    }
  }
  else {
    cVar2 = FUN_18057a5b0(4);
    if (cVar2 != '\0') {
      uVar4 = FUN_18057a310();
      puStack_70 = &UNK_1807d59d8;
      puStack_78 = (undefined1 *)CONCAT44(puStack_78._4_4_,0x13c0);
      FUN_18057a4b0(uVar4,4,&UNK_1807cc748,&UNK_1807d85f0);
    }
  }
  if (0xf < uStack_18) {
    __3_YAXPEAX_Z(CONCAT71(uStack_2f,uStack_30));
  }
  uStack_18 = 0xf;
  uStack_20 = 0;
  uStack_30 = 0;
  if (0xf < uStack_38) {
    __3_YAXPEAX_Z(CONCAT71(uStack_4f,uStack_50));
  }
  FUN_180745b60(uStack_10 ^ (ulonglong)auStack_98);
  return;
}

