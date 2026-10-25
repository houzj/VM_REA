
undefined8 *
FUN_180101240(undefined8 *param_1,undefined8 param_2,undefined8 *param_3,undefined8 *param_4,
             undefined4 *param_5)

{
  int *piVar1;
  undefined8 uVar2;
  
  *param_1 = FUN_1800ffd00;
  *(undefined4 *)(param_1 + 1) = *param_5;
  param_1[2] = 0;
  param_1[3] = 0;
  uVar2 = *param_4;
  if (param_4[1] != 0) {
    LOCK();
    piVar1 = (int *)(param_4[1] + 8);
    *piVar1 = *piVar1 + 1;
    UNLOCK();
  }
  FUN_180015c40(param_1 + 2,uVar2);
  param_1[4] = *param_3;
  return param_1;
}

