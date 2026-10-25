
void FUN_180082e30(longlong param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4)

{
  undefined8 *puVar1;
  undefined1 *puVar2;
  undefined8 uStack_40;
  undefined8 uStack_38;
  undefined1 auStack_30 [24];
  undefined8 uStack_18;
  
  uStack_40 = *(undefined8 *)(param_1 + 8);
  uStack_38 = *(undefined8 *)(param_1 + 0x10);
  uStack_18 = 0;
  puVar1 = *(undefined8 **)(param_1 + 0x30);
  if (puVar1 == (undefined8 *)0x0) {
    uStack_18 = 0;
  }
  else {
    if (puVar1 == (undefined8 *)(param_1 + 0x18)) {
      puVar2 = auStack_30;
    }
    else {
      puVar2 = (undefined1 *)0x0;
    }
    uStack_18 = (**(code **)*puVar1)(puVar1,puVar2,param_3,param_4,0xfffffffffffffffe);
  }
  FUN_180082eb0(param_1,*(undefined8 *)(param_1 + 0x38),&uStack_40);
  return;
}

