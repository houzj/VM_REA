
void FUN_180025c40(undefined8 param_1,undefined8 *param_2)

{
  longlong lVar1;
  
  lVar1 = FUN_180009510(param_2);
  *(undefined4 *)(lVar1 + 0x43c) = 0;
  lVar1 = FUN_180009510(param_2);
  *(undefined4 *)(lVar1 + 0x450) = 0;
  lVar1 = FUN_180009510(param_2);
  if (*(longlong *)(lVar1 + 0x440) != 0) {
    lVar1 = FUN_180009510(param_2);
    operator_delete__(*(void **)(lVar1 + 0x440));
    lVar1 = FUN_180009510(param_2);
    *(undefined8 *)(lVar1 + 0x440) = 0;
    lVar1 = FUN_180009510(param_2);
    *(undefined4 *)(lVar1 + 0x450) = 0;
  }
  lVar1 = FUN_180009510(param_2);
  if (*(longlong *)(lVar1 + 0x458) != 0) {
    lVar1 = FUN_180009510(param_2);
    operator_delete__(*(void **)(lVar1 + 0x458));
    lVar1 = FUN_180009510(param_2);
    *(undefined8 *)(lVar1 + 0x458) = 0;
    lVar1 = FUN_180009510(param_2);
    *(undefined4 *)(lVar1 + 0x450) = 0;
  }
  lVar1 = FUN_180009510(param_2);
  if (*(longlong *)(lVar1 + 0x460) != 0) {
    lVar1 = FUN_180009510(param_2);
    operator_delete__(*(void **)(lVar1 + 0x460));
    lVar1 = FUN_180009510(param_2);
    *(undefined8 *)(lVar1 + 0x460) = 0;
    lVar1 = FUN_180009510(param_2);
    *(undefined4 *)(lVar1 + 0x450) = 0;
  }
  lVar1 = FUN_180009510(param_2);
  if (*(longlong *)(lVar1 + 0x468) != 0) {
    lVar1 = FUN_180009510(param_2);
    if (*(longlong **)(lVar1 + 0x468) != (longlong *)0x0) {
      FUN_18001ba20(*(longlong **)(lVar1 + 0x468),3);
    }
    lVar1 = FUN_180009510(param_2);
    *(undefined8 *)(lVar1 + 0x468) = 0;
    lVar1 = FUN_180009510(param_2);
    *(undefined4 *)(lVar1 + 0x450) = 0;
  }
  lVar1 = FUN_180009510(param_2);
  if (*(longlong *)(lVar1 + 0x470) != 0) {
    lVar1 = FUN_180009510(param_2);
    if (*(void **)(lVar1 + 0x470) != (void *)0x0) {
      FUN_18001bc70(*(void **)(lVar1 + 0x470),3);
    }
    lVar1 = FUN_180009510(param_2);
    *(undefined8 *)(lVar1 + 0x470) = 0;
    lVar1 = FUN_180009510(param_2);
    *(undefined4 *)(lVar1 + 0x450) = 0;
  }
  FUN_180016480((longlong)param_2);
  return;
}

