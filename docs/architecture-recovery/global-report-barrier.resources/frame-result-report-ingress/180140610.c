
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8 VM_M_SetResultCallback(undefined *param_1,undefined8 param_2)

{
                    /* 0x140610  228  VM_M_SetResultCallback */
  _DAT_1802b1188 = param_2;
  _DAT_1802a5260 = &UNK_180001c30;
  if (param_1 != (undefined *)0x0) {
    _DAT_1802a5260 = param_1;
  }
  return 0;
}

