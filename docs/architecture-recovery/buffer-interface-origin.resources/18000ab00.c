
longlong FUN_18000ab00(undefined8 param_1,undefined8 *param_2,undefined8 *param_3,
                      undefined8 *param_4)

{
  undefined8 *puVar1;
  longlong lVar2;
  undefined8 uVar3;
  longlong lVar4;
  
  lVar4 = FUN_18000abe0(param_1,param_2,param_3);
  puVar1 = (undefined8 *)(lVar4 + 0x10);
  if (puVar1 != (undefined8 *)0x0) {
    *puVar1 = 0;
    *(undefined8 *)(lVar4 + 0x18) = 0;
    lVar2 = param_4[1];
    uVar3 = *param_4;
    if (lVar2 != 0) {
      LOCK();
      *(int *)(lVar2 + 8) = *(int *)(lVar2 + 8) + 1;
      UNLOCK();
    }
    FUN_18000a7b0(puVar1,uVar3,lVar2);
  }
  return lVar4;
}

