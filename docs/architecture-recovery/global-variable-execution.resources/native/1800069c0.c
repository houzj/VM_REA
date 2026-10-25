
void FUN_1800069c0(longlong param_1)

{
  int iVar1;
  longlong lVar2;
  longlong lVar3;
  
  iVar1 = *(int *)(param_1 + 8);
  if (0 < iVar1) {
    lVar2 = *(longlong *)(param_1 + 0x10);
    lVar3 = 0;
    do {
      (**(code **)(**(longlong **)(lVar2 + 8 + lVar3 * 8) + 0x30))();
      lVar3 = lVar3 + 1;
    } while (lVar3 < iVar1);
    *(undefined4 *)(param_1 + 8) = 0;
  }
  return;
}

