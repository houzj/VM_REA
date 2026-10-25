
undefined8 FUN_18000fd30(longlong param_1,undefined4 param_2)

{
  longlong lVar1;
  
  FUN_18000fb90();
  lVar1 = *(longlong *)(param_1 + 0x10);
  if (*(int *)(lVar1 + 0x40) == *(int *)(lVar1 + 0x44)) {
    FUN_18000ee00(lVar1 + 0x40,*(int *)(lVar1 + 0x44) + 1);
  }
  *(undefined4 *)(*(longlong *)(lVar1 + 0x48) + 8 + (longlong)*(int *)(lVar1 + 0x40) * 4) = param_2;
  *(int *)(lVar1 + 0x40) = *(int *)(lVar1 + 0x40) + 1;
  return 0;
}

