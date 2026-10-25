
void FUN_18000f190(undefined8 param_1,undefined8 *param_2,undefined8 *param_3)

{
  undefined8 *puVar1;
  
  puVar1 = operator_new(0x48);
  if (puVar1 == (void *)0x0) {
                    /* WARNING: Subroutine does not return */
    std::_Xbad_alloc();
  }
  if (param_2 == (undefined8 *)0x0) {
    param_2 = puVar1;
    param_3 = puVar1;
  }
  *puVar1 = param_2;
  if (puVar1 + 1 != (undefined8 *)0x0) {
    puVar1[1] = param_3;
  }
  return;
}

