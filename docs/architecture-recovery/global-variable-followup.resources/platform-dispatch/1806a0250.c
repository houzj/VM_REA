
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8
PF_M_MsgDeal(undefined8 param_1,undefined8 param_2,longlong param_3,ulonglong param_4,
            undefined8 param_5)

{
  char cVar1;
  undefined8 uVar2;
  undefined8 in_stack_ffffffffffffffe8;
  undefined4 uVar3;
  
                    /* 0x6a0250  4  PF_M_MsgDeal */
  uVar3 = (undefined4)((ulonglong)in_stack_ffffffffffffffe8 >> 0x20);
  if ((param_3 == 0) || (param_4 < 0x200)) {
    cVar1 = FUN_18057a5b0(4);
    if (cVar1 != '\0') {
      uVar2 = FUN_18057a310();
      FUN_18057a4b0(uVar2,4,&UNK_1807ea9f8,&UNK_1807eaac0,CONCAT44(uVar3,0x58));
    }
    uVar2 = 0xe0000012;
  }
  else {
    if ((_DAT_1808f6ae8 & 1) == 0) {
      _DAT_1808f6ae8 = _DAT_1808f6ae8 | 1;
      FUN_180602280();
      FUN_180745c88(&UNK_18075ed20);
    }
    uVar2 = FUN_180602df0(&DAT_1808f6af0,param_1,param_2,param_3,param_4,param_5);
  }
  return uVar2;
}

