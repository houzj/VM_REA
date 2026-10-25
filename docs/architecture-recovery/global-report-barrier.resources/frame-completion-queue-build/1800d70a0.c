
void FUN_1800d70a0(void)

{
  undefined8 uVar1;
  longlong lVar2;
  undefined8 *in_R9;
  
  lVar2 = FUN_18001f150();
  if ((undefined8 *)(lVar2 + 0x10) != (undefined8 *)0x0) {
    uVar1 = in_R9[1];
    *(undefined8 *)(lVar2 + 0x10) = *in_R9;
    *(undefined8 *)(lVar2 + 0x18) = uVar1;
  }
  return;
}

