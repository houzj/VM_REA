
void FUN_18001e4e0(longlong param_1,undefined4 param_2,undefined8 param_3,undefined8 param_4)

{
  undefined4 *puVar1;
  int iVar2;
  undefined4 auStackX_10 [2];
  undefined8 uVar3;
  
  uVar3 = 0xfffffffffffffffe;
  iVar2 = _Mtx_lock(param_1 + 0x40);
  if (iVar2 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar2);
  }
  *(undefined4 *)(param_1 + 0x58) = param_2;
  puVar1 = *(undefined4 **)(param_1 + 0x60);
  *(undefined4 **)(param_1 + 0x68) = puVar1;
  auStackX_10[0] = param_2;
  if ((auStackX_10 < puVar1) && (puVar1 <= auStackX_10)) {
    if (puVar1 == *(undefined4 **)(param_1 + 0x70)) {
      FUN_18001f010(param_1 + 0x60,1,param_3,param_4,uVar3);
    }
    if (*(undefined4 **)(param_1 + 0x68) != (undefined4 *)0x0) {
      **(undefined4 **)(param_1 + 0x68) =
           *(undefined4 *)
            (*(longlong *)(param_1 + 0x60) + ((longlong)auStackX_10 - (longlong)puVar1 >> 2) * 4);
    }
  }
  else {
    if (puVar1 == *(undefined4 **)(param_1 + 0x70)) {
      FUN_18001f010(param_1 + 0x60,1,param_3,param_4,uVar3);
    }
    if (*(undefined4 **)(param_1 + 0x68) != (undefined4 *)0x0) {
      **(undefined4 **)(param_1 + 0x68) = param_2;
    }
  }
  *(longlong *)(param_1 + 0x68) = *(longlong *)(param_1 + 0x68) + 4;
  iVar2 = _Mtx_unlock(param_1 + 0x40);
  if (iVar2 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar2);
  }
  return;
}

