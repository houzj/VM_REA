
undefined8 FUN_1800f41e0(longlong *param_1,int param_2,char *param_3)

{
  int iVar1;
  undefined8 uVar2;
  longlong lVar3;
  
  lVar3 = 0;
  if (param_2 < 0) {
    uVar2 = 0xe0000014;
  }
  else {
    FUN_18000fb90();
    iVar1 = (**(code **)(*param_1 + 0x18))(param_1);
    if (param_2 < iVar1) {
      if (*param_3 != '\0') {
        lVar3 = -1;
        do {
          lVar3 = lVar3 + 1;
        } while (param_3[lVar3] != '\0');
      }
      FUN_180006300(*(undefined8 *)(*(longlong *)(param_1[2] + 0x38) + 8 + (longlong)param_2 * 8),
                    param_3,lVar3);
    }
    else {
      uVar2 = FUN_18000de60(param_1[2] + 0x28);
      if (*param_3 != '\0') {
        lVar3 = -1;
        do {
          lVar3 = lVar3 + 1;
        } while (param_3[lVar3] != '\0');
      }
      FUN_180006300(uVar2,param_3,lVar3);
    }
    uVar2 = 0;
  }
  return uVar2;
}

