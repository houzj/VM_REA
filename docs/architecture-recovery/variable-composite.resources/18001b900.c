
void FUN_18001b900(longlong param_1)

{
  if (*(longlong *)(param_1 + 0x108) != 0) {
    operator_delete__(*(void **)(param_1 + 0x108));
    *(undefined8 *)(param_1 + 0x108) = 0;
    *(undefined4 *)(param_1 + 0x104) = 0;
  }
  if (*(longlong *)(param_1 + 0x110) != 0) {
    operator_delete__(*(void **)(param_1 + 0x110));
    *(undefined8 *)(param_1 + 0x110) = 0;
    *(undefined4 *)(param_1 + 0x104) = 0;
  }
  if (*(longlong *)(param_1 + 0x118) != 0) {
    if (*(longlong **)(param_1 + 0x118) != (longlong *)0x0) {
      FUN_18001ba20(*(longlong **)(param_1 + 0x118),3);
    }
    *(undefined8 *)(param_1 + 0x118) = 0;
    *(undefined4 *)(param_1 + 0x104) = 0;
  }
  return;
}

