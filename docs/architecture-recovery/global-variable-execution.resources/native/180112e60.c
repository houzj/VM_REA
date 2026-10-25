
void FUN_180112e60(longlong param_1,undefined8 param_2)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  int iVar3;
  ulonglong *puVar4;
  undefined8 uStackX_10;
  
  uStackX_10 = param_2;
  iVar3 = _Mtx_lock(param_1 + 0x118);
  if (iVar3 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar3);
  }
  puVar4 = (ulonglong *)(param_1 + 0x100);
  puVar1 = *(undefined8 **)(param_1 + 0x108);
  if (&uStackX_10 < puVar1) {
    puVar2 = (undefined8 *)*puVar4;
    if (puVar2 <= &uStackX_10) {
      if (puVar1 == *(undefined8 **)(param_1 + 0x110)) {
        FUN_180035110(puVar4);
      }
      if (*(undefined8 **)(param_1 + 0x108) != (undefined8 *)0x0) {
        **(undefined8 **)(param_1 + 0x108) =
             *(undefined8 *)(*puVar4 + ((longlong)&uStackX_10 - (longlong)puVar2 >> 3) * 8);
      }
      goto LAB_180112f10;
    }
  }
  if (puVar1 == *(undefined8 **)(param_1 + 0x110)) {
    FUN_180035110(puVar4);
  }
  if (*(undefined8 **)(param_1 + 0x108) != (undefined8 *)0x0) {
    **(undefined8 **)(param_1 + 0x108) = param_2;
  }
LAB_180112f10:
  *(longlong *)(param_1 + 0x108) = *(longlong *)(param_1 + 0x108) + 8;
  iVar3 = _Mtx_unlock(param_1 + 0x118);
  if (iVar3 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar3);
  }
  return;
}

