
undefined4
FUN_180082eb0(undefined8 *param_1,undefined8 param_2,undefined8 *param_3,undefined8 param_4)

{
  longlong *plVar1;
  longlong *plVar2;
  undefined4 uVar3;
  undefined1 *puVar4;
  undefined8 uStack_50;
  undefined8 uStack_48;
  undefined1 auStack_40 [24];
  undefined8 uStack_28;
  
  uStack_50 = *param_3;
  uStack_48 = param_3[1];
  plVar1 = param_3 + 2;
  uStack_28 = 0;
  plVar2 = (longlong *)param_3[5];
  if (plVar2 == (longlong *)0x0) {
    uStack_28 = 0;
  }
  else {
    if (plVar2 == plVar1) {
      puVar4 = auStack_40;
    }
    else {
      puVar4 = (undefined1 *)0x0;
    }
    uStack_28 = (**(code **)*plVar2)(plVar2,puVar4,param_3,param_4,0xfffffffffffffffe);
  }
  uVar3 = (*(code *)*param_1)(param_2,&uStack_50);
  plVar2 = (longlong *)param_3[5];
  if (plVar2 != (longlong *)0x0) {
    (**(code **)(*plVar2 + 0x20))(plVar2,plVar2 != plVar1);
    param_3[5] = 0;
  }
  plVar2 = (longlong *)param_3[5];
  if (plVar2 != (longlong *)0x0) {
    (**(code **)(*plVar2 + 0x20))(plVar2,plVar2 != plVar1);
    param_3[5] = 0;
  }
  return uVar3;
}

