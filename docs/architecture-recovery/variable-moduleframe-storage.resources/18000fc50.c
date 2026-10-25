
undefined8 FUN_18000fc50(longlong param_1,undefined4 param_2)

{
  longlong lVar1;
  
  FUN_18000fb90();
  lVar1 = *(longlong *)(param_1 + 0x10);
  if (*(int *)(lVar1 + 0x68) == *(int *)(lVar1 + 0x6c)) {
    FUN_18000ee00(lVar1 + 0x68,*(int *)(lVar1 + 0x6c) + 1);
  }
  *(undefined4 *)(*(longlong *)(lVar1 + 0x70) + 8 + (longlong)*(int *)(lVar1 + 0x68) * 4) = param_2;
  *(int *)(lVar1 + 0x68) = *(int *)(lVar1 + 0x68) + 1;
  return 0;
}

