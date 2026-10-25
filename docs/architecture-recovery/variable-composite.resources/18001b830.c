
void FUN_18001b830(longlong *param_1)

{
  if (((int)param_1[10] != 0) && (*param_1 != 0)) {
    operator_delete__((void *)*param_1);
    *param_1 = 0;
    *(undefined4 *)(param_1 + 1) = 0;
  }
  *(undefined4 *)((longlong)param_1 + 0xc) = 0;
  return;
}

