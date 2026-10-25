
undefined8 FUN_1800f3ee0(longlong *param_1,int param_2,undefined4 param_3)

{
  longlong lVar1;
  int iVar2;
  undefined8 uVar3;
  
  if (param_2 < 0) {
    uVar3 = 0xe0000014;
  }
  else {
    FUN_18000fb90();
    iVar2 = (**(code **)(*param_1 + 0x18))(param_1);
    if (param_2 < iVar2) {
      *(undefined4 *)(*(longlong *)(param_1[2] + 0x70) + 8 + (longlong)param_2 * 4) = param_3;
    }
    else {
      lVar1 = param_1[2];
      if (*(int *)(lVar1 + 0x68) == *(int *)(lVar1 + 0x6c)) {
        FUN_18000ee00(lVar1 + 0x68,*(int *)(lVar1 + 0x6c) + 1);
      }
      *(undefined4 *)(*(longlong *)(lVar1 + 0x70) + 8 + (longlong)*(int *)(lVar1 + 0x68) * 4) =
           param_3;
      *(int *)(lVar1 + 0x68) = *(int *)(lVar1 + 0x68) + 1;
    }
    uVar3 = 0;
  }
  return uVar3;
}

