
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800140a0(undefined8 param_1,undefined8 param_2,undefined8 param_3,ulonglong param_4,
                  undefined8 param_5)

{
  undefined *puVar1;
  undefined1 auStack_68 [32];
  undefined1 auStack_48 [32];
  ulonglong uStack_28;
  
  uStack_28 = _DAT_180025000 ^ (ulonglong)auStack_68;
  if ((param_4 & 1) == 0) {
    if ((param_4 & 2) == 0) {
      if ((param_4 & 4) == 0) {
        if ((param_4 & 8) == 0) goto LAB_180014123;
        puVar1 = &UNK_18001d040;
      }
      else {
        puVar1 = &UNK_18001d030;
      }
    }
    else {
      puVar1 = &UNK_18001d020;
    }
  }
  else {
    puVar1 = &UNK_18001d008;
  }
  FUN_180018930(auStack_48,puVar1);
  FUN_180014140(param_1,param_2,auStack_48,param_5);
LAB_180014123:
  FUN_180018cd0(uStack_28 ^ (ulonglong)auStack_68);
  return;
}

