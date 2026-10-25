
undefined8 * CreateModule(undefined8 param_1)

{
  int iVar1;
  longlong lVar2;
  undefined8 *puVar3;
  
                    /* 0x16b00  1  CreateModule */
  lVar2 = FUN_180018fc4(0x78,0x1800258e0);
  if (lVar2 != 0) {
    puVar3 = (undefined8 *)FUN_180012bb0(lVar2);
    if (puVar3 != (undefined8 *)0x0) {
      puVar3[1] = param_1;
      iVar1 = FUN_180012cf0(puVar3);
      if (iVar1 != 0) {
        VM_M_SetLastError(param_1,iVar1);
        (**(code **)*puVar3)(puVar3,1);
        return (undefined8 *)0x0;
      }
      return puVar3;
    }
  }
  VM_M_SetLastError(param_1,0xe0000002);
  return (undefined8 *)0x0;
}

