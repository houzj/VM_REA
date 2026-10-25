
undefined8 *
FUN_180006470(undefined8 *param_1,undefined8 *param_2,ulonglong param_3,ulonglong param_4)

{
  code *pcVar1;
  undefined8 *puVar2;
  ulonglong uVar3;
  
  if ((ulonglong)param_2[2] < param_3) {
    __Xout_of_range_std__YAXPEBD_Z(&DAT_18021a360);
    pcVar1 = (code *)swi(3);
    puVar2 = (undefined8 *)(*pcVar1)();
    return puVar2;
  }
  uVar3 = param_2[2] - param_3;
  if (param_4 < uVar3) {
    uVar3 = param_4;
  }
  if (param_1 == param_2) {
    uVar3 = uVar3 + param_3;
    if ((ulonglong)param_1[2] < uVar3) {
      __Xout_of_range_std__YAXPEBD_Z(&DAT_18021a360);
      pcVar1 = (code *)swi(3);
      puVar2 = (undefined8 *)(*pcVar1)();
      return puVar2;
    }
    param_1[2] = uVar3;
    puVar2 = param_1;
    if (0xf < (ulonglong)param_1[3]) {
      puVar2 = (undefined8 *)*param_1;
    }
    *(undefined1 *)((longlong)puVar2 + uVar3) = 0;
    FUN_1800066a0(param_1,0);
  }
  else {
    if (uVar3 == 0xffffffffffffffff) {
      __Xlength_error_std__YAXPEBD_Z(&DAT_18021a350);
      pcVar1 = (code *)swi(3);
      puVar2 = (undefined8 *)(*pcVar1)();
      return puVar2;
    }
    if ((ulonglong)param_1[3] < uVar3) {
      FUN_180006770(param_1,uVar3,param_1[2]);
      if (uVar3 == 0) {
        return param_1;
      }
    }
    else if (uVar3 == 0) {
      param_1[2] = 0;
      if (0xf < (ulonglong)param_1[3]) {
        *(undefined1 *)*param_1 = 0;
        return param_1;
      }
      *(undefined1 *)param_1 = 0;
      return param_1;
    }
    if (0xf < (ulonglong)param_2[3]) {
      param_2 = (undefined8 *)*param_2;
    }
    puVar2 = param_1;
    if (0xf < (ulonglong)param_1[3]) {
      puVar2 = (undefined8 *)*param_1;
    }
    if (uVar3 != 0) {
      memcpy(puVar2,(longlong)param_2 + param_3,uVar3);
    }
    param_1[2] = uVar3;
    puVar2 = param_1;
    if (0xf < (ulonglong)param_1[3]) {
      puVar2 = (undefined8 *)*param_1;
    }
    *(undefined1 *)((longlong)puVar2 + uVar3) = 0;
  }
  return param_1;
}

