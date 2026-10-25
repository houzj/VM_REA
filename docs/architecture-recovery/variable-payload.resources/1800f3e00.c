
void FUN_1800f3e00(longlong param_1,char *param_2,char *param_3,longlong *param_4)

{
  longlong *plVar1;
  undefined8 uVar2;
  longlong *plVar3;
  longlong lVar4;
  longlong lVar5;
  
  lVar4 = -1;
  if (*param_2 == '\0') {
    lVar5 = 0;
  }
  else {
    lVar5 = -1;
    do {
      lVar5 = lVar5 + 1;
    } while (param_2[lVar5] != '\0');
  }
  FUN_180006300(param_1 + 0x38,param_2,lVar5);
  if (*param_3 == '\0') {
    lVar4 = 0;
  }
  else {
    do {
      lVar4 = lVar4 + 1;
    } while (param_3[lVar4] != '\0');
  }
  FUN_180006300(param_1 + 0x58,param_3,lVar4);
  plVar3 = (longlong *)(param_1 + 0x18);
  if (plVar3 != param_4) {
    plVar1 = *(longlong **)(param_1 + 0x30);
    if (plVar1 != (longlong *)0x0) {
      (**(code **)(*plVar1 + 0x20))(plVar1,plVar1 != plVar3);
      *(undefined8 *)(param_1 + 0x30) = 0;
    }
    plVar1 = (longlong *)param_4[3];
    if (plVar1 == (longlong *)0x0) {
      *(undefined8 *)(param_1 + 0x30) = 0;
    }
    else {
      if (plVar1 != param_4) {
        plVar3 = (longlong *)0x0;
      }
      uVar2 = (**(code **)*plVar1)(plVar1,plVar3);
      *(undefined8 *)(param_1 + 0x30) = uVar2;
    }
  }
  *(undefined **)(param_1 + 0x80) = &DAT_1802a5510;
  *(undefined **)(param_1 + 0x10) = &DAT_1802a5510;
  return;
}

