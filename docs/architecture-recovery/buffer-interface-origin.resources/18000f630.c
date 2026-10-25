
void FUN_18000f630(undefined8 param_1,undefined8 *param_2,undefined8 *param_3,undefined8 *param_4)

{
  undefined8 uVar1;
  longlong lVar2;
  
  lVar2 = FUN_18000f190(param_1,param_2,param_3);
  if ((undefined8 *)(lVar2 + 0x10) != (undefined8 *)0x0) {
    uVar1 = param_4[1];
    *(undefined8 *)(lVar2 + 0x10) = *param_4;
    *(undefined8 *)(lVar2 + 0x18) = uVar1;
    uVar1 = param_4[3];
    *(undefined8 *)(lVar2 + 0x20) = param_4[2];
    *(undefined8 *)(lVar2 + 0x28) = uVar1;
    uVar1 = param_4[5];
    *(undefined8 *)(lVar2 + 0x30) = param_4[4];
    *(undefined8 *)(lVar2 + 0x38) = uVar1;
    *(undefined8 *)(lVar2 + 0x40) = param_4[6];
  }
  return;
}

