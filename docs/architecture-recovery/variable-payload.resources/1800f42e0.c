
void FUN_1800f42e0(longlong param_1)

{
  int iVar1;
  longlong lVar2;
  longlong lVar3;
  undefined8 *puVar4;
  longlong lVar5;
  
  lVar5 = 0;
  lVar2 = *(longlong *)(param_1 + 0x10);
  iVar1 = *(int *)(lVar2 + 0x30);
  if (0 < iVar1) {
    lVar3 = *(longlong *)(lVar2 + 0x38);
    do {
      puVar4 = *(undefined8 **)(lVar3 + 8 + lVar5 * 8);
      puVar4[2] = 0;
      if (0xf < (ulonglong)puVar4[3]) {
        puVar4 = (undefined8 *)*puVar4;
      }
      lVar5 = lVar5 + 1;
      *(undefined1 *)puVar4 = 0;
    } while (lVar5 < iVar1);
    *(undefined4 *)(lVar2 + 0x30) = 0;
  }
  return;
}

