
void FUN_1800f42b0(longlong param_1)

{
  int iVar1;
  
  iVar1 = *(int *)(*(longlong *)(param_1 + 0x10) + 0x30);
  if (iVar1 < *(int *)(param_1 + 8)) {
    FUN_1801a39f0(*(longlong *)(param_1 + 0x10) + 0x28,*(int *)(param_1 + 8) - iVar1);
    return;
  }
  return;
}

