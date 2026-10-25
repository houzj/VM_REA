
void FUN_1800f7410(longlong param_1)

{
  longlong *plVar1;
  
  if (0xf < *(ulonglong *)(param_1 + 0x70)) {
    __3_YAXPEAX_Z(*(undefined8 *)(param_1 + 0x58));
  }
  *(undefined8 *)(param_1 + 0x70) = 0xf;
  *(undefined8 *)(param_1 + 0x68) = 0;
  *(undefined1 *)(param_1 + 0x58) = 0;
  if (0xf < *(ulonglong *)(param_1 + 0x50)) {
    __3_YAXPEAX_Z(*(undefined8 *)(param_1 + 0x38));
  }
  *(undefined8 *)(param_1 + 0x50) = 0xf;
  *(undefined8 *)(param_1 + 0x48) = 0;
  *(undefined1 *)(param_1 + 0x38) = 0;
  plVar1 = *(longlong **)(param_1 + 0x30);
  if (plVar1 != (longlong *)0x0) {
    (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != (longlong *)(param_1 + 0x18));
    *(undefined8 *)(param_1 + 0x30) = 0;
  }
  plVar1 = *(longlong **)(param_1 + 0x30);
  if (plVar1 != (longlong *)0x0) {
    (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != (longlong *)(param_1 + 0x18));
    *(undefined8 *)(param_1 + 0x30) = 0;
  }
  return;
}

