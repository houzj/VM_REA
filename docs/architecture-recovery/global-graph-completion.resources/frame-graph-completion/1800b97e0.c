
undefined8 FUN_1800b97e0(longlong param_1)

{
  int iVar1;
  
  FUN_18001e4e0(param_1,0);
  iVar1 = SetEvent(*(undefined8 *)(param_1 + 0x128));
  if (iVar1 == 0) {
    GetLastError();
  }
  return 0;
}

