
undefined8 *
FUN_1800820a0(undefined8 *param_1,undefined8 param_2,undefined8 *param_3,undefined8 *param_4)

{
  undefined8 *puVar1;
  undefined8 uVar2;
  undefined8 *puVar3;
  
  *param_1 = FUN_180080990;
  param_1[1] = *param_4;
  param_1[2] = param_4[1];
  puVar3 = param_1 + 3;
  param_1[6] = 0;
  puVar1 = (undefined8 *)param_4[5];
  if (puVar1 == (undefined8 *)0x0) {
    param_1[6] = 0;
  }
  else {
    if (puVar1 != param_4 + 2) {
      puVar3 = (undefined8 *)0x0;
    }
    uVar2 = (**(code **)*puVar1)(puVar1,puVar3,param_3,param_4 + 2,0,0xfffffffffffffffe);
    param_1[6] = uVar2;
  }
  param_1[7] = *param_3;
  return param_1;
}

