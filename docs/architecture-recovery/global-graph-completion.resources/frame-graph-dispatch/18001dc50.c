
undefined8 * FUN_18001dc50(undefined8 *param_1)

{
  int iVar1;
  undefined8 uVar2;
  
  *param_1 = &UNK_18021d398;
  param_1[1] = 0;
  param_1[2] = 0;
  uVar2 = FUN_180020380(param_1 + 1);
  param_1[1] = uVar2;
  FUN_1801e5140(param_1 + 3,0);
  *param_1 = &UNK_18021d350;
  iVar1 = _Mtx_init(param_1 + 7,2);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  iVar1 = _Mtx_init(param_1 + 8,2);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  param_1[9] = 0;
  param_1[10] = 0;
  uVar2 = FUN_18001f150();
  param_1[9] = uVar2;
  param_1[0xc] = 0;
  param_1[0xd] = 0;
  param_1[0xe] = 0;
  param_1[0x14] = 0xf;
  param_1[0x13] = 0;
  *(undefined1 *)(param_1 + 0x11) = 0;
  param_1[0x18] = 0xf;
  param_1[0x17] = 0;
  *(undefined1 *)(param_1 + 0x15) = 0;
  param_1[0x1c] = 0xf;
  param_1[0x1b] = 0;
  *(undefined1 *)(param_1 + 0x19) = 0;
  param_1[0x23] = 0xf;
  param_1[0x22] = 0;
  *(undefined1 *)(param_1 + 0x20) = 0;
  param_1[4] = 0;
  *(undefined4 *)(param_1 + 5) = 0;
  *(undefined4 *)((longlong)param_1 + 0x2c) = 0xffffffff;
  *(undefined4 *)(param_1 + 6) = 0;
  *(undefined4 *)(param_1 + 0xb) = 0;
  *(undefined1 *)(param_1 + 0xf) = 0;
  *(undefined4 *)((longlong)param_1 + 0x7c) = 0;
  *(undefined2 *)(param_1 + 0x10) = 1;
  *(undefined1 *)((longlong)param_1 + 0x82) = 1;
  *(undefined4 *)((longlong)param_1 + 0x84) = 1;
  FUN_180006300(param_1 + 0x11,&UNK_18021cf35,0);
  FUN_180006300(param_1 + 0x15,&UNK_18021cf35,0);
  FUN_180006300(param_1 + 0x19,&UNK_18021cf35,0);
  *(undefined4 *)(param_1 + 0x1d) = 0xffffffff;
  *(undefined8 *)((longlong)param_1 + 0xec) = 0;
  param_1[0x1f] = 0;
  FUN_180006300(param_1 + 0x20,&UNK_18021cf35,0);
  *(undefined1 *)(param_1 + 0x24) = 0;
  *(undefined1 *)((longlong)param_1 + 0x83) = 0;
  return param_1;
}

