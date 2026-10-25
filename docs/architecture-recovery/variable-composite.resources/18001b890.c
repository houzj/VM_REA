
undefined1 * FUN_18001b890(undefined1 *param_1)

{
  longlong lVar1;
  undefined1 *puVar2;
  
  *(undefined4 *)(param_1 + 0x104) = 0;
  puVar2 = param_1;
  for (lVar1 = 0x100; lVar1 != 0; lVar1 = lVar1 + -1) {
    *puVar2 = 0;
    puVar2 = puVar2 + 1;
  }
  *(undefined4 *)(param_1 + 0x100) = 6;
  *(undefined8 *)(param_1 + 0x108) = 0;
  *(undefined8 *)(param_1 + 0x110) = 0;
  *(undefined8 *)(param_1 + 0x118) = 0;
  return param_1;
}

