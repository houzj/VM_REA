
undefined8 * FUN_1806a3030(undefined8 *param_1)

{
  undefined8 uVar1;
  
  FUN_180652860();
  *param_1 = &UNK_1807ebac0;
  param_1[0x43] = &UNK_1807ebc48;
  *(undefined4 *)(param_1 + 0x44) = 100;
  param_1[0x45] = 0;
  param_1[0x46] = 0;
  uVar1 = FUN_1805a18b0();
  param_1[0x45] = uVar1;
  InitializeCriticalSection(param_1 + 0x47);
  *(undefined4 *)(param_1 + 0x4c) = 0;
  InitializeCriticalSection(param_1 + 0x4d);
  param_1[0x52] = 0;
  param_1[0x53] = 0;
  uVar1 = FUN_1806a5a40();
  param_1[0x52] = uVar1;
  *(undefined1 *)(param_1 + 0x54) = 0;
  *(undefined8 *)((longlong)param_1 + 0x2a4) = 0;
  InitializeCriticalSection(param_1 + 0x56);
  param_1[0x5b] = 0;
  param_1[0x5c] = 0;
  param_1[0x5d] = 0;
  param_1[0x5e] = 0;
  return param_1;
}

