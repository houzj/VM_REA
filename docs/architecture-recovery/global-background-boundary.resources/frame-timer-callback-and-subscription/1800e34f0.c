
void FUN_1800e34f0(void)

{
  longlong lVar1;
  undefined8 *in_R9;
  
  lVar1 = FUN_18001f150();
  if ((undefined8 *)(lVar1 + 0x10) != (undefined8 *)0x0) {
    *(undefined8 *)(lVar1 + 0x10) = *in_R9;
    *(undefined4 *)(lVar1 + 0x18) = *(undefined4 *)(in_R9 + 1);
  }
  return;
}

