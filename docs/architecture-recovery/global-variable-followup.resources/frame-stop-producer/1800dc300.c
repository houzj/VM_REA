
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800dc300(undefined8 *param_1,undefined4 param_2,undefined8 *param_3)

{
  int iVar1;
  undefined8 uVar2;
  undefined1 auStack_88 [32];
  undefined1 auStack_68 [16];
  undefined8 uStack_58;
  undefined8 uStack_50;
  undefined8 *puStack_48;
  undefined8 uStack_40;
  undefined8 *puStack_38;
  ulonglong uStack_30;
  
  uStack_40 = 0xfffffffffffffffe;
  uStack_30 = _DAT_1802a5230 ^ (ulonglong)auStack_88;
  uStack_50 = 0xf;
  uStack_58 = 0;
  auStack_68[0] = 0;
  puStack_48 = param_1;
  puStack_38 = param_3;
  FUN_180006300(auStack_68,&UNK_180225478,9);
  FUN_180010370(param_1,param_2,auStack_68);
  param_1[6] = &UNK_18021d398;
  param_1[7] = 0;
  param_1[8] = 0;
  uVar2 = FUN_180020380(param_1 + 7);
  param_1[7] = uVar2;
  FUN_1801e5140(param_1 + 9,0);
  *param_1 = &UNK_18022d6a0;
  param_1[6] = &UNK_18022d690;
  *(undefined4 *)(param_1 + 10) = param_2;
  param_1[0xe] = 0xf;
  param_1[0xd] = 0;
  *(undefined1 *)(param_1 + 0xb) = 0;
  FUN_180006470(param_1 + 0xb,param_3,0,0xffffffffffffffff);
  *(undefined4 *)(param_1 + 0xf) = 0;
  *(undefined1 *)((longlong)param_1 + 0x7c) = 0;
  param_1[0x10] = 0;
  param_1[0x11] = 0;
  param_1[0x12] = 0;
  param_1[0x18] = 0xf;
  param_1[0x17] = 0;
  *(undefined1 *)(param_1 + 0x15) = 0;
  FUN_180006300(param_1 + 0x15,&UNK_18021cf35,0);
  param_1[0x19] = 0;
  param_1[0x1a] = 0;
  *(undefined4 *)(param_1 + 0x1b) = 1;
  *(undefined4 *)(param_1 + 0x1c) = 0;
  LOCK();
  *(undefined4 *)((longlong)param_1 + 0xe4) = 0;
  UNLOCK();
  *(undefined2 *)(param_1 + 0x1d) = 0;
  *(undefined1 *)((longlong)param_1 + 0xea) = 0;
  param_1[0x1e] = 0;
  param_1[0x1f] = 0;
  iVar1 = _Mtx_init(param_1 + 0x20,2);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  iVar1 = _Mtx_init(param_1 + 0x21,2);
  if (iVar1 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar1);
  }
  *(undefined4 *)(param_1 + 0x22) = 3;
  *(undefined1 *)((longlong)param_1 + 0x114) = 0;
  param_1[0x23] = 0;
  param_1[0x24] = 0;
  uVar2 = FUN_18001f150();
  param_1[0x23] = uVar2;
  *(undefined1 *)(param_1 + 0x25) = 0;
  *(undefined4 *)((longlong)param_1 + 300) = 0;
  *(undefined1 *)(param_1 + 0x26) = 0;
  *(undefined4 *)(param_1 + 0x29) = 0;
  *(undefined1 *)((longlong)param_1 + 0x14c) = 0;
  param_1[0x2a] = 0;
  param_1[0x2b] = 0;
  uVar2 = FUN_180070300();
  param_1[0x2a] = uVar2;
  param_1[0x2c] = 0;
  param_1[0x2d] = 0;
  uVar2 = FUN_180070300();
  param_1[0x2c] = uVar2;
  *(undefined4 *)(param_1 + 0x2e) = 0;
  param_1[0x2f] = 0;
  param_1[0x30] = 0;
  param_1[0x31] = 0;
  *(undefined1 *)(param_1 + 0x32) = 1;
  param_1[0x33] = 0;
  param_1[0x34] = 0;
  *(undefined2 *)(param_1 + 0x35) = 0x100;
  param_1[0x39] = 0;
  *(undefined1 *)(param_1 + 0x3a) = 1;
  param_1[0x3b] = 0;
  *(undefined1 *)(param_1 + 0x3c) = 0;
  param_1[0x3d] = 0;
  param_1[0x3e] = 0;
  param_1[0x3f] = 0;
  uVar2 = FUN_18001f150();
  param_1[0x3e] = uVar2;
  param_1[0x40] = 0;
  param_1[0x41] = 0;
  uVar2 = FUN_180079000(param_1 + 0x40);
  param_1[0x40] = uVar2;
  param_1[0x42] = 0;
  param_1[0x43] = 0;
  *(undefined4 *)(param_1 + 0x44) = 0;
  param_1[0x45] = 0;
  param_1[0x46] = 0;
  *(undefined4 *)(param_1 + 0x47) = 0;
  param_1[0x48] = 0;
  param_1[0x49] = 0;
  uVar2 = FUN_1800a8ba0();
  param_1[0x48] = uVar2;
  param_1[0x27] = 0;
  param_1[0x28] = 0;
  uVar2 = CreateEventW(0,0,0,0);
  param_1[0x12] = uVar2;
  uVar2 = CreateEventW(0,1,1,0);
  param_1[0x13] = uVar2;
  uVar2 = CreateEventW(0,0,1);
  param_1[0x14] = uVar2;
  if (0xf < (ulonglong)param_3[3]) {
    __3_YAXPEAX_Z(*param_3);
  }
  param_3[3] = 0xf;
  param_3[2] = 0;
  *(undefined1 *)param_3 = 0;
  FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_88);
  return;
}

