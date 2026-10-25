
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180049cb0(undefined8 param_1,longlong param_2,int param_3,longlong param_4,
                  ulonglong *param_5,undefined8 param_6)

{
  char cVar1;
  undefined8 uVar2;
  ulonglong uVar3;
  undefined1 *puVar4;
  undefined1 auStack_78 [32];
  undefined4 uStack_58;
  undefined8 uStack_48;
  undefined1 uStack_40;
  undefined7 uStack_3f;
  undefined8 uStack_30;
  ulonglong uStack_28;
  ulonglong uStack_20;
  
  uStack_48 = 0xfffffffffffffffe;
  uStack_20 = _DAT_180192230 ^ (ulonglong)auStack_78;
  if ((param_2 == 0) || (param_3 == 0)) {
    cVar1 = FUN_18000c2c0(3);
    if (cVar1 != '\0') {
      uVar2 = FUN_18000bfe0();
      uStack_58 = 0x1fb;
      FUN_18000c1c0(uVar2,3,&UNK_18014ce48,&UNK_18014ce10);
    }
    goto LAB_180049dee;
  }
  uStack_28 = 0xf;
  uStack_30 = 0;
  uStack_40 = 0;
  FUN_1800094c0(&uStack_40,&UNK_18014552d,0);
  uVar2 = FUN_18004a5b0();
  FUN_18004a610(uVar2,param_2,&uStack_40,param_6);
  uVar3 = (ulonglong)(int)uStack_30;
  if ((int)uStack_30 < 1) {
LAB_180049d99:
    *param_5 = uVar3;
  }
  else if (param_4 != 0) {
    if ((ulonglong)(longlong)((int)uStack_30 + 1) <= *param_5) {
      puVar4 = &uStack_40;
      if (0xf < uStack_28) {
        puVar4 = (undefined1 *)CONCAT71(uStack_3f,uStack_40);
      }
      memcpy_s(param_4,*param_5,puVar4,uVar3);
      *(undefined1 *)(uVar3 + param_4) = 0;
      goto LAB_180049d99;
    }
    *param_5 = (longlong)((int)uStack_30 + 3);
  }
  if (0xf < uStack_28) {
    __3_YAXPEAX_Z(CONCAT71(uStack_3f,uStack_40));
  }
LAB_180049dee:
  FUN_18011da90(uStack_20 ^ (ulonglong)auStack_78);
  return;
}

